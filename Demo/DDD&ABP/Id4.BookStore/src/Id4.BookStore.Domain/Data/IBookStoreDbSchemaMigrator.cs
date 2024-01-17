using System.Threading.Tasks;

namespace Id4.BookStore.Data;

public interface IBookStoreDbSchemaMigrator
{
    Task MigrateAsync();
}
