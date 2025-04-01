using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AS1_HE180034_PRN222.Models;

public partial class SystemAccount
{
    public short AccountId { get; set; }

    [Required(ErrorMessage = "Please enter User Name !")]
    [StringLength(30, ErrorMessage = "User Name dot not exceed 30 characters")]
    public string? AccountName { get; set; }

    [Required(ErrorMessage = "Please enter Email !")]
    [EmailAddress(ErrorMessage = "Invalid Email")]
    public string? AccountEmail { get; set; }

    [Required(ErrorMessage = "Please enter Role !")]
    [Range(1, 2, ErrorMessage = "Invalid Role (1: Staff, 2: Lecturer")]
    public int? AccountRole { get; set; }

    [DataType(DataType.Password)]
    public string? AccountPassword { get; set; }

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
