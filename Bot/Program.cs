﻿using Bot;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

string programDir = Directory.GetCurrentDirectory();
string dataDir = Path.Combine(programDir, "DataAndConfigurations");
string configurationsPath = Path.Combine(dataDir, "appsettings.json");
string logPath = Path.Combine(dataDir, "log.txt");

var dataDirectory = new DirectoryInfo(dataDir);
if (!dataDirectory.Exists) {
	dataDirectory.Create();
}

var logger = new SimpleLogger(logPath);
logger.Info("The program has been launched");

if (!File.Exists(configurationsPath)) {
	var defaultSettings = new AppConfigurations("", 10, "Images", true);
	var json = JsonConvert.SerializeObject(defaultSettings);
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

using var client = new DiscordSocketClient(
		new DiscordSocketConfig {
			GatewayIntents = GatewayIntents.All
		}
);

client.Log += async message => await logger.DiscordAsync(message.ToString());

await client.LoginAsync(TokenType.Bot, configurations.BotToken);
await client.StartAsync();

