using Microsoft.EntityFrameworkCore;
using EmployeeRegistrationWPF.Models;
using System.IO;
using System;

namespace EmployeeRegistrationWPF.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databaseFolder = Path.Combine(AppContext.BaseDirectory, "Database");
            Directory.CreateDirectory(databaseFolder);
            string databasePath = Path.Combine(databaseFolder, "EmployeeRegistration.db");

            optionsBuilder.UseSqlite($"Data Source={databasePath}");
        }
    }
}
