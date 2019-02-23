﻿using System;
using System.Collections.Generic;
using System.Text;
using Remotely_Library.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Remotely_Server.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            this.Database.Migrate();
        }
        public DbSet<CommandContext> CommandContexts { get; set; }

        public DbSet<Drive> Drives { get; set; }

        public DbSet<Machine> Machines { get; set; }

        public DbSet<Organization> Organizations { get; set; }

        public new DbSet<RemotelyUser> Users { get; set; }

        public DbSet<EventLog> EventLogs { get; set; }

        public DbSet<SharedFile> SharedFiles { get; set; }

        public DbSet<InviteLink> InviteLinks { get; set; }

        public DbSet<PermissionGroup> PermissionGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<IdentityUser>().ToTable("RemotelyUsers");
            builder.Entity<RemotelyUser>().ToTable("RemotelyUsers");

            builder.Entity<Organization>()
                .HasMany(x => x.Machines)
                .WithOne(x=>x.Organization);
            builder.Entity<Organization>()
                .HasMany(x => x.RemotelyUsers)
                .WithOne(x=> x.Organization);
            builder.Entity<Organization>()
                .HasMany(x => x.CommandContexts)
                .WithOne(x => x.Organization);
            builder.Entity<Organization>()
                .HasMany(x => x.EventLogs)
                .WithOne(x => x.Organization);
            builder.Entity<Organization>()
                .HasMany(x => x.PermissionGroups)
                .WithOne(x => x.Organization);
            builder.Entity<Organization>()
              .HasMany(x => x.InviteLinks)
              .WithOne(x => x.Organization);
            builder.Entity<Organization>()
              .HasMany(x => x.SharedFiles)
              .WithOne(x => x.Organization);


            builder.Entity<CommandContext>()
                .Property(x=>x.TargetMachineIDs)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<string[]>(x));
            builder.Entity<CommandContext>()
               .Property(x => x.PSCoreResults)
               .HasConversion(
                   x => JsonConvert.SerializeObject(x),
                   x => JsonConvert.DeserializeObject<List<PSCoreCommandResult>>(x));
            builder.Entity<CommandContext>()
                .Property(x => x.CommandResults)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<List<GenericCommandResult>>(x));

            builder.Entity<RemotelyUser>()
                .Property(x => x.UserOptions)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<Remotely_Library.Models.RemotelyUserOptions>(x));
            builder.Entity<RemotelyUser>()
                .Property(x => x.PermissionGroups)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<List<PermissionGroup>>(x)
                );
            builder.Entity<RemotelyUser>()
               .HasOne(x => x.Organization);

            builder.Entity<Machine>()
                .HasMany(x => x.Drives);
            builder.Entity<RemotelyUser>()
               .Property(x => x.PermissionGroups)
               .HasConversion(
                   x => JsonConvert.SerializeObject(x),
                   x => JsonConvert.DeserializeObject<List<PermissionGroup>>(x)
               );
        }
    }
}