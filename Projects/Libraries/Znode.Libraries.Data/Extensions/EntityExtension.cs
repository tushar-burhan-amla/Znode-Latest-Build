using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Znode.Libraries.Data.DataModel;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace Znode.Libraries.Data
{
    public static class EntityExtension
    {
        #region Public
        public static TEntity FindExtension<TId, TEntity>(this DbSet<TEntity> source, TId id) where TEntity : class where TId : struct => source.Find(id);

        public static async Task<TEntity> FindExtensionAsync<TId, TEntity>(this DbSet<TEntity> source, TId id) where TEntity : class where TId : struct => await source.FindAsync(id);

        //Convert datetime to utc date time format according to time zone.
        public static DateTime ToSiteConfigEntityUtcDateTime(this DateTime dateTimeFromDatabase, string timeZone = "")
        {
            if (!Equals(dateTimeFromDatabase, DateTime.MinValue))
            {
                dateTimeFromDatabase = DateTime.Parse(dateTimeFromDatabase.ToString());
                ZnodeGlobalSetting globalSetting = GetDefaultGlobalSettingData();
                dateTimeFromDatabase = TimeZoneInfo.ConvertTimeFromUtc(dateTimeFromDatabase, GetTimeZoneInformation(globalSetting?.FeatureValues));
                return TimeZoneInfo.ConvertTimeToUtc(dateTimeFromDatabase, GetTimeZoneInformation(globalSetting?.FeatureValues));
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        
        //Convert datetime to locale date time format according to time zone.
        public static DateTime ToSiteConfigEntityLocaleDateTime(this DateTime dateTimeFromDatabase, string timeZone = "")
        {
            dateTimeFromDatabase = DateTime.SpecifyKind(dateTimeFromDatabase, DateTimeKind.Utc);
            return  Equals(dateTimeFromDatabase, DateTime.MinValue) ? DateTime.MinValue : TimeZoneInfo.ConvertTimeFromUtc(dateTimeFromDatabase, GetTimeZoneInformation(GetDefaultGlobalSettingData()?.FeatureValues));
        }
        #endregion

        #region Private

        //Gets the current time zone
        private static ZnodeGlobalSetting GetDefaultGlobalSettingData()
        {
            if (Equals(HttpContext.Current.Cache["DefaultGlobalConfigEntityCache"], null))
            {
                IZnodeRepository<ZnodeGlobalSetting> _globalSettingRepository = new ZnodeRepository<ZnodeGlobalSetting>();
                ZnodeGlobalSetting model = _globalSettingRepository.Table.Where(x => x.FeatureName.Equals("TimeZone"))?.FirstOrDefault();

                if (!Equals(model, null))
                    HttpContext.Current.Cache.Insert("DefaultGlobalConfigEntityCache", model, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(20));

            }
            return HttpContext.Current.Cache["DefaultGlobalConfigEntityCache"] as ZnodeGlobalSetting;
        }
       

        //Get time zone by Id.
        private static TimeZoneInfo GetTimeZoneInformation(string timeZone)
          => TimeZoneInfo.FindSystemTimeZoneById(string.IsNullOrEmpty(timeZone) ? "Central Standard Time" : timeZone);
        #endregion
    }
}
