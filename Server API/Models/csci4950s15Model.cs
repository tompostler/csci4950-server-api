namespace Server_API.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class csci4950s15Model : DbContext
    {
        public csci4950s15Model()
            : base("name=csci4950s15ModelConnection")
        {
        }

        public virtual DbSet<activity> activities { get; set; }
        public virtual DbSet<activityunit> activityunits { get; set; }
        public virtual DbSet<auth> auths { get; set; }
        public virtual DbSet<course> courses { get; set; }
        public virtual DbSet<location> locations { get; set; }
        public virtual DbSet<setting> settings { get; set; }
        public virtual DbSet<tag> tags { get; set; }
        public virtual DbSet<user> users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<activity>()
                .Property(e => e.course_id)
                .IsUnicode(false);

            modelBuilder.Entity<activity>()
                .HasMany(e => e.activityunits)
                .WithRequired(e => e.activity)
                .HasForeignKey(e => e.activity_id);

            modelBuilder.Entity<activity>()
                .HasMany(e => e.tags)
                .WithMany(e => e.activities)
                .Map(m => m.ToTable("tags_activities"));

            modelBuilder.Entity<activityunit>()
                .HasMany(e => e.tags)
                .WithMany(e => e.activityunits)
                .Map(m => m.ToTable("tags_activityunits"));

            modelBuilder.Entity<auth>()
                .Property(e => e.token)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<course>()
                .Property(e => e.id)
                .IsUnicode(false);

            modelBuilder.Entity<course>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<course>()
                .HasMany(e => e.activities)
                .WithRequired(e => e.cours)
                .HasForeignKey(e => e.course_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<location>()
                .HasMany(e => e.activityunits)
                .WithRequired(e => e.location)
                .HasForeignKey(e => e.location_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tag>()
                .Property(e => e.default_color)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<user>()
                .Property(e => e.password)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<user>()
                .HasMany(e => e.activities)
                .WithRequired(e => e.user)
                .HasForeignKey(e => e.user_id);

            modelBuilder.Entity<user>()
                .HasOptional(e => e.auth)
                .WithRequired(e => e.user)
                .WillCascadeOnDelete();

            modelBuilder.Entity<user>()
                .HasMany(e => e.locations)
                .WithRequired(e => e.user)
                .HasForeignKey(e => e.user_id);

            modelBuilder.Entity<user>()
                .HasOptional(e => e.setting)
                .WithRequired(e => e.user)
                .WillCascadeOnDelete();
        }
    }
}
