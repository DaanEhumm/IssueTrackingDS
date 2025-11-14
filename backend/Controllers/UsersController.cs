using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IssueTrackingDS.Data;
using IssueTrackingDS.Models;
using System.Security.Cryptography;
using System.Text;

namespace IssueTrackingDS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // HASH FUNCTIE
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // ============================
        // REGISTER
        // ============================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest req)
        {
        if (await _context.Users.AnyAsync(u => u.Username == req.Username))
        return BadRequest("Username bestaat al.");

        // Alleen user-registratie toegestaan
        if (req.Role == UserRole.admin)
        return BadRequest("Registratie als admin is niet toegestaan.");

#pragma warning disable CS8604 // Possible null reference argument.
            var user = new User
        {
        Username = req.Username,
        PasswordHash = HashPassword(req.Password),
        Role = UserRole.user
        };
#pragma warning restore CS8604 // Possible null reference argument.

            _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
        message = "Account aangemaakt",
        user.UserID,
        user.Username,
        user.Role
        });
        }

        // ============================
        // LOGIN
        // ============================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest req)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == req.Username);

            if (user == null)
                return Unauthorized("Onbekende gebruiker.");

#pragma warning disable CS8604 // Possible null reference argument.
            if (user.PasswordHash != HashPassword(req.Password))
                return Unauthorized("Wachtwoord onjuist.");
#pragma warning restore CS8604 // Possible null reference argument.

            return Ok(new
            {
                message = "Ingelogd",
                user.UserID,
                user.Username,
                user.Role
            });
        }

        // ============================
        // GET ALL USERS (ADMIN)
        // ============================
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] string userRole)
        {
            if (userRole != "admin")
                return Forbid();

            return Ok(await _context.Users.ToListAsync());
        }

        // ============================
        // GET SINGLE USER
        // ============================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
    }

    // DTO classes
    public class UserRegisterRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public UserRole Role { get; set; }
    }

    public class UserLoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}