using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace MvcProject.Models
{
    [Index("AbsolutePath",IsUnique =false,Name ="IDX_Files")]
    [Table("Files")]
    public class FileReference
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [Column("Path")]
        public string AbsolutePath { get; set; }
        public User? User { get; set; }
        public long? UserId { get; set; }
    }
}
