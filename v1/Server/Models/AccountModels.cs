
namespace Server.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Web.Mvc;
    using System.Web.Security;

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class NewRegisterModel
    {
        [Required]
        [Display(Name = "Username")]
        [StringLength(50, ErrorMessage = "Must be between {2} and {0} characters long", MinimumLength = 2)]
        public string RUserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string REmail { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "Username")]
        [StringLength(50, ErrorMessage="Must be between {2} and {0} characters long", MinimumLength=2)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Must be between {2} and {0} characters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name="Secret Question")]
        [StringLength(200, ErrorMessage="Must be less than {0} characters long")]
        public string Question { get; set; }

        [Display(Name = "Secret Answer")]
        [StringLength(100, ErrorMessage = "Must be less than {0} characters long")]
        public string Answer { get; set; }
    }

    public class ProfileModel
    {
        [Display(Name="First Name")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; }

        public bool UseGravatar { get; set; }

        [Display(Name="Allow Contact")]
        public bool AllowContact { get; set; }
    }

    public enum CoverageLevel : byte
    {
        Free = 1,
        Hacker = 2,
        Developer = 3,
        Provider = 4
    }
}
