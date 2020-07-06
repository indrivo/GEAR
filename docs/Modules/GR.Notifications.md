# Notification Module

## Description
This module is intended for sending notifications to users, notifications can be sent to one or more users simultaneously, notifications are received real time

## Install

### Hub registration
```csharp
	public override void Configure(IApplicationBuilder app)
		{
			...
			app.UseNotificationsHub<GearNotificationHub>();
			...
		}
```

### Module registration
```csharp
			//-------------------------------Notification Module-------------------------------------
			config.GearServices.AddNotificationModule<NotifyWithDynamicEntities<GearIdentityDbContext, GearRole, GearUser>, GearRole>()
				.AddNotificationSubscriptionModule<NotificationSubscriptionService>()
				.AddNotificationModuleEvents()
				.AddNotificationSubscriptionModuleStorage<NotificationDbContext>(options =>
				{
					options.GetDefaultOptions(Configuration);
					options.EnableSensitiveDataLogging();
				})
				.RegisterNotificationsHubModule<CommunicationHub>()
				.AddNotificationRazorUIModule();
```

## Usage

In order to send notifications, it is necessary to inject the service:

```csharp
        /// <summary>
        /// Inject notifier
        /// </summary>
        private readonly INotify<GearRole> _notify;
```

## Examples

1. An example of sending a notification to the user with id {userId} with the message "Hello from admin"
```csharp
await _notify.SendNotificationAsync(new List<Guid> { userId }, new Notification
                {
                    Subject = $"Notification subject",
                    SendLocal = true,
                    SendEmail = true,
                    Content = "Hello from admin"
                });
```