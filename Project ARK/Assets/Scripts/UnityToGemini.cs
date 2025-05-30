using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;
using Backend;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public enum GeminiRequestType { Option, Conversation, None }

public class UnityToGemini : MonoBehaviour
{
    // Necessary information for Gemini
    [BoxGroup("Gemini Info")] public string apiKey;
    [BoxGroup("Gemini Info")] public string url;
    [BoxGroup("Gemini Info")] public string arkActionURL;
    [BoxGroup("Gemini Info")] public string logTurnURL;
    [BoxGroup("Gemini Info")] public string chatEvaluateURL;
    [BoxGroup("Gemini Info")] public string putPromptURL;


    [BoxGroup("Ark Info")] public int arkSanity = 100;
    
    [BoxGroup("Gemini Info")] public string lastJsonRequest;
    [BoxGroup("Gemini Info")] public string OGPrompt;
    [BoxGroup("Gemini Info")] public string updatedPrompt;

    [BoxGroup("Error Handling")] static public event Action OnWebRequestError;
    [BoxGroup("Error Handling")] public int WebErrorCounter = 0;
    [BoxGroup("Error Handling")] private int maxWebError = 3;
    private Coroutine webErrorCoroutine;

    private static readonly List<string> InvalidJsonFormatPattern = new List<string>() { "```json", "```" };


    public string playerName;
    




    public static UnityToGemini Instance;

