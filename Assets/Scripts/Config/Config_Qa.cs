public class Config_Qa
{
    public const string SERVER_URL = "https://qa-tournament.playdapp.com";
    public const string SERVER_PATH = "/spacecrush";
    public const int SERVER_PORT = 443;
    public readonly string[] ORIGIN = { "https://tournament.qa.playdapp.com", "http://localhost:3000" };
    public const string AES_KEY = "a41f3ba385449450a196e0c815dc139a";
    public const bool LOG_ENABLE = true;
    public const bool EDITOR = false;   // editor ½ÇÇà½Ã
    public const string ZONE = "qa";
    public const bool CHECKDEBUGGER = true;
}
