Library Management System – Technical Documentation <br>
**Overview** <br>
The Library Management System (LMS) is a web-based application designed to manage the operations of a library, including cataloging, issuing books, returning books, and maintaining user accounts. The system will be developed using ASP.NET Core framework, with a focus on modularity, scalability, and maintainability.
________________________________________
System Requirements
Functional Requirements
1.	User Management
    o	Register new users (Admin, Librarians, and Members).
    o	Authenticate and authorize users based on roles.
    o	View and manage user profiles.
2.	Book Management
    o	Add, edit, and delete book records.
    o	Search for books by title, author, or category.
    o	Categorize books by genre and author.
3.	Lending Management
    o	Issue books to members.
    o	Record book returns and calculate late fees.
    o	View member borrowing history.
4.	Notifications
    o	Notify users of due dates and overdue books.
    o	Notify administrators when books are returned late.
5.	Reports
    o	Generate reports on book inventory, borrowing trends, and overdue items.
________________________________________
Technology Stack
Component	Technology
Frontend Framework	Razor Pages / Blazor
Backend Framework	ASP.NET Core MVC
Database	SQL Server
ORM	Entity Framework Core
Version Control	Git
________________________________________
Database Design
1. Tables
  Users
    Column Name	Data Type	Description
    UserId	INT	Primary Key
    Username	NVARCHAR	Unique
    PasswordHash	NVARCHAR	Encrypted
    Email	NVARCHAR	User email address
    Role	NVARCHAR	Admin, Librarian, or Member
  Books
    Column Name	Data Type	Description
    BookId	INT	Primary Key
    Title	NVARCHAR	Title of the book
    Author	NVARCHAR	Author of the book
    Genre	NVARCHAR	Genre
    ISBN	NVARCHAR	International Standard Book Number
    CopiesAvailable	INT	Number of copies available
  Borrowing
    Column Name	Data Type	Description
    BorrowId	INT	Primary Key
    UserId	INT	Foreign Key (Users)
    BookId	INT	Foreign Key (Books)
    BorrowDate	DATETIME	Date of borrowing
    DueDate	DATETIME	Due date for return
    ReturnDate	DATETIME	Actual return date
    LateFee	DECIMAL	Calculated late fee

