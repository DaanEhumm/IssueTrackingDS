const userInfo = loadUserInfo();
if (!userInfo) {
    // redirect done in loadUserInfo
}
const { username, role, userID } = userInfo;
const params = new URLSearchParams(window.location.search);
const ticketId = params.get("id");

const assignedCol = document.getElementById("assignedCol");
if (role === "admin") {
    assignedCol.style.display = "block";
    loadUsers();
} else {
    assignedCol.style.display = "none";
}

document.getElementById("saveTicketBtn").addEventListener("click", saveTicket);
document.getElementById("cancelBtn").addEventListener("click", () => window.location.href = "dashboard.html");
document.getElementById("deleteBtn").addEventListener("click", async () => {
    if(!confirm("Weet je zeker dat je dit ticket wilt verwijderen?")) return;
    try {
        const res = await fetch(`${apiTickets}/${ticketId}`, {
            method: "DELETE",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ isAdmin: role === 'admin' })
        });
        if (res.ok) window.location.href = "dashboard.html";
        else alert("Kon ticket niet verwijderen");
    } catch (err) { console.error(err); alert("Fout bij verwijderen"); }
});

async function loadUsers() {
    try {
        const res = await fetch(`${apiUsers}?userRole=${encodeURIComponent(role)}`);
        if (!res.ok) return;
        const users = await res.json();
        const sel = document.getElementById("assignedTo");
        sel.innerHTML = '<option value="">- Geen -</option>';
        users.forEach(u => sel.innerHTML += `<option value="${u.userID}">${u.username}</option>`);
    } catch (err) { console.error(err); }
}

async function loadTicket(){
    if(!ticketId) return;

    try {
        const res = await fetch(`${apiTickets}/${ticketId}`);
        if (!res.ok) {
            console.error("Kan ticket niet laden");
            return;
        }
        const t = await res.json();

        document.getElementById("ticketTitle").innerText = `Ticket #${t.ticketID}: ${t.title}`;
        document.getElementById("title").value = t.title;
        document.getElementById("description").value = t.description;
        document.getElementById("status").value = t.status;
        document.getElementById("priority").value = t.priority;
        if (t.assignedTo) document.getElementById("assignedTo").value = t.assignedTo;

        // show delete button only for admins
        if (role === "admin") document.getElementById("deleteBtn").style.display = "inline-block";
    } catch (err) {
        console.error(err);
    }
}

async function saveTicket(){
    const ticket = {
        title: document.getElementById("title").value.trim(),
        description: document.getElementById("description").value.trim(),
        status: document.getElementById("status").value,
        priority: document.getElementById("priority").value,
        createdBy: parseInt(userID),
        assignedTo: document.getElementById("assignedTo")?.value || null
    };

    if(!ticket.title || !ticket.description){
        alert("Titel en beschrijving zijn verplicht");
        return;
    }

    try {
        let res;
        if(ticketId){
            res = await fetch(`${apiTickets}/${ticketId}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(ticket)
            });
        } else {
            res = await fetch(apiTickets, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(ticket)
            });
        }

        if(res.ok){
            alert("Ticket opgeslagen!");
            window.location.href = "dashboard.html";
        } else {
            const err = await res.json().catch(()=>null);
            alert(err?.message ?? "Fout bij opslaan");
        }
    } catch (err) {
        console.error(err);
        alert("Fout bij opslaan");
    }
}

loadTicket();