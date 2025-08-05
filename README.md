# Meeting Scheduler API

A .NET Core Web API for scheduling conflict-free meetings using clean architecture, in-memory storage, and Swagger UI for testing.

## Features
- Create users (`POST /api/users`)
- Schedule meetings in earliest available slot (`POST /api/meetings`)
- Get user meetings (`GET /api/users/{userId}/meetings`)
- Handles partial overlaps, back-to-back meetings, no available slots
- Thread-safe in-memory storage
- Async/await for scalability
- Swagger UI for testing
- Unit tests

## Setup Instructions
1. **Clone Repository**
   ```bash
   git clone <repository-url>
   cd MeetingScheduler
   ```
2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```
3. **Build**
   ```bash
   dotnet build
   ```
4. **Run Application**
   ```bash
   cd MeetingScheduler.API
   dotnet run
   ```
   Access Swagger at `http://localhost:5044`.
5. **Run Tests**
   ```bash
   cd Tests
   dotnet test
   ```
6. **VS Code Setup**
   - Install C# extension
   - Use terminal for commands

## API Endpoints
- **POST /api/users**: `{ "name": "Alice" }` → `201` with user
- **POST /api/meetings**: `{ "participantIds": [1,2], "durationMinutes": 60, "earliestStart": "2025-06-20T09:00:00Z", "latestEnd": "2025-06-20T17:00:00Z" }` → `201` or `404`
- **GET /api/users/{userId}/meetings**: Returns meetings or `404`

## Known Limitations
- **UTC Only**: No time zone support
- **No Duration Validation**: Negative/large durations not checked
- **Fixed Business Hours**: 9:00–17:00 UTC
- **No Cancellation**: Cannot delete/update meetings
- **In-Memory Storage**: Data lost on restart
- **No Authentication**: Open endpoints
- **Limited Error Handling**: Basic validation only

## Contributing
Fork, branch (`git checkout -b feature-name`), commit, push, and create PR.