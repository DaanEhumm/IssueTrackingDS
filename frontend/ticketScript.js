const { username, role, userID } = loadUserInfo();
const params = new URLSearchParams(window.location.search);
const ticketId = params.get("id");

const assignedCol = document.getElementById("assignedCol");
if(role === "admin"){
    assignedCol.style.display = "block";
    loadUsers();
}

document.getElementById("saveTicketBtn").addEventListener("click", saveTicket);

async function loadUsers(){
    const res = await fetch(`${apiUsers}?userRole=${role}`);
    const users = await res.json();
    const sel = document.getElementById("assignedTo");
    sel.innerHTML = '<option value="">- Geen -</option>';
    users.forEach(u => sel.innerHTML += `<option value="${u.userID}">${u.username}</option>`);
}

async function loadTicket(){
    if(!ticketId) return;

    const res = await fetch(`${apiTickets}/${ticketId}`);
    const t = await res.json();

    document.getElementById("title").value = t.title;
    document.getElementById("description").value = t.description;
    document.getElementById("status").value = t.status;
    document.getElementById("priority").value = t.priority;
    if(t.assignedTo) document.getElementById("assignedTo").value = t.assignedTo;
}

async function saveTicket(){
    const ticket = {
        title: document.getElementById("title").value,
        description: document.getElementById("description").value,
        status: document.getElementById("status").value,
        priority: document.getElementById("priority").value,
        createdBy: parseInt(userID),
        assignedTo: document.getElementById("assignedTo").value || null
    };

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
        window.location.href="dashboard.html";
    } else alert("Fout bij opslaan");
}

loadTicket();