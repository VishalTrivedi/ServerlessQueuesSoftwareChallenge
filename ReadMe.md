
# Serverless Queues Software Challenge

This project demonstrates a solution using Azure Functions where two functions communicate via Azure Storage Queues. The solution is designed to run locally using Visual Studio and the Azurite emulator, with an in-memory SQLite database to store user details and SVG data.

## Project Overview

The solution consists of two Azure Functions:

1. **CreateUser Function**  
   - This function is the entry point in the workflow. It is triggered by an HTTP request containing a JSON payload with two properties: `FirstName` and `LastName`. For example:
 ```json
{
	"firstName": "John",
	"lastName": "Doe"
}
```
   - Saves the provided data to an in-memory SQLite database and publishes a message to an Azure Storage Queue.
   - Returns `200 OK` to the caller if successful, appropriate error codes with messages otherwise.

2. **GetUserSVG Function**  
   - This function is triggered by messages from the Azure Storage Queue.
   - Calls a third-party API to fetch SVG initials for the full name (concatenation of `FirstName` and `LastName`).
   - Saves the SVG response to the SQLite database, associating it with the provided `FirstName` and `LastName`.

## Features

- **Locally runnable:** The solution can be run locally using Visual Studio's F5 functionality and the Azurite emulator for storage.
- **Database:** In-memory SQLite is used to store the user details (`FirstName`, `LastName`) and the corresponding SVG data.
- **Unique Entries:** The combination of `FirstName` and `LastName` must be unique. Duplicates are not allowed.
- **Validation and Error Handling:** The system validates incoming data and provides appropriate error messages.
- **Dependency Injection:** The project follows a DI (Dependency Injection) pattern for better maintainability and testability.

## Architecture

The solution is divided into two main projects:
1. **FunctionApp**: Contains the trigger/entry point functions (CreateUser, GetUserSVG).
2. **BusinessLogic**: Contains the core business logic, including database operations and API interactions.

## Prerequisites

- [Visual Studio 2022](https://visualstudio.microsoft.com/) with Azure Development workload.
- [Azurite Emulator](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite) for simulating Azure Storage locally.
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (or higher).

## Running the Solution Locally

1. **Clone the repository:**

   ```bash
   git clone https://github.com/VishalTrivedi/ServerlessQueuesSoftwareChallenge.git

## Known issues

1. **Data persistence:** Despite using an in-memory SQLLite Database, the data in the database persisted, even between application restarts.
