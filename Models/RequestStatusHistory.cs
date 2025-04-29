using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Models
{
    public class RequestStatusHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RequestId { get; set; }

        [ForeignKey("RequestId")]
        public RequestDetails Request { get; set; } = null!;

        [Required]
        public string UpdatedBy { get; set; } = null!; // Username of the user who updated

        [Required]
        public string NewStatus { get; set; } = null!;

        [Required]
        public string Remarks { get; set; } = null!;

        [Required]
        public DateTime Timestamp { get; set; }
    }
}
