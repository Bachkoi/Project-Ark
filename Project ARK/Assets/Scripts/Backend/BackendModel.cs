using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

namespace Backend
{

    #region Audits
    public class AuditRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        
#nullable enable
        [JsonProperty("prompt")]
        public string? Prompt { get; set; }
#nullable disable
    }

    public class AuditResponse
    {
        [JsonProperty("auditResult")]
        public string AuditResult { get; set; }
        
        [JsonProperty("usageMetadata")]
        public UsageMetadata UsageMetadata { get; set; }
        
        [JsonProperty("errorCode")]
        public GeminiErrorCode? ErrorCode { get; set; }
        
#nullable enable
        [JsonProperty("errorMessage")]
        public string? ErrorMessage { get; set; }
#nullable disable
    }


    #endregion Audits

    #region Prompt

    public class PromptRequest
    {
        [JsonProperty("promptId")]
        public string PromptId { get; set; }
        [JsonProperty("version")]
        public int Version { get; set; }
#nullable enable
        [JsonProperty("text")]
        public string? Text { get; set; }
#nullable disable
    }

    public class PromptResponse
    {
        
    }
    
    #endregion Prompt
    

    #region Chat
    [System.Serializable]
    public class ChatRequest
    {
        //[JsonProperty("character")]
        //public Character Character { get; set; }

        [JsonProperty("playerName")]
        public string? PlayerName { get; set; } 
        [JsonProperty("arkSanity")]
        public int ArkSanity { get; set; }
        [JsonProperty("currentStats")]
        public List<StatStructure> CurrentStats { get; set; }
        [JsonProperty("projectedStats")]
        public List<StatStructure> ProjectedStats { get; set; }
        [JsonProperty("daysUntilNext")]
        public int DaysUntilNext { get; set; }
        [JsonProperty("playerAction")]
        public string PlayerAction { get; set; }

        //[JsonProperty("aiRequest")]
        //public GeminiRequest AiRequest { get; set; }

#nullable enable
        //[JsonProperty("prompt")]
        //public string? Prompt { get; set; }
#nullable disable


    }

    [System.Serializable]
    public class StatStructure
    {
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("value")]
        public int Value { get; set; }
    }

    public class NpcReactions
    {
        [JsonProperty("npcSentenceToPlayer")]
        public string NpcSentenceToPlayer { get; set; }

        [JsonProperty("npcDeltaSanity")]
        public int NpcDeltaSanity { get; set; }

        //[JsonProperty("npcSanity")]
        //public string NpcSanity { get; set; }
    }

    [System.Serializable]
    public class ChatResponse
    {
        //[JsonProperty("npcReactions")]
        //public NpcReactions NpcReactions { get; set; }

        [JsonProperty("npcSentenceToPlayer")]
        public string NpcSentenceToPlayer { get; set; }
        [JsonProperty("npcDeltaSanity")]
        public int NpcDeltaSanity { get; set; }
        
        [JsonProperty("usageMetadata")]
        public UsageMetadata UsageMetadata { get; set; }

        [JsonProperty("errorCode")]
        //[JsonIgnore( = JsonIgnoreAttribute.)]
        public GeminiErrorCode? ErrorCode { get; set; }

#nullable enable
        [JsonProperty("errorMessage")]
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ErrorMessage { get; set; }
#nullable disable
    }
    #endregion

    
    #region Ark

    [System.Serializable]
    public class ArkRequest
    {
        [JsonProperty("playerId")]
        public string PlayerId { get; set; }
        [JsonProperty("turnNumber")]
        public int TurnNumber { get; set; }
        [JsonProperty("currentStats")]
        public List<StatStructure> CurrentStats { get; set; }
        [JsonProperty("projectedStats")]
        public List<StatStructure> ProjectedStats { get; set; }
        [JsonProperty("crewList")]
        public List<CrewMember> CrewList { get; set; }
        [JsonProperty("pastActions")]
        public List<PastActions> PastActions { get; set; }
        [JsonProperty("sanity")]
        public int Sanity { get; set; }
    }

    [System.Serializable]
    public class ArkResponse
    {
        [JsonProperty("names")]
        public List<string> Names { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
    
    
    #endregion


    [System.Serializable]
    public class CrewMember
    {
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("role")]
        public string? Role { get; set; }
    }

    [System.Serializable]
    public class PastActions
    {
        [JsonProperty("turn")]
        public int Turn { get; set; }
        [JsonProperty("action")]
        public string? Action { get; set; }
    }

    #region Gemini Models
    // Gemini request models
    [System.Serializable]
    public class GeminiRequest
    {
        [JsonProperty("contents")]
        public List<Content> Contents { get; set; }

        [JsonProperty("systemInstruction")]
        public Content SystemInstruction { get; set; }


    }

    [System.Serializable]
    public class Content
    {
        [JsonProperty("role")]
        public string Role { get; set; }
        [JsonProperty("parts")]
        public List<Part> Parts { get; set; }


    }

    [System.Serializable]
    public class Part
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        public Part(string text)
        {
            Text = text;
        }
    }

    // Gemini response models
    [System.Serializable]
    public class GeminiResponse
    {
        [JsonProperty("candidates")]
        public List<Candidate> Candidates { get; set; }
        [JsonProperty("usageMetadata")]
        public UsageMetadata UsageMetadata { get; set; }
    }

    [System.Serializable]
    public class Candidate
    {
        [JsonProperty("content")]
        public Content Contents { get; set; }
        [JsonProperty("finishReason")]
        public string FinishReason { get; set; }
        [JsonProperty("index")]
        public int Index { get; set; }
        [JsonProperty("safeRatings")]
        public List<SafetyRating> SafetyRatings { get; set; }
    }

    [System.Serializable]
    public class SafetyRating
    {
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("probability")]
        public string Probability { get; set; }
    }

    [System.Serializable]
    public class UsageMetadata
    {
        [JsonProperty("promptTokenCount")]
        public int PromptTokenCount { get; set; }
        [JsonProperty("candidatesTokenCount")]
        public int CandidatesTokenCount { get; set; }
        [JsonProperty("totalTokenCount")]
        public int TotalTokenCount { get; set; }

    }
    #endregion

    [System.Serializable]
    public class GeminiErrorResponse
    {
        [JsonProperty("error")]
        public GeminiErrorDetail Error { get; set; }
    }

    [System.Serializable]

    public class GeminiErrorDetail
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("details")]
        public List<GeminiErrorDetailItem> Details { get; set; }
    }

    [System.Serializable]
    public class GeminiErrorDetailItem
    {
        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("violations")]
        public List<GeminiQuotaViolation> Violations { get; set; }

        [JsonProperty("links")]
        public List<GeminiHelpLink> Links { get; set; }

        [JsonProperty("retryDelay")]
        public string RetryDelay { get; set; }
    }

    [System.Serializable]
    public class GeminiQuotaViolation
    {
        [JsonProperty("quotaMetric")]
        public string QuotaMetric { get; set; }

        [JsonProperty("quotaId")]
        public string QuotaId { get; set; }

        [JsonProperty("quotaDimensions")]
        public Dictionary<string, string> QuotaDimensions { get; set; }

        [JsonProperty("quotaValue")]
        public string QuotaValue { get; set; }
    }

    [System.Serializable]
    public class GeminiHelpLink
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }


    public class GameEventModel
    {
        [JsonProperty("event")]
        public EventName Event { get; set; }

        [JsonProperty("gameId")]
        public string GameId { get; set; }

        [JsonProperty("balance")]
        public float Balance { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("geminiStatusCode")]
        public long GeminiStatusCode { get; set; }

        [JsonProperty("geminiErrorMessage")]
        public string GeminiErrorMessage { get; set; }

        [JsonProperty("geminiAPIKey")]
        public string GeminiAPIKey { get; set; }
    }

    #region Enum
    // Internal conflict status of customers
    public enum InternalConflictStatus
    {
        VeryGood = 2,
        Good = 1,
        Neutral = 0,
        Bad = -1,
        VeryBad = -2
    }

    // Character of customers
    public enum Character
    {
        Soldier = 0,
    }


    // Gemini Error Code
    public enum GeminiErrorCode
    {
        ApiKeyInvalidError = 101, // Tested
        RequestFormatInvalidError = 102, // Tested
        RequestTooFrequentError = 201, // Tested
        RequestTooLargeError = 202, // Failed to reach, will test again when we have image/file data sent to Gemini
        GeminiInternalError = 301, // Have not encountered
        JsonParsingError = 303,
        UnknownError = 401, // Have not encountered
        RateLimitReached = 429
    }

    public enum EventName
    {
        GameStarted, // Game started
        GameQuit, // Game quit

    }
    // Gemini message role
    public enum GeminiRole
    {
        User = 0,
        Model = 1,
        System = 2
    }

    #endregion
}