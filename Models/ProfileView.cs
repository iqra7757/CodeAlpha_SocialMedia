using System;
using System.Collections.Generic;

namespace CodeAlpha_SocialMedia.Models;

public partial class ProfileView
{
    public int ViewId { get; set; }

    public int ViewerId { get; set; }

    public int TargetUserId { get; set; }

    public DateTime? ViewDate { get; set; }

    public virtual User TargetUser { get; set; } = null!;

    public virtual User Viewer { get; set; } = null!;
}
