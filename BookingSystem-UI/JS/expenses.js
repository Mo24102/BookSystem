const API_URL = "https://localhost:7024";

document.addEventListener('DOMContentLoaded', () => {
    loadExpenses();
});

// 1. تحميل المصروفات وعرضها
async function loadExpenses() {
    try {
        const token = localStorage.getItem('token');
        const res = await fetch(`${API_URL}/api/expenses`, { 
            headers: { 'Authorization': `Bearer ${token}` } 
        });
        
        const data = await res.json();
        const tableBody = document.getElementById('expensesTableBody');
        
        // التعامل مع البيانات سواء كانت Array مباشرة أو جوه Object
        const expenses = Array.isArray(data) ? data : (data.items || []);
        
        tableBody.innerHTML = expenses.map(exp => `
            <tr class="expense-card">
                <td><strong>${exp.category}</strong></td>
                <td class="text-danger fw-bold">${exp.amount} EGP</td>
                <td>${new Date(exp.expenseDate).toLocaleDateString('ar-EG')}</td>
                <td>
                    <button class="btn btn-sm btn-outline-danger" onclick="deleteExpense(${exp.id})">
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            </tr>
        `).join('');
    } catch (err) { 
        console.error("Load Error:", err); 
    }
}

// 2. دالة حفظ مصروف جديد (Submit)
const expenseForm = document.getElementById('expenseForm');
if (expenseForm) {
    expenseForm.onsubmit = async (e) => {
        e.preventDefault();
        
        const token = localStorage.getItem('token');
        const expData = {
            category: document.getElementById('eCat').value,
            amount: parseFloat(document.getElementById('eAmount').value),
            expenseDate: new Date(document.getElementById('eDate').value).toISOString(),
            notes: document.getElementById('eNotes').value || "لا يوجد ملاحظات"
        };

        try {
            const res = await fetch(`${API_URL}/api/expenses`, {
                method: 'POST',
                headers: { 
                    'Content-Type': 'application/json', 
                    'Authorization': `Bearer ${token}` 
                },
                body: JSON.stringify(expData)
            });

            if (res.ok) {
                alert("تم تسجيل المصروف بنجاح ✅");
                bootstrap.Modal.getInstance(document.getElementById('expenseModal')).hide();
                loadExpenses(); // تحديث الجدول بدون ريفريش كامل
                expenseForm.reset(); // تصفير الفورم
            } else {
                alert("فشل في الحفظ، تأكد من تسجيل الدخول كأدمن");
            }
        } catch (err) {
            console.error("Save Error:", err);
            alert("حدث خطأ أثناء الاتصال بالسيرفر");
        }
    };
}

// 3. حذف مصروف
async function deleteExpense(id) {
    if(confirm("هل تريد حذف هذا المصروف؟")) {
        const token = localStorage.getItem('token');
        const res = await fetch(`${API_URL}/api/expenses/${id}`, { 
            method: 'DELETE', 
            headers: { 'Authorization': `Bearer ${token}` } 
        });
        if(res.ok) loadExpenses();
    }
}