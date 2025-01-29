using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FitnessAPI.FitnessDB;

public partial class GymContext : DbContext
{
    public GymContext()
    {
    }

    public GymContext(DbContextOptions<GymContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppointmentsForClass> AppointmentsForClasses { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Coach> Coaches { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=GYM;Integrated Security=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppointmentsForClass>(entity =>
        {
            entity.HasKey(e => e.IdAppointmentsForClasses).HasName("PK__Appointm__8BD952D90D15470D");

            entity.Property(e => e.IdAppointmentsForClasses).HasColumnName("ID_AppointmentsForClasses");
            entity.Property(e => e.DateRecording).HasColumnName("Date_recording");
            entity.Property(e => e.IdCustomers).HasColumnName("ID_Customers");
            entity.Property(e => e.IdLessons).HasColumnName("ID_Lessons");
            entity.Property(e => e.StatusRecording)
                .HasMaxLength(50)
                .IsUnicode(true)
                .HasColumnName("Status_recording");

            entity.HasOne(d => d.IdCustomersNavigation).WithMany(p => p.AppointmentsForClasses)
                .HasForeignKey(d => d.IdCustomers)
                .HasConstraintName("FK__Appointme__ID_Cu__7A3223E8");

            entity.HasOne(d => d.IdLessonsNavigation).WithMany(p => p.AppointmentsForClasses)
                .HasForeignKey(d => d.IdLessons)
                .HasConstraintName("FK__Appointme__ID_Le__7B264821");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.IdBracnches).HasName("PK__Branches__699B818B8AF4309A");

            entity.Property(e => e.IdBracnches).HasColumnName("ID_Bracnches");
            entity.Property(e => e.AddressBranches)
                .HasMaxLength(100)
                .IsUnicode(true);
            entity.Property(e => e.NameBranches)
                .HasMaxLength(100)
                .IsUnicode(true);
        });

        modelBuilder.Entity<Coach>(entity =>
        {
            entity.HasKey(e => e.IdCoaches).HasName("PK__Coaches__9785CADD59A07889");

            entity.HasIndex(e => e.IdUserAccounts, "UQ__Coaches__2F50983FB3FDF2FE").IsUnique();

            entity.Property(e => e.IdCoaches).HasColumnName("ID_Coaches");
            entity.Property(e => e.IdUserAccounts).HasColumnName("ID_UserAccounts");
            entity.Property(e => e.LessonsSchedule).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(255)
                .IsUnicode(true);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(true);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(true);
            entity.Property(e => e.Specialization)
                .HasMaxLength(100)
                .IsUnicode(true);
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .IsUnicode(true);

            entity.HasOne(d => d.IdUserAccountsNavigation).WithOne(p => p.Coach)
                .HasForeignKey<Coach>(d => d.IdUserAccounts)
                .HasConstraintName("FK__Coaches__ID_User__719CDDE7");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.IdCustomers).HasName("PK__Customer__11DD6D95433A724E");

            entity.Property(e => e.IdCustomers).HasColumnName("ID_Customers");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(255)
                .IsUnicode(true);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(true);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(true);
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .IsUnicode(true);
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.IdLessons).HasName("PK__Lessons__A6F3A0BA3E88AD87");

            entity.Property(e => e.IdLessons).HasColumnName("ID_Lessons");
            entity.Property(e => e.DurationClasses).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IdBranches).HasColumnName("ID_Branches");
            entity.Property(e => e.IdCoaches).HasColumnName("ID_Coaches");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(true);

            entity.HasOne(d => d.IdBranchesNavigation).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.IdBranches)
                .HasConstraintName("FK__Lessons__ID_Bran__7755B73D");

            entity.HasOne(d => d.IdCoachesNavigation).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.IdCoaches)
                .HasConstraintName("FK__Lessons__ID_Coac__76619304");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.IdPayment).HasName("PK__Payment__C2118ADE1C308E38");

            entity.ToTable("Payment");

            entity.Property(e => e.IdPayment).HasColumnName("ID_Payment");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IdCustomers).HasColumnName("ID_Customers");
            entity.Property(e => e.IdSubscription).HasColumnName("ID_Subscription");

            entity.HasOne(d => d.IdCustomersNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.IdCustomers)
                .HasConstraintName("FK__Payment__ID_Cust__6CD828CA");

            entity.HasOne(d => d.IdSubscriptionNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.IdSubscription)
                .HasConstraintName("FK__Payment__ID_Subs__6DCC4D03");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRoles).HasName("PK__Roles__30F6299304712781");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160A963D8E0").IsUnique();

            entity.Property(e => e.IdRoles).HasColumnName("ID_Roles");
            entity.Property(e => e.RoleName)
                .HasMaxLength(30)
                .IsUnicode(true);
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.IdSubscription).HasName("PK__Subscrip__1305B0050D811ECA");

            entity.ToTable("Subscription");

            entity.Property(e => e.IdSubscription).HasColumnName("ID_Subscription");
            entity.Property(e => e.Conditions)
                .HasMaxLength(255)
                .IsUnicode(true);
            entity.Property(e => e.Cost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TypeofSubscriptuion)
                .HasMaxLength(50)
                .IsUnicode(true);
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.IdUserAccounts).HasName("PK__UserAcco__2F50983EC0409E73");

            entity.HasIndex(e => e.Login, "UQ__UserAcco__5E55825B28D8D69D").IsUnique();

            entity.Property(e => e.IdUserAccounts).HasColumnName("ID_UserAccounts");
            entity.Property(e => e.IdRoles).HasColumnName("ID_Roles");
            entity.Property(e => e.Login)
                .HasMaxLength(30)
                .IsUnicode(true);
            entity.Property(e => e.Password)
                .HasMaxLength(500)
                .IsUnicode(true);
            entity.Property(e => e.Salt)
                .HasMaxLength(255)
                .IsUnicode(true);

            entity.HasOne(d => d.IdRolesNavigation).WithMany(p => p.UserAccounts)
                .HasForeignKey(d => d.IdRoles)
                .HasConstraintName("FK__UserAccou__ID_Ro__662B2B3B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
