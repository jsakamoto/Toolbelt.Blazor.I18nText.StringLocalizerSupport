using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Toolbelt.Blazor.I18nText.StringLocalizerSupport;

namespace Microsoft.Extensions.Localization
{
    public static class IStringLocalizerExtensions
    {
        public static ValueTask WaitForReadyAsync(this IStringLocalizer localizer, ComponentBase component)
        {
            var localizerMonitor = localizer as IStringLocalizerMonitor;
            if (localizerMonitor != null)
            {
                return localizerMonitor.WaitForReadyAsync(component);
            }
            return new ValueTask();
        }
    }
}
