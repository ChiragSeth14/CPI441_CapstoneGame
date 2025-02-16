using UnityEngine;

public class DatabasePath
{
    private string dbPath;

    // Constructor (optional) - Can initialize with a default path
    public DatabasePath(string initialPath = "URI=file:C:/Users/nawaf/OneDrive/Desktop/CPI441/database/databaseMonster.db")
    {
        dbPath = initialPath;
    }

    // Getter and Setter for Database Path
    public string DbPath
    {
        get { return dbPath; }
       // set { dbPath = value; }
    }
}

