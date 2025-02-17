using System;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class AnswerRepository
{
    private readonly string _dbPath;

    public AnswerRepository(string dbPath)
    {
        _dbPath = dbPath;
    }

    private SqliteConnection GetConnection()
    {
        return new SqliteConnection(_dbPath);
    }

    public List<Answer> GetAnswersForQuestion(int questionId)
    {
        List<Answer> answers = new List<Answer>();
        using (var connection = GetConnection())
        {
            connection.Open();
            string query = "SELECT Answers.IDAnswer, answer_text, IDAnswer FROM Answers WHERE question_id = @questionId";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@questionId", questionId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        answers.Add(new Answer { Id = reader.GetInt32(0), Text = reader.GetString(1), IDAnswer = reader.GetInt32(2) });
                    }
                }
            }
        }
        return answers;
    }
}
