PRAGMA foreign_keys = ON;

-- Companies
CREATE TABLE Companies (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE
);

-- Employees
CREATE TABLE Employees (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FirstName TEXT NOT NULL,
    LastName TEXT NOT NULL,
    MiddleName TEXT,
    Email TEXT NOT NULL UNIQUE,
    IsActive INTEGER NOT NULL DEFAULT 1
);

-- Projects
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

-- ProjectEmployees
CREATE TABLE ProjectEmployees (
    ProjectId INTEGER NOT NULL,
    EmployeeId INTEGER NOT NULL,
    Role TEXT,

    PRIMARY KEY (ProjectId, EmployeeId),

    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE
);

-- ProjectDocuments
CREATE TABLE ProjectDocuments (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ProjectId INTEGER NOT NULL,
    FileName TEXT NOT NULL,
    FilePath TEXT NOT NULL,
    UploadedAt TEXT DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE
);

-- Indexes
CREATE INDEX idx_employees_email ON Employees(Email);
CREATE INDEX idx_employees_name ON Employees(FirstName, LastName);
CREATE INDEX idx_projects_startdate ON Projects(StartDate);
CREATE INDEX idx_projects_priority ON Projects(Priority);
CREATE INDEX idx_projectemployees_employee ON ProjectEmployees(EmployeeId);
CREATE INDEX idx_projectemployees_project ON ProjectEmployees(ProjectId);

-- Test data: Companies
INSERT INTO Companies (Name) VALUES ('Sibers');
INSERT INTO Companies (Name) VALUES ('Client Corp');
INSERT INTO Companies (Name) VALUES ('Tech Solutions');

-- Test data: Employees
INSERT INTO Employees (FirstName, LastName, MiddleName, Email, IsActive) VALUES ('Ivan', 'Ivanov', 'Ivanovich', 'ivan@sibers.com', 1);
INSERT INTO Employees (FirstName, LastName, MiddleName, Email, IsActive) VALUES ('Petr', 'Petrov', 'Petrovich', 'petr@sibers.com', 1);
INSERT INTO Employees (FirstName, LastName, MiddleName, Email, IsActive) VALUES ('Sergey', 'Sidorov', 'Sergeevich', 'sergey@client.com', 1);
INSERT INTO Employees (FirstName, LastName, MiddleName, Email, IsActive) VALUES ('Anna', 'Smirnova', 'Anatolievna', 'anna@sibers.com', 1);
INSERT INTO Employees (FirstName, LastName, MiddleName, Email, IsActive) VALUES ('Maria', 'Kuznetsova', 'Mikhailovna', 'maria@tech.com', 1);

-- Test data: Projects
INSERT INTO Projects (Name, CustomerCompanyId, ContractorCompanyId, ManagerId, StartDate, EndDate, Priority, Status)
VALUES ('Website Redesign', 2, 1, 1, '2024-01-15', '2024-06-30', 1, 'Active');

INSERT INTO Projects (Name, CustomerCompanyId, ContractorCompanyId, ManagerId, StartDate, EndDate, Priority, Status)
VALUES ('Mobile App Development', 3, 1, 2, '2024-02-01', '2024-08-15', 2, 'Active');

INSERT INTO Projects (Name, CustomerCompanyId, ContractorCompanyId, ManagerId, StartDate, EndDate, Priority, Status)
VALUES ('CRM System', 2, 1, 1, '2024-03-10', '2024-12-31', 1, 'Planned');

-- Test data: ProjectEmployees
INSERT INTO ProjectEmployees (ProjectId, EmployeeId, Role) VALUES (1, 1, 'Manager');
INSERT INTO ProjectEmployees (ProjectId, EmployeeId, Role) VALUES (1, 2, 'Developer');
INSERT INTO ProjectEmployees (ProjectId, EmployeeId, Role) VALUES (1, 4, 'Designer');
INSERT INTO ProjectEmployees (ProjectId, EmployeeId, Role) VALUES (2, 2, 'Manager');
INSERT INTO ProjectEmployees (ProjectId, EmployeeId, Role) VALUES (2, 5, 'Developer');
INSERT INTO ProjectEmployees (ProjectId, EmployeeId, Role) VALUES (3, 1, 'Manager');
INSERT INTO ProjectEmployees (ProjectId, EmployeeId, Role) VALUES (3, 3, 'Analyst');

-- Test data: ProjectDocuments
INSERT INTO ProjectDocuments (ProjectId, FileName, FilePath) VALUES (1, 'technical_requirements.pdf', '/documents/reqs.pdf');
INSERT INTO ProjectDocuments (ProjectId, FileName, FilePath) VALUES (1, 'design_mockups.fig', '/documents/mockups.fig');
INSERT INTO ProjectDocuments (ProjectId, FileName, FilePath) VALUES (2, 'api_specification.yaml', '/documents/api_spec.yaml');
