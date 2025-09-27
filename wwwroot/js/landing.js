// Navigation: smooth scroll and active link highlight
const navLinks = document.querySelectorAll('.nav-link');
navLinks.forEach(link => {
    link.addEventListener('click', function(e) {
        if (this.hash) {
            e.preventDefault();
            document.querySelector(this.hash).scrollIntoView({ behavior: 'smooth' });
            navLinks.forEach(l => l.classList.remove('active'));
            this.classList.add('active');
        }
    });
});

// Highlight nav link on scroll
window.addEventListener('scroll', () => {
    let fromTop = window.scrollY + 80;
    navLinks.forEach(link => {
        if (link.hash) {
            const section = document.querySelector(link.hash);
            if (section && section.offsetTop <= fromTop && section.offsetTop + section.offsetHeight > fromTop) {
                navLinks.forEach(l => l.classList.remove('active'));
                link.classList.add('active');
            }
        }
    });
});

document.getElementById('loginBtn').onclick = function() {
    window.location.href = '/Account/Login';
};
document.getElementById('signupBtn').onclick = function() {
    window.location.href = '/Account/Register';
};

// Add shadow to navbar on scroll
const navbar = document.querySelector('.navbar');
window.addEventListener('scroll', () => {
    if (window.scrollY > 10) {
        navbar.style.boxShadow = '0 4px 24px rgba(0,180,255,0.10)';
    } else {
        navbar.style.boxShadow = '0 2px 12px rgba(0,0,0,0.08)';
    }
});

// Fade-in animation for sections
const faders = document.querySelectorAll('section, .feature-cards .card, .popular-card');
const appearOptions = {
    threshold: 0.1,
    rootMargin: '0px 0px -50px 0px'
};
const appearOnScroll = new IntersectionObserver(function(entries, appearOnScroll) {
    entries.forEach(entry => {
        if (!entry.isIntersecting) return;
        entry.target.classList.add('fade-in');
        appearOnScroll.unobserve(entry.target);
    });
}, appearOptions);
faders.forEach(fader => {
    fader.classList.add('fade-init');
    appearOnScroll.observe(fader);
});

// Scroll-to-top button
const scrollBtn = document.createElement('button');
scrollBtn.innerHTML = '↑';
scrollBtn.style.position = 'fixed';
scrollBtn.style.bottom = '32px';
scrollBtn.style.right = '32px';
scrollBtn.style.background = '#00b4ff';
scrollBtn.style.color = '#fff';
scrollBtn.style.border = 'none';
scrollBtn.style.borderRadius = '50%';
scrollBtn.style.width = '48px';
scrollBtn.style.height = '48px';
scrollBtn.style.fontSize = '2rem';
scrollBtn.style.cursor = 'pointer';
scrollBtn.style.boxShadow = '0 4px 16px rgba(0,180,255,0.18)';
scrollBtn.style.display = 'none';
scrollBtn.style.zIndex = '200';
document.body.appendChild(scrollBtn);
window.addEventListener('scroll', () => {
    if (window.scrollY > 300) {
        scrollBtn.style.display = 'block';
    } else {
        scrollBtn.style.display = 'none';
    }
});
scrollBtn.onclick = () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
};
