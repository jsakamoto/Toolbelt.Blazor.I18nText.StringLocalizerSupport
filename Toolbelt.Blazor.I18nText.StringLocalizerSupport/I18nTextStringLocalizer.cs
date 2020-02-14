using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Toolbelt.Blazor.I18nText.Interfaces;
using Toolbelt.Blazor.I18nText.Internals;

namespace Toolbelt.Blazor.I18nText.StringLocalizerSupport
{
    internal class I18nTextStringLocalizer : IStringLocalizer, IStringLocalizerMonitor
    {
        private TextTable TextTable;

        private string LangCode;

        private readonly string SearchedLocation;

        private readonly I18nTextStringLocalizerFactory Owner;

        internal I18nTextStringLocalizer(TextTable textTable, string langCode, string searchedLocation, I18nTextStringLocalizerFactory owner)
        {
            this.TextTable = textTable;
            this.LangCode = langCode;
            this.SearchedLocation = textTable?.TableObject?.GetType().FullName ?? searchedLocation;
            this.Owner = owner;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var (value, notFound) = GetStringCore(name);
                return new LocalizedString(name, value, notFound, SearchedLocation);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var (value, notFound) = GetStringCore(name);
                return new LocalizedString(name, string.Format(value, arguments), notFound, SearchedLocation);
            }
        }

        private (string value, bool notFound) GetStringCore(string name)
        {
            this.RefetchTextTable(CultureInfo.CurrentUICulture.Name);
            var value = (this.TextTable?.TableObject as I18nTextLateBinding)?.GetFieldValueWithNoFallback(name);
            return (value: value ?? name, notFound: value == null);
        }

        internal ValueTask RefetchTextTable(string newLang)
        {
            if (this.LangCode != newLang && this.TextTable?.TableObject != null)
            {
                this.LangCode = newLang;
                this.TextTable = this.Owner.RefetchTextTable(this.TextTable.TableObject.GetType(), this.LangCode);
                return this.TextTable?.FetchTask ?? new ValueTask();
            }
            return new ValueTask();
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            if (this.TextTable?.TableObject == null) return Enumerable.Empty<LocalizedString>();
            var tableObject = this.TextTable.TableObject;
            return tableObject.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Select(field => (field.Name, Value: field.GetValue(tableObject) as string))
                .Where(x => x.Value != null)
                .Select(x => new LocalizedString(x.Name, x.Value));
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            var langCode = culture.Name;
            if (this.TextTable == null) return new I18nTextStringLocalizer(null, langCode, this.SearchedLocation, this.Owner);

            var textTable = this.Owner.RefetchTextTable(this.TextTable.GetType(), langCode);
            return new I18nTextStringLocalizer(textTable, langCode, this.SearchedLocation, this.Owner);
        }

        public ValueTask WaitForReadyAsync(ComponentBase component)
        {
            this.Owner.RegisterComponent(component);
            return this.TextTable?.FetchTask ?? new ValueTask();
        }
    }
}
