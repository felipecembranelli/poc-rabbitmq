using System.ComponentModel.DataAnnotations;

namespace APIMensagens.Models
{
    public class MessageContent
    {
        [Required]
        public string Message { get; set; }
        public string Source { get; set; }
        public string Severity { get; set;}
    }
}