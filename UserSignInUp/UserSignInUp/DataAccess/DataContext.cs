using Microsoft.EntityFrameworkCore;
using UserSignInUp.Models;

namespace UserSignInUp.DataAccess
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext>options):base(options) 
        {
                
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                .UseSqlServer("Data Source=DESKTOP-I4EMULH\\SQLEXPRESS01;Initial Catalog=UserDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }
        public DbSet<User> Users { get; set; }


    }
    
}
