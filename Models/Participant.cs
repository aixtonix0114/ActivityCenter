using System;
using System.ComponentModel.DataAnnotations;

namespace ActivityCenter.Models
{
    public class Participant
    {
        [Key]
        public int ParticipantId {get;set;}
        public int UserId {get;set;}
        public int ActivitiId {get;set;}
        public User User {get;set;}
        public Activiti Activiti {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}