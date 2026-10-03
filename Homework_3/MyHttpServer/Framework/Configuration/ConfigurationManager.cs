using System.Text.Json;

namespace MyHttpServer.Framework.Configuration; 
//TODO клaсс конфигурации приложений синглтон  
public class ConfigurationManager
{
    private SettingsModel _settings;
    private static ConfigurationManager _configurationManager;
    
    private ConfigurationManager()
    {
        string settingsText = File.ReadAllText("settings.json");
        _settings = JsonSerializer.Deserialize<SettingsModel>(settingsText);
    }
    public static ConfigurationManager GetInstance()
    {
        if (_configurationManager == null)
        {
            _configurationManager = new ConfigurationManager();
        }
        
        return _configurationManager;
        
    }
    public SettingsModel SettingsModel => _settings;
}


