using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityCenter.Models
{
    public class Activiti
    {
        [Key]
        public int ActivitiId {get;set;}

        [Required(ErrorMessage="Title is required")]
        public string Title {get;set;}

        [Required(ErrorMessage="Time has to be FUTURE!!")]
        [DataType(DataType.Time)]
        public DateTime Time {get;set;}

        [Required(ErrorMessage="Date has to be FUTURE!!")]
        [DataType(DataType.Date)]
        public DateTime Date {get;set;}

        [Required(ErrorMessage="Duration is required!!")]
        public int Duration {get;set;}
        public string DurationUnit {get;set;}

        [Required(ErrorMessage="Description is required!!")]
        public string Description {get;set;}



        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;


        public int UserId {get;set;}
        public List<Participant> ActivitiParticipant {get;set;}
        public User Coordinator {get;set;}
    }
}