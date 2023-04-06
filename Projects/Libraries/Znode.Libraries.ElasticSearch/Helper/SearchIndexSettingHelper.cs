using Nest;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Znode.Libraries.ElasticSearch
{
    [Obsolete("This class is deprecated and will be discontinued in upcoming versions." +
    "In order to make configurations overridable a new class has been created with the name ZnodeSearchIndexSettingHelper." +
    "This newly created class have the same kind of methods for configurations.")]
    public static class SearchIndexSettingHelper
    {
        //Set default filter
        public static void SetShingleTokenFilter(List<string> tokenFilters, IndexSettings indexsettings)
        {
            indexsettings.Analysis.TokenFilters.Add("shingleTokenFilter", new ShingleTokenFilter
            {
                MinShingleSize = 2,
                MaxShingleSize = 10
            });
            tokenFilters.Add("shingleTokenFilter");
        }

        //Set Ngram Token Filter.
        public static void SetNgramTokenFilter(IndexSettings indexsettings)
        {
            indexsettings.Analysis.TokenFilters.Add("ngramtokenfilter", new NGramTokenFilter
            {
                MinGram = 1,
                MaxGram = 40,
            });
        }
        [System.Obsolete("SetStemmingFilter is deprecated, please use SetEdgeNgramFilter instead.")]
        //Set stemming filter
        public static void SetStemmingFilter(List<string> tokenFilters, IndexSettings indexsettings)
        {
            if (ConfigurationManager.AppSettings["EnableStemmingFilter"]?.Equals("1") ?? false)
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["StemExclusionWords"]))
                {
                    indexsettings.Analysis.TokenFilters.Add("preventStemmingFilter", new KeywordMarkerTokenFilter
                    {
                        Keywords = ConfigurationManager.AppSettings["StemExclusionWords"].Split(',')
                    });
                    tokenFilters.Add("preventStemmingFilter");
                }
                indexsettings.Analysis.TokenFilters.Add("stemmerTokenFilter", new StemmerTokenFilter
                {
                    Language = Language.English.ToString()
                });
                tokenFilters.Add("stemmerTokenFilter");
            }
        }

        //Set Edge Ngram filter
        public static void SetEdgeNgramFilter(List<string> tokenFilters, IndexSettings indexsettings)
        {
            //EdgeNGramTokenFilter also considers the grammar which was not previously considered by stemmerTokenFilter.
            indexsettings.Analysis.TokenFilters.Add("edgeNgramTokenFilter", new EdgeNGramTokenFilter
            {
                MinGram = 1,
                MaxGram = 20
            });
            tokenFilters.Add("edgeNgramTokenFilter");
        }

        //Set tokenizers for search index
        public static void SetTokenizers(IndexSettings indexsettings)
        {
            indexsettings.Analysis.Tokenizers.Add("standard", new StandardTokenizer() { MaxTokenLength = 12 });
            indexsettings.Analysis.Tokenizers.Add("ngram", new NGramTokenizer()
            {
                MinGram = 1,
                MaxGram = 15,
                TokenChars = new List<TokenChar> { TokenChar.Letter, TokenChar.Digit, TokenChar.Punctuation, TokenChar.Symbol, TokenChar.Whitespace }
            });
        }

        //Set Custom Analyzer.
        public static CustomAnalyzer SetCustomAnalyzer(string tokenizer, List<string> tokenFilters, List<string> charFilters = null)
        {
            CustomAnalyzer customAnalyzer = new CustomAnalyzer();
            customAnalyzer.Tokenizer = tokenizer;
            customAnalyzer.Filter = tokenFilters;

            if (charFilters?.Count > 0)
                customAnalyzer.CharFilter = charFilters;
            return customAnalyzer;
        }

        //Set character filter
        public static void SetCharacterTokenFilter(IndexSettings indexsettings)
        {
            indexsettings.Analysis.CharFilters.Add("my_char_filter", new HtmlStripCharFilter { });
        }
    }
}
