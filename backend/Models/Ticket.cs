using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueTrackingDS.Models
{
    public class Ticket
    {
        [Key]
        public int TicketID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [EnumDataType(typeof(TicketStatus))]
        public TicketStatus Status { get; set; } = TicketStatus.Open;

        [Required]
        [EnumDataType(typeof(TicketPriority))]
        public TicketPriority Priority { get; set; } = TicketPriority.Low;

        // Foreign Keys
        public int? AssignedTo { get; set; }
        [ForeignKey("AssignedTo")]
        public User AssignedUser { get; set; }

        public int CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User Creator { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public enum TicketStatus
    {
        Open,
        InProgress,
        Closed
    }

    public enum TicketPriority
    {
        Low,
        Medium,
        High
    }
}