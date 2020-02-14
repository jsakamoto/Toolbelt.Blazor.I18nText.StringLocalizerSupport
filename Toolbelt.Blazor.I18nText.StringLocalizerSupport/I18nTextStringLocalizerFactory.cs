using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Toolbelt.Blazor.I18nText.Interfaces;
using Toolbelt.Blazor.I18nText.Internals;

namespace Toolbelt.Blazor.I18nText.StringLocalizerSupport
{
    internal class I18nTextStringLocalizerFactory : IStringLocalizerFactory, IDisposable
    {
        private readonly I18nTextRepository I18nTextRepository;

        private readonly WeakRefCollection<I18nTextStringLocalizer> StringLocalizers = new WeakRefCollection<I18nTextStringLocalizer>();

        private readonly WeakRefCollection<ComponentBase> Components = new WeakRefCollection<ComponentBase>();

        private static readonly bool RunningOnWasm = RuntimeInformation.OSDescription == "web";

        internal I18nTextStringLocalizerFactory(I18nTextRepository i18nTextRepository)
        {
            I18nTextRepository = i18nTextRepository;
            if (RunningOnWasm) I18nTextRepository.ChangeLanguage += I18nTextRepository_ChangeLanguage;
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == location);
            return CreateCore(asm, baseName);
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            var asm = resourceSource.Assembly;
            return CreateCore(asm, resourceSource.FullName);
        }

        private I18nTextStringLocalizer CreateCore(Assembly asm, string name)
        {
            var langCode = CultureInfo.CurrentUICulture.Name;
            var textTableType = GetTextTableTypeFromName(asm, name);
            var textTable = this.I18nTextRepository.GetLazyTextTable(Guid.Empty, langCode, textTableType, singleLangInAScope: false);

            var stringLocalizer = new I18nTextStringLocalizer(textTable, langCode, name, this);
            this.StringLocalizers.Add(stringLocalizer);
            return stringLocalizer;
        }

        internal TextTable RefetchTextTable(Type textTableType, string langCode)
        {
            var textTable = this.I18nTextRepository.GetLazyTextTable(Guid.Empty, langCode, textTableType, singleLangInAScope: false);
            return textTable;
        }

        internal static Type GetTextTableTypeFromName(Assembly asm, string name)
        {
            if (asm == null) return null;
            var nameParts = name.Split('.').Reverse().ToArray();

            static int GetScore(Type type, string[] nameParts)
            {
                var nameParts2 = type.FullName.Split('.').Reverse().ToArray();
                var numOfParts = Math.Min(nameParts.Length, nameParts2.Length);
                for (int i = 0; i < numOfParts; i++)
                {
                    if (nameParts[i] != nameParts2[i]) return i;
                }
                return numOfParts;
            }

            var textTableTypes = asm.GetTypes()
                .Where(t => t.GetInterface(typeof(I18nTextLateBinding).FullName) != null)
                .ToArray();

            return textTableTypes
                .Select(type => (Type: type, Score: GetScore(type, nameParts), Weight: type.FullName.Length))
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score).ThenBy(x => x.Weight)
                .FirstOrDefault()
                .Type;
        }

        internal void RegisterComponent(ComponentBase component)
        {
            if (component == null || !RunningOnWasm) return;
            this.Components.Add(component);
        }

        private void I18nTextRepository_ChangeLanguage(object sender, I18nTextChangeLanguageEventArgs e)
        {
            var tasks = new List<ValueTask>();
            this.StringLocalizers.ForEach(stringLocalizer =>
            {
                var task = stringLocalizer.RefetchTextTable(e.LanguageCode);
                tasks.Add(task);
            });

            Task.WhenAll(tasks.Where(t => !t.IsCompleted).Select(t => t.AsTask()))
                .ContinueWith(_ =>
                {
                    this.Components.InvokeStateHasChanged();
                });
        }

        public void Dispose()
        {
            if (RunningOnWasm) I18nTextRepository.ChangeLanguage -= I18nTextRepository_ChangeLanguage;
        }
    }
}
