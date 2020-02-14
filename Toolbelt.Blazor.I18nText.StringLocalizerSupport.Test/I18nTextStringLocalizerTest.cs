using System;
using System.Linq;
using System.Threading.Tasks;
using Toolbelt.Blazor.I18nText.Internals;
using Xunit;

namespace Toolbelt.Blazor.I18nText.StringLocalizerSupport.Test
{
    public class I18nTextStringLocalizerTest
    {
        [Fact]
        public void GetAllStrings_Test()
        {
            var textTable = new TextTable(typeof(Here.MyNameSpace.I18nText.Index), "", (_, __) => new ValueTask());
            var localizer = new I18nTextStringLocalizer(textTable, "", "", null);
            localizer.GetAllStrings(includeParentCultures: true)
                .OrderBy(localizedText => localizedText.Name)
                .Select(localizedText => $"{localizedText.Name}|{localizedText.Value}|{localizedText.ResourceNotFound}")
                .Is("DoValidation|Do Validation|False",
                    "LabelForName|Name|False");
        }
    }
}
