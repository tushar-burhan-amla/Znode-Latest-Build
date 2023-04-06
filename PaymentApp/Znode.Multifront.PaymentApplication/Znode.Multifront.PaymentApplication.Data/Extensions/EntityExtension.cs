using System.Data.Entity;
using System.Threading.Tasks;

namespace Znode.Multifront.PaymentApplication.Data
{
    public static class EntityExtension
    {
        public static TEntity FindExtension<TId, TEntity>(this DbSet<TEntity> source, TId id) where TEntity : class where TId : struct => source.Find(id);

        public static async Task<TEntity> FindExtensionAsync<TId, TEntity>(this DbSet<TEntity> source, TId id) where TEntity : class where TId : struct => await source.FindAsync(id);
    }
}
