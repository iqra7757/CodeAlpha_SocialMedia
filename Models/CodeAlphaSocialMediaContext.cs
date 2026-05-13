using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CodeAlpha_SocialMedia.Models;

public partial class CodeAlphaSocialMediaContext : DbContext
{
    public CodeAlphaSocialMediaContext()
    {
    }

    public CodeAlphaSocialMediaContext(DbContextOptions<CodeAlphaSocialMediaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Follow> Follows { get; set; }

    public virtual DbSet<History> Histories { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<ProfileView> ProfileViews { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=CodeAlpha_SocialMedia;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0B22830BEA");

            entity.Property(e => e.CategoryName).HasMaxLength(50);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFCAB542850F");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comments__PostId__656C112C");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comments__UserId__66603565");
        });

        modelBuilder.Entity<Follow>(entity =>
        {
            entity.HasKey(e => new { e.FollowerId, e.FollowingId }).HasName("PK__Follows__79CB0335861B7CB4");

            entity.Property(e => e.FollowDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Follower).WithMany(p => p.FollowFollowers)
                .HasForeignKey(d => d.FollowerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Follows__Followe__6EF57B66");

            entity.HasOne(d => d.Following).WithMany(p => p.FollowFollowings)
                .HasForeignKey(d => d.FollowingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Follows__Followi__6FE99F9F");
        });

        modelBuilder.Entity<History>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__Historie__4D7B4ABD465FC624");

            entity.Property(e => e.ViewedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.Histories)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_History_Post");

            entity.HasOne(d => d.User).WithMany(p => p.Histories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_History_User");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.LikeId).HasName("PK__Likes__A2922C14FAFABDEC");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.Likes)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Likes__PostId__6A30C649");

            entity.HasOne(d => d.User).WithMany(p => p.Likes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Likes__UserId__6B24EA82");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Posts__AA1260185F30A708");

            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasDefaultValue("General");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Posts__UserId__619B8048");
        });

        modelBuilder.Entity<ProfileView>(entity =>
        {
            entity.HasKey(e => e.ViewId).HasName("PK__ProfileV__1E371CF611C88B7D");

            entity.Property(e => e.ViewDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.TargetUser).WithMany(p => p.ProfileViewTargetUsers)
                .HasForeignKey(d => d.TargetUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TargetUser");

            entity.HasOne(d => d.Viewer).WithMany(p => p.ProfileViewViewers)
                .HasForeignKey(d => d.ViewerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Viewer");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC072057A2BB");

            entity.Property(e => e.SubscribedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Channel).WithMany(p => p.SubscriptionChannels)
                .HasForeignKey(d => d.ChannelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Channel");

            entity.HasOne(d => d.Subscriber).WithMany(p => p.SubscriptionSubscribers)
                .HasForeignKey(d => d.SubscriberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subscriber");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CC93B17F4");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053413894807").IsUnique();

            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.ChannelViews).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsHistoryPaused).HasDefaultValue(false);
            entity.Property(e => e.Password).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
