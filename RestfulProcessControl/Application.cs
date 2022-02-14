namespace RestfulProcessControl;

public class Application
{
	public string FolderName { get; }
	public ApplicationConfig? Config { get; private set; }
	public bool Running { get; private set; }

	public Application() {
		FolderName = "Testapp";
		Config = null;
		Running = false;
	}

	public Application(string folderName) {
		FolderName = folderName;
	}

	public void ReloadConfig() => Config = LoadConfig();

	private ApplicationConfig? LoadConfig() {
		string jsonFile = Path.Combine(Directory.GetCurrentDirectory(), "apps", FolderName, "config.json");
		if (!File.Exists(jsonFile)) return null;
		string jsonString = File.ReadAllText(jsonFile);
		ApplicationConfig? config = ApplicationConfig.Deserialize(jsonString);
		if (config != null) return config;
		return null;
	}

	public bool Start() {
		return false;
	}

	public bool Stop() {
		return false;
	}
}