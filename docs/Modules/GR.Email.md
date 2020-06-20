# Email Sender module

## Description
Email sender is an implementation of SMTP sender, it has a base `IEmailSender` interface that implement `Microsoft.AspNetCore.Identity.UI.Services.IEmailSender`
It also extend `ISender` that allow to send messages without inject `IEmailSender`
## Installation
For us this module, it must be registered in your Startup class 
```csharp
//----------------------------------------Email Module-------------------------------------
			config.GearServices.AddEmailModule<EmailSender>()
				.AddEmailRazorUIModule()
				.BindEmailSettings(Configuration);
```

In app settings is need to add the following settings:
```json
  "EmailSettings": {
    "Enabled": true,
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Timeout": 5000,
    "EnableSsl": true,
    "NetworkCredential": {
      "Email": "",
      "Password": ""
    }
  },
```

If register the Razor module, these settings can be edited from UI page , see in admin configurations of GEAR

## Usage
This module ca be used in 2 ways:
1. Inject `ISender`
Example: 
```csharp
using GR.Core.Services;
...
private readonly AppSender _sender;
...
var result = await _sender.SendAsync("email", $"Invite to {GearApplication.ApplicationName}", message, model.Email);
```
2. Inject `IEmailSender` 
Example:
```csharp
using GR.Email.Abstractions;
...
private readonly IEmailSender _emailSender;

await _emailSender.SendEmailAsync("user@mail.com", "Subject", "text");
```