using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socail.BL.Dtos
{
    public class RegisterDTO
    {
        public RegisterDTO()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
        [EmailAddress]
        public string Email { get; set; }
        [MinLength(6)]
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        [Range(0,1)]
        public int Gender { get; set; }
        //public int Gender { get; set; }
        //public DateTime DateOfBirth { get; set; }

        //public string Introduction { get; set; }
        //public string LookingFor { get; set; }
        //public string Interests { get; set; }
        //public string City { get; set; }
        //public string Country { get; set; }
    }
}
