using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace PeachPied.Symfony.AspNetCore.Templating
{
    /// <summary>
    /// Simple interface for Razor view into string rendering service
    /// </summary>
    public interface IRazorRenderService
    {
        string RenderToString(string viewPath, ViewDataDictionary data);
    }
}
