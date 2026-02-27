var API_URL = "https://localhost:7024"; 

document.addEventListener('DOMContentLoaded', () => {
    loadClients();
    const cForm = document.getElementById('clientForm');
    if (cForm) cForm.onsubmit = saveClient;
});

// دالة جلب البيانات من السيرفر
async function loadClients() {
    const tableBody = document.getElementById('clientsTable');
    if (!tableBody) return;

    try {
        const token = localStorage.getItem('token');
        const res = await fetch(`${API_URL}/api/clients`, { 
            headers: { 'Authorization': `Bearer ${token}`, 'Accept': 'application/json' } 
        });
        
        const data = await res.json();
        const clientsArray = Array.isArray(data) ? data : (data.items || data.data || []); 

        tableBody.innerHTML = clientsArray.map(client => {
            // التعامل مع القيم المالية والربح
            const profit = client.profit || 0;
            const lastDate = client.lastPaymentDate ? new Date(client.lastPaymentDate).toLocaleDateString('ar-EG') : '---';
            
            return `
            <tr>
                <td class="fw-bold">${client.clientName}</td>
                <td>${client.phone}</td>
                <td class="small text-muted">${client.nationalId}</td>
                <td><span class="badge bg-light text-primary border">${client.serviceType}</span></td>
                <td>${(client.actualCost || 0).toLocaleString()}</td>
                <td>${(client.totalDue || 0).toLocaleString()}</td>
                <td class="text-primary">${(client.paidAmount || 0).toLocaleString()}</td>
                <td class="text-danger fw-bold">${(client.remainingAmount || 0).toLocaleString()}</td>
                <td class="${profit >= 0 ? 'profit-pos' : 'profit-neg'}">${profit.toLocaleString()}</td>
                <td>
                    <span class="badge ${client.paymentStatus === 'مدفوع' ? 'bg-success' : 'bg-warning text-dark'}">
                        ${client.paymentStatus}
                    </span>
                </td>
                <td>${client.paidInstallments} / ${client.numberOfInstallments}</td>
                <td class="small">${client.createdBy || '---'}</td>
                <td class="small">${lastDate}</td>
                <td class="small text-muted text-truncate" style="max-width: 150px" title="${client.notes}">${client.notes || '---'}</td>
                <td class="text-center">
                    <button class="btn btn-sm btn-outline-info" onclick='fillEditForm(${JSON.stringify(client)})'>
                        <i class="fas fa-edit"></i>
                    </button>
                </td>
            </tr>`;
        }).join('');

    } catch (err) { 
        tableBody.innerHTML = '<tr><td colspan="15" class="text-center text-danger">تعذر الاتصال بالسيرفر</td></tr>';
    }
}

// دالة البحث الشامل الذكي
function filterClients() {
    const filter = document.getElementById('searchInput').value.toLowerCase();
    const rows = document.querySelectorAll('#clientsTable tr');
    rows.forEach(row => {
        const text = row.innerText.toLowerCase();
        row.style.display = text.includes(filter) ? "" : "none";
    });
}

// دالة الحفظ
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