using Microsoft.EntityFrameworkCore;
using FileStorage.Entities;

namespace FileStorage.Data;

// контекст базы данных для хранения загруженных файлов
public class StorageDbContext : DbContext
{
    public DbSet<StoredFile> Files { get; set; }

    public StorageDbContext(DbContextOptions<StorageDbContext> options) : base(options)
    { }
}