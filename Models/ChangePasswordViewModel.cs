using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AS1_HE180034_PRN222.Models;

public partial class ChangePasswordViewModel
{
    public short AccountId { get; set; }

    [Required(ErrorMessage = "Current password is required")]
    [Display(Name = "Current Password")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "New password is required")]
    [Display(Name = "New Password")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm password is required")]
    [Display(Name = "Confirm New Password")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}
