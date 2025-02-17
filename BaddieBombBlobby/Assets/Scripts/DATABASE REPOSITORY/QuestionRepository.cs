using System;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using TMPro;
using UnityEngine.UI;



public class QuestionRepository
{
    private readonly string _dbPath;

    public QuestionRepository(string dbPath)
    {
        _dbPath = dbPath;
    }

    private SqliteConnection GetConnection()
    {
        return new SqliteConnection(_dbPath);
    }

    // Update this method to accept a Level object
    public List<Question> GetQuestionsByLevel(Level level)
    {
        List<Question> questions = new List<Question>();
        using (var connection = GetConnection())
        {
            connection.Open();
            string query = "SELECT question_id, question_text FROM Questions WHERE level_id = @levelId";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@levelId", level.LevelId);  // Use level.LevelId here
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        questions.Add(new Question { Id = reader.GetInt32(0), Text = reader.GetString(1) });
                    }
                }
            }
        }
        return questions;
    }
}
