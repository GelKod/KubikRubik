const API = "http://localhost:5084";

async function login() {
    const res = await fetch(API + "/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            username: username.value,
            password: password.value
        })
    });

    if (!res.ok) {
        result.textContent = "Invalid credentials";
        return;
    }

    const data = await res.json();

    // сохраняем данные пользователя
    localStorage.setItem("userId", data.userId);

    // переход на основную страницу
    window.location.href = "index.html";
}