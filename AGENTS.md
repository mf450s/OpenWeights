# AGENTS.md - Development Guidelines for Weights API

This document provides guidelines for agentic coding agents working on the Weights API codebase.

## Build, Test, and Lint Commands

### Build Commands
```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build src/Weights.API/Weights.API.csproj

# Build and run tests
dotnet test

# Build solution with specific configuration
dotnet build --configuration Release
```

### Test Commands
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test src/Weights.Application.Tests/Weights.Application.Tests.csproj

# Run tests with verbose output
dotnet test --logger "console;verbosity=detailed"

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Run Single Test
```bash
# Run specific test file
dotnet test --filter "FullyQualifiedName~TestClassName"

# Run specific test method
dotnet test --filter "TestMethodName"

# Example: Run specific test
dotnet test --filter "CreateAsync_WithValidRequest_ShouldCreateSessionSuccessfully"
```

### Entity Framework Commands
```bash
# Add new migration
cd src/Weights.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../Weights.API

# Update database
dotnet ef database update --startup-project ../Weights.API

# Remove last migration
dotnet ef migrations remove --startup-project ../Weights.API

# Script migration
dotnet ef migrations script --startup-project ../Weights.API
```

### Docker Commands
```bash
# Build Docker image
docker build -t weights-api .

# Run with Docker Compose
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop services
docker-compose down
```

## Code Style Guidelines

### General Principles
- Follow Clean Architecture principles with strict layer separation
- Use dependency inversion throughout the codebase
- Maintain consistency with existing patterns and conventions
- Write clean, self-documenting code with clear naming

### Naming Conventions

**Classes and Interfaces**
- PascalCase for all public members
- Interface names prefixed with 'I' (e.g., `ITemplateService`)
- Clear, descriptive names that indicate purpose

**Methods and Properties**
- PascalCase for public methods and properties
- Async methods marked with `Async` suffix
- Verb-based names for actions (e.g., `Create`, `Get`, `Update`)
- Private fields prefixed with underscore (e.g., `_templateService`)

**DTOs and Enums**
- PascalCase for all DTO properties and enum values
- Clear naming indicating purpose (Request/Response)
- Use proper data annotations for validation

### File Organization

**Layer Structure**
```
src/
├── Weights.API/              # Controllers and API-specific logic
├── Weights.Application/      # Business logic, services, DTOs
├── Weights.Domain/           # Entities, interfaces, abstractions
└── Weights.Infrastructure/   # Data access, implementations
```

**Namespace Organization**
- Each layer has its own namespace
- Logical grouping within namespaces (e.g., DTOs in subfolders)
- No circular dependencies between layers

### Import/Using Statements

**Order and Organization**
1. Framework imports (e.g., `using System;`)
2. External library imports (e.g., `using FluentAssertions;`)
3. Project-specific imports (e.g., `using Weights.Application.DTOs;`)

**Best Practices**
- No unused imports
- No redundant aliases
- Organized in logical groups separated by blank lines

### Code Formatting

**General Rules**
- Consistent 4-space indentation
- Empty lines between logical sections
- Braces always on new lines
- Proper line length management (aim for < 120 characters)

**C# Specific Patterns**
- Use expression-bodied properties where appropriate
- Null conditional operators (e.g., `user?.Name`)
- Null coalescing operators (e.g., `?? default`)
- String interpolation preferred over concatenation

### Error Handling

**Controller Level**
- Try-catch blocks for specific exceptions
- Appropriate HTTP status codes (400, 401, 404, 201)
- Structured error responses with consistent format

**Validation**
- DataAnnotations for DTO validation
- Model validation in controllers
- FluentValidation for complex business rules

**Domain Level**
- Domain exceptions handled at application layer
- Clean separation between technical and business exceptions

### Async/Await Patterns

**Consistency Rules**
- All async methods properly marked with `async` keyword
- `await` used consistently for async operations
- Async methods return `Task` or `Task<T>`
- No async void methods (except event handlers)

