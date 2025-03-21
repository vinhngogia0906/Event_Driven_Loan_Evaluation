# Event_Driven_Loan_Evaluation
This is my attempt to learn about event-driven architecture in .NET by building a Loan Evaluation platform
1. Download SQLite
Go to the official SQLite download page: https://sqlite.org/download.html.

Under Precompiled Binaries for Windows, download the following files:

sqlite-tools-win32-x86-*.zip: Contains the SQLite command-line tools (sqlite3.exe).

sqlite-dll-win32-x86-*.zip: Contains the SQLite DLL (optional, if you need it for development).

Extract the downloaded ZIP files to a directory of your choice (e.g., C:\sqlite).

2. Add SQLite to Your System PATH
To make it easier to run SQLite from the command line, add the SQLite directory to your system PATH:

Open the Start menu and search for Environment Variables.

Click Edit the system environment variables.

In the System Properties window, click the Environment Variables button.

Under System variables, find the Path variable and click Edit.

Click New and add the path to the directory where you extracted SQLite (e.g., C:\sqlite).

Click OK to save the changes.

3. Verify the Installation
Open a new Command Prompt (Windows + R, type cmd, and press Enter).

Run the following command to verify that SQLite is installed:

bash
Copy
sqlite3 --version
This should display the SQLite version (e.g., 3.45.1).

4. Create and Use a SQLite Database
Open a Command Prompt.

Navigate to the directory where you want to create your SQLite database:

bash
Copy
cd C:\path\to\your\project
Start SQLite and create a new database file:

bash
Copy
sqlite3 mydatabase.db
This will create a new SQLite database file named mydatabase.db (if it doesn't already exist) and open the SQLite shell.

You can now run SQL commands in the SQLite shell. For example:

sql
Copy
-- Create a table
CREATE TABLE customers (
    id INTEGER PRIMARY KEY,
    name TEXT NOT NULL,
    email TEXT NOT NULL
);

-- Insert data
INSERT INTO customers (name, email) VALUES ('John Doe', 'john@example.com');

-- Query data
SELECT * FROM customers;
To exit the SQLite shell, type:

sql
Copy
.exit
5. Using SQLite with a GUI Tool (Optional)
If you prefer a graphical interface to manage your SQLite database, you can use tools like:

DB Browser for SQLite: A free, open-source GUI tool for SQLite.

Download: https://sqlitebrowser.org/dl/

Install and open the tool, then load your .db file to interact with the database.

SQLite Studio: Another powerful GUI tool for SQLite.

Download: https://sqlitestudio.pl/

6. Using SQLite in Your Applications
If you're developing an application (e.g., a .NET application), you can use SQLite as your database by installing the appropriate library.

For .NET Applications:
Install the Microsoft.EntityFrameworkCore.Sqlite NuGet package:

bash
Copy
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
Configure your DbContext to use SQLite:

csharp
Copy
public class CustomerDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=mydatabase.db");
    }
}
Run migrations to create the database schema:

bash
Copy
dotnet ef migrations add InitialCreate
dotnet ef database update
7. Persisting the Database
SQLite databases are stored as files (e.g., mydatabase.db). You can move, copy, or back up the file as needed. To ensure the database is always available, store it in a persistent location (e.g., your project directory).

Summary
Download and install SQLite from the official website.

Add SQLite to your system PATH for easy access.

Use the sqlite3 command-line tool to create and manage databases.

Optionally, use a GUI tool like DB Browser for SQLite for a graphical interface.

Integrate SQLite into your applications using libraries like Microsoft.EntityFrameworkCore.Sqlite.