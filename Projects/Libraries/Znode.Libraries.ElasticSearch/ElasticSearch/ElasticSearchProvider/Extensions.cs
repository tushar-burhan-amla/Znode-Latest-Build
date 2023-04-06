using System.Collections.Generic;
using System.Linq;

namespace Znode.Libraries.Search
{
    public static class Extensions
    {
        //CartesianProduct - Lambda
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new IEnumerable<T>[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(
                emptyProduct,
                (accumulator, sequence) =>
                {
                    return accumulator.SelectMany(
                        (accseq => sequence),
                        (accseq, item) => accseq.Concat(new T[] { item })
                    );
                }
            );
        }

        //CartesianProduct - LINQ
        //http://blogs.msdn.com/b/ericlippert/archive/2010/06/28/computing-a-cartesian-product-with-linq.aspx
        static IEnumerable<IEnumerable<T>> CartesianProduct2<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(
                emptyProduct,
                (accumulator, sequence) =>
                    from accseq in accumulator
                    from item in sequence
                    select accseq.Concat(new[] { item }));
        }
    }
}
