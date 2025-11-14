using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IssueTrackingDS.Data;
using IssueTrackingDS.Models;

namespace IssueTrackingDS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TicketsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/tickets (dashboard + filter)
        [HttpGet]
        public async Task<IActionResult> GetTickets(
        [FromQuery] TicketStatus? status,
        [FromQuery] TicketPriority? priority,
        [FromQuery] string? userRole,
        [FromQuery] int? userId)
        {
        var query = _context.Tickets
        .Include(t => t.Creator)
        .Include(t => t.AssignedUser)
        .AsQueryable();

        // ADMIN ziet alles
        if (userRole != "admin")
        {
        if (userId == null)
            return BadRequest("userId is verplicht voor gewone gebruikers.");

        query = query.Where(t => t.CreatedBy == userId);
        }

        if (status.HasValue)
        query = query.Where(t => t.Status == status.Value);

        if (priority.HasValue)
        query = query.Where(t => t.Priority == priority.Value);

        return Ok(await query.ToListAsync());
        }

        // GET: api/tickets/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Creator)
                .Include(t => t.AssignedUser)
                .FirstOrDefaultAsync(t => t.TicketID == id);

            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }

        // POST: api/tickets
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] Ticket ticket)
        {
            if (!ModelState.IsValid)
            return BadRequest(ModelState);

        ticket.CreatedAt = DateTime.Now;
        ticket.UpdatedAt = DateTime.Now;

        _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return Ok(ticket);
        }

        // PUT: api/tickets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] Ticket ticket)
        {
            if (!ModelState.IsValid)
            return BadRequest(ModelState);

            var existing = await _context.Tickets.FindAsync(id);
            if (existing == null)
            return NotFound();

        existing.Title = ticket.Title;
        existing.Description = ticket.Description;
        existing.Status = ticket.Status;
        existing.Priority = ticket.Priority;
        existing.AssignedTo = ticket.AssignedTo;
        existing.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }
       
        // PUT: api/tickets/5/priority   (alleen admin)
        [HttpPut("{id}/priority")]
        public async Task<IActionResult> UpdatePriority(
        int id,
        [FromQuery] string userRole,
        [FromBody] TicketPriority priority)
        {       
        if (userRole != "admin")
        return Forbid();

        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
        return NotFound();

        ticket.Priority = priority;
        ticket.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return Ok(ticket);
        }

        // PUT: api/tickets/5/assign/2
        [HttpPut("{id}/assign/{userId}")]
        public async Task<IActionResult> AssignTicket(
        int id,
        int userId,
        [FromQuery] string userRole)
        {
        if (userRole != "admin")
        return Forbid();

        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
        return NotFound();

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        return NotFound($"User {userId} bestaat niet.");

        ticket.AssignedTo = userId;
        ticket.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return Ok(ticket);
        }

        // DELETE: api/tickets/5 (alleen admin)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id, [FromQuery] bool isAdmin = false)
        {
            if (!isAdmin)
                return Forbid();

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}