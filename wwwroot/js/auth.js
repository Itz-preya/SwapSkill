// Tab switching for login/register
const tabButtons = document.querySelectorAll('.auth-tabs button');
if (tabButtons.length) {
    const loginForm = document.querySelector('.auth-form.login');
    const registerForm = document.querySelector('.auth-form.register');

    // Set active by current path
    const path = (window.location.pathname || '').toLowerCase();
    tabButtons.forEach(b => b.classList.remove('active'));
    const activeTab = path.includes('/register') ? 'register' : 'login';
    const activeBtn = Array.from(tabButtons).find(b => b.dataset.tab === activeTab);
    if (activeBtn) activeBtn.classList.add('active');

    tabButtons.forEach(btn => {
        btn.addEventListener('click', function() {
            // If page uses navigation (onclick), allow it; this is a safety toggle for SPA versions
            tabButtons.forEach(b => b.classList.remove('active'));
            this.classList.add('active');

            if (this.dataset.tab === 'login') {
                if (loginForm) loginForm.style.display = 'block';
                if (registerForm) registerForm.style.display = 'none';
            } else {
                if (loginForm) loginForm.style.display = 'none';
                if (registerForm) registerForm.style.display = 'block';
            }
        });
    });
}
