public class Config_Prod
{
    public const string SERVER_URL = "https://prod-tournament.playdapp.com";
    public const string SERVER_PATH = "/spacecrush";
    public const int SERVER_PORT = 443;
    public readonly string[] ORIGIN = { "https://tournament.playdapp.com", "https://tournament.staging.playdapp.com" };
    public const string AES_KEY = "a41f3ba385449450a196e0c815dc139a";
    public const bool LOG_ENABLE = false;
    public const bool EDITOR = false;
    public const string ZONE = "prod";
    public const bool CHECKDEBUGGER = true;
}
