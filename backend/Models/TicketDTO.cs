using System;

namespace IssueTrackingDS.Models.DTOs
{
    public class TicketDTO
    {
        public int TicketID { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public UserDTO? Creator { get; set; }
        public UserDTO? AssignedUser { get; set; }
    }

    public class UserDTO
    {
        public int UserID { get; set; }
        public string? Username { get; set; }
    }
}