using System;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using TMPro;
using UnityEngine.UI;



public class LevelRepository
{
    private readonly string _dbPath;

    public LevelRepository(string dbPath)
    {
        _dbPath = dbPath;
    }

    private SqliteConnection GetConnection()
    {
        return new SqliteConnection(_dbPath);
    }

    public Level GetLevel(int levelNumber)
    {
        Level level = null;
        using (var connection = GetConnection())
        {
            connection.Open();
            string query = "SELECT level_id, level_number FROM Levels WHERE level_number = @levelNumber"; // No description here
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@levelNumber", levelNumber);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        level = new Level(reader.GetInt32(0), reader.GetInt32(1)); // Only reading level_id and level_number
                    }
                }
            }
        }
        return level;
    }
}
