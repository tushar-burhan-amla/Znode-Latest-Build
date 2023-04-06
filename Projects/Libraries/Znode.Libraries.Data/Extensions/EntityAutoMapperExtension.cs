using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Znode.Libraries.Data
{
    public static class EntityAutoMapperExtension
    {
        public static async Task<List<TViewModel>> ProjectToListAsync<TViewModel>(this IQueryable queryable) where TViewModel : class => await queryable.ProjectTo<TViewModel>().ToListAsync();

        public static async Task<TViewModel[]> ProjectToArrayAsync<TViewModel>(this IQueryable queryable) where TViewModel : class => await queryable.ProjectTo<TViewModel>().ToArrayAsync();

        public static async Task<TViewModel> ProjectToSingleOrDefaultAsync<TViewModel>(this IQueryable queryable) where TViewModel : class => await queryable.ProjectTo<TViewModel>().SingleOrDefaultAsync();

        public static async Task<TViewModel> ProjectToSingleAsync<TViewModel>(this IQueryable queryable) where TViewModel : class => await queryable.ProjectTo<TViewModel>().SingleAsync();

        public static async Task<TViewModel> ProjectToFirstOrDefaultAsync<TViewModel>(this IQueryable queryable) where TViewModel : class => await queryable.ProjectTo<TViewModel>().FirstOrDefaultAsync();

        public static async Task<TViewModel> ProjectToFirstAsync<TViewModel>(this IQueryable queryable) where TViewModel : class => await queryable.ProjectTo<TViewModel>().FirstAsync();

        public static List<TViewModel> ProjectToList<TViewModel>(this IQueryable queryable) where TViewModel : class => queryable.ProjectTo<TViewModel>().ToList();

        public static TViewModel[] ProjectToArray<TViewModel>(this IQueryable queryable) where TViewModel : class => queryable.ProjectTo<TViewModel>().ToArray();

        public static TViewModel ProjectToSingleOrDefault<TViewModel>(this IQueryable queryable) where TViewModel : class => queryable.ProjectTo<TViewModel>().SingleOrDefault();

        public static TViewModel ProjectToSingle<TViewModel>(this IQueryable queryable) where TViewModel : class => queryable.ProjectTo<TViewModel>().Single();

        public static TViewModel ProjectToFirstOrDefault<TViewModel>(this IQueryable queryable) where TViewModel : class => queryable.ProjectTo<TViewModel>().FirstOrDefault();

        public static TViewModel ProjectToFirst<TViewModel>(this IQueryable queryable) where TViewModel : class => queryable.ProjectTo<TViewModel>().First();

        public static IQueryable<TViewModel> ProjectToQueryable<TViewModel>(this IQueryable queryable) where TViewModel : class => queryable.ProjectTo<TViewModel>();
    }
}

