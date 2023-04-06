﻿using Newtonsoft.Json;
using System;
using Znode.Engine.Api.Models;

namespace Znode.Engine.WebStore.ViewModels
{
    public class RMAReturnOrderDetailViewModel : BaseViewModel
    {
        public string OrderNumber { get; set; }
        public string ReturnNumber { get; set; }
        public int OmsOrderId { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public RMAReturnCartViewModel ReturnCartViewModel { get; set; }
        public string CultureCode { get; set; }
        public string ReturnNote { get; set; }
        public RMAReturnCalculateViewModel ReturnCalculateViewModel { get; set; }
    }
}