using System;
using System.Collections.Generic;

namespace CodeAlpha_SocialMedia.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? ProfilePicture { get; set; }

    public string? Bio { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsHistoryPaused { get; set; }

    public int? ChannelViews { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Follow> FollowFollowers { get; set; } = new List<Follow>();

    public virtual ICollection<Follow> FollowFollowings { get; set; } = new List<Follow>();

    public virtual ICollection<History> Histories { get; set; } = new List<History>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<ProfileView> ProfileViewTargetUsers { get; set; } = new List<ProfileView>();

    public virtual ICollection<ProfileView> ProfileViewViewers { get; set; } = new List<ProfileView>();

    public virtual ICollection<Subscription> SubscriptionChannels { get; set; } = new List<Subscription>();

    public virtual ICollection<Subscription> SubscriptionSubscribers { get; set; } = new List<Subscription>();
}
