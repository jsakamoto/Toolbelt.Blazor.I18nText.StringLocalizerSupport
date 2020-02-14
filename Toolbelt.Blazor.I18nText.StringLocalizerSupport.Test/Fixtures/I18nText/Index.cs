using System;
using Toolbelt.Blazor.I18nText.Interfaces;

namespace Here.MyNameSpace.I18nText
{
    public class Index : I18nTextLateBinding, I18nTextFallbackLanguage
    {
        public string this[string key] => throw new NotImplementedException();

        string I18nTextFallbackLanguage.FallBackLanguage => "en";

        public string LabelForName = "Name";

        public string DoValidation = "Do Validation";
    }
}
