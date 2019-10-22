using System;
using System.ComponentModel.DataAnnotations;

namespace Hackathon.Models
{
    public class Ad
    {
        public Guid Id { get; set; }
        [Required]
        public int Number { get; set; }
        public User User { get; set; }
        public Guid UserId{ get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        [Range(minimum:1,maximum:10)]
        public byte Rating { get; set; }
        public byte[] Image { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
    }
}
