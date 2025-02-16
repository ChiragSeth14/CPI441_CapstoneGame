using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Collections;
using System.IO;

// CONTROLLER (BUSINESS LOGIC LAYER)
public class GameController : MonoBehaviour
{
    private DatabasePath databasePath;
    private QuestionRepository _questionRepository;
    private AnswerRepository _answerRepository;
    private CharacterRepository _characterRepository;
    private LevelRepository _levelRepository;  // Add a repository for levels
    private List<Question> _questions;
    private Level _currentLevel;  // Use Level instead of just level number
    private int _currentQuestionIndex = 0;

    public TextMeshProUGUI questionText;
    public List<TextMeshProUGUI> answerTexts;
    public List<Button> answerButtons;
    public TextMeshProUGUI feedbackText;  // To display feedback (e.g., "Correct" or "Incorrect")
    public TextMeshProUGUI characterNameText;  // To display the character's name
    public Image characterImage;  // To display the character's image
    public TextMeshProUGUI levelText;  // To display the level number
    public int levelNumber;

    void Start()
    {
       

        databasePath = new DatabasePath();
        // string dbPath = "URI=file:C:/Users/nawaf/OneDrive/Desktop/CPI441/database/databaseMonster.db";
        string dbPath = databasePath.DbPath;
        _questionRepository = new QuestionRepository(dbPath);
        _answerRepository = new AnswerRepository(dbPath);
        _characterRepository = new CharacterRepository(dbPath);
        _levelRepository = new LevelRepository(dbPath);  // Initialize level repository

        // Start with level 1
        _currentLevel = _levelRepository.GetLevel(levelNumber);
        _questions = _questionRepository.GetQuestionsByLevel(_currentLevel);
        UpdateLevelUI();  // Display the current level number
        DisplayQuestion();
    }

    private void Update()
    {
        
    }


    void DisplayQuestion()
    {
        if (_questions.Count == 0 || _currentQuestionIndex >= _questions.Count) return;

        Question currentQuestion = _questions[_currentQuestionIndex];
        questionText.text = currentQuestion.Text;
        List<Answer> answers = _answerRepository.GetAnswersForQuestion(currentQuestion.Id);

        // Clear previous feedback and character info
        feedbackText.text = "";
        characterNameText.text = "";
        characterImage.sprite = characterImage.sprite;


        // Load character data for the current level and question
        var character = _characterRepository.GetCharacterForQuestion(currentQuestion.Id, _currentLevel.LevelNumber);
        characterNameText.text = "Name :"+" "+character.Name;

        

        // Display answers
        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (i < answers.Count)
            {
                answerTexts[i].text = answers[i].Text;
                int answerId = answers[i].Id;
                answerButtons[i].onClick.AddListener(() => CheckAnswer(answerId));
            }
        }
    }

    void CheckAnswer(int answerId)
    {
        if (answerId == 1) // Correct answer
        {
            feedbackText.text = "Correct";
            feedbackText.color = Color.green;
            Invoke("NextQuestion", 1f); // 1 second delay before displaying next question
        }
        else
        {
            feedbackText.text = "Incorrect";
            feedbackText.color = Color.red;
        }

        Debug.Log("Answer selected: " + answerId);
    }

    void NextQuestion()
    {
        _currentQuestionIndex++;
        if (_currentQuestionIndex < _questions.Count)
        {
            DisplayQuestion(); // Show next question
        }
        else
        {
            if (_currentQuestionIndex >= _questions.Count)
            {
               // feedbackText.text = "You've completed all questions for level "+levelNumber;
              //  feedbackText.color = Color.yellow;
            }
            
        }
    }

    // Update the level number in the UI
    void UpdateLevelUI()
    {
        levelText.text = "Level: " + _currentLevel.LevelNumber;  // Display the current level
    }
}
