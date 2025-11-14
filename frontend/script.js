const apiUsers = "http://localhost:5204/api/users";
const apiTickets = "http://localhost:5204/api/tickets";

// Login
async function login() {
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;
    const message = document.getElementById("message");

    if (!username || !password) {
        message.innerText = "Vul alles in!";
        return;
    }

    const res = await fetch(`${apiUsers}/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password })
    });

    const data = await res.json();

    if (!res.ok) {
        message.innerText = data || "Login mislukt";
        return;
    }

    localStorage.setItem("userID", data.userID);
    localStorage.setItem("username", data.username);
    localStorage.setItem("role", data.role);

    window.location.href = "dashboard.html";
}

// Register
async function register() {
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;
    const role = document.getElementById("role").value;
    const message = document.getElementById("message");

    if (!username || !password) {
        message.innerText = "Vul alles in!";
        return;
    }

    const res = await fetch(`${apiUsers}/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password, role })
    });

    const data = await res.json();
    if (!res.ok) {
        message.innerText = data || "Registratie mislukt";
        return;
    }

    message.style.color = "green";
    message.innerText = "Account aangemaakt, log in!";
}

// User info
function loadUserInfo() {
    const username = localStorage.getItem("username");
    const role = localStorage.getItem("role");
    const userID = localStorage.getItem("userID");

    if (!username || !role || !userID) {
        window.location.href = "index.html";
        return null;
    }

    return { username, role, userID };
}

// Logout
function logout() {
    localStorage.clear();
    window.location.href = "index.html";
}

document.addEventListener("DOMContentLoaded", () => {
    const loginBtn = document.getElementById("loginBtn");
    const registerBtn = document.getElementById("registerBtn");

    if (loginBtn) loginBtn.addEventListener("click", login);
    if (registerBtn) registerBtn.addEventListener("click", register);
});