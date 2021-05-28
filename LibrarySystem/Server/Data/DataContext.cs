using LibrarySystem.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibrarySystem.Server.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Admins> Admins { get; set; }
        public DbSet<Appointments> Appointments { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Hours> Hours { get; set; }
        public DbSet<ItemHistories> ItemHistories { get; set; }
        public DbSet<Items> Items { get; set; }
        public DbSet<Rooms> Rooms { get; set; }
        public DbSet<Users> Users { get; set; }
    }
}
