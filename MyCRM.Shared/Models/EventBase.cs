using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCRM.Shared.Models
{
    public abstract class EventBase
    {
        public Guid Id { get; set; }

        [StringLength(30)]
        public string Summary { get; set; }

        public string Note { get; set; }
        public string Location { get; set; }
        public DateTime EventStartDateTime { get; set; }

        [NotMapped]
        public bool IsOverdue => EventStartDateTime < DateTime.Now;

        public int DurationMinutes { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompleteTime { get; set; }
    }
}