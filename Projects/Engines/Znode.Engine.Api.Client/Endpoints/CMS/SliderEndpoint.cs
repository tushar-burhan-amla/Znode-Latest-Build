namespace Znode.Engine.Api.Client.Endpoints
{
    public class SliderEndpoint : BaseEndpoint
    {
        #region Slider
        //Get slider list endpoint
        public static string SliderList() => $"{ApiRoot}/slider/slider/list";

        //Create slider Endpoint.
        public static string CreateSlider() => $"{ApiRoot}/slider/createslider";

        //Get slider on the basis of cmsSliderId Endpoint.
        public static string GetSlider(int cmsSliderId) => $"{ApiRoot}/slider/getslider/{cmsSliderId}";

        //Update slider Endpoint.
        public static string UpdateSlider() => $"{ApiRoot}/slider/updateslider";

        //Delete slider Endpoint.
        public static string DeleteSlider() => $"{ApiRoot}/slider/deleteslider";

        //Publish slider endpoint.
        public static string PublishSlider() => $"{ApiRoot}/slider/publish";

        //Publish slider with preview endpoint.
        public static string PublishSliderWithPreview() => $"{ApiRoot}/slider/publishwithpreview";

        #endregion

        #region Banner

        //Banner list endpoint.
        public static string GetBannerList() => $"{ApiRoot}/slider/Banner/list";

        //Create banner Endpoint.
        public static string CreateBanner() => $"{ApiRoot}/slider/createbanner";

        //Get banner on the basis of cmsSliderBannerId Endpoint.
        public static string GetBanner(int cmsSliderBannerId) => $"{ApiRoot}/slider/getbanner/{cmsSliderBannerId}";

        //Update banner Endpoint.
        public static string UpdateBanner() => $"{ApiRoot}/slider/updatebanner";

        //Delete banner Endpoint.
        public static string DeleteBanner() => $"{ApiRoot}/slider/deletebanner";

        #endregion
    }
}
