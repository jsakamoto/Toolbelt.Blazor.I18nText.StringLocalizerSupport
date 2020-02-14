using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Toolbelt.Blazor.I18nText.StringLocalizerSupport
{
    public class I18nTextStringLocalizer<TResourceSource> : IStringLocalizer<TResourceSource>, IStringLocalizerMonitor
    {
        private readonly IStringLocalizer _localizer;

        public virtual LocalizedString this[string name] => name != null ? _localizer[name] : throw new ArgumentNullException("name");

        public virtual LocalizedString this[string name, params object[] arguments] => name != null ? _localizer[name, arguments] : throw new ArgumentNullException("name");

        public I18nTextStringLocalizer(IStringLocalizerFactory factory)
        {
            _localizer = factory.Create(typeof(TResourceSource));
        }

        public virtual IStringLocalizer WithCulture(CultureInfo culture)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return _localizer.WithCulture(culture);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _localizer.GetAllStrings(includeParentCultures);
        }

        public ValueTask WaitForReadyAsync(ComponentBase component)
        {
            var localizerMonitor = _localizer as IStringLocalizerMonitor;
            return localizerMonitor?.WaitForReadyAsync(component) ?? new ValueTask();
        }
    }
}
