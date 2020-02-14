using System;
using Toolbelt.Blazor.I18nText.Interfaces;

namespace Here.MyNameSpace.I18nText.Foo.Pages
{
    public class Index : I18nTextLateBinding
    {
        public string this[string key] => throw new NotImplementedException();
    }
}
