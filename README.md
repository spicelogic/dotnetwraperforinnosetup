Fork of: https://bitbucket.org/spicelogic-team/dotnetwraperforinnosetup/src/master/

# .NET Wrapper For Inno Setup

A .NET 10 class library that enables .NET developers to create Windows installers using [Inno Setup](http://www.jrsoftware.org/isinfo.php) without learning Pascal scripting.

## Requirements

- .NET 10 SDK
- Windows (required for WPF and Inno Setup)
- Inno Setup (for building installers)

## Features

- Create installers programmatically from C#
- Automatic script generation from configuration objects
- Prerequisites management (.NET Framework, SQL Express)
- Code signing integration
- File extension associations
- Windows shell context menu integration
- Multi-platform targeting (x86, x64, AnyCPU)

## Quick Start

```csharp
var settings = new SetupSettings
{
    CompanyName = "My Company",
    ProductName = "My Product",
    ProductVersion = new Version("1.0.0"),
    // ... configure other settings
};

var service = new InnoSetupService(settings);
service.BuildSetupFile(@"C:\Program Files (x86)\Inno Setup 6\ISCC.exe");
```

## Documentation

For detailed features and code examples, see: http://www.spicelogic.com/Blog/DotNet-Wrapper-for-Inno-Setup-8
