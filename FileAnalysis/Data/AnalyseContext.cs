using Microsoft.EntityFrameworkCore;
using FileAnalysis.Entities;

namespace FileAnalysis.Data;

// контекст базы данных In-Memory для хранения отчетов анализа и проанализированных документов
public class AnalyseContext : DbContext
{
    public DbSet<CheckReport> Reports { get; set; }
    public DbSet<AnalyzedDocument> Documents { get; set; }

    public AnalyseContext(DbContextOptions<AnalyseContext> options) : base(options) { }
}