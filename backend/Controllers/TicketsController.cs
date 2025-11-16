using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IssueTrackingDS.Data;
using IssueTrackingDS.Models;
using IssueTrackingDS.Models.DTOs;

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

        // GET: api/tickets
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

            // Alleen eigen tickets voor gewone users
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

            // Projecteer naar DTO om circulaire referentie te vermijden
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var tickets = await query
                .Select(t => new TicketDTO
                {
                    TicketID = t.TicketID,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString(),
                    Creator = new UserDTO
                    {
                        UserID = t.Creator.UserID,
                        Username = t.Creator.Username
                    },
                    AssignedUser = t.AssignedUser == null ? null : new UserDTO
                    {
                        UserID = t.AssignedUser.UserID,
                        Username = t.AssignedUser.Username
                    }
                })
                .ToListAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return Ok(tickets);
        }

        // GET: api/tickets/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(int id)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var ticket = await _context.Tickets
                .Include(t => t.Creator)
                .Include(t => t.AssignedUser)
                .Where(t => t.TicketID == id)
                .Select(t => new TicketDTO
                {
                    TicketID = t.TicketID,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString(),
                    Creator = new UserDTO
                    {
                        UserID = t.Creator.UserID,
                        Username = t.Creator.Username
                    },
                    AssignedUser = t.AssignedUser == null ? null : new UserDTO
                    {
                        UserID = t.AssignedUser.UserID,
                        Username = t.AssignedUser.Username
                    }
                })
                .FirstOrDefaultAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

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

        // DELETE: api/tickets/5
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