    public static event Action<string, GeminiRequestType> GeminiResponseCallback;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Instance.apiKey ??= apiKey;
            Instance.url ??= url;
            Instance.lastJsonRequest ??= lastJsonRequest;
            Instance.OGPrompt ??= OGPrompt;


            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Set the API key
        //apiKey = GeminiAPIVerifier.VerifiedApiKey;
    }

    // Start is called before the first frame update
    void Start()
    {


    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(UpdatePrompts("test", GeminiRequestType.None));
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(SendRequestToGeminiCo("TEST", GeminiRequestType.Conversation));
        }

    }

    public void SendRequest(string request, GeminiRequestType type)
    {
        StartCoroutine((SendRequestToGeminiCo(request, type)));
    }


    public IEnumerator SendRequestToGeminiCo(string request, GeminiRequestType type)
    {

        Debug.Log("Started API Validation Request");
        string url = "";

        switch (type)
        {
            case GeminiRequestType.Conversation:
                url = chatEvaluateURL;
                break;
        }
        //url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=" + apiKey;

        ChatRequest chatRequest = new ChatRequest();
        chatRequest.PlayerName = playerName;
        chatRequest.ArkSanity = Random.Range(0, 100);
        chatRequest.CurrentStats = BuildStats();
        chatRequest.ProjectedStats = BuildStats();
        chatRequest.DaysUntilNext = Random.Range(0, 10);
        chatRequest.PlayerAction = "TEST ACTION" + Random.Range(0, 100);
        // Serialize the object to JSON
        //Backend.GeminiRequest geminiRequest = new Backend.GeminiRequest();
        //geminiRequest.Contents = new List<Backend.Content>();
        //List<Backend.Part> tempParts = new List<Backend.Part>();
        //Backend.Part tempPart = new Backend.Part("\"text\":\"" + request + "\"");
        //tempParts.Add(tempPart);
        //Backend.Content tempContent = new Backend.Content();
        //tempContent.Role = "user";
        //tempContent.Parts = tempParts;
        //geminiRequest.Contents.Add(tempContent);

        string jsonData = JsonConvert.SerializeObject(chatRequest, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });
        Debug.Log("JSON DATA: " + jsonData);
        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");


            // Send the request
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                //resultText.text = www.error;

                Debug.LogError(www.error);
                //errorMessage = www.error;
                GeminiErrorResponse geminiErrorResponse = new GeminiErrorResponse();
                geminiErrorResponse = JsonConvert.DeserializeObject<GeminiErrorResponse>(www.downloadHandler.text);
                geminiErrorResponse = UnpackGeminiErrorResponse(www.downloadHandler.text);  
                if (geminiErrorResponse != null)
                {
                    if (geminiErrorResponse.Error != null)
                    {
                        if (!String.IsNullOrEmpty(geminiErrorResponse.Error.Details[0].Violations[0].QuotaId))
                        {
                            Debug.Log(geminiErrorResponse.Error.Details[0].Violations[0].QuotaId);

                        }
                        else
                        {
                            Debug.Log(www.error);
                        }
                    }

                }
                UponWebError();
                // Here is where we would relay to the user the reason as to why their key didn't validate (no tokens, wrong format, illegal key, etc)
            }
            else
            {
                // Unpack Chat Response
                Debug.Log("NO ERRORS");
                ChatResponse chatResponse = UnpackChatResponse(www.downloadHandler.text);
                if (chatResponse != null)
                {
                    Debug.Log("UNPACKED CR");
                    if(chatResponse.NpcSentenceToPlayer != null)
                        Debug.Log(chatResponse.NpcSentenceToPlayer);
                    Debug.Log("ARK DSANITY: " + chatResponse.NpcDeltaSanity);
                }
                
                GeminiResponseCallback?.Invoke(www.downloadHandler.text, type);
            }
        }
    }

    public IEnumerator SendKeyValidationToGemini(string apiInput)
    {
        Debug.Log("Started API Validation Request");
        string url = "";

        url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=" + apiInput;

        // Serialize the object to JSON
        Backend.GeminiRequest geminiRequest = new Backend.GeminiRequest();
        geminiRequest.Contents = new List<Backend.Content>();
        List<Backend.Part> tempParts = new List<Backend.Part>();
        Backend.Part tempPart = new Backend.Part("Test validation request");
        tempParts.Add(tempPart);
        Backend.Content tempContent = new Backend.Content();
        tempContent.Role = "user";
        tempContent.Parts = tempParts;
        geminiRequest.Contents.Add(tempContent);

        string jsonData = JsonConvert.SerializeObject(geminiRequest, Formatting.None);
        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");


            // Send the request
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"API Key validation failed: {www.error}");

                // Trigger callback with error information so UI can be updated
                GeminiResponseCallback?.Invoke("ERROR:" + www.error, GeminiRequestType.None);
            }
            else
            {
                // Set the API key and notify listeners that validation was successful
                apiKey = apiInput.Trim();
                Debug.Log("API Key validation successful");

                // Trigger the callback with the successful response
                GeminiResponseCallback?.Invoke(www.downloadHandler.text, GeminiRequestType.None);
            }
        }
    }
    
    
    public IEnumerator UpdatePrompts(string request, GeminiRequestType type)
    {

        Debug.Log("Started Prompt Update Request");
        string url = "";

        //url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=" + apiKey;
        url = putPromptURL;

        PromptRequest promptRequest = new PromptRequest();
        promptRequest.PromptId = "chatPrompt";
        promptRequest.Version = 1;
        UpdatePrompt();
        promptRequest.Text = updatedPrompt;
        // Serialize the object to JSON
        // Backend.GeminiRequest geminiRequest = new Backend.GeminiRequest();
        // geminiRequest.Contents = new List<Backend.Content>();
        // List<Backend.Part> tempParts = new List<Backend.Part>();
        // Backend.Part tempPart = new Backend.Part("\"text\":\"" + request + "\"");
        // tempParts.Add(tempPart);
        // Backend.Content tempContent = new Backend.Content();
        // tempContent.Role = "user";
        // tempContent.Parts = tempParts;
        // geminiRequest.Contents.Add(tempContent);

        string jsonData = JsonConvert.SerializeObject(promptRequest, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });
        Debug.Log("JSON DATA: " + jsonData);
        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);
        using (UnityWebRequest www = new UnityWebRequest(url, "PUT"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");


            // Send the request
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                //resultText.text = www.error;

                Debug.LogError(www.error);
                //errorMessage = www.error;
                GeminiErrorResponse geminiErrorResponse = new GeminiErrorResponse();
                geminiErrorResponse = JsonConvert.DeserializeObject<GeminiErrorResponse>(www.downloadHandler.text);
                geminiErrorResponse = UnpackGeminiErrorResponse(www.downloadHandler.text);
                if (geminiErrorResponse != null)
                {
                    if (geminiErrorResponse.Error != null)
                    {
                        if (!String.IsNullOrEmpty(geminiErrorResponse.Error.Details[0].Violations[0].QuotaId))
                        {
                            Debug.Log(geminiErrorResponse.Error.Details[0].Violations[0].QuotaId);

                        }
                        else
                        {
                            Debug.Log(www.error);
                        }
                    }

                }
                UponWebError();
                // Here is where we would relay to the user the reason as to why their key didn't validate (no tokens, wrong format, illegal key, etc)
            }
            else
            {

                //GeminiResponseCallback?.Invoke(www.downloadHandler.text, type);



            }
        }
    }
    
    

    #region Unpack Responses
    public GeminiResponse UnpackGeminiResponse(string rawResponse)
    {

        GeminiResponse geminiResponse = JsonConvert.DeserializeObject<GeminiResponse>(rawResponse);
        if (geminiResponse != null && geminiResponse.UsageMetadata != null)
            Debug.Log(geminiResponse.UsageMetadata?.TotalTokenCount);
        CleanText(geminiResponse.Candidates[0].Contents.Parts[0].Text);
        return geminiResponse;
    }

    public GeminiErrorResponse UnpackGeminiErrorResponse(string rawResponse)
    {
        GeminiErrorResponse geminiErrorResponse = JsonConvert.DeserializeObject<GeminiErrorResponse>(rawResponse);
        if (geminiErrorResponse != null && geminiErrorResponse.Error != null)
        {
            Debug.Log(geminiErrorResponse.Error.ToString());
        }
        return geminiErrorResponse;
    }

    public ChatResponse UnpackChatResponse(string rawResponse)
    {
        ChatResponse chatResponse = JsonConvert.DeserializeObject<ChatResponse>(rawResponse);
        if (chatResponse != null && chatResponse.UsageMetadata != null)
        {
            Debug.Log(chatResponse.UsageMetadata.TotalTokenCount);
        }

        //CleanText(chatResponse.NpcReactions.NpcSentenceToPlayer);
        return chatResponse;
    }
    
    #endregion



    /// <summary>
    /// Get the content based on the Gemini role and the text (prompt).
    /// </summary>
    /// <param name="geminiRole">Gemini role in the request to Gemini.</param>
    /// <param name="text">Complete prompt in the request to Gemini.</param>
    /// <returns>Content object which could in AiRequest</returns>
    /// <exception cref="ArgumentOutOfRangeException">If non-existent role used, throw ArgumentOutOfRangeException</exception>
    private static Backend.Content BuildContent(Backend.GeminiRole geminiRole, string text)
    {
        Backend.Content tempContent = new Backend.Content();
        tempContent.Role = geminiRole.ToString();
        Backend.Part tempPart = new Backend.Part(text);
        tempContent.Parts = new List<Backend.Part>();
        tempContent.Parts.Add(tempPart);
        Debug.Log("Temp Part: " + tempPart.ToString());
        return tempContent;
        //return new Content
        //{
        //    Role = geminiRole.ToString()
        //    //switch
        //    //{
        //    //    GeminiRole.User => GeminiRole.User,
        //    //    GeminiRole.Model => GeminiRole.Model,
        //    //    _ => throw new ArgumentOutOfRangeException()
        //    //}
        //    ,
        //    Parts =
        //    {
        //        new Part(text)
        //        {
        //            Text = text,
        //        }
        //    }
        //};
    }

    /// <summary>
    /// Clean the text from the invalid patterns (here only invalid json format is applied).
    /// </summary>
    /// <param name="text">Text to be cleaned.</param>
    /// <returns>Cleaned text.</returns>
    public string CleanText(string text)
    {
        foreach (var pattern in InvalidJsonFormatPattern)
        {
            text = text.Replace(pattern, string.Empty);
        }
        return text;
    }




    public void UponWebError()
    {
        WebErrorCounter++;
        Debug.Log($"Web error occurred. Count: {WebErrorCounter}");
        //SaveManager.Instance?.SaveGame(); // Save the game on error

        if (WebErrorCounter >= maxWebError)
        {
            OnWebRequestError.Invoke();
        }

        // Start the coroutine if it is not already running
        if (webErrorCoroutine == null)
        {
            webErrorCoroutine = StartCoroutine(WebErrorTimerCoroutine());
        }
    }

    private IEnumerator WebErrorTimerCoroutine()
    {
        while (WebErrorCounter > 0)
        {
            yield return new WaitForSeconds(30f);
            WebErrorCounter--;
            Debug.Log($"30 seconds passed. Decreasing error count. New count: {WebErrorCounter}");
        }
        // Once the counter reaches zero, reset the coroutine reference so we can restart it later.
        webErrorCoroutine = null;
    }

    public void UpdatePrompt()
    {
        updatedPrompt = OGPrompt;
        updatedPrompt.Replace("{0}", playerName);
        updatedPrompt.Replace("{1}", $"{arkSanity} out of 100"); // Ark Sanity
        updatedPrompt.Replace("{2}", playerName); // Current Status of Resources
        updatedPrompt.Replace("{3}", playerName); // Resources at next time cycle
        updatedPrompt.Replace("{4}", playerName); // Time till next cycle updates
        updatedPrompt.Replace("{5}", playerName); // Player chosen action -- nullable
        
    }
    
    // public virtual void AddToChatsWithHeader(CharacterRole role, string content, string orderInformation, string ingredientInformation, string npcEmotion)
    // {
    // if (chats == null)
    //     chats = new List<(string role, string content, string orderInformation, string ingredientInformation, string npcEmotion, int visitIndex, bool showInChatHistory)>();
    // switch (role)
    // {
    //     case CharacterRole.Player:
    //
    //         // Determine which sense to use
    //         if(desiredSense != -1)
    //         {
    //             switch (desiredSense) {
    //                 case 0:
    //                     chats.Add(new("user", $"{{\"Player\": \"{content}\",", $"\"Order Information\": \"{orderInformation}\",", $"\"Ingredient Information\": \"{ingredientInformation}\",", $"\"sense\": \"{visionSense}\"}}", visitNumber, true));
    //                     visionSense = "";
    //                     break;
    //                 case 1:
    //                     chats.Add(new("user", $"{{\"Player\": \"{content}\",", $"\"Order Information\": \"{orderInformation}\",", $"\"Ingredient Information\": \"{ingredientInformation}\",", $"\"sense\": \"{smellSense}\"}}", visitNumber, true));
    //                     smellSense = "";
    //                     break;
    //                 case 2:
    //                     chats.Add(new("user", $"{{\"Player\": \"{content}\",", $"\"Order Information\": \"{orderInformation}\",", $"\"Ingredient Information\": \"{ingredientInformation}\",", $"\"sense\": \"{feelSense}\"}}", visitNumber, true));
    //                     feelSense = "";
    //                     break;
    //                 case 3:
    //                     chats.Add(new("user", $"{{\"Player\": \"{content}\",", $"\"Order Information\": \"{orderInformation}\",", $"\"Ingredient Information\": \"{ingredientInformation}\",", $"\"sense\": \"{hearingSense}\"}}", visitNumber, true));
    //                     hearingSense = "";
    //                     break;
    //                 case 4:
    //                     chats.Add(new("user", $"{{\"Player\": \"{content}\",", $"\"Order Information\": \"{orderInformation}\",", $"\"Ingredient Information\": \"{ingredientInformation}\",", $"\"sense\": \"{DetermineHunger(hungerSense)}\"}}", visitNumber, true));
    //                     hungerSense = -1;
    //                     break;
    //                 case 5:
    //                     chats.Add(new("user", $"{{\"Player\": \"{content}\",", $"\"Order Information\": \"{orderInformation}\",", $"\"Ingredient Information\": \"{ingredientInformation}\",", $"\"sense\": \"{DetermineThirst(thirstSense)}\"}}", visitNumber, true));
    //                     thirstSense = -1;
    //                     break;
    //                 default:
    //                     chats.Add(new("user", $"{{\"Player\": \"{content}\",", $"\"Order Information\": \"{orderInformation}\",", $"\"Ingredient Information\": \"{ingredientInformation}\",", $"\"sense\": \"\"}}", visitNumber, true));
    //
    //                     break;
    //             }
    //
    //         }
    //         else
    //         {
    //             // Randomly pick available ones
    //             List<int> desiredSenseIndex = new List<int>();
    //             if (visionSense != "")
    //             {
    //                 desiredSenseIndex.Add(0);
    //             }
    //             if (smellSense != "")
    //             {
    //                 desiredSenseIndex.Add(1);
    //             }
    //             if (feelSense != "")
    //             {
    //                 desiredSenseIndex.Add(2);
    //             }
    //             if (hearingSense != "")
    //             {
    //                 desiredSenseIndex.Add(3);
    //             }
    //             if (hungerSense != -1)
    //             {
    //                 desiredSenseIndex.Add(4);
    //             }
    //             if (thirstSense != -1)
    //             {
    //                 desiredSenseIndex.Add(5);
    //             }
    //             int randomIndex = UnityEngine.Random.Range(0, desiredSenseIndex.Count);
    //             switch (desiredSenseIndex[randomIndex])
    //             {
    //                 case 0:
    //                     chats.Add(new("user", $"{{\"Player\": \"{content}\",", $"\"Order Information\": \"{orderInformation}\",", $"\"Ingredient Information\": \"{ingredientInformation}\",", $"\"sense\": \"{visionSense}\"}}", visitNumber, true));
    //                     visionSense = "";
    //                     break;
    //                 case 1:
    //                     chats.Add(new("user", $"{{\"Player\": \"{content}\",", $"\"Order Information\": \"{orderInformation}\",", $"\"Ingredient Information\": \"{ingredientInformation}\",", $"\"sense\": \"{smellSense}\"}}", visitNumber, true));
    //                     smellSense = "";
    //                     break;
    //                 case 2:
    //                     chats.Add(new("user", $"{{\"Player\": \"{content}\",", $"\"Order Information\": \"{orderInformation}\",", $"\"Ingredient Information\": \"{ingredientInformation}\",", $"\"sense\": \"{feelSense}\"}}", visitNumber, true));
    //                     feelSense = "";
    //                     break;
    //                 case 3:
    //                     chats.Add(new("user", $"{{\"Player\": \"{content}\",", $"\"Order Information\": \"{orderInformation}\",", $"\"Ingredient Information\": \"{ingredientInformation}\",", $"\"sense\": \"{hearingSense}\"}}", visitNumber, true));
    //                     hearingSense = "";
    //                     break;
    //                 case 4:
    //                     chats.Add(new("user", $"{{\"Player\": \"{content}\",", $"\"Order Information\": \"{orderInformation}\",", $"\"Ingredient Information\": \"{ingredientInformation}\",", $"\"sense\": \"{DetermineHunger(hungerSense)}\"}}", visitNumber, true));
    //                     hungerSense = -1;
    //                     break;
    //                 case 5:
    //                     chats.Add(new("user", $"{{\"Player\": \"{content}\",", $"\"Order Information\": \"{orderInformation}\",", $"\"Ingredient Information\": \"{ingredientInformation}\",", $"\"sense\": \"{DetermineThirst(thirstSense)}\"}}", visitNumber, true));
    //                     thirstSense = -1;
    //                     break;
    //
    //                 default:
    //                     chats.Add(new("user", $"{{\"Player\": \"{content}\",", $"\"Order Information\": \"{orderInformation}\",", $"\"Ingredient Information\": \"{ingredientInformation}\",", $"\"sense\": \"\"}}", visitNumber, true));
    //                     break;
    //             }
    //
    //
    //         }
    //         //chats.Add(new("user", $"{{\"Player\": \"{content}\"}}", ""));
    //         break;
    //     case CharacterRole.Customer:
    //         if (content.Contains("${0}"))
    //         {
    //             MilkshakeDecision milkDecision = CustomerManager.Instance.currentCustomer.GetComponent<MilkshakeDecision>();
    //             if(milkDecision.DesiredMilkshake != null)
    //             {
    //                 string request = milkDecision.DesiredMilkshakeToString();
    //                 content = content.Replace("${0}", request);
    //             }
    //
    //         }
    //         chats.Add(new("model", $"{{\"npcSentenceToPlayer\": \"{content}\",", "", "",  $"\"npcEmotion\": \"{npcEmotion}\"}}", visitNumber, true));
    //         break;
    //     case CharacterRole.Judge:
    //         chats.Add(new("model", $"{{\"Judge\": \"{content}\"}}", "", "", "", 0, false));
    //         break;
    // } }


    public List<StatStructure> BuildStats()
    {
        List<StatStructure> newList = new List<StatStructure>();
        StatStructure foodStat = new StatStructure();
        foodStat.Name = "food";
        foodStat.Value = (int)ResourcesManager.Instance.foodSupply;
        newList.Add(foodStat);
        StatStructure fuelStat = new StatStructure();
        fuelStat.Name = "fuel";
        fuelStat.Value = (int)ResourcesManager.Instance.fuel;
        newList.Add(fuelStat);
        StatStructure moraleStat = new StatStructure();
        moraleStat.Name = "morale";
        moraleStat.Value = (int)ResourcesManager.Instance.morale;
        newList.Add(moraleStat);
        StatStructure healthStat = new StatStructure();
        healthStat.Name = "health";
        healthStat.Value = (int)ResourcesManager.Instance.healthPoints;
        newList.Add(healthStat);
        StatStructure gearsStat = new StatStructure();
        gearsStat.Name = "gears";
        gearsStat.Value = (int)ResourcesManager.Instance.gears;
        newList.Add(gearsStat);
        StatStructure securityStat = new StatStructure();
        securityStat.Name = "security";
        securityStat.Value = (int)ResourcesManager.Instance.securityLevel;
        newList.Add(securityStat);
        StatStructure explorationStat = new StatStructure();
        explorationStat.Name = "exploration";
        explorationStat.Value = (int)ResourcesManager.Instance.explorationPoints;
        newList.Add(explorationStat);
        return newList;
    }

    public List<StatStructure> BuildProjectedStats()
    {
        List<StatStructure> newList = new List<StatStructure>();
        
        return newList;
    }

    public List<CrewMember> BuildCrewList()
    {
        List<CrewMember> newList = new List<CrewMember>();
        foreach (Medic m in CrewManager.Instance.medics)
        {
            CrewMember newCrew = new CrewMember();
            newCrew.Name = m.name;
            newCrew.Role = "medic";
            newList.Add(newCrew);
        }
        foreach (Engineer e in CrewManager.Instance.engineers)
        {
            CrewMember newCrew = new CrewMember();
            newCrew.Name = e.name;
            newCrew.Role = "engineer";
            newList.Add(newCrew);
        }
        foreach (Enforcer e in CrewManager.Instance.enforcers)
        {
            CrewMember newCrew = new CrewMember();
            newCrew.Name = e.name;
            newCrew.Role = "Enforcer";
            newList.Add(newCrew);
        }
        foreach (Farmer f in CrewManager.Instance.farmers)
        {
            CrewMember newCrew = new CrewMember();
            newCrew.Name = f.name;
            newCrew.Role = "farmer";
            newList.Add(newCrew);
        }
        foreach (Navigator n in CrewManager.Instance.navigators)
        {
            CrewMember newCrew = new CrewMember();
            newCrew.Name = n.name;
            newCrew.Role = "navigator";
            newList.Add(newCrew);
        }

        return newList;
    }

    public void CurrentStats()
    {
        
    }





    public void GoToEndState()
    {
        SceneManager.LoadScene("EndState");
    }

    public void GoToMenuState()
    {
        SceneManager.LoadScene("MenuState");
        Destroy(gameObject);
    }

    private void HandleObjectClicked(GameObject clickedObject)
    {
        Debug.Log($"Clicked on: {clickedObject.name}");
    }

    private void HandleObjectHovered(GameObject hoveredObject)
    {
        Debug.Log($"Hovering over: {hoveredObject.name}");
    }

}
