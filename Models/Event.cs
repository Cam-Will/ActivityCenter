using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityCenter.Models
{
    public class Event
    {
        [Key]
        public int EventId {get;set;}

        [Required]
        [MinLength(2)]
        [Display(Name = "Title:")]
        public string Name {get;set;}

        [Required]
        [Display(Name = "Date:")]
        public DateTime EventDate {get;set;}

        [Required]
        [Display(Name = "Duration:")]
        public string Duration {get;set;}

        [Required]
        [Display(Name = "Description:")]
        public string Description {get;set;}

        [Required]
        public int UserId {get;set;}

        public User Coordinator {get;set;}

        public List<Participation> Participators {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}