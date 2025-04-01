using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AS1_HE180034_PRN222.Models;

public partial class NewsArticle
{
    public string NewsArticleId { get; set; }= Ulid.NewUlid().ToString().Substring(2, 12);

    [Required(ErrorMessage = "News title cannot empty !!!")]
    [MaxLength(100, ErrorMessage = "New Title not exceed 100 characters")]
    public string? NewsTitle { get; set; }

    [Required(ErrorMessage = "Headline cannot empty !!!")]
    [MaxLength(200, ErrorMessage = "Headline do not exceed 200 characters")]
    public string Headline { get; set; } = null!;
    public DateTime? CreatedDate { get; set; }

    [Required(ErrorMessage = "News Content cannot empty !!!")]
    public string? NewsContent { get; set; }

    public string? NewsSource { get; set; }

    public short? CategoryId { get; set; }

    public bool? NewsStatus { get; set; }

    public short? CreatedById { get; set; }

    public short? UpdatedById { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Category? Category { get; set; }

    public virtual SystemAccount? CreatedBy { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public bool IsValidCreatedDate()
    {
        return CreatedDate == null || CreatedDate <= DateTime.Now;
    }
}
