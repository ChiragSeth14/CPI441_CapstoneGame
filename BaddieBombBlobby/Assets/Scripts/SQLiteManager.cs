using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private string dbPath;

    void Start()
    {
        // Updated database path with correct format
        dbPath = "URI=file:C:/Users/nawaf/OneDrive/Desktop/CPI441/database/databaseMonster.db";

        TestConnection();
        FetchQuestions();
    }

    private void TestConnection()
    {
        try
        {
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    Debug.Log("SQLite Connection Successful!");
                }
                else
                {
                    Debug.LogError("SQLite Connection Failed!");
                }
                connection.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"SQLite Connection Error: {e.Message}\n{e.StackTrace}");
        }
    }

    private void FetchQuestions()
    {
        try
        {
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                string query = "SELECT question_text FROM Questions";
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string questionText = reader.GetString(0);
                            Debug.Log("Question: " + questionText);
                        }
                    }
                }
                connection.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching questions: {e.Message}\n{e.StackTrace}");
        }
    }
}
