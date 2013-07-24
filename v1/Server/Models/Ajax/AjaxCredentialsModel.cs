
namespace Server.Models.Ajax
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class AjaxCredentialsModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        public bool AllowContact { get; set; }

        public bool UseGravatar { get; set; }
    }
}