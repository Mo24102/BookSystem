<<<<<<< HEAD
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
=======
const API_URL = "https://localhost:7024";

// 1. تشغيل الوظائف عند تحميل الصفحة
document.addEventListener('DOMContentLoaded', () => {
    updateStats();
    loadClients();
    checkAdminPermissions();
});

// 2. التحقق من الصلاحيات (إخفاء أزرار الإضافة لغير الأدمن)
function checkAdminPermissions() {
    const role = localStorage.getItem('userRole');
    if (role !== 'Admin') {
        // إخفاء أزرار إضافة عميل ومصروف لو مش أدمن
        const adminButtons = document.querySelectorAll('[data-bs-target="#clientModal"], [data-bs-target="#expenseModal"]');
        adminButtons.forEach(btn => btn.style.display = 'none');
    }
}

// 3. تحديث الإحصائيات المالية
async function updateStats() {
    try {
        const res = await fetch(`${API_URL}/api/revenue/summary`, { 
            headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` } 
        });
        if (!res.ok) return;
        const data = await res.json();
        
        document.getElementById('statTotalDue').innerText = (data.totalExpectedRevenue || 0) + " EGP";
        document.getElementById('statRevenue').innerText = (data.netProfit || 0) + " EGP";
        document.getElementById('statExpenses').innerText = (data.totalExpenses || 0) + " EGP";
    } catch (err) { console.error("Stats Error:", err); }
}

// 4. عرض جدول العملاء مع زراير التحكم
async function loadClients() {
    try {
        const res = await fetch(`${API_URL}/api/clients?pageSize=100`, { 
            headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` } 
        });
        const data = await res.json();
        const role = localStorage.getItem('userRole');
        const tableBody = document.getElementById('clientsTable');
        
        tableBody.innerHTML = data.items.map(client => `
            <tr>
                <td>${client.clientName}</td>
                <td>${client.phone}</td>
                <td>${client.serviceType}</td>
                <td>${client.totalDue}</td>
                <td class="text-success">${client.paidAmount}</td>
                <td class="text-danger fw-bold">${client.totalDue - client.paidAmount}</td>
                <td>
                    ${role === 'Admin' ? `
                        <button class="btn btn-sm btn-outline-info" onclick='fillEditForm(${JSON.stringify(client)})'><i class="fas fa-edit"></i></button>
                        <button class="btn btn-sm btn-outline-danger" onclick="deleteClient(${client.id})"><i class="fas fa-trash"></i></button>
                    ` : '<span class="badge bg-light text-dark">عرض فقط</span>'}
                </td>
            </tr>
        `).join('');
    } catch (err) { console.error("Load Clients Error:", err); }
}

// 5. دالة واحدة للإضافة والتعديل (Smart Submit)
document.getElementById('clientForm').onsubmit = async (e) => {
    e.preventDefault();
    const id = document.getElementById('editClientId').value; // حقل مخفي للـ ID
    
    const clientData = {
        clientName: document.getElementById('cName').value,
        phone: document.getElementById('cPhone').value,
        nationalId: document.getElementById('cNationalId').value,
        serviceType: document.getElementById('cService').value,
        totalDue: parseFloat(document.getElementById('cTotal').value) || 0,
        paidAmount: parseFloat(document.getElementById('cPaid').value) || 0,
        actualCost: parseFloat(document.getElementById('cCost').value) || 0,
        notes: document.getElementById('cNotes').value,
        finalDueDate: new Date().toISOString()
    };

    const method = id ? 'PUT' : 'POST';
    const url = id ? `${API_URL}/clients/${id}` : `${API_URL}/api/clients`;

    const res = await fetch(url, {
        method: method,
        headers: { 
            'Content-Type': 'application/json', 
            'Authorization': `Bearer ${localStorage.getItem('token')}` 
        },
        body: JSON.stringify(clientData)
    });

    if (res.ok) {
        alert(id ? "تم التعديل بنجاح" : "تمت الإضافة بنجاح");
        location.reload();
    } else {
        alert("حدث خطأ، تأكد من البيانات وصلاحيات الأدمن");
    }
};

// 6. ملء الفورم عند الضغط على تعديل
function fillEditForm(client) {
    document.getElementById('editClientId').value = client.id;
    document.getElementById('cName').value = client.clientName;
    document.getElementById('cPhone').value = client.phone;
    document.getElementById('cNationalId').value = client.nationalId;
    document.getElementById('cService').value = client.serviceType;
    document.getElementById('cTotal').value = client.totalDue;
    document.getElementById('cPaid').value = client.paidAmount;
    document.getElementById('cCost').value = client.actualCost;
    document.getElementById('cNotes').value = client.notes;
    
    // فتح المودال برمجياً
    new bootstrap.Modal(document.getElementById('clientModal')).show();
}

// 7. حذف عميل (للأدمن فقط)
async function deleteClient(id) {
    if(confirm("هل أنت متأكد من الحذف النهائي؟")) {
        const res = await fetch(`${API_URL}/api/clients/${id}`, { 
            method: 'DELETE', 
            headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` } 
        });
        if(res.ok) loadClients();
        else alert("فشل الحذف، قد لا تملك صلاحية أدمن");
    }
}

// 8. إضافة مصروف
document.getElementById('expenseForm').onsubmit = async (e) => {
    e.preventDefault();
    const expData = {
        category: document.getElementById('eCat').value,
        amount: parseFloat(document.getElementById('eAmount').value),
        expenseDate: new Date(document.getElementById('eDate').value).toISOString(),
        notes: document.getElementById('eNotes').value
    };

    await fetch(`${API_URL}/api/expenses`, {
        method: 'POST',
        headers: { 
            'Content-Type': 'application/json', 
            'Authorization': `Bearer ${localStorage.getItem('token')}` 
        },
        body: JSON.stringify(expData)
    });
    location.reload();
};
>>>>>>> df3c73617a03f5b0497150d154bacba039e0876d
