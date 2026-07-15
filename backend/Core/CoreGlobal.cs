namespace Core;

public static class CoreGlobal
{
    public static Settings Settings { get; set; } = new Settings(); 
}

public class Settings
{
    public string? GeminiApiKey { get; set; }
    public string GeminiModelId { get; set; } = "gemini-3.1-flash-lite";
}