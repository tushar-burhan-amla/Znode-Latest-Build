using Nest;
using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Libraries.ElasticSearch
{
    public interface IZnodeSearchIndexSettingHelper
    {
        #region Token filters

        /// <summary>
        /// To set the Shingle token filter in the tokenFilters list. 
        /// </summary>
        /// <param name="tokenFilters">List of token filters</param>
        /// <param name="indexSettings">IndexSettings</param>
        void SetShingleTokenFilter(List<string> tokenFilters, IndexSettings indexSettings);

        /// <summary>
        /// To set the NGram token filter in the tokenFilters list. 
        /// </summary>
        /// <param name="indexSettings">IndexSettings</param>
        /// <param name="publishSearchProfileModel">PublishSearchProfileModel</param>
        void SetNGramTokenFilter(IndexSettings indexSettings, PublishSearchProfileModel publishSearchProfileModel);

        /// <summary>
        /// To set the Stemming token filter in the tokenFilters list.
        /// </summary>
        /// <param name="tokenFilters">List of token filters</param>
        /// <param name="indexSettings">IndexSettings</param>
        void SetStemmingFilter(List<string> tokenFilters, IndexSettings indexSettings);

        /// <summary>
        /// To set the Edge NGram token filter in the tokenFilters list.
        /// </summary>
        /// <param name="tokenFilters">List of token filters</param>
        /// <param name="indexSettings">IndexSettings</param>
        void SetEdgeNGramFilter(List<string> tokenFilters, IndexSettings indexSettings);

        /// <summary>
        /// To set the tokenizers in the index settings.
        /// </summary>
        /// <param name="indexSettings">IndexSettings</param>
        /// <param name="publishSearchProfileModels">PublishSearchProfileModel</param>
        void SetTokenizers(IndexSettings indexSetting, PublishSearchProfileModel publishSearchProfileModels);

        /// <summary>
        /// To set the CustomAnalyzer instance.
        /// </summary>
        /// <param name="tokenizer">Name of the tokenizer</param>
        /// <param name="tokenFilters">List containing names of token filters</param>
        /// <param name="characterFilters">List containing names of character filters</param>
        /// <returns></returns>
        CustomAnalyzer SetCustomAnalyzer(string tokenizer, List<string> tokenFilters, List<string> characterFilters = null);

        /// <summary>
        /// To set the porter stem token filter in the IndexSettings.
        /// </summary>
        /// <param name="indexSettings">IndexSettings</param>
        void SetPorterStemTokenFilter(IndexSettings indexSettings);

        #endregion Token filters

        #region Character filters

        /// <summary>
        /// To set the HTML Strip character filter.
        /// </summary>
        /// <param name="indexSettings">IndexSettings</param>
        void SetHtmlStripCharacterFilter(IndexSettings indexSettings);

        /// <summary>
        /// To set the mapping character filters in the IndexSettings. 
        /// </summary>
        /// <param name="indexSettings">IndexSettings</param>
        /// <param name="publishSearchProfileModel">PublishSearchProfileModel</param>
        void SetMappingCharacterFilter(IndexSettings indexSettings, PublishSearchProfileModel publishSearchProfileModel);

        #endregion Character filters

        /// <summary>
        /// To get the feature value by feature code.
        /// </summary>
        /// <param name="searchFeatureListJson">searchFeatureListJson</param>
        /// <param name="featureCode">featureCode</param>
        /// <returns>feature value</returns>
        string GetSearchFeatureByFeatureCode(string searchFeatureListJson, string featureCode);
    }
}
