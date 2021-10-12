﻿using BankEF.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankEF
{
    class DataContext : DbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DataContext() { }
        public DataContext(DbContextOptions options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=BankEFDB;Trusted_Connection=True;");
        }

    }
}
