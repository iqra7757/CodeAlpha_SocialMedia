using System;
using System.Collections.Generic;

namespace CodeAlpha_SocialMedia.Models;

public partial class History
{
    public int HistoryId { get; set; }

    public int UserId { get; set; }

    public int PostId { get; set; }

    public DateTime? ViewedAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
