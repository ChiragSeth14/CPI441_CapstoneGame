using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public ChatGPTAPI chatGPT;
    public Text responseText;

    public void OnUserSelectsDialogueOption(string userInput)
    {
        StartCoroutine(chatGPT.GetChatGPTResponse(userInput, UpdateDialogueResponse));
    }

    private void UpdateDialogueResponse(string refinedResponse)
    {
        responseText.text = refinedResponse;
    }
}
