using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Hackathon.ViewModels
{
    public class AdViewModel
    {
        [Required]
        public int Number { get; set; }  
        public Guid UserId { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        [Range(minimum: 1, maximum: 10)]
        public byte Rating { get; set; }
        public IFormFile Image { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
    }
}
