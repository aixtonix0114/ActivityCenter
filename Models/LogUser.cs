using System;
using System.ComponentModel.DataAnnotations;

namespace ActivityCenter.Models
{
    public class LogUser
    {
        [Required(ErrorMessage="Email is required")]
        [EmailAddress]
        public string LEmail {get;set;}
        
        [Required(ErrorMessage="Password is required")]
        [DataType(DataType.Password)]
        public string LPassword {get;set;}
    }
}