using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Toolbelt.Blazor.I18nText;
using Toolbelt.Blazor.I18nText.StringLocalizerSupport;

namespace Toolbelt.Blazor.Extensions.DependencyInjection
{
    public static class I18nTextStringLocalizerDependencyInjection
    {
        public static IServiceCollection AddI18nText(this IServiceCollection services, bool stringLocalizerSupport, Action<I18nTextOptions> configure = null)
        {
            services.AddI18nText(configure);
            if (stringLocalizerSupport)
            {
                services.TryAddSingleton<IStringLocalizerFactory>(serviceProvider => new I18nTextStringLocalizerFactory(serviceProvider.GetService<I18nTextRepository>()));
                services.TryAddTransient(typeof(IStringLocalizer<>), typeof(I18nTextStringLocalizer<>));
            }
            return services;
        }
    }
}
