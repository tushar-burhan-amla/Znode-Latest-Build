namespace Znode.Engine.Api.Cache
{
    public interface ICSSCache 
    {
        string GetCssListByThemeId(int cmsThemeId, string routeUri, string routeTemplate);
    }
}
