
namespace Server.Models.View
{
    using System.ComponentModel.DataAnnotations;

    public class RecoveryModel
    {
        [Required]
        [StringLength(50, MinimumLength=1)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}