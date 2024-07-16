using Bot;
using Bot.Exceptions;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

string programDir = Directory.GetCurrentDirectory();
string dataDir = Path.Combine(programDir, "DataAndConfigurations");
string configurationsPath = Path.Combine(dataDir, "appsettings.json");
string logPath = Path.Combine(dataDir, "log.txt");
string publishedImagesPath = Path.Combine(dataDir, "published.json");

var dataDirectory = new DirectoryInfo(dataDir);
if (!dataDirectory.Exists) {
	dataDirectory.Create();
}

var logger = new SimpleLogger(logPath);
logger.Info("The program has been launched");

if (!File.Exists(configurationsPath)) {
	var defaultSettings = new AppConfigurations("", 0, 10, "Images", true);
	var json = JsonConvert.SerializeObject(defaultSettings, Formatting.Indented);
	File.WriteAllText(configurationsPath, json);

	logger.Info("Created the default appsettings.json in the DataAndConfigurations directory. You must initialize this file and rerun the program");

	return;
}

AppConfigurations configurations;

try {
	var json = File.ReadAllText(configurationsPath);

	configurations = JsonConvert.DeserializeObject<AppConfigurations>(json)
		?? throw new NullReferenceException("JsonConvert.DeserializeObject returns null");
}
catch (NullReferenceException e) {
	logger.Error(e.Message);
	return;
}
catch (JsonSerializationException e) {
	logger.Error($"appsettings.json is invalid: {e.Message}");
	return;
}

string imagesDir = Path.Combine(dataDir, configurations.ImagesDirName);

if (!Directory.Exists(imagesDir)) {
	logger.Error($"Directory {imagesDir} does not exist");
	return;
}

HashSet<string> images = Directory.GetFiles(imagesDir).OrderBy(i => Guid.NewGuid()).ToHashSet();
HashSet<string> publishedImages;
try {
	if (File.Exists(publishedImagesPath)) {
		var json = File.ReadAllText(publishedImagesPath);

		publishedImages = JsonConvert.DeserializeObject<HashSet<string>>(json)
			?? throw new NullReferenceException("JsonConvert.DeserializeObject returns null");
	}
	else
		publishedImages = [];
}
catch (NullReferenceException e) {
	logger.Error(e.Message);
	return;
}

images = images.Where(i => !publishedImages.Contains(i)).ToHashSet();

using var client = new DiscordSocketClient(
	new DiscordSocketConfig {
		GatewayIntents = GatewayIntents.DirectMessages
	}
);

client.Log += async message => await logger.DiscordAsync(message.ToString());

await client.LoginAsync(TokenType.Bot, configurations.BotToken);
await client.StartAsync();

try {
	if (configurations.PublishOnLaunch) {
		await PublishAsync();
	}

	while (true) {
		await Task.Delay(configurations.PublicationIntervalSeconds * 1000);

		await PublishAsync();
	}
}
catch (NoImagesToPost e) {
	logger.Info(e.Message);
	return;
}
catch (Exception e) {
	logger.Critical($"Expected error: {e}");
}



async Task PublishAsync() {
	if (images.Count == 0)
		throw new NoImagesToPost();

	string imageName = images.First();

	await SendImageAsync(imageName);

	MoveToPublishdAndSave(imageName);
}

async Task SendImageAsync(string imageName) {
	var channel = (IMessageChannel)await client.GetChannelAsync(configurations.ChannelIdForPublish);

	await channel.SendFileAsync(imageName);

	await logger.InfoAsync($"Image {imageName} published");
}

void MoveToPublishdAndSave(string imageName) {
	images.Remove(imageName);
	publishedImages.Add(imageName);

	var json = JsonConvert.SerializeObject(publishedImages);
	File.WriteAllText(publishedImagesPath, json);
}