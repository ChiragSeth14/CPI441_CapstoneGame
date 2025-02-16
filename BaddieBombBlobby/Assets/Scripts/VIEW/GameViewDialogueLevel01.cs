using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameViewDialogueLevel01 : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public List<TextMeshProUGUI> answerTexts;
    public List<Button> answerButtons;

    public void UpdateQuestionText(string question)
    {
        questionText.text = question;
    }

    public void UpdateAnswers(List<string> answers, List<UnityEngine.Events.UnityAction> answerActions)
    {
        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (i < answers.Count)
            {
                answerTexts[i].text = answers[i];
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(answerActions[i]);
            }
            else
            {
                answerTexts[i].text = "";
                answerButtons[i].onClick.RemoveAllListeners();
            }
        }
    }
}
