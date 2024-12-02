# Installation Guide

This demo project is available for Windows, Linux, and MacOS. This guide will help you install the app on your operating system.

## Requirements

-   Windows 10, Linux (Debian-based), or MacOS 10.15 or later
-   [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
-   SQLServer 2019 or SQLite

## Prerequisites

By default, the project requires a local instance of _SQLServer_. You can download the Developer Edition for free from [here](https://www.microsoft.com/en-us/sql-server/sql-server-downloads).

If you're using _SQLite_, make sure it is available on your path.

Also, make sure to trust dev certificates by running the following command:

```sh
dotnet dev-certs https --trust
```

## Installation

1. Clone the repository

    ```
    git clone https://github.com/dan-koller/jwtauthapi
    ```

2. Navigate to the project directory

    ```
    cd jwtauthapi
    ```

3. Build the project

    > By default, the app uses `SQLServer` as the database provider. If you are on macOS or Linux, you will need to change the database provider to `Sqlite`.

    If you are using a **local SQLServer** instance, you can directly build the project.

    ```
    dotnet build
    ```

    If you're using **SQLite**, you will **need to update** the database provider in the references of the `JAuth.Api.csproj` file:

    ```xml
        <ItemGroup>
            <!-- Change to Sqlite if you prefer -->
            <ProjectReference Include="..\JAuth.UserDataContext.Sqlite\JAuth.UserDataContext.Sqlite.csproj" />
            <!-- <ProjectReference Include="..\JAuth.UserDataContext.SqlServer\JAuth.UserDataContext.SqlServer.csproj" /> -->
        </ItemGroup>
    ```

    Rebuild the project to update the references using `dotnet build` in the `JAuth.Api` directory.

4. Open the application in your browser on [https://localhost:5001/swagger/index.html](https://localhost:5001/swagger/index.html)

## Configuration

### Swagger

Swagger is configured to support token authentication. You can change this behaviour in the `Program.cs` of the `Jauth.Api` project:

```csharp
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
```

### Settings

You can modify the `appsettings.json` file to change the configuration of the application.
