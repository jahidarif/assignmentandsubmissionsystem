# Assignment & Submission Management System

A role-based, full-stack web application for a school or college, allowing teachers to create and grade assignments, students to submit their work, and administrators to manage users, classes, and the overall system.

Built for the OnnoRokom Projukti Limited Assistant Software Engineer recruitment project.

---

## Table of Contents

- [Project Overview](#project-overview)
- [Main Features](#main-features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Setup Instructions](#setup-instructions)
- [Database Setup](#database-setup)
- [Running the Backend](#running-the-backend)
- [Running the Frontend](#running-the-frontend)
- [Running the Tests](#running-the-tests)
- [Demo Credentials](#demo-credentials)
- [Assumptions](#assumptions)
- [Known Limitations](#known-limitations)

---

## Project Overview

This system supports three roles — **Admin**, **Teacher**, and **Student** — each with a dedicated dashboard and permission-scoped API access. Teachers create assignments for specific class/subject combinations, students enrolled in that class submit answers before the deadline, and teachers review, grade, and give feedback. Admins manage the underlying data: user accounts, classes, subjects, teacher assignments, and student enrollments.

Authentication is JWT-based with short-lived access tokens and rotating refresh tokens, enforced through role-based authorization on every protected endpoint.

---

## Main Features

### Admin
- Manage users (view, update, deactivate/reactivate — soft delete only)
- Manage classes/courses and subjects (create, update, delete)
- Link subjects to classes, and assign teachers to specific class-subject combinations
- Manage student enrollments
- View all assignments and submissions system-wide (read-only)

### Teacher
- Create, update, and delete assignments (only for class-subjects they're assigned to teach)
- Define title, description, deadline, and maximum marks
- Publish an assignment or keep it as a draft
- View student submissions for their assignments
- Assign marks and provide feedback
- Change a submission's status when necessary

### Student
- View assignments for their enrolled class/course
- View assignment details and deadline
- Submit an answer
- Update a submission before the deadline (locked automatically afterward)
- View submission status, marks, and teacher feedback

### Cross-cutting
- JWT access + refresh token authentication, with automatic silent refresh on the frontend
- Role-based authorization enforced on every backend endpoint
- Deactivated accounts are blocked immediately — on their next API call, on refresh, and on login — not just at their next scheduled token expiry
- Paginated list views (10 rows per page) across every admin/teacher/student table

---

## Technology Stack

**Frontend**
- Next.js (App Router) + React + TypeScript
- Tailwind CSS
- TanStack Query (data fetching/caching)
- react-hook-form (form validation)
- Axios (HTTP client)
- sonner (toast notifications)

**Backend**
- ASP.NET Core Web API (C#)
- Clean Architecture: Domain / Application / Infrastructure / Api
- Entity Framework Core (PostgreSQL provider)
- FluentValidation
- JWT Bearer authentication
- BCrypt password hashing
- Swagger / OpenAPI

**Database**
- PostgreSQL

**Testing**
- xUnit
- Moq

---

## Project Structure

```
AssignmentandSubmissionSystem/
├── backend/
│   ├── AssignmentSubmissionSystem.sln
│   ├── Domain/                  # Entities, enums — no external dependencies
│   ├── Application/             # Services, DTOs, validators, interfaces (business logic)
│   ├── Infrastructure/          # EF Core, repositories, JWT, password hashing
│   ├── Api/                     # Controllers, Program.cs, appsettings
│   └── Tests/                   # xUnit unit tests (mocked repositories, no real DB)
└── frontend/
    ├── app/
    │   ├── auth/                # sign-in, sign-up pages
    │   ├── api/                 # Next.js route handlers (proxy to backend, manage cookies)
    │   └── dashboard/
    │       ├── admin/
    │       ├── teacher/
    │       └── student/
    ├── components/
    ├── hooks/
    ├── lib/api/                 # Typed API client functions per role
    └── middleware.ts            # Route-level auth/role protection
```

---

## Setup Instructions

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18+)
- [PostgreSQL](https://www.postgresql.org/download/) (running locally)
- `dotnet-ef` CLI tool:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### Clone the repository

```bash
git clone <repository-url>
cd AssignmentandSubmissionSystem
```

---

## Database Setup

1. Create the database (or let EF Core create it automatically — see below):
   ```sql
   CREATE DATABASE assignmentdb;
   ```

2. Configure the connection string. In `backend/Api/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=assignmentdb;Username=postgres;Password=YOUR_PASSWORD"
     },
     "AdminSeed": {
       "FullName": "System Administrator",
       "Email": "admin@school.com",
       "Password": "Admin@12345"
     },
     "JwtSettings": {
       "Secret": "REPLACE_WITH_A_LONG_RANDOM_SECRET_AT_LEAST_32_CHARACTERS",
       "Issuer": "AssignmentSubmissionSystem",
       "Audience": "AssignmentSubmissionSystemClient",
       "AccessTokenExpirationMinutes": 15,
       "RefreshTokenExpirationDays": 7
     }
   }
   ```
   Generate a real JWT secret rather than typing one by hand:
   ```bash
   openssl rand -base64 48
   ```

3. Migrations and database creation run **automatically** on backend startup (`Database.MigrateAsync()` in `Program.cs`) — no manual `dotnet ef database update` step is required. The evaluator does not need to manually create any tables.

4. The Admin account is also seeded automatically on first startup, using the `AdminSeed` values above — see [Demo Credentials](#demo-credentials).

---

## Running the Backend

From `backend/`:

```bash
dotnet restore
dotnet build
dotnet run --project Api
```

The API starts at `http://localhost:5133`. Swagger UI is available at:

```
http://localhost:5133/swagger
```

---

## Running the Frontend

From `frontend/`:

1. Copy the example environment file and fill in real values:
   ```bash
   cp .env.example .env.local
   ```
   ```
   NEXT_PUBLIC_API_URL=http://localhost:5133
   JWT_SECRET=<same value as backend appsettings JwtSettings:Secret>
   JWT_ISSUER=AssignmentSubmissionSystem
   JWT_AUDIENCE=AssignmentSubmissionSystemClient
   ```
   `JWT_SECRET` must match the backend's secret exactly — the frontend's `middleware.ts` verifies the JWT independently for route protection.

2. Install dependencies and run:
   ```bash
   npm install
   npm run dev
   ```

3. Open `http://localhost:3000` — this redirects automatically to the sign-in page.

---

## Running the Tests

From `backend/`:

```bash
dotnet test Tests/Tests.csproj
```

Tests are unit tests against the Application layer's services, with `IUnitOfWork`/repositories mocked via Moq — no live database connection is required to run them. Coverage includes authentication and authorization rules, ownership checks (a teacher can't touch another teacher's assignment, a student can't touch another student's submission), and submission workflow rules (deadline enforcement, duplicate-submission prevention, marks-cannot-exceed-max validation).

---

## Demo Credentials

| Role | Email | Password |
|---|---|---|
| Admin | `admin@school.com` | `Admin@12345` |
| Teacher | *(register via the sign-up page, selecting "Teacher")* | — |
| Student | *(register via the sign-up page, selecting "Student")* | — |

The Admin account above is seeded automatically on first backend startup. Teacher and Student accounts are created by registering through the app's sign-up page — after registering, an Admin must link the Teacher to a class/subject (Teacher Assignments) and enroll the Student in a class (Enrollments) before those role-specific features become usable.

---

## Assumptions

- A subject can be offered across multiple classes, and a class can offer multiple subjects — modeled as a many-to-many `ClassSubjects` junction table, rather than a subject belonging to exactly one class.
- A student's enrollment is at the **class/course** level, not per-subject — once enrolled in a class, a student sees all published assignments across every subject offered in that class.
- A teacher's assignment is scoped to a specific class **and** subject combination — a teacher assigned to teach Math in Grade 10 cannot create assignments for English in that same class unless separately assigned.
- User accounts are never hard-deleted; "deleting" a user via the Admin panel deactivates the account instead, preserving referential integrity with their historical assignments/submissions.
- Self-registration (`/auth/sign-up`) only allows Teacher or Student roles. Admin accounts are provisioned only via the seeded startup account, not through self-registration.
- "Manage application-level settings," listed in the BRD's Admin responsibilities, was treated as out of scope, since the brief does not specify what those settings are.

---

## Known Limitations

- No password-reset / "forgot password" flow.
- No file upload for submissions — students provide an optional attachment as a URL rather than uploading a file directly.
- Published assignments can currently still be edited or deleted by their teacher even after students have submitted against them; there is no lock once submissions exist.
- No "unpublish" action — an assignment can move from Draft to Published, but not back.
- Runs over plain HTTP in local development (no TLS certificate configured); cookies are not marked `Secure`. This should be revisited before any production deployment.
- No end-to-end/integration tests against a real database — the test suite covers unit-level business logic only, per the BRD's stated requirement.
