# Calendar Module Abstraction

Calendar is a module for the GEAR framework that allows the creation of events, inviting people to events within an organization. There is a nice interface like the Outlook application. It is possible to synchronize with providers such as Google and Outlook

# Install
### Calendar Abstractions
###### Command line 
```sh
dotnet add package GR.Calendar.Abstractions --version 1.0.3
```
###### Package Manager

```sh
PM> Install-Package GR.Calendar.Abstractions -Version 1.0.3
```


#Add calendar extension to GEAR

  ```csharp
  //------------------------------------Calendar Module-------------------------------------
			config.GearServices.AddCalendarModule<CalendarManager>()
				.AddCalendarModuleStorage<CalendarDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.AddCalendarRazorUIModule()
				.SetSerializationFormatSettings(settings =>
				{
					settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				})
				.AddCalendarRuntimeEvents()
				.RegisterSyncOnExternalCalendars()
				.RegisterTokenProvider<CalendarExternalTokenProvider>()
				.RegisterCalendarUserPreferencesProvider<CalendarUserSettingsService>()
				.RegisterGoogleCalendarProvider()
				.RegisterOutlookCalendarProvider(options =>
				{
					options.ClientId = "d883c965-781c-4520-b7e7-83543eb92b4a";
					options.ClientSecretId = "./7v5Ns0cT@K?BdD85J/r1MkE1rlPran";
					options.TenantId = "f24a7cfa-3648-4303-b392-37bb02d09d28";
				})
				.AddCalendarGraphQlApi();

 ```