using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityCenter.Models
{
    public class LoginUser
    {
        public string logEmail {get; set;}
        public string logPassword { get; set; }
    }

}
