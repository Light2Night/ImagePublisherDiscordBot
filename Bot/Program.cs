using Bot;
using Newtonsoft.Json;

string programDir = Directory.GetCurrentDirectory();
string dataDir = Path.Combine(programDir, "DataAndConfigurations");
string configurationsPath = Path.Combine(dataDir, "appsettings.json");

var dataDirectory = new DirectoryInfo(dataDir);
if (!dataDirectory.Exists)
	dataDirectory.Create();

if (!File.Exists(configurationsPath)) {
	var defaultSettings = new AppConfigurations("", 10, "Images", true);
	var json = JsonConvert.SerializeObject(defaultSettings);
	File.WriteAllText(configurationsPath, json);

	// Log

	return;
}