using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Toolbelt.Blazor.I18nText.StringLocalizerSupport
{
    public interface IStringLocalizerMonitor
    {
        ValueTask WaitForReadyAsync(ComponentBase component);
    }
}
