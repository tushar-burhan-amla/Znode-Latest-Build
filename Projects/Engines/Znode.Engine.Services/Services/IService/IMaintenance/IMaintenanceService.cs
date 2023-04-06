﻿namespace Znode.Engine.Services
{
    public interface IMaintenanceService
    {
        /// <summary>
        /// To delete published data of all catalog, store,cms & elastic search.
        /// </summary>
        /// <returns>If successfully perform then return true else false</returns>
        bool PurgeAllPublishedData();
    }
}
