const API = "http://localhost:5084";

// Auth check for main page
if (window.location.pathname.endsWith('index.html') || window.location.pathname === '/' || window.location.pathname === '') {
    if (!localStorage.getItem('userId')) {
        // Only redirect if not on login page
        if (!window.location.pathname.endsWith('login.html')) {
            window.location.href = 'login.html';
        }
    }
}

function showNotification(message, type = 'error') {
    const container = document.getElementById('notification-container');
    if (!container) return;

    const notification = document.createElement('div');
    notification.className = `notification ${type}`;

    let icon = '⚠️';
    if (type === 'success') icon = '✅';
    if (type === 'info') icon = 'ℹ️';

    notification.innerHTML = `
        <span class="notification-icon">${icon}</span>
        <span class="notification-message">${message}</span>
    `;

    container.appendChild(notification);

    setTimeout(() => {
        notification.classList.add('fade-out');
        setTimeout(() => {
            notification.remove();
        }, 300);
    }, 3000);
}

async function handleLogin() {
    const usernameInput = document.getElementById('username');
    const passwordInput = document.getElementById('password');

    if (!usernameInput || !passwordInput) return;

    if (!usernameInput.value || !passwordInput.value) {
        showNotification("Пожалуйста, заполните все поля");
        return;
    }

    try {
        const res = await fetch(API + "/auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                username: usernameInput.value,
                password: passwordInput.value
            })
        });

        if (res.status === 401) {
            showNotification("Неверное имя пользователя или пароль");
            return;
        }

        if (!res.ok) {
            showNotification("Произошла ошибка при входе");
            return;
        }

        const data = await res.json();
        localStorage.setItem("userId", data.userId);
        window.location.href = "index.html";
    } catch (error) {
        console.error("Login error:", error);
        showNotification("Не удалось подключиться к серверу");
    }
}

function logout() {
    localStorage.removeItem('userId');
    window.location.href = 'login.html';
}

async function handleRegister() {
    const usernameInput = document.getElementById('reg-username');
    const passwordInput = document.getElementById('reg-password');
    const confirmInput = document.getElementById('reg-password-confirm');

    if (!usernameInput || !passwordInput || !confirmInput) return;

    if (!usernameInput.value || !passwordInput.value || !confirmInput.value) {
        showNotification("Пожалуйста, заполните все поля");
        return;
    }

    if (passwordInput.value !== confirmInput.value) {
        showNotification("Пароли не совпадают");
        return;
    }

    try {
        const res = await fetch(API + "/auth/register", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                username: usernameInput.value,
                password: passwordInput.value
            })
        });

        if (res.status === 409) {
            showNotification("Пользователь с таким именем уже существует");
            return;
        }

        if (!res.ok) {
            const errorText = await res.text();
            showNotification(errorText || "Ошибка при регистрации");
            return;
        }

        showNotification("Регистрация успешна! Теперь вы можете войти", "success");

        setTimeout(() => {
            if (typeof toggleForm === 'function') {
                toggleForm();
            }
        }, 1500);

    } catch (error) {
        console.error("Register error:", error);
        showNotification("Не удалось подключиться к серверу");
    }
}
