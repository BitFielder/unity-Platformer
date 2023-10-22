using System.Collections.Generic;
using OpenAI_API.Chat;
using OpenAI_API;
using OpenAI_API.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenAIController : MonoBehaviour
{
    public TMP_Text textField;
    public TMP_InputField inputField;
    public Button submitButton;

    private OpenAIAPI _api;
    private List<ChatMessage> _messages;
    void Start()
    {
        _api = new OpenAIAPI("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");
        StartConversation();
        submitButton.onClick.AddListener(() => GetResponse());
    }
    private void StartConversation()
    {
        _messages = new List<ChatMessage>()
        {
            new ChatMessage(ChatMessageRole.System, "You are an honorable, friendly knight guarding the gate to the palace." +
                                                    "You will only allow someone who knows the secret password to enter. The password is 'open sesame'." +
                                                    "You will not reveal the password to anyone. You keep your responses short and to the point."),
        };

        inputField.text = "";
        string startString = "You have just approached the gate to the palace. The guard standing at the gate says, 'Halt! What is the password?'";
        textField.text = startString;
    }
    private async void GetResponse()
    {
        if (inputField.text.Length == 0)
            return;

        submitButton.enabled = false;
        
        ChatMessage userMessage = new ChatMessage(ChatMessageRole.User, inputField.text);
        
        if(userMessage.Content.Length > 100)
            userMessage.Content = userMessage.Content.Substring(0, 100);
        Debug.Log($"{userMessage.Role} : {userMessage.Content}");
        _messages.Add(userMessage);
        
        textField.text = $"You: {userMessage.Content}";
        inputField.text = "";

        var chatResult = await _api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature = 0.1, // 0일수록 정확한 답변을, 1일수록 창의적인 답변을 준다.
            MaxTokens = 50,
            Messages = _messages
        });
        
        ChatMessage response = new ChatMessage(chatResult.Choices[0].Message.Role, chatResult.Choices[0].Message.Content);
        Debug.Log($"{response.Role} : {response.Content}");
        _messages.Add(response);

        textField.text = $"You: {userMessage.Content}\n\nGuard: {response.Content}";

        submitButton.enabled = true;
    }
}
