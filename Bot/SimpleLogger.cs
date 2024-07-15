namespace Bot;

public class SimpleLogger(string filePath) {

	public void Info(string message) => LogText("Info", message);
	public void Log(string message) => LogText("Log", message);
	public void Warning(string message) => LogText("Warning", message);
	public void Error(string message) => LogText("Error", message);
	public void Critical(string message) => LogText("Critical", message);
	public void Discord(string message) => LogText("Discord", message);

	private void LogText(string logType, string text) => LogTextAsync(logType, text).Wait();

	public async Task InfoAsync(string message) => await LogTextAsync("Info", message);
	public async Task LogAsync(string message) => await LogTextAsync("Log", message);
	public async Task WarningAsync(string message) => await LogTextAsync("Warning", message);
	public async Task ErrorAsync(string message) => await LogTextAsync("Error", message);
	public async Task CriticalAsync(string message) => await LogTextAsync("Critical", message);
	public async Task DiscordAsync(string message) => await LogTextAsync("Discord", message);

	private async Task LogTextAsync(string logType, string text) {
		string log = $"{logType,-8} | {DateTime.Now,-10} | {text}\n\n";

		await Console.Out.WriteAsync(log);
		await File.AppendAllTextAsync(filePath, log);
	}
}
