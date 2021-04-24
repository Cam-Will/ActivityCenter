using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityCenter.Models
{
    public class Participation
    {
        [Key]
        public int ParticipationId {get;set;}

        [Required]
        public int UserId {get;set;}

        [Required]
        public int EventId {get;set;}

        public User User {get;set;}

        public Event Event {get;set;}

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}