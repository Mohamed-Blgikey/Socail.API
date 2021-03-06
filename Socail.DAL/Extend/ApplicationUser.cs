using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Socail.DAL.Entity;

namespace Socail.DAL.Extend
{
    public class ApplicationUser:IdentityUser
    {
        public ApplicationUser()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string PhotoName { get; set; }
        [Range(0,1)]
        public int Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string? Introduction { get; set; }
        public string? LookingFor { get; set; }
        public string? Interests { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        public IEnumerable<Photo> Photos { get; set; }
        public IEnumerable<Like> Likers { get; set; }
        public IEnumerable<Like> Likees { get; set; }
        public IEnumerable<Message> MessageSent { get; set; }
        public IEnumerable<Message> MessageRecive { get; set; }

        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}
