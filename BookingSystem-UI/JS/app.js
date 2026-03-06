Chart.defaults.color = '#94a3b8';       // لون النصوص (رمادي فاتح يتناسب مع الدارك مود)
Chart.defaults.borderColor = '#334155';
const API_URL = "https://localhost:7024";
let mainChart = null;
let pieChart = null;

document.addEventListener('DOMContentLoaded', () => {
    // تشغيل الـ Dashboard بمجرد تحميل الصفحة بالقيم الافتراضية في الـ HTML (فبراير 2026)
    updateDashboard();
});

async function updateDashboard() {
    try {
        const month = document.getElementById('selectMonth').value;
        const year = document.getElementById('selectYear').value;
        const token = localStorage.getItem('token');

        // تحديث النص قبل الجلب
        document.getElementById('currentDate').innerText = `جاري جلب بيانات شهر ${month} لسنة ${year}...`;

        const res = await fetch(`${API_URL}/api/expenses/summary/monthly/${year}/${month}`, { 
            headers: { 'Authorization': `Bearer ${token}` } 
        });

        if (!res.ok) {
            console.error("خطأ في السيرفر:", res.status);
            resetDashboardCards();
            return;
        }

        const data = await res.json();
        console.log("البيانات المستلمة:", data);

        // 1. تحديث واجهة المستخدم بالأرقام
        updateUI(data, month, year);

        // 2. تحديث الرسوم البيانية
        renderCharts(data);

    } catch (err) { 
        console.error("Dashboard Error:", err);
        resetDashboardCards();
    }
}

function updateUI(data, month, year) {
    document.getElementById('totalRevenue').innerText = (data.totalRevenue || 0).toLocaleString() + " EGP";
    document.getElementById('totalDirectCosts').innerText = (data.totalDirectCosts || 0).toLocaleString() + " EGP";
    document.getElementById('totalExpenses').innerText = (data.totalExpenses || 0).toLocaleString() + " EGP";
    document.getElementById('netProfit').innerText = (data.netProfit || 0).toLocaleString() + " EGP";
    document.getElementById('currentDate').innerText = `إحصائيات شهر ${month} لسنة ${year}`;
}

function renderCharts(data) {
    // تدمير الرسوم القديمة لمنع تداخل الأكشن
    if (mainChart) mainChart.destroy();
    if (pieChart) pieChart.destroy();

    // الرسم البياني للأعمدة
    const ctxBar = document.getElementById('profitChart').getContext('2d');
    mainChart = new Chart(ctxBar, {
        type: 'bar',
        data: {
            labels: ['إيرادات', 'تكاليف', 'مصروفات', 'صافي ربح'],
            datasets: [{
                label: 'القيمة بالجنيه',
                data: [data.totalRevenue, data.totalDirectCosts, data.totalExpenses, data.netProfit],
                backgroundColor: ['#3b82f6', '#f97316', '#ef4444', '#22c55e'],
                borderRadius: 8
            }]
        },
        options: { responsive: true, plugins: { legend: { display: false } } }
    });

    // الرسم البياني الدائري
    const ctxPie = document.getElementById('pieChart').getContext('2d');
    pieChart = new Chart(ctxPie, {
        type: 'doughnut',
        data: {
            labels: ['صافي ربح', 'تكاليف', 'مصروفات'],
            datasets: [{
                data: [data.netProfit, data.totalDirectCosts, data.totalExpenses],
                backgroundColor: ['#22c55e', '#f97316', '#ef4444']
            }]
        },
        options: { cutout: '70%', plugins: { legend: { position: 'bottom' } } }
        
    });


}

function resetDashboardCards() {
    ['totalRevenue', 'totalDirectCosts', 'totalExpenses', 'netProfit'].forEach(id => {
        document.getElementById(id).innerText = "0 EGP";
    });
    document.getElementById('currentDate').innerText = "لا توجد بيانات متاحة لهذا الشهر";
}

function logout() {
    localStorage.clear();
    window.location.href = "/HTML/Auth/Login.html";
}