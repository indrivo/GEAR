# Localization module

## Description

Localization module is used for translate text from UI, auto translations, add/edit text, add new languages

## Install

### Register services
At the moment, are implement 2 methods:

* With json files

Add in `startup.cs` in ConfigureServices method  

``` csharp
			//---------------------------------Localization Module-------------------------------------
			config.GearServices
				.AddLocalizationModule<JsonFileLocalizationService, JsonStringLocalizer>()
				.BindLanguagesFromJsonFile(Configuration.GetSection(nameof(LocalizationConfig)))
				.RegisterTranslationService<YandexTranslationProvider>(
					Configuration.GetSection(nameof(LocalizationProviderSettings)))
				.AddLocalizationRazorModule()
```

Need to add in appsettings this section:

``` json
  "LocalizationConfig": {
    "Languages": [
      {
        "IsDisabled": false,
        "Identifier": "en",
        "Name": "English"
      },
      {
        "IsDisabled": false,
        "Identifier": "ro",
        "Name": "Romanian"
      },
      {
        "IsDisabled": true,
        "Identifier": "ru",
        "Name": "Russian"
      },
      //Here you can add another languages
    ],
    "Path": "Localization",
    "SessionStoreKeyName": "lang",
    "DefaultLanguage": "en"
  }
```

For all added  languages in this json file, is needed to add a localization json file in Localization folder, of root folder or you can customize with another folder in your appsettings file

The name of file must be in this format: {LanguageIdentifier}.json
For current configuration:
* ./Localization/en.json
* ./Localization/ro.json
* ./Localization/ru.json

`Note` : for all your environments `appsettings.{EnvName}.json` , is needed to add configuration of languages

For change the default language you must change the property `DefaultLanguage` from appsettings file.
To disable a language, you can set `true` to `IsDisabled` property, and the language will be not showed in list for change language, also this can be done with UI module or api call (view this with swagger)

* With database

``` csharp
			//---------------------------------Localization Module-------------------------------------
			config.GearServices
				.AddLocalizationModule<DbLocalizationService, DbStringLocalizer>()
				.BindLanguagesFromDatabase()
				.AddLocalizationModuleStorage<TranslationsDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.RegisterTranslationService<YandexTranslationProvider>(
					Configuration.GetSection(nameof(LocalizationProviderSettings)))
				.AddLocalizationRazorModule()
```

## Usage

This module can be used in multiple modes:

* with javascript

``` javascript
window.translate("key"); // the result will be the translated text with current language from session
```

* in Razor

``` csharp
@Inject IStringLocalizer Localizer

var value = Localizer["key"]; //with C#
@Localizer["key"] //with razor
```

* with html attributes

``` html
<span translate="text_in_english_key">Text in english</span>
```

To translate a html block after document load, you can call `window.forceTranslate` with selector of block, as in example

``` js
window.forceTranslate("#MySelectorId");
window.forceTranslate(".MySelectorClass");
window.forceTranslate("span");
window.forceTranslate("*"); // to translate all tags
```

`Note` : if call of this method is done whitout selector, all document will be translated where is present `translate` attribute 
`Note` : The response of method is a promise, if you want to do some actions after translations, you must to apply `then` as in example

``` js
window.forceTranslate().then(() => {
    alert("Text in page was translated");
});
```
## Available language packets
To find the available default language packs click  [Languages](/../Assets/LocalizationPacks)