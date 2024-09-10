# JwtAuthApi

This project contains a complete backend API for user authentication using JWT (JSON Web Token) in .NET. It is a RESTful API that allows users to register, login, and access protected and public routes. It is completely cross-platform and can be run on Windows, Linux, and macOS with database support for SQLite and MS SQL Server. The application complies with the OpenAPI specification and provides a Swagger documentation for easy testing and integration.

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

5. Open your browser and navigate to `https://localhost:5001/swagger/index.html` to view the Swagger documentation.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
