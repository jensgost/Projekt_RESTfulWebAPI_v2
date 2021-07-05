using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Projekt_RESTfulWebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Projekt_RESTfulWebAPI.Data
{
    public class OurDbContext : DbContext
    {
        public OurDbContext(DbContextOptions<OurDbContext> options)
            : base(options)
        {
        }

        public DbSet<GeoMessage> GeoMessages { get; set; }
        public DbSet<ApiToken> ApiTokens { get; set; }

        public async Task Seed(UserManager<User> userManager)
        {
            await Database.EnsureDeletedAsync();
            await Database.EnsureCreatedAsync();

            var user = new User
            {
                UserName = "Name",
                FirstName = "Us",
                LastName = "Er"
            };
            await userManager.CreateAsync(user);

            var geoMessage = new GeoMessage
            {
                Message = "Message Test",
                Longitude = 10.5,
                Latitude = 5.10
            };

            var geoMessageV2 = new GeoMessage
            {
                Title = "Title Test",
                Body = "Body Test",
                Author = user.FirstName + " " + user.LastName,
                Latitude = 51.0,
                Longitude = 1.05
            };

            var apiToken = new ApiToken 
            {
                Key = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"),
                User = user
            };

            await AddAsync(apiToken);
            await AddAsync(geoMessage);
            await AddAsync(geoMessageV2);
            await SaveChangesAsync();
        }
    }
 }

