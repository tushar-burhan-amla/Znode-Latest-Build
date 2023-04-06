using Nest;
using System.Collections.Generic;
using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public class CountQuery : BaseQuery, ICountQuery
    {
        #region Protected Variable
        protected readonly FunctionScoreQuery functionScoreQuery = new FunctionScoreQuery();
        protected readonly BoolQuery finalBoolQuery = new BoolQuery();
        #endregion

        #region Public Method

        //Generate count query for elasticsearch api to get count.
        public virtual CountRequest<dynamic> GenerateCountQuery(IZnodeSearchRequest request)
        {
            MultiMatchQuery multiMatchQuery = new MultiMatchQuery();
            List<Field> multipleFields = new List<Field>();

            foreach (ElasticSearchAttributes item in request.SearchableAttribute)
            {
                multipleFields.Add(new Field(item.AttributeCode.ToLower(), item.BoostValue));
            }

            multiMatchQuery.Query = request.SearchText;
            multiMatchQuery.Fields = multipleFields.ToArray();

            multiMatchQuery.Operator = GetOperator(request);

            functionScoreQuery.ScoreMode = FunctionScoreMode.Sum;
            functionScoreQuery.BoostMode = FunctionBoostMode.Sum;

            functionScoreQuery.Functions = AddFunctionToSearchQuery(request);

            finalBoolQuery.Must = new List<QueryContainer> { multiMatchQuery };

            finalBoolQuery.Filter = request.FilterValues;

            functionScoreQuery.Query = finalBoolQuery;

            CountRequest<dynamic> countRequest = new CountRequest<dynamic>(request.IndexName);

            countRequest.Query = functionScoreQuery;

            return countRequest;
        }
        #endregion
    }
}
