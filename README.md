# SIBERS Project Management API

RESTful API для управления проектами, сотрудниками и компаниями. Разработано на основе **ASP.NET Core 8.0** с использованием **Entity Framework Core** и **SQLite**.

## 📋 Описание

API предоставляет функционал для:

- Управления проектами (создание, редактирование, удаление, фильтрация)
- Управления сотрудниками (CRUD операции)
- Связывания сотрудников с проектами
- Хранения документации проектов
- Фильтрации и сортировки проектов по различным параметрам

## 🏗 Структура проекта

```
SIBERS/
├── sibersProject/           # Основное приложение
│   ├── Controllers/         # API контроллеры
│   │   ├── EmployeesController.cs
│   │   └── ProjectsController.cs
│   ├── Data/                # Модели данных
│   │   ├── DTO/             # DTO объекты
│   │   ├── Entities/        # Сущности базы данных
│   │   └── AppDbContext.cs  # Контекст EF Core
│   ├── Services/            # Бизнес-логика
│   │   ├── IEmployeeService.cs
│   │   ├── EmployeeService.cs
│   │   ├── IProjectService.cs
│   │   └── ProjectService.cs
│   ├── Database/            # База данных SQLite
│   │   └── script.sql       # Скрипт инициализации БД
│   ├── Dockerfile           # Docker конфигурация
│   └── Program.cs           # Точка входа
├── sibersProject.Tests/     # Тесты
│   ├── EmployeesControllerTests.cs
│   ├── ProjectFilterTests.cs
│   └── ProjectsControllerTests.cs
├── docker-entrypoint/       # Скрипты для Docker
│   └── init-db.sh
├── docker-compose.yml       # Docker Compose конфигурация
└── sibersProject.sln        # Solution файл
```

## 🚀 Быстрый старт

### Локальный запуск (без Docker)

```bash
# Перейти в директорию проекта
cd sibersProject

# Восстановить зависимости
dotnet restore

# Запустить приложение
dotnet run
```

Приложение будет доступно по адресу: `http://localhost:5298`
Swagger UI: `http://localhost:5298/swagger`

### Запуск через Docker

```bash
# Собрать и запустить контейнеры
docker-compose up -d
```

Приложение будет доступно по адресу: `http://localhost:8080`
Swagger UI: `http://localhost:8080/swagger`

### Остановка

```bash
# Остановить контейнеры
docker-compose down
```

## 📡 API Эндпоинты

### Base URL

```
http://localhost:5298/api  (локально)
http://localhost:8080/api  (Docker)
```

### Projects

| Метод  | Эндпоинт         | Описание                                |
| ------ | ---------------- | --------------------------------------- |
| GET    | `/projects`      | Получение всех проектов (с фильтрацией) |
| GET    | `/projects/{id}` | Получение проекта по ID                 |
| POST   | `/projects`      | Создание нового проекта                 |
| PUT    | `/projects/{id}` | Обновление проекта                      |
| DELETE | `/projects/{id}` | Удаление проекта                        |

**Параметры фильтрации для GET /projects:**

- `StartDateFrom` / `StartDateTo` — фильтр по дате начала
- `EndDateFrom` / `EndDateTo` — фильтр по дате окончания
- `Priority` — фильтр по приоритету
- `Status` — фильтр по статусу
- `CustomerCompanyId` — фильтр по компании-заказчику
- `ContractorCompanyId` — фильтр по компании-исполнителю
- `ManagerId` — фильтр по менеджеру проекта
- `SortBy` — поле для сортировки (Name, StartDate, EndDate, Priority, Status, CreatedAt)
- `Descending` — порядок сортировки (по умолчанию: false)

### Employees

| Метод  | Эндпоинт                                       | Описание                       |
| ------ | ---------------------------------------------- | ------------------------------ |
| GET    | `/employees`                                   | Получение всех сотрудников     |
| GET    | `/employees/{id}`                              | Получение сотрудника по ID     |
| POST   | `/employees`                                   | Создание нового сотрудника     |
| PUT    | `/employees/{id}`                              | Обновление сотрудника          |
| DELETE | `/employees/{id}`                              | Удаление сотрудника            |
| GET    | `/employees/by-project/{projectId}`            | Получение сотрудников проекта  |
| GET    | `/employees/{employeeId}/projects`             | Получение проектов сотрудника  |
| POST   | `/employees/{employeeId}/projects/{projectId}` | Добавление сотрудника в проект |
| DELETE | `/employees/{employeeId}/projects/{projectId}` | Удаление сотрудника из проекта |

## 📦 Модели данных

### Сущности

#### Company

```csharp
Id (int)
Name (string) - Название компании
```

#### Employee

```csharp
Id (int)
FirstName (string) - Имя
LastName (string) - Фамилия
MiddleName (string?) - Отчество
Email (string) - Email
IsActive (int) - Статус активности (0/1)
```

#### Project

```csharp
Id (int)
Name (string) - Название проекта
CustomerCompanyId (int) - Компания-заказчик
ContractorCompanyId (int) - Компания-исполнитель
ManagerId (int) - Менеджер проекта
StartDate (DateOnly) - Дата начала
EndDate (DateOnly?) - Дата окончания
Priority (int) - Приоритет (1-5)
Status (string?) - Статус проекта
CreatedAt (DateTime?) - Дата создания
```

#### ProjectEmployee (Связь многие-ко-многим)

```csharp
ProjectId (int)
EmployeeId (int)
Role (string?) - Роль в проекте
```

#### ProjectDocument

```csharp
Id (int)
ProjectId (int)
FileName (string) - Имя файла
FilePath (string) - Путь к файлу
UploadedAt (DateTime?) - Дата загрузки
```

## 🔧 Технологии

- **Язык**: C# 12
- **Фреймворк**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0
- **База данных**: SQLite
- **API документация**: Swagger/OpenAPI
- **Контейнеризация**: Docker
- **Тестирование**: xUnit

## 📝 Примеры запросов

### Создание проекта

**Request:**

```http
POST /api/projects
Content-Type: application/json

{
  "name": "Новый проект",
  "customerCompanyId": 1,
  "contractorCompanyId": 2,
  "managerId": 1,
  "startDate": "2024-01-01",
  "endDate": "2024-12-31",
  "priority": 3,
  "status": "Active"
}
```

**Response:**

```json
{
  "id": 1,
  "name": "Новый проект",
  "customerCompanyId": 1,
  "contractorCompanyId": 2,
  "managerId": 1,
  "startDate": "2024-01-01",
  "endDate": "2024-12-31",
  "priority": 3,
  "status": "Active",
  "createdAt": "2024-05-24T10:00:00"
}
```

### Фильтрация проектов

**Request:**

```http
GET /api/projects?Priority=3&Status=Active&SortBy=StartDate&Descending=true
```

**Response:**

```json
[
  {
    "id": 1,
    "name": "Новый проект",
    "priority": 3,
    "status": "Active",
    "startDate": "2024-01-01",
    "endDate": "2024-12-31"
  }
]
```

### Добавление сотрудника в проект

**Request:**

```http
POST /api/employees/1/projects/1?role=Developer
```

**Response:**

```json
{
  "message": "Employee added to project successfully"
}
```

## ✅ Тестирование

Запуск тестов:

```bash
cd sibersProject.Tests
dotnet test
```

Тесты покрывают:

- Контроллеры Projects и Employees
- Фильтрацию проектов
- Валидацию данных

## 🛠 Конфигурация

### Переменные окружения

| Переменная               | Описание           | Значение по умолчанию  |
| ------------------------ | ------------------ | ---------------------- |
| `ASPNETCORE_ENVIRONMENT` | Окружение          | Development            |
| `ASPNETCORE_URLS`        | URL приложения     | http://+:8080          |
| `SQLITE_PATH`            | Путь к базе данных | ./Database/sibersDB.db |

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## 📊 Приоритеты проектов

| Значение | Описание    |
| -------- | ----------- |
| 1        | Низкий      |
| 2        | Средний     |
| 3        | Высокий     |
| 4        | Критический |
| 5        | Неотложный  |
