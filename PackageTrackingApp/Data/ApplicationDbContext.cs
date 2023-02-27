using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PackageTrackingApp.Models;
using PackageTrackingApp.Data;

//separate databases
//https://stackoverflow.com/questions/57176368/separate-dbcontext-for-identity-and-business-operations-with-same-database-is

// identity guide
//https://endjin.com/blog/2022/03/adding-authentication-and-authorisation-to-aspnet-core-web-applications

// link list to user
//https://stackoverflow.com/questions/65768016/how-to-link-asp-net-core-identity-to-another-entity
namespace PackageTrackingApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }
    
    }
}