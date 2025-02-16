using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;

public class ChatGPTAPI : MonoBehaviour
{
    private string apiKey = "";
    private string apiUrl = "https://api.openai.com/v1/chat/completions";

    [System.Serializable]
    public class OpenAIRequest
    {
        public string model = "gpt-3.5-turbo";
        public List<Message> messages;
        public float temperature = 0.7f;
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;

        public Message(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
    }

    [System.Serializable]
    public class OpenAIResponse
    {
        public List<Choice> choices;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }

    public IEnumerator GetChatGPTResponse(string userInput, System.Action<string> callback)
    {
        OpenAIRequest requestData = new OpenAIRequest
        {
            messages = new List<Message>
            {
                new Message("system", "You are a witty and charming blob in a dating simulator. You refine the player's response to be more engaging."),
                new Message("user", userInput)
            }
        };

        string jsonData = JsonConvert.SerializeObject(requestData);
        byte[] postData = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            OpenAIResponse response = JsonConvert.DeserializeObject<OpenAIResponse>(request.downloadHandler.text);
            string refinedText = response.choices[0].message.content;
            callback(refinedText);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
            callback("Sorry, something went wrong.");
        }
    }
}
