var API_URL = "https://localhost:7024"; 

document.addEventListener('DOMContentLoaded', () => {
    loadClients();
    const cForm = document.getElementById('clientForm');
    if (cForm) cForm.onsubmit = saveClient;
});

async function loadClients() {
    const tableBody = document.getElementById('clientsTable');
    if (!tableBody) return;

    try {
        const token = localStorage.getItem('token');
        // أضفنا Timestamp لمنع الكاش (Cache)
        const res = await fetch(`${API_URL}/api/clients?t=${new Date().getTime()}`, { 
            headers: { 'Authorization': `Bearer ${token}`, 'Accept': 'application/json' } 
        });
        
        if (!res.ok) throw new Error("فشل الاتصال بالسيرفر");

        const data = await res.json();
        console.log("البيانات القادمة من السيرفر:", data); // لتتبع القيم
        
        const clientsArray = Array.isArray(data) ? data : (data.items || data.data || []); 

      // داخل دالة loadClients - استبدل الـ map بهذا الجزء المتوافق مع PascalCase
tableBody.innerHTML = clientsArray.map(client => {
    // نستخدم الـ || للتحقق من وجود الحقل بأي من الصيغتين (Camel أو Pascal)
    const clientName = client.clientName || client.ClientName;
    const phone = client.phone || client.Phone;
    const paidAmount = client.paidAmount || client.PaidAmount || 0;
    const totalDue = client.totalDue || client.TotalDue || 0;
    const remainingAmount = client.remainingAmount || client.RemainingAmount || 0;
    const profit = client.profit || client.Profit || 0;
    const status = client.paymentStatus || client.PaymentStatus || '---';
    const lastDate = client.lastPaymentDate ? new Date(client.lastPaymentDate).toLocaleDateString('ar-EG') : '---';
    const clientId = client.id || client.Id;

    return `
    <tr>
        <td class="fw-bold">${clientName}</td>
        <td>${phone}</td>
        <td class="national-id-text">${client.nationalId || client.NationalId || ''}</td>
        <td><span class="badge bg-light text-primary border">${client.serviceType || client.ServiceType || ''}</span></td>
        <td>${(client.actualCost || client.ActualCost || 0).toLocaleString()}</td>
        <td>${totalDue.toLocaleString()}</td>
        <td class="text-primary">${paidAmount.toLocaleString()}</td>
        <td class="text-danger fw-bold">${remainingAmount.toLocaleString()}</td>
        <td class="${profit >= 0 ? 'profit-pos' : 'profit-neg'}">${profit.toLocaleString()}</td>
        <td>
            <span class="badge ${status === 'مدفوع' ? 'bg-success' : 'bg-warning text-dark'}">
                ${status}
            </span>
        </td>
        <td>${client.paidInstallments || client.PaidInstallments || 0} / ${client.numberOfInstallments || client.NumberOfInstallments || 0}</td>
        <td class="small">${client.createdBy || client.CreatedBy || '---'}</td>
        <td class="small">${lastDate}</td>
        <td class="notes-cell text-truncate" style="max-width: 250px">${client.notes || client.Notes || 'لا يوجد ملاحظات'}</td>
        <td class="text-center">
            <div class="btn-group" style="gap: 8px;"> <button class="btn btn-sm btn-success" title="دفع دفعة" onclick='openPaymentModal(${JSON.stringify(client)})'>
            <i class="fas fa-hand-holding-usd"></i> </button>
                </button>
                <button class="btn btn-sm btn-outline-info" title="تعديل" onclick='fillEditForm(${JSON.stringify(client)})'>
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-sm btn-outline-danger" title="حذف" onclick='deleteClient("${clientId}")'>
                    <i class="fas fa-trash-alt"></i>
                </button>
            </div>
        </td>
    </tr>`;
}).join('');
        
    } catch (err) { 
        console.error(err);
        tableBody.innerHTML = '<tr><td colspan="15" class="text-center text-danger">تعذر الاتصال بالسيرفر</td></tr>';
    }
}

window.openPaymentModal = (client) => {
    document.getElementById('payClientId').value = client.id || client.Id;
    document.getElementById('newPayAmount').value = '';
    window.currentClientData = client; 
    new bootstrap.Modal(document.getElementById('paymentModal')).show();
};