**CancellationToken**
- All public async methods accept `CancellationToken cancellationToken = default`
- Proper propagation through call stack
- Default parameter allows optional cancellation

### Dependency Injection

**Constructor Injection**
- All dependencies injected via constructor
- Primary constructor syntax used
- Private field assignment in constructor

**Interface-Based Design**
- Dependencies always on interfaces, not concrete classes
- Clear separation of contracts and implementations

**Lifetime Management**
- Scoped lifetime for most services
- Singleton for shared services (configuration, etc.)
- Proper disposal patterns

### Repository Pattern

**Base Repository**
- Generic repository base class (`Repository<TEntity, TId>`)
- Common CRUD operations implemented
- Proper async/await patterns

**Specialized Repositories**
- Specific repositories inherit from base
- Domain-specific methods added as needed
- Proper use of EF Core features (Include, ThenInclude)

### API Design Patterns

**Controller Structure**
- `[ApiController]` attribute on all controllers
- `[Route]` attributes for endpoint definition
- Proper HTTP method attributes (`[HttpGet]`, `[HttpPost]`)

**Response Formatting**
- Consistent response types (`ActionResult<T>`)
- Proper status codes with `[ProducesResponseType]`
- Standardized error response format

### Testing Guidelines

**Test Structure**
- Xunit test framework
- FluentAssertions for assertions
- Comprehensive test coverage for edge cases
- Mock objects for external dependencies (Moq)

**Test Organization**
- Clear test method names describing scenarios
- Arrange-Act-Assert pattern consistently used
- Logical grouping of related tests
- Comprehensive test coverage for business logic

### Entity Framework Patterns

**DbContext**
- Properly configured with connection strings
- DbSet properties for each entity
- OnModelCreating for entity configurations

**Entity Configuration**
- Separate configuration classes for each entity
- Fluent API for complex configurations
- Proper relationship setup

### Security Considerations

**Authentication**
- JWT tokens with proper expiration
- BCrypt password hashing
- Environment-based configuration (no hardcoded secrets)

**API Security**
- HTTPS enforcement in production
- Input validation through model validation
- Proper error handling that doesn't expose sensitive information

### Database Design

**Entity Relationships**
- User: Central entity with workout templates and sessions
- WorkoutTemplate: Template with exercises and configurations
- WorkoutSession: Actual performed workouts with tracking data
- Exercise: Exercise database with muscle group targeting

**Track Types**
- WeightReps: Weight + repetitions (e.g., Bench Press)
- BodyweightReps: Bodyweight exercises (e.g., Pull-ups)
- Duration: Time-based exercises (e.g., Plank)
- Distance: Distance-based exercises (e.g., Running)

### Development Workflow

**New Features**
1. Start with domain entities and interfaces
2. Implement application layer with DTOs and services
3. Add infrastructure implementations
4. Create API controllers
5. Write comprehensive tests
6. Update database migrations

**Code Quality**
- Follow existing patterns and conventions
- Write tests for all business logic
- Ensure proper error handling
- Use meaningful variable and method names
- Keep methods focused and single-purpose

### Internationalization

**Multi-language Support**
- API supports German (de-DE) and English (en-US)
- Set language via Accept-Language header
- Use resource strings for user-facing text
- Proper localization of error messages

### Docker and Deployment

**Configuration**
- Environment variables for sensitive data
- Multi-stage builds for production images
- Health checks for all services
- Proper resource limits in production

**Development Setup**
- Use docker-compose for local development
- Configure appsettings.Development.json
- Run database migrations before starting API
- Use HTTPS in development with proper certificates

## Important Notes for AI Agents

- Always follow the existing architecture patterns
- Maintain strict layer separation
- Use dependency inversion
- Write comprehensive tests for new features
- Follow the established naming conventions
- Use async/await properly throughout
- Handle errors appropriately at each layer
- Document complex business logic with clear comments
- Ensure database migrations are properly tested
- Follow security best practices
- Use proper HTTP status codes and response formats

This codebase follows modern .NET 9 practices with Clean Architecture principles. Always maintain consistency with existing patterns and conventions.