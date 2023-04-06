namespace Znode.Libraries.Search
{
    public static class ElasticLibraryConstants
    {
        public const string indexDirectory = "ElasticIndexLocation";

        // This field value change get wrong result. (This will be change in case of "public List<ElasticAttribute> Attributes { get; set; }" is changed.
        // Make sure its properly updated.
        public const string NestedPath = "attributes";
        public const string AttributesCode = "attributes.attributecode";
        public const string AttributesValues = "attributes.attributevalues";
        public const string RawAttributesValues = "attributes.rawattributevalues";
        public const string RawLowerCaseAttributeValues = "attributes.rawlowercaseattributevalues";
        public const string ElasticProductIndexType = "elasticproduct";
    }
}
