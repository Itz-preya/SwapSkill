// Tab switching for login/register
const tabButtons = document.querySelectorAll('.auth-tabs button');
if (tabButtons.length) {
    tabButtons.forEach(btn => {
        btn.addEventListener('click', function() {
            tabButtons.forEach(b => b.classList.remove('active'));
            this.classList.add('active');
            if (this.dataset.tab === 'login') {
                document.querySelector('.auth-form.login').style.display = 'block';
                document.querySelector('.auth-form.register').style.display = 'none';
            } else {
                document.querySelector('.auth-form.login').style.display = 'none';
                document.querySelector('.auth-form.register').style.display = 'block';
            }
        });
    });
}
