using RestfulProcessControl.Models;

namespace RestfulProcessControl;

public class Application
{
	public string FolderName { get; }
	public ApplicationConfigModel? Config { get; private set; }
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

	private ApplicationConfigModel? LoadConfig() {
		var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "apps", FolderName, "config.json");
		if (!File.Exists(jsonFilePath)) return null;
		var jsonString = File.ReadAllText(jsonFilePath);
		return ApplicationConfigModel.Deserialize(jsonString);
	}

	public bool Start() {
		return false;
	}

	public bool Stop() {
		return false;
	}
}