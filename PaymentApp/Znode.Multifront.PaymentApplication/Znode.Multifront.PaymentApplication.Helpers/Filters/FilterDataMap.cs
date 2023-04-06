namespace Znode.Multifront.PaymentApplication.Helpers
{
    public static class FilterDataMap
    {
        public static FilterCollection ToFilterDataCollection(this FilterCollection filters)
        {
            FilterCollection dataCollection = new FilterCollection();
            if (!Equals(filters, null))
            {
                dataCollection.AddRange(filters.ToModel<FilterTuple, FilterTuple>());
            }
            return dataCollection;
        }
    }
}
