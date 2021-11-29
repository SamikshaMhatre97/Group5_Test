using Microsoft.EntityFrameworkCore;
using MVC_Crime_Start.Models;

namespace MVC_Crime_Start.DataAccess
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
      
        public DbSet<CrimeResult> CrimeResults { get; set; }
        public DbSet<CrimeRoot> CrimeRoots { get; set; }
        public DbSet<Pagination> Paginations { get; set; }

    }
}