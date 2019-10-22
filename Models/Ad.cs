using System;

namespace Hackathon.Models
{
    public class Ad
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public User User { get; set; }
        public Guid UserId{ get; set; }
        public string Content { get; set; }
        public byte Rating { get; set; }
        public byte[] Image { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
