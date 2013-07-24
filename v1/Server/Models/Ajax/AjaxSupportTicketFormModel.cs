
namespace Server.Models.Ajax
{
    using System.ComponentModel.DataAnnotations;

    public class AjaxSupportTicketFormModel
    {
        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [StringLength(4000)]
        public string Description { get; set; }
    }
}