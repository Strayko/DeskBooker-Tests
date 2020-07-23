using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DeskBooker.DataAccess
{
    public class DeskBookerContext : DbContext
    {
        public DeskBookerContext(DbContextOptions<DeskBookerContext> options) : base(options)
        {

        }

        public DbSet<DeskBooking> DeskBooking { get; set; }
    }
}