async function submitQuickPayment() {
    const id = document.getElementById('payClientId').value;
    const amountToAdd = parseFloat(document.getElementById('newPayAmount').value) || 0;
    const token = localStorage.getItem('token');

    if (amountToAdd <= 0) {
        alert("يرجى إدخال مبلغ صحيح");
        return;
    }

    const newTotalPaid = (window.currentClientData.paidAmount || 0) + amountToAdd;
    const totalDue = window.currentClientData.totalDue || 0;

    // تجهيز الكائن بنفس مفاتيح الـ API (PascalCase)
    const updatedData = {
        Id: id,
        ClientName: window.currentClientData.clientName,
        Phone: window.currentClientData.phone,
        NationalId: window.currentClientData.nationalId,
        ServiceType: window.currentClientData.serviceType,
        ActualCost: window.currentClientData.actualCost,
        TotalDue: totalDue,
        PaidAmount: newTotalPaid,
        NumberOfInstallments: window.currentClientData.numberOfInstallments,
        PaidInstallments: window.currentClientData.paidInstallments,
        PaymentStatus: newTotalPaid >= totalDue ? "مدفوع" : "دفع جزئي",
        Notes: window.currentClientData.notes
    };

    try {
        const res = await fetch(`${API_URL}/api/clients/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
            body: JSON.stringify(updatedData)
        });

        if (res.ok) {
            const modalElement = document.getElementById('paymentModal');
            const modalInstance = bootstrap.Modal.getInstance(modalElement);
            modalInstance.hide();
            
            await loadClients(); // تحديث الجدول
            alert(`تم إضافة ${amountToAdd} بنجاح ✅`);
        } else {
            const errData = await res.json();
            alert("فشل التحديث: " + (errData.message || "خطأ في البيانات"));
        }
    } catch (err) {
        alert("خطأ في الاتصال بالسيرفر");
    }
}

async function deleteClient(id) {
    if (!confirm("هل أنت متأكد من حذف هذا العميل نهائياً؟")) return;

    try {
        const token = localStorage.getItem('token');
        const res = await fetch(`${API_URL}/api/clients/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (res.ok) {
            loadClients();
            alert("تم الحذف بنجاح");
        } else {
            alert("فشل الحذف");
        }
    } catch (err) {
        alert("خطأ في الاتصال");
    }
}

function filterClients() {
    const filter = document.getElementById('searchInput').value.toLowerCase();
    const rows = document.querySelectorAll('#clientsTable tr');
    rows.forEach(row => {
        const text = row.innerText.toLowerCase();
        row.style.display = text.includes(filter) ? "" : "none";
    });
}

async function saveClient(e) {
    e.preventDefault();
    const id = document.getElementById('editClientId').value;
    const token = localStorage.getItem('token');
    
    const clientData = {
        ClientName: document.getElementById('cName').value,
        Phone: document.getElementById('cPhone').value,
        NationalId: document.getElementById('cNationalId').value, 
        ServiceType: document.getElementById('cService').value,
        ActualCost: parseFloat(document.getElementById('cCost').value) || 0,
        TotalDue: parseFloat(document.getElementById('cTotal').value) || 0,
        PaidAmount: parseFloat(document.getElementById('cPaid').value) || 0,
        NumberOfInstallments: parseInt(document.getElementById('cNumInstall').value) || 0,
        PaidInstallments: parseInt(document.getElementById('cPaidInstall').value) || 0,
        PaymentStatus: document.getElementById('cStatus').value,
        Notes: document.getElementById('cNotes').value || ""
    };

    try {
        const res = await fetch(id ? `${API_URL}/api/clients/${id}` : `${API_URL}/api/clients`, {
            method: id ? 'PUT' : 'POST',
            headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
            body: JSON.stringify(clientData)
        });

        if (res.ok) {
            bootstrap.Modal.getInstance(document.getElementById('clientModal')).hide();
            loadClients();
            alert("تم حفظ البيانات بنجاح ✅");
        } else {
            alert("حدث خطأ أثناء الحفظ");
        }
    } catch (err) {
        alert("خطأ في الاتصال");
    }
}

window.resetForm = () => {
    document.getElementById('clientForm').reset();
    document.getElementById('editClientId').value = '';
};

window.fillEditForm = (client) => {
    document.getElementById('editClientId').value = client.id || client.Id || '';
    document.getElementById('cName').value = client.clientName;
    document.getElementById('cPhone').value = client.phone;
    document.getElementById('cNationalId').value = client.nationalId;
    document.getElementById('cService').value = client.serviceType;
    document.getElementById('cCost').value = client.actualCost;
    document.getElementById('cTotal').value = client.totalDue;
    document.getElementById('cPaid').value = client.paidAmount;
    document.getElementById('cNumInstall').value = client.numberOfInstallments;
    document.getElementById('cPaidInstall').value = client.paidInstallments;
    document.getElementById('cStatus').value = client.paymentStatus;
    document.getElementById('cNotes').value = client.notes;
    new bootstrap.Modal(document.getElementById('clientModal')).show();
};