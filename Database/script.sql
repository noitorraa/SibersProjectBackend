PRAGMA foreign_keys = ON;

CREATE TABLE Companies (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE
);

CREATE TABLE Employees (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FirstName TEXT NOT NULL,
    LastName TEXT NOT NULL,
    MiddleName TEXT,
    Email TEXT NOT NULL UNIQUE,
    IsActive INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE Projects (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    CustomerCompanyId INTEGER NOT NULL,
    ContractorCompanyId INTEGER NOT NULL,
    ManagerId INTEGER NOT NULL,
    StartDate TEXT NOT NULL,
    EndDate TEXT,
    Priority INTEGER NOT NULL,
    Status TEXT DEFAULT 'Active',
    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (CustomerCompanyId) REFERENCES Companies(Id) ON DELETE RESTRICT,
    FOREIGN KEY (ContractorCompanyId) REFERENCES Companies(Id) ON DELETE RESTRICT,
    FOREIGN KEY (ManagerId) REFERENCES Employees(Id) ON DELETE RESTRICT
);

CREATE TABLE ProjectEmployees (
    ProjectId INTEGER NOT NULL,
    EmployeeId INTEGER NOT NULL,
    Role TEXT,

    PRIMARY KEY (ProjectId, EmployeeId),

    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE
);

CREATE TABLE ProjectDocuments (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ProjectId INTEGER NOT NULL,
    FileName TEXT NOT NULL,
    FilePath TEXT NOT NULL,
    UploadedAt TEXT DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE
);

CREATE INDEX idx_employees_email ON Employees(Email);
CREATE INDEX idx_employees_name ON Employees(FirstName, LastName);

CREATE INDEX idx_projects_startdate ON Projects(StartDate);
CREATE INDEX idx_projects_priority ON Projects(Priority);

CREATE INDEX idx_projectemployees_employee ON ProjectEmployees(EmployeeId);
CREATE INDEX idx_projectemployees_project ON ProjectEmployees(ProjectId);
