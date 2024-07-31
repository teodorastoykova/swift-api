
# Swift API Testing

This project is a Web API that handles Swift MT799 messages. The API can read a message from an uploaded file and store its fields in an SQLite database.
## Prerequisites


- .NET SDK (version 8.0 or later)
- xUnit for testing framework
- FluentAssertions for more expressive assertions in tests

## Installation

To install this project:

```bash
  git clone <repository-url>
  cd SwiftApi
```

Restore the project dependencies using the following command:

```bash
  dotnet restore

```
## Running the application

To run application, run the following command:

```bash
  dotnet run
```

Once the application is build, visit: http://localhost:5116/swagger/index.html and use the /Swift/upload to upload a file and record it. 
## Running Tests

To run tests, run the following command:

```bash
  dotnet test
```

