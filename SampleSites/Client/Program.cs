using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Hosting;
using MyNameSpace.Components;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace SampleSite.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services
                .AddI18nText(stringLocalizerSupport: true, options =>
                 {
                     // options.PersistanceLevel = PersistanceLevel.Session;
                     options.GetInitialLanguageAsync = (svcs, opt) => new ValueTask<string>(CultureInfo.CurrentUICulture.Name.Split('-').First());
                 });

            await builder.Build().RunAsync();
        }
    }
}
