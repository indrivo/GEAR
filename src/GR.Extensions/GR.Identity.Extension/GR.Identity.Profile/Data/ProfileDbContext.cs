﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using GR.Audit.Contexts;
using GR.Core.Abstractions;
using GR.Identity.Profile.Abstractions;
using ProfileModels = GR.Identity.Profile.Abstractions.Models;

namespace GR.Identity.Profile.Data
{
    public class ProfileDbContext : TrackerDbContext, IProfileContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "Identity";

        public ProfileDbContext(DbContextOptions<ProfileDbContext> options) : base(options)
        {
        }

        public DbSet<ProfileModels.Profile> Profiles { get; set; }
        public DbSet<ProfileModels.UserProfile> UserProfiles { get; set; }
        public DbSet<ProfileModels.RoleProfile> RoleProfiles { get; set; }


        public virtual DbSet<T> SetEntity<T>() where T : class, IBaseModel
        {
            return Set<T>();
        }

        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        public Task InvokeSeedAsync(IServiceProvider services)
        {
            return Task.CompletedTask;
        }
    }
    public class ProfileDbContextContextFactory : IDesignTimeDbContextFactory<ProfileDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ProfileDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProfileDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=1111;Database=ISODMS.DEV;");
            return new ProfileDbContext(optionsBuilder.Options);
        }
    }
}
