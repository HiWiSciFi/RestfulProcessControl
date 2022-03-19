using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestfulProcessControl.Util;

public static class Globals
{
	private class GlobalsModel
	{
		[JsonPropertyName("DB_ConnectionString")]
		public string? DatabaseConnectionString { get; set; }

		public string Serialize() => Serialize(this);
		public static string Serialize(GlobalsModel data) => JsonSerializer.Serialize(data);

		public static GlobalsModel? Deserialize(string data)
		{
			try { return JsonSerializer.Deserialize<GlobalsModel>(data); }
			catch { return null; }
		}
	}

	private static GlobalsModel? GlobalsObj { get; set; }

	public static string ConnectionString => GlobalsObj?.DatabaseConnectionString ?? @"Data Source=.\users.db";

	public static bool Reload()
	{
		var globalsFilePath = Path.Combine(".", "globals.json");
		if (!File.Exists(globalsFilePath))
		{
			Logger.LogWarning("\"Globals.json\" could not be found! It has to be present in \"{0}\"!",
				Directory.GetCurrentDirectory());
			return false;
		}
		using StreamReader reader = new(globalsFilePath);
		var json = reader.ReadToEnd();
		try
		{
			var obj = GlobalsModel.Deserialize(json);
			GlobalsObj = obj;
		}
		catch
		{
			Logger.LogWarning("Invalid \"Globals.json\" file!");
			return false;
		}
		return true;
	}
}