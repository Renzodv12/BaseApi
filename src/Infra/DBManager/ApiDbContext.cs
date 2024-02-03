using System;
using Core.Entities;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Core.Entities.Models;
namespace Infra.DBManager
{
    public class ApiDbContext : IdentityDbContext
    {
        public DbSet<IdentityUser> Users { get; set; }
        public DbSet<Perfil> Perfil { get; set; }
        public DbSet<Projects> Projects { get; set; }
        public DbSet<Certificates> Certificates { get; set; }
        public DbSet<Skills> Skills { get; set; }

        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Entity>();
            modelBuilder.Entity<Perfil>()
                                .HasKey(p => new { p.Id });
            modelBuilder.Entity<Projects>()
                                .HasKey(p => new { p.Id });
            modelBuilder.Entity<Certificates>()
                                .HasKey(p => new { p.Id });
            modelBuilder.Entity<Skills>()
                                .HasKey(p => new { p.Id });




            base.OnModelCreating(modelBuilder);



        }
    }
}

