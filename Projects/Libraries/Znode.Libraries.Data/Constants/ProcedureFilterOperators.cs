namespace Znode.Libraries.Data
{
    public struct ProcedureFilterOperators
    {
        public static string Contains { get; } = "cn";
        public static new string Equals { get; } = "eq";
        public static string EndsWith { get; } = "ew";
        public static string GreaterThan { get; } = "gt";
        public static string GreaterThanOrEqual { get; } = "ge";
        public static string LessThan { get; } = "lt";
        public static string LessThanOrEqual { get; } = "le";
        public static string NotEquals { get; } = "ne";
        public static string StartsWith { get; } = "sw";
        public static string Like { get; } = "lk";
        public static string Is { get; } = "is";
        public static string In { get; } = "in";
        public static string Or { get; } = "or";
        public static string NotIn { get; } = "not in";
        public static string NotContains { get; } = "ncn";
        public static string Between { get; } = "bw";
    }
}
