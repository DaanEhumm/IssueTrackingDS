const apiUsers = "http://localhost:5204/api/users";
const apiTickets = "http://localhost:5204/api/tickets";

// --- Auth helpers ---
async function login() {
    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value.trim();
    const message = document.getElementById("message");

    message.innerText = "";
    if (!username || !password) {
        message.innerText = "Vul alle velden in!";
        return;
    }

    try {
        const res = await fetch(`${apiUsers}/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password })
        });

        const data = await res.json().catch(()=>null);
        if (!res.ok) {
            message.innerText = (data && data.message) ? data.message : "Login mislukt";
            return;
        }

        // Verwachte response: { userID, username, role }
        localStorage.setItem("userID", data.userID);
        localStorage.setItem("username", data.username);
        localStorage.setItem("role", data.role);

        window.location.href = "dashboard.html";
    } catch (err) {
        message.innerText = "Geen verbinding met server.";
        console.error(err);
    }
}

// Register (ALTIJD user)
async function register() {
    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value.trim();
    const message = document.getElementById("message");

    message.innerText = "";
    if (!username || !password) {
        message.innerText = "Vul alle velden in!";
        return;
    }

    try {
        // role hardcoded to "user"
        const res = await fetch(`${apiUsers}/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password, role: "user" })
        });

        const data = await res.json().catch(()=>null);
        if (!res.ok) {
            message.innerText = (data && data.message) ? data.message : "Registratie mislukt";
            return;
        }

        message.style.color = "green";
        message.innerText = "Account aangemaakt — log in!";
        // switch back to login view
        showLogin();
    } catch (err) {
        message.innerText = "Geen verbinding met server.";
        console.error(err);
    }
}

// Returns user info or redirects to index.html if missing
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

function logout() {
    localStorage.clear();
    window.location.href = "index.html";
}

// UI toggles for login/register on index
function showRegister() {
    document.getElementById("authTitle").innerText = "Registreren";
    document.getElementById("loginBtn").classList.add("d-none");
    document.getElementById("registerBtn").classList.remove("d-none");
    document.getElementById("switchText").innerText = "Heb je al een account?";
    document.getElementById("switchBtn").innerText = "Log in";
    document.getElementById("message").innerText = "";
    document.getElementById("message").style.color = "red";
}

function showLogin() {
    document.getElementById("authTitle").innerText = "Inloggen";
    document.getElementById("loginBtn").classList.remove("d-none");
    document.getElementById("registerBtn").classList.add("d-none");
    document.getElementById("switchText").innerText = "Nog geen account?";
    document.getElementById("switchBtn").innerText = "Maak er één aan";
    document.getElementById("message").innerText = "";
    document.getElementById("message").style.color = "red";
}

// Wire events
document.addEventListener("DOMContentLoaded", () => {
    const loginBtn = document.getElementById("loginBtn");
    const registerBtn = document.getElementById("registerBtn");
    const switchBtn = document.getElementById("switchBtn");
    const toLogin = document.getElementById("toLogin");
    const toRegister = document.getElementById("toRegister");

    if (loginBtn) loginBtn.addEventListener("click", login);
    if (registerBtn) registerBtn.addEventListener("click", register);
    if (switchBtn) switchBtn.addEventListener("click", () => {
        // toggle
        if (document.getElementById("registerBtn").classList.contains("d-none")) showRegister();
        else showLogin();
    });
    if (toLogin) toLogin.addEventListener("click", () => { showLogin(); window.scrollTo({top: document.querySelector('#authCard').offsetTop, behavior:'smooth'}); });
    if (toRegister) toRegister.addEventListener("click", () => { showRegister(); window.scrollTo({top: document.querySelector('#authCard').offsetTop, behavior:'smooth'}); });
});