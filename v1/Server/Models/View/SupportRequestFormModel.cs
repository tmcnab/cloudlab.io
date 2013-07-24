
namespace Server.Models.View
{
    using System.ComponentModel.DataAnnotations;

    public class SupportRequestFormModel
    {
        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [StringLength(4000)]
        public string Description { get; set; }
    }
}