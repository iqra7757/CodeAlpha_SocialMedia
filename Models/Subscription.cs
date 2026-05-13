using System;
using System.Collections.Generic;

namespace CodeAlpha_SocialMedia.Models;

public partial class Subscription
{
    public int Id { get; set; }

    public int SubscriberId { get; set; }

    public int ChannelId { get; set; }

    public DateTime? SubscribedAt { get; set; }

    public virtual User Channel { get; set; } = null!;

    public virtual User Subscriber { get; set; } = null!;
}
