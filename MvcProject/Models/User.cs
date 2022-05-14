using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace MvcProject.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsAlive { get; set; }
        public byte[] Timestamp { get; set; }
        public ICollection<FileReference> References { get; set; }
    }
}
