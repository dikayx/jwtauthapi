# JwtAuthApi

[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Build Status](https://github.com/dikayx/jwtauthapi/actions/workflows/dotnet.yml/badge.svg)](https://github.com/dikayx/jwtauthapi/actions/workflows/dotnet.yml)

This project contains a complete backend API for user authentication using JWT (JSON Web Token) in .NET. It is a RESTful API that allows users to register, login, and access protected and public routes. It is completely cross-platform and can be run on Windows, Linux, and macOS with database support for SQLite and MS SQL Server. The application complies with the OpenAPI specification and provides a Swagger documentation for easy testing and integration.

![Screenshot of the available endpoint in Swagger UI](./_Assets/swagger.png)

## Get started

1. Clone the repository

    ```bash
    git clone https://github.com/dikayx/JwtAuthApi
    ```

2. Change directory

    ```bash
    cd JwtAuthApi
    ```

3. Build the project

    ```bash
    dotnet build
    ```

4. Run the project

    ```bash
    dotnet run --project JAuth.Api --launch-profile https
    ```

5. Open your browser and navigate to [https://localhost:5001/swagger/index.html](https://localhost:5001/swagger/index.html) to view the Swagger documentation.

For more information, please take a look at the [installation guide](./_Docs/INSTALLATION.md).

## Usage

For a basic workflow, open [Swagger](https://localhost:5001/swagger/index.html), register a new user and get a token. Then click `Authorize` in the upper right corner and enter your token.

That's it! You can now try to access routes that require authorization or explore other endpoints.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
