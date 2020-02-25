using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityCenter.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

        [Required(ErrorMessage="Name is required")]
        [MinLength(2, ErrorMessage="Name is required and atleast 2 characters.")]
        public string Name {get;set;}


        [Required(ErrorMessage="Email is required")]
        [EmailAddress]
        public string Email {get;set;}

        [Required(ErrorMessage="Password is required")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage="Password should be atleast 8 characters.")]
        [RegularExpression("^(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage="Password must contain at least 8 characters, at least 1 number and both lower and uppercase letters and specail characters.")]
        public string Password {get;set;}

        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string PWConfirm {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        public List<Participant> UserGuest {get;set;}
    }
}