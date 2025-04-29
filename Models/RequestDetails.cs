using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Models
{
    public class RequestDetails
    {
        [Key]
        public int RequestId { get; set; }

        [Required]
        public string RequestNumber { get; set; } = null!;

        [Required]
        public string RequestType { get; set; } = null!;
        
        [Required]
        [MaxLength(100)]
        public string Subject { get; set; } = null!;
        
        [Required]
        public string Details { get; set; } = null!;
        
        public string Username { get; set; } = null!;
        public DateTime DateOfIssue { get; set; }
        public string CurrentStatus { get; set; } = null!;
        
        public UserDetails User { get; set; } = null!;
        public ICollection<RequestStatusHistory> StatusHistory { get; set; } = new List<RequestStatusHistory>();
    }
}
