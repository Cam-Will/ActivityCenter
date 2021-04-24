using System.Data;
using ActivityCenter.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ActivityCenter.Models
{ 
    // the MyContext class representing a session with our MySQL 
    // database allowing us to query for or save data
    public class Context : DbContext 
    { 
        public Context(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Participation> Participants { get; set; }
    }
}