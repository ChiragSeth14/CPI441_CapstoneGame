using System;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using TMPro;
using UnityEngine.UI;



public class CharacterRepository
{
    private readonly string _dbPath;

    public CharacterRepository(string dbPath)
    {
        _dbPath = dbPath;
    }

    private SqliteConnection GetConnection()
    {
        return new SqliteConnection(_dbPath);
    }

    public Character GetCharacterForQuestion(int questionId, int levelNumber)
    {
        Character character = null;
        using (var connection = GetConnection())
        {
            connection.Open();
            string query = @"
                SELECT 
                    Characters.name, 
                    Characters.image_url 
                FROM 
                    Characters
                INNER JOIN Levels ON Characters.level_id = Levels.level_id
                INNER JOIN Questions ON Levels.level_id = Questions.level_id
                WHERE 
                    Levels.level_number = @levelNumber 
                    AND Questions.question_id = @questionId";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@levelNumber", levelNumber);
                command.Parameters.AddWithValue("@questionId", questionId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        character = new Character
                        {
                            Name = reader.GetString(0),
                            ImageUrl = reader.GetString(1)
                        };
                    }
                }
            }
        }
        return character;
    }
}
