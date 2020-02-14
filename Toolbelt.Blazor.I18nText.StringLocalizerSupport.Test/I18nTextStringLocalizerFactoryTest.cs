using System;
using Xunit;

namespace Toolbelt.Blazor.I18nText.StringLocalizerSupport.Test
{
    public class I18nTextStringLocalizerFactoryTest
    {
        [Theory]
        [InlineData("Here.MyNameSpace.Index", typeof(Here.MyNameSpace.I18nText.Index))]
        [InlineData("Here.MyNameSpace.Foo.Index", typeof(Here.MyNameSpace.I18nText.Foo.Index))]
        [InlineData("Here.MyNameSpace.Pages.Index", typeof(Here.MyNameSpace.I18nText.Pages.Index))]
        [InlineData("Here.MyNameSpace.Foo.Pages.Index", typeof(Here.MyNameSpace.I18nText.Foo.Pages.Index))]
        [InlineData("Here.MyNameSpace.Foo.Pages._Host", default(Type))]
        public void GetTextTableTypeFromName_Test(string name, Type expectedType)
        {
            var assembly = this.GetType().Assembly;
            var textTableType = I18nTextStringLocalizerFactory.GetTextTableTypeFromName(assembly, name);
            textTableType.Is(expectedType);
        }

        [Fact(DisplayName = "Create by baseName/location")]
        public void Create_by_baseName_and_location_Test()
        {
            var location = this.GetType().Assembly.GetName().Name;
            using var factory = new I18nTextStringLocalizerFactory(new I18nTextRepository(null));
            var localizer = factory.Create(baseName: $"{location}.Pages.Index", location);
            var lstr = localizer["HelloWorld"];
            lstr.Name.Is("HelloWorld");
            lstr.Value.Is("Hello, World!");
            lstr.ResourceNotFound.IsFalse();
            lstr.SearchedLocation.Is("Here.MyNameSpace.I18nText.Pages.Index");
        }

        [Fact(DisplayName = "Create by baseName/location - key not found")]
        public void Create_by_baseName_and_location_Key_not_Found_Test()
        {
            var location = this.GetType().Assembly.GetName().Name;
            using var factory = new I18nTextStringLocalizerFactory(new I18nTextRepository(null));
            var localizer = factory.Create(baseName: $"{location}.Pages.Index", location);
            var lstr = localizer["Greeting"];
            lstr.Name.Is("Greeting");
            lstr.Value.Is("Greeting");
            lstr.ResourceNotFound.IsTrue();
            lstr.SearchedLocation.Is("Here.MyNameSpace.I18nText.Pages.Index");
        }

        [Fact(DisplayName = "Create by baseName/location - baseName not found")]
        public void Create_by_baseName_and_location_baseName_not_Found_Test()
        {
            var location = this.GetType().Assembly.GetName().Name;
            using var factory = new I18nTextStringLocalizerFactory(new I18nTextRepository(null));
            var localizer = factory.Create(baseName: $"{location}.Pages.Counter", location);
            var lstr = localizer["Season"];
            lstr.Name.Is("Season");
            lstr.Value.Is("Season");
            lstr.ResourceNotFound.IsTrue();
            lstr.SearchedLocation.Is($"{location}.Pages.Counter");
        }

        [Fact(DisplayName = "Create by baseName/location - location not found")]
        public void Create_by_baseName_and_location_location_not_Found_Test()
        {
            var location = this.GetType().Assembly.GetName().Name;
            using var factory = new I18nTextStringLocalizerFactory(new I18nTextRepository(null));
            var localizer = factory.Create(baseName: $"{location}.Pages.Index", "NotFoundAssembly");
            var lstr = localizer["HelloWorld"];
            lstr.Name.Is("HelloWorld");
            lstr.Value.Is("HelloWorld");
            lstr.ResourceNotFound.IsTrue();
            lstr.SearchedLocation.Is($"{location}.Pages.Index");
        }


        [Fact(DisplayName = "Create by ResourceSource")]
        public void Create_by_ResourceSource_Test()
        {
            using var factory = new I18nTextStringLocalizerFactory(new I18nTextRepository(null));
            var localizer = factory.Create(typeof(Here.MyNameSpace.Pages.Index));
            var lstr = localizer["HelloWorld"];
            lstr.Name.Is("HelloWorld");
            lstr.Value.Is("Hello, World!");
            lstr.ResourceNotFound.IsFalse();
            lstr.SearchedLocation.Is("Here.MyNameSpace.I18nText.Pages.Index");
        }

        [Fact(DisplayName = "Create by ResourceSource - key not found")]
        public void Create_by_ResourceSource_Key_not_Found_Test()
        {
            using var factory = new I18nTextStringLocalizerFactory(new I18nTextRepository(null));
            var localizer = factory.Create(typeof(Here.MyNameSpace.Pages.Index));
            var lstr = localizer["Greeting"];
            lstr.Name.Is("Greeting");
            lstr.Value.Is("Greeting");
            lstr.ResourceNotFound.IsTrue();
            lstr.SearchedLocation.Is("Here.MyNameSpace.I18nText.Pages.Index");
        }

        [Fact(DisplayName = "Create by ResourceSource - text table not found")]
        public void Create_by_ResourceSource_TextTable_not_Found_Test()
        {
            using var factory = new I18nTextStringLocalizerFactory(new I18nTextRepository(null));
            var localizer = factory.Create(typeof(Here.MyNameSpace.Pages.Counter));
            var lstr = localizer["Season"];
            lstr.Name.Is("Season");
            lstr.Value.Is("Season");
            lstr.ResourceNotFound.IsTrue();
            lstr.SearchedLocation.Is("Here.MyNameSpace.Pages.Counter");
        }
    }
}
