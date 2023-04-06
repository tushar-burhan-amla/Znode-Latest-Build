﻿namespace Znode.Engine.Admin.ViewModels
{
    public class StateViewModel : BaseViewModel
    {
        public int StateId { get; set; }
        public string CountryCode { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
    }
}