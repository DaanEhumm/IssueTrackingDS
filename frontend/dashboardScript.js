const { username, role, userID } = loadUserInfo();
document.getElementById("userDisplay").innerText = `${username} (${role})`;

document.getElementById("logoutBtn").addEventListener("click", logout);
document.getElementById("filterBtn").addEventListener("click", loadTickets);
document.getElementById("addTicketBtn").addEventListener("click", () => window.location.href="ticket.html");

function badgeStatus(status){
    let cls = "bg-secondary";
    if(status === "Open") cls = "bg-success";
    if(status === "InProgress") cls = "bg-warning text-dark";
    if(status === "Closed") cls = "bg-danger";
    return `<span class="badge ${cls}">${status}</span>`;
}

function badgePriority(priority){
    let cls = "bg-secondary";
    if(priority === "Low") cls = "bg-success";
    if(priority === "Medium") cls = "bg-warning text-dark";
    if(priority === "High") cls = "bg-danger";
    return `<span class="badge ${cls}">${priority}</span>`;
}

async function loadTickets() {
    const status = document.getElementById("statusFilter").value;
    const priority = document.getElementById("priorityFilter").value;

    let url = `${apiTickets}?userRole=${role}&userId=${userID}`;
    if(status) url += `&status=${status}`;
    if(priority) url += `&priority=${priority}`;

    const res = await fetch(url);
    const tickets = await res.json();

    const table = document.getElementById("ticketTable");
    table.innerHTML = "";

    tickets.forEach(t => {
        table.innerHTML += `
            <tr>
                <td>${t.ticketID}</td>
                <td>${t.title}</td>
                <td>${badgeStatus(t.status)}</td>
                <td>${badgePriority(t.priority)}</td>
                <td>${t.creator?.username ?? "-"}</td>
                <td>
                    <button class="btn btn-sm btn-primary me-1" onclick="editTicket(${t.ticketID})">Open</button>
                    ${role==='admin'? `<button class="btn btn-sm btn-danger" onclick="deleteTicket(${t.ticketID})">🗑️</button>` : ''}
                </td>
            </tr>
        `;
    });
}

function editTicket(id){
    window.location.href = `ticket.html?id=${id}`;
}

async function deleteTicket(id){
    if(!confirm("Weet je zeker dat je dit ticket wilt verwijderen?")) return;

    await fetch(`${apiTickets}/${id}?isAdmin=${role==='admin'}`, { method: "DELETE" });
    loadTickets();
}

document.addEventListener("DOMContentLoaded", () => {
    const addTicketBtn = document.getElementById("loginBtn");
    const filterBtn = document.getElementById("registerBtn");

    if (addTicketBtn) loginBtn.addEventListener("click", login);
    if (filterBtn) loginBtn.addEventListener("click", login);
});


loadTickets();