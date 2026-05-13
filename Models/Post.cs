using System;
using System.Collections.Generic;

namespace CodeAlpha_SocialMedia.Models;

public partial class Post
{
    public int PostId { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public string? ImagePath { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Category { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<History> Histories { get; set; } = new List<History>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual User User { get; set; } = null!;
}
