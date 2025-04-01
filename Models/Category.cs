﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AS1_HE180034_PRN222.Models;

public partial class Category
{
    public short CategoryId { get; set; }

    [Required]
    [StringLength(50)]
    public string CategoryName { get; set; } = null!;

    [Required]
    [StringLength(250)]
    public string CategoryDesciption { get; set; } = null!;

    public short? ParentCategoryId { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();

    public virtual Category? ParentCategory { get; set; }
}
