using Toolbelt.Blazor.I18nText;
using Toolbelt.Blazor.I18nText.Interfaces;

namespace Here.MyNameSpace.I18nText.Pages
{
    public class Index : I18nTextLateBinding
    {
        public string this[string key] => I18nTextExtensions.GetFieldValue(this, key);

        public string HelloWorld;
    }
}
