using Microsoft.EntityFrameworkCore;

namespace Financeiro.CashFlow.DataModels.Data
{
    public class LancamentoAppDbContext : DbContext
    {
        public LancamentoAppDbContext(DbContextOptions<LancamentoAppDbContext> options)
        :base(options){ }   
        
        public DbSet<LancamentoDataModel> Lancamentos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Database=lancamentos;Username=postgres;Password=Hdm9r11@",
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly("Financeiro.CashFlow.DataModels"));
            }
        }
    }
}
