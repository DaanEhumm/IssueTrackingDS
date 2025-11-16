// Haal gebruiker info op
const user = loadUserInfo();
if (!user) {
    // redirect gedaan in loadUserInfo()
} else {
    const { username, role, userID } = user;
    const userDisplay = document.getElementById("userDisplay");
    if (userDisplay) userDisplay.innerText = `${username} (${role})`;
}

// Buttons
document.getElementById("logoutBtn")?.addEventListener("click", logout);
document.getElementById("filterBtn")?.addEventListener("click", loadTickets);
document.getElementById("addTicketBtn")?.addEventListener("click", () => window.location.href = "ticket.html");

// Badge helpers
function badgeStatus(status){
    let cls = "bg-secondary text-white";
    if(status === "Open") cls = "bg-success text-white";
    if(status === "InProgress") cls = "bg-warning text-dark";
    if(status === "Closed") cls = "bg-danger text-white";
    return `<span class="badge ${cls}">${status}</span>`;
}

function badgePriority(priority){
    let cls = "bg-secondary text-white";
    if(priority === "Low") cls = "bg-success text-white";
    if(priority === "Medium") cls = "bg-warning text-dark";
    if(priority === "High") cls = "bg-danger text-white";
    return `<span class="badge ${cls}">${priority}</span>`;
}

// Kleine helper om XSS te voorkomen
function escapeHtml(unsafe) {
    if (!unsafe) return "";
    return unsafe.replaceAll("&", "&amp;").replaceAll("<", "&lt;").replaceAll(">", "&gt;");
}

// Mapt frontend filter naar enum voor backend
function mapStatusFilter(status) {
    if(status === "In Progress") return "InProgress";
    if(status === "Open" || status === "Closed") return status;
    return null; // geen filter
}

// Laad tickets
async function loadTickets() {
    const { role, userID } = loadUserInfo();
    if (!role) return;

    const statusRaw = document.getElementById("statusFilter").value;
    const priority = document.getElementById("priorityFilter").value;

    const status = mapStatusFilter(statusRaw);

    try {
        // Bouw URL
        let url = `${apiTickets}?userRole=${encodeURIComponent(role)}`;
        if (role !== "admin") {
            url += `&userId=${encodeURIComponent(userID)}`;
        }
        if (status) url += `&status=${encodeURIComponent(status)}`;
        if (priority) url += `&priority=${encodeURIComponent(priority)}`;

        console.log("Fetching tickets from URL:", url);

        const res = await fetch(url);
        if (!res.ok) {
            console.error("Kan tickets niet laden", res.status);
            return;
        }

        const tickets = await res.json();
        console.log("Tickets received:", tickets);

        // Check of het een array is
        if (!Array.isArray(tickets)) {
            console.error("Tickets is geen array:", tickets);
            return;
        }

        const table = document.getElementById("ticketTable");
        table.innerHTML = "";

        if (tickets.length === 0) {
            table.innerHTML = `<tr><td colspan="6" class="text-center text-muted">Geen tickets gevonden</td></tr>`;
            return;
        }

        tickets.forEach(t => {
            table.innerHTML += `
                <tr>
                    <td>${t.ticketID}</td>
                    <td>${escapeHtml(t.title)}</td>
                    <td>${badgeStatus(t.status)}</td>
                    <td>${badgePriority(t.priority)}</td>
                    <td>${t.creator?.username ?? "-"}</td>
                    <td>
                        <button class="btn btn-sm btn-primary me-1" onclick="openTicket(${t.ticketID})">Open</button>
                        ${role === 'admin' ? `<button class="btn btn-sm btn-danger" onclick="deleteTicket(${t.ticketID})">🗑️</button>` : ''}
                    </td>
                </tr>
            `;
        });

    } catch(err){
        console.error("Fout bij loadTickets:", err);
    }
}

// Open ticket
function openTicket(id){
    window.location.href = `ticket.html?id=${id}`;
}

// Delete ticket
async function deleteTicket(id){
    const { role } = loadUserInfo();
    if (!confirm("Weet je zeker dat je dit ticket wilt verwijderen?")) return;

    try {
        const res = await fetch(`${apiTickets}/${id}`, {
            method: "DELETE",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ isAdmin: role === 'admin' })
        });

        if(res.ok) loadTickets();
        else alert("Kon ticket niet verwijderen");
    } catch(err){
        console.error("Fout bij deleteTicket:", err);
        alert("Fout bij verwijderen");
    }
}

// Laad tickets direct bij openen
document.addEventListener("DOMContentLoaded", loadTickets);