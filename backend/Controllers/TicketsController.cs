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
            [FromQuery] TicketPriority? priority)
        {
            var query = _context.Tickets
                .Include(t => t.Creator)
                .Include(t => t.AssignedUser)
                .AsQueryable();

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