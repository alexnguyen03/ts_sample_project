using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace ServerProject.Models
{
    public class StartupDbInitializer
    {
        public static void SeedData(ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            dbContext.Database.EnsureCreated();
            AddRoles(dbContext);
            AddUser(dbContext, userManager);
            AddUserRoles(dbContext, userManager);
        }
    }
}
