using System;
using System.Collections.Generic;
using System.Text;
using MyLMS.MVVM.Models;
using Microsoft.EntityFrameworkCore;

namespace MyLMS.Data
{
    internal class LibraryContext : DbContext
    {
        public DbSet<Book> Books => Set<Book>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Loan> Loans => Set<Loan>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=library.db");
        }
    }
}
