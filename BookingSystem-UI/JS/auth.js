
var API_URL = "https://localhost:7024";

// --- وظيفة تسجيل الدخول المطورة ---
if (document.getElementById('loginForm')) {
    document.getElementById('loginForm').onsubmit = async (e) => {
        e.preventDefault();
        
        const loginData = {
            email: document.getElementById('loginEmail').value,
            password: document.getElementById('loginPassword').value
        };

        try {
            const res = await fetch(`${API_URL}/api/auth/login`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(loginData)
            });

            if (res.ok) {
                const data = await res.json();
                // حفظ التوكن والرتبة في المتصفح
                localStorage.setItem('token', data.token);
                localStorage.setItem('userRole', data.role); 
                
                alert("تم تسجيل الدخول بنجاح!");
                window.location.href = "../dashboard.html"; 
            } 
            else {
                // تعديل هنا لقراءة أخطاء الـ Validation من السيرفر
                const errorData = await res.json();
                
                if (errorData.errors) {
                    // لو فيه أخطاء من FluentValidation (مثل الإيميل فارغ)
                    let errorMessages = "";
                    for (let key in errorData.errors) {
                        errorMessages += `${errorData.errors[key][0]}\n`;
                    }
                    alert(errorMessages);
                } else {
                    // لو الخطأ كلمة مرور غلط أو حساب مش موجود
                    alert(errorData.message || "خطأ في الإيميل أو كلمة المرور");
                }
            }
        } catch (error) {
            console.error("Connection Error:", error);
            alert("فشل الاتصال بالسيرفر، تأكد أن الـ Backend يعمل!");
        }
    };
}

// --- وظيفة تسجيل حساب جديد ---
const registerForm = document.getElementById('registerForm');

if (registerForm) {
    console.log("النظام شايف الفورم وجاهز ✅");

    registerForm.onsubmit = async (e) => {
        e.preventDefault();
        
        // التنبيه الأول: عشان نتأكد إن الزرار شغال
        alert("جاري تجميع البيانات..."); 

        const regData = {
            fullName: document.getElementById('regName').value,
            email: document.getElementById('regEmail').value,
            password: document.getElementById('regPassword').value
        };

        console.log("البيانات اللي رايحة للباك:", regData);

        try {
            // التنبيه الثاني: قبل ما نكلم السيرفر
            alert("بنحاول نكلم السيرفر دلوقتي...");

            const res = await fetch(`${API_URL}/api/auth/register`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(regData)
            });

            if (res.ok) {
                alert("مبروك! تم إنشاء الحساب بنجاح 🎉");
                window.location.href = "Login.html";
            } else {
                const errorData = await res.json();
                console.error("خطأ من السيرفر:", errorData);
                alert("السيرفر رفض الطلب: " + (errorData.message || "تأكد من البيانات"));
            }
        } catch (error) {
            console.error("فشل الاتصال تماماً:", error);
            alert("مشكلة في الشبكة: السيرفر (Backend) مش شغال أو الرابط غلط!");
        }
    };
} else {
    console.error("تحذير: النظام مش شايف فورم باسم registerForm!");

}

