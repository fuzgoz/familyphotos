using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FamilyPhotos.Entities;

public partial class FamilyPhotoContext : IdentityDbContext<IdentityUser>
{
    public FamilyPhotoContext()
    {
    }

    public FamilyPhotoContext(DbContextOptions<FamilyPhotoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Folder> Folders { get; set; }

    public virtual DbSet<FolderPerson> FolderPeople { get; set; }

    public virtual DbSet<FolderPhoto> FolderPhotos { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<Photo> Photos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=familyphoto;Username=postgres;Password=post");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        #region Identity

        modelBuilder.Entity<IdentityUserLogin<string>>()
            .HasKey(ul => new { ul.UserId, ul.LoginProvider });

        modelBuilder.Entity<IdentityUserRole<string>>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<IdentityUserToken<string>>()
                .HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });



        #endregion

        #region Folder
        modelBuilder.Entity<Folder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("folders_pkey");

            entity.ToTable("folders");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.IsFavorite).HasColumnName("is_favorite");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasMany(f => f.FolderPhotos)
                .WithOne(fp => fp.Folder)
                .HasForeignKey(fp => fp.FolderId)
                .OnDelete(DeleteBehavior.Cascade);


        });

        modelBuilder.Entity<Folder>()
        .HasOne(f => f.User)
        .WithMany(u => u.Folders)
        .HasForeignKey(f => f.UserId)
        .OnDelete(DeleteBehavior.Cascade);

        #endregion


        #region FolderPerson
        modelBuilder.Entity<FolderPerson>(entity =>
            {
                /*
                entity.ToTable("folder_people");
                entity.HasKey(fp => new { fp.FolderId, fp.PersonId });
                entity.Property(e => e.FolderId).HasColumnName("folder_id");
                entity.Property(e => e.PersonId).HasColumnName("person_id");

                entity.HasOne(d => d.Folder).WithMany()
                    .HasForeignKey(d => d.FolderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_folder");

                entity.HasOne(d => d.Person).WithMany()
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_person");
                */

                entity.ToTable("folder_people");
                entity.HasKey(fp => new { fp.FolderId, fp.PersonId });

                entity.Property(e => e.FolderId).HasColumnName("folder_id");
                entity.Property(e => e.PersonId).HasColumnName("person_id");

                entity.HasOne(fp => fp.Folder)
                    .WithMany(f => f.FolderPeople) // A Folder navigációs tulajdonsága
                    .HasForeignKey(fp => fp.FolderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_folder");

                entity.HasOne(fp => fp.Person)
                    .WithMany(p => p.FolderPeople) // A Person navigációs tulajdonsága
                    .HasForeignKey(fp => fp.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_person");
            });
        #endregion

        #region FolderPhoto
        modelBuilder.Entity<FolderPhoto>(entity =>
        {
            entity.HasKey(fp => new { fp.FolderId, fp.PhotoId }); // Összetett kulcs

            entity.ToTable("folder_photos");

            entity.Property(fp => fp.FolderId).HasColumnName("folder_id");
            entity.Property(fp => fp.PhotoId).HasColumnName("photo_id");

            entity.HasOne(fp => fp.Folder)
                  .WithMany(f => f.FolderPhotos)
                  .HasForeignKey(fp => fp.FolderId)
                  .OnDelete(DeleteBehavior.Cascade) // Cascade törlés
                  .HasConstraintName("fk_folder");

            entity.HasOne(fp => fp.Photo)
                  .WithMany(p => p.FolderPhotos)
                  .HasForeignKey(fp => fp.PhotoId)
                  .OnDelete(DeleteBehavior.Cascade) // Cascade törlés
                  .HasConstraintName("fk_photo");
        });
        #endregion

        #region Person
        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("people_pkey");

            entity.ToTable("people");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });
        #endregion

        #region Photo
        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("photos_pkey");

            entity.ToTable("photos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FilePath).HasColumnName("file_path");
            entity.Property(e => e.TimeStamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time_stamp");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Photo>()
        .HasOne(p => p.User) // Photo.User navigáció
        .WithMany(u => u.Photos) // ApplicationUser.Photos navigáció
        .HasForeignKey(p => p.UserId) // Idegen kulcs
        .OnDelete(DeleteBehavior.Cascade); // Törlési viselkedés

        #endregion


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

//using System;
//using System.Collections.Generic;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;

//namespace FamilyPhotos.Entities;

//public partial class FamilyPhotoContext : IdentityDbContext<IdentityUser>
//{
//    public FamilyPhotoContext()
//    {
//    }

//    public FamilyPhotoContext(DbContextOptions<FamilyPhotoContext> options)
//        : base(options)
//    {
//    }

//    public virtual DbSet<Folder> Folders { get; set; }

//    public virtual DbSet<FolderPerson> FolderPeople { get; set; }

//    public virtual DbSet<FolderPhoto> FolderPhotos { get; set; }

//    public virtual DbSet<Person> People { get; set; }

//    public virtual DbSet<Photo> Photos { get; set; }
//    public virtual DbSet<ApplicationUser> Users { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//    {
//        if (!optionsBuilder.IsConfigured)
//        {
//            optionsBuilder.UseNpgsql("Host=localhost;Database=familyphoto;Username=postgres;Password=post");
//        }
//    }

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        base.OnModelCreating(modelBuilder); // Identity alapértelmezett konfigurációk

//        modelBuilder.Entity<ApplicationUser>()
//                .ToTable("Users");  // Beállítjuk a táblát "Users" névre

//        modelBuilder.Entity<IdentityUser>()
//                .ToTable("Users");  // Beállítjuk a táblát "Users" névre


//        #region Identity Configurations
//        modelBuilder.Entity<IdentityUserLogin<string>>()
//            .HasKey(ul => new { ul.UserId, ul.LoginProvider });

//        modelBuilder.Entity<IdentityUserRole<string>>()
//            .HasKey(ur => new { ur.UserId, ur.RoleId });

//        modelBuilder.Entity<IdentityUserToken<string>>()
//            .HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });
//        #endregion

//        #region Folder Configuration
//        modelBuilder.Entity<Folder>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("folders_pkey");

//            entity.ToTable("folders");

//            entity.Property(e => e.Id).HasColumnName("id");
//            entity.Property(e => e.Color)
//                .HasMaxLength(50)
//                .HasColumnName("color");
//            entity.Property(e => e.CreatedAt)
//                .HasColumnType("timestamp without time zone")
//                .HasColumnName("created_at");
//            entity.Property(e => e.Date).HasColumnName("date");
//            entity.Property(e => e.IsFavorite).HasColumnName("is_favorite");
//            entity.Property(e => e.Location)
//                .HasMaxLength(100)
//                .HasColumnName("location");
//            entity.Property(e => e.Name)
//                .HasMaxLength(100)
//                .HasColumnName("name");
//            entity.Property(e => e.UserId).HasColumnName("user_id");


//            entity.HasOne(p => p.User)
//                .WithMany(u => u.Folders)
//                .HasForeignKey(p => p.UserId)
//                .OnDelete(DeleteBehavior.Cascade);

//            entity.HasMany(f => f.FolderPhotos)
//                .WithOne(fp => fp.Folder)
//                .HasForeignKey(fp => fp.FolderId)
//                .OnDelete(DeleteBehavior.Cascade);
//        });
//        #endregion
//        #region FolderPerson Configuration
//        modelBuilder.Entity<FolderPerson>(entity =>
//        {
//            entity.ToTable("folder_people");
//            entity.HasKey(fp => new { fp.FolderId, fp.PersonId });

//            entity.Property(e => e.FolderId).HasColumnName("folder_id");
//            entity.Property(e => e.PersonId).HasColumnName("person_id");

//            entity.HasOne(fp => fp.Folder)
//                .WithMany(f => f.FolderPeople)
//                .HasForeignKey(fp => fp.FolderId)
//                .OnDelete(DeleteBehavior.ClientSetNull)
//                .HasConstraintName("fk_folder");

//            entity.HasOne(fp => fp.Person)
//                .WithMany(p => p.FolderPeople)
//                .HasForeignKey(fp => fp.PersonId)
//                .OnDelete(DeleteBehavior.ClientSetNull)
//                .HasConstraintName("fk_person");
//        });
//        #endregion

//        #region FolderPhoto Configuration
//        modelBuilder.Entity<FolderPhoto>(entity =>
//        {
//            entity.HasKey(fp => new { fp.FolderId, fp.PhotoId });

//            entity.ToTable("folder_photos");

//            entity.Property(fp => fp.FolderId).HasColumnName("folder_id");
//            entity.Property(fp => fp.PhotoId).HasColumnName("photo_id");

//            entity.HasOne(fp => fp.Folder)
//                .WithMany(f => f.FolderPhotos)
//                .HasForeignKey(fp => fp.FolderId)
//                .OnDelete(DeleteBehavior.Cascade);

//            entity.HasOne(fp => fp.Photo)
//                .WithMany(p => p.FolderPhotos)
//                .HasForeignKey(fp => fp.PhotoId)
//                .OnDelete(DeleteBehavior.Cascade);
//        });
//        #endregion

//        #region Person Configuration
//        modelBuilder.Entity<Person>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("people_pkey");

//            entity.ToTable("people");

//            entity.Property(e => e.Id).HasColumnName("id");
//            entity.Property(e => e.Name)
//                .HasMaxLength(100)
//                .HasColumnName("name");
//        });
//        #endregion

//        #region Photo Configuration
//        modelBuilder.Entity<Photo>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("photos_pkey");

//            entity.ToTable("photos");

//            entity.Property(e => e.Id).HasColumnName("id");
//            entity.Property(e => e.FilePath).HasColumnName("file_path");
//            entity.Property(e => e.TimeStamp)
//                .HasColumnType("timestamp without time zone")
//                .HasColumnName("time_stamp");
//            entity.Property(e => e.UserId).HasColumnName("user_id");

//            entity.HasOne(p => p.User)
//                .WithMany(u => u.Photos)
//                .HasForeignKey(p => p.UserId)
//                .OnDelete(DeleteBehavior.Cascade);
//        });
//        #endregion

//        OnModelCreatingPartial(modelBuilder);
//    }

//    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
//}
