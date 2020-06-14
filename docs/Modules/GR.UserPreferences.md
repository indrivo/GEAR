
# User Preferences module

## Module description
Module name - `GR.UserPreferences`
This module is created for store and get user preferences values from another modules with a custom configuration for each key, there is a web API that is responsible for all keys, there is validation at values and each key can be registered by several types such as list or text or something else, only the behavior in the UI remains and the configuration at values and validation

## Abstractions
- `IUserPreferencesContext` - it contains abstractions to the entities with which the given module works
- `IUserPreferencesService` - represent the abstraction with that another modules or service can work
	 - `GetValueByKeyAsync` - get the value of user preference by key name, the value is provided for currently logged user
		 ```csharp
		 Task<ResultModel<string>> GetValueByKeyAsync(string key);
		```
	 - `AddOrUpdatePreferenceSettingAsync` - set value for some key on user preference
	   ```csharp
		Task<ResultModel> AddOrUpdatePreferenceSettingAsync(string key, string value);
		```
	 - `GetPreferenceConfigurationAsync` - get configuration of preference item with user selected option
		 ```csharp
	         Task<BaseBuildResponse<object>> GetPreferenceConfigurationAsync(string key);
	   ```
	- `GetAvailableKeys` - get registered keys

## API
The api with docs can be found on swagger, for that, you can write in url: http://you_gear_app_url/swagger , the category is  `UserPreferencesApi`
	
## Integration

### Registration of module

Here we have a standard recording of the module with default configurations
```csharp
//---------------------------------User preferences Module --------------------------------
			config.GearServices
				.AddUserPreferencesModule<UserPreferencesService>()
				.RegisterPreferencesProvider<DefaultUserPreferenceProvider>()
				.AddUserPreferencesModuleStorage<UserPreferencesDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				});
```

###  Register of key
For do this is need to bind module extension on startup configuration with interface IServiceCollection
- key need to be unique and not empty
- TPreferenceItem can be anything class that inherit the PreferenceItem class and implement all parent class methods
- The Func is used for set and customize key configurations 
```csharp
/// <summary>
        /// Register user preferences key
        /// </summary>
        /// <typeparam name="TPreferenceItem"></typeparam>
        /// <param name="services"></param>
        /// <param name="key"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterUserPreferenceKey<TPreferenceItem>(this IServiceCollection services, string key, Func<TPreferenceItem, TPreferenceItem> options)
            where TPreferenceItem : PreferenceItem, new()
        {
            var provider = IoC.Resolve<DefaultUserPreferenceProvider>("PreferencesProvider_Instance");
            provider.RegisterPreferenceItem(key, options(new TPreferenceItem
            {
                Key = key
            }));
            return services;
        }
```


### Examples
#### List Type
Here is an example of register the user time zone preference with key: UserPreferencesResources.UserTimeZoneKey
```csharp 
 //Register timeZone
            services.RegisterUserPreferenceKey<ListPreferenceItem>(UserPreferencesResources.UserTimeZoneKey, options =>
            {
                options.IsRequired = true;
                options.IsValidValue = value =>
                {
                    var response = new ResultModel();
                    if (value.IsNullOrEmpty()) return response;
                    try
                    {
                        TimeZoneInfo.FindSystemTimeZoneById(value);
                        response.IsSuccess = true;
                        return response;
                    }
                    catch (Exception e)
                    {
                        response.AddError(e.Message);
                    }

                    return response;
                };

                options.ResolveListItems = selectedZone =>
                {
                    //$"({zone.StandardName}) {zone.Id}"
                    var data = TimeZoneInfo.GetSystemTimeZones()
                        .Select(zone =>
                            new DisplayItem
                            {
                                Id = zone.Id,
                                Label = zone.DisplayName,
                                Selected = selectedZone == zone.Id
                            }).ToList();

                    return Task.FromResult<IEnumerable<DisplayItem>>(data);
                };
                return options;
            });
```
#### Text Type
here a simple key named `someKey` that the value of is required and on save the value must be equal with 5
```csharp
 services.RegisterUserPreferenceKey<TextPreferenceItem>("someKey", options =>
            {
                options.IsRequired = true;
                options.IsValidValue = value =>
                {
                    var response = new ResultModel();
                    if (value == "5")
                    {
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.AddError("Value is not equal at 5");
                    }

                    return response;
                };
                return options;
            });
```

### Extend
To add new preference class, you must inherit from abstract class PreferenceItem and implement the methods, also you can extend current types as ListPreferenceItem and TextPreferenceItem

The implementation of list type
```csharp
    public class ListPreferenceItem : PreferenceItem
    {
        /// <summary>
        /// Type
        /// </summary>
        public override string Type => "List";

        /// <summary>
        /// Get configuration
        /// </summary>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public override async Task<BaseBuildResponse<object>> GetConfigurationAsync(string currentValue)
        {
            var data = await GetListValuesAsync(currentValue);

            return new BaseBuildResponse<object>
            {
                IsSuccess = true,
                Result = data,
                Type = Type
            };
        }

        /// <summary>
        /// Check if is valid
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
        {
            var isValid = ResolveListItems != null;
            return isValid;
        }

        /// <summary>
        /// Task for resolve list of items
        /// </summary>
        public Func<string, Task<IEnumerable<DisplayItem>>> ResolveListItems = null;

        /// <summary>
        /// Get list of items
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<DisplayItem>> GetListValuesAsync(string selected)
        {
            var items = await ResolveListItems(selected);
            return items;
        }
    }
```