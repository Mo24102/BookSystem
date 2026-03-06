const API_URL = "https://localhost:7024";
<<<<<<< HEAD
let isEditMode = false; // متغير للتحكم هل نحن في وضع إضافة أم تعديل
=======
>>>>>>> df3c73617a03f5b0497150d154bacba039e0876d

document.addEventListener('DOMContentLoaded', () => {
    loadExpenses();
});

<<<<<<< HEAD
// 1. جلب المصروفات وعرضها
=======
// 1. تحميل المصروفات وعرضها
>>>>>>> df3c73617a03f5b0497150d154bacba039e0876d
async function loadExpenses() {
    try {
        const token = localStorage.getItem('token');
        const res = await fetch(`${API_URL}/api/expenses`, { 
            headers: { 'Authorization': `Bearer ${token}` } 
        });
        
        const data = await res.json();
        const tableBody = document.getElementById('expensesTableBody');
<<<<<<< HEAD
        const expenses = Array.isArray(data) ? data : (data.items || []);
        
        tableBody.innerHTML = expenses.map(exp => `
            <tr>
                <td><strong>${exp.category}</strong></td>
                <td class="text-danger fw-bold">${exp.amount} EGP</td>
                <td>${new Date(exp.expenseDate).toLocaleDateString('ar-EG')}</td>
                <td class="text-muted small">${exp.notes || '---'}</td>
                <td class="text-center">
                    <button class="btn btn-sm btn-edit me-1" onclick='prepareEditModal(${JSON.stringify(exp)})'>
                        <i class="fas fa-edit"></i> تعديل
                    </button>
                    <button class="btn btn-sm btn-outline-danger" onclick="deleteExpense(${exp.id})">
                        <i class="fas fa-trash"></i> حذف
=======
        
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
>>>>>>> df3c73617a03f5b0497150d154bacba039e0876d
                    </button>
                </td>
            </tr>
        `).join('');
    } catch (err) { 
<<<<<<< HEAD
        console.error("Fetch Error:", err); 
    }
}

// 2. التحضير لإضافة مصروف جديد
function prepareAddModal() {
    isEditMode = false;
    document.getElementById('expenseForm').reset();
    document.getElementById('editExpenseId').value = "";
    document.getElementById('modalTitle').innerText = "تسجيل مصروف جديد";
    document.getElementById('submitBtn').innerText = "حفظ المصروف";
    new bootstrap.Modal(document.getElementById('expenseModal')).show();
}

// 3. التحضير لتعديل مصروف موجود
function prepareEditModal(exp) {
    isEditMode = true;
    document.getElementById('modalTitle').innerText = "تعديل المصروف";
    document.getElementById('submitBtn').innerText = "تحديث البيانات";
    
    // ملء الحقول بالبيانات الحالية
    document.getElementById('editExpenseId').value = exp.id;
    document.getElementById('eCat').value = exp.category;
    document.getElementById('eAmount').value = exp.amount;
    document.getElementById('eNotes').value = exp.notes;
    
    // تنسيق التاريخ ليظهر بشكل صحيح في الـ input
    if(exp.expenseDate) {
        const date = new Date(exp.expenseDate);
        date.setMinutes(date.getMinutes() - date.getTimezoneOffset());
        document.getElementById('eDate').value = date.toISOString().slice(0, 16);
    }

    new bootstrap.Modal(document.getElementById('expenseModal')).show();
}

// 4. دالة الحفظ (إرسال POST للإضافة أو PUT للتعديل)
document.getElementById('expenseForm').onsubmit = async (e) => {
    e.preventDefault();
    
    const token = localStorage.getItem('token');
    const id = document.getElementById('editExpenseId').value;
    
    const expData = {
        category: document.getElementById('eCat').value,
        amount: parseFloat(document.getElementById('eAmount').value),
        expenseDate: new Date(document.getElementById('eDate').value).toISOString(),
        notes: document.getElementById('eNotes').value || "لا يوجد ملاحظات"
    };

    const url = isEditMode ? `${API_URL}/api/expenses/${id}` : `${API_URL}/api/expenses`;
    const method = isEditMode ? 'PUT' : 'POST';

    try {
        const res = await fetch(url, {
            method: method,
            headers: { 
                'Content-Type': 'application/json', 
                'Authorization': `Bearer ${token}` 
            },
            body: JSON.stringify(expData)
        });

        if (res.ok) {
            alert(isEditMode ? "تم تحديث المصروف بنجاح ✅" : "تمت إضافة المصروف بنجاح ✅");
            bootstrap.Modal.getInstance(document.getElementById('expenseModal')).hide();
            loadExpenses();
        } else {
            alert("فشل الإجراء. تأكد من صلاحيات الآدمن.");
        }
    } catch (err) {
        console.error("Submit Error:", err);
        alert("حدث خطأ أثناء الاتصال بالسيرفر");
    }
};

// 5. حذف مصروف
async function deleteExpense(id) {
    if(confirm("هل أنت متأكد من حذف هذا المصروف؟ لا يمكن التراجع عن هذه الخطوة.")) {
        try {
            const token = localStorage.getItem('token');
            const res = await fetch(`${API_URL}/api/expenses/${id}`, { 
                method: 'DELETE', 
                headers: { 'Authorization': `Bearer ${token}` } 
            });
            
            if(res.ok) {
                loadExpenses();
            } else {
                alert("فشل الحذف. ربما المصروف غير موجود أو لا تملك الصلاحية.");
            }
        } catch (err) {
            alert("خطأ في الاتصال بالسيرفر");
        }
=======
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
>>>>>>> df3c73617a03f5b0497150d154bacba039e0876d
    }
}