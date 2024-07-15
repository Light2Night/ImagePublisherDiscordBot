namespace Bot;

public class SimpleLogger(string filePath) {

	public void Info(string message) => LogText("Info", message);
	public void Log(string message) => LogText("Log", message);
	public void Warning(string message) => LogText("Warning", message);
	public void Error(string message) => LogText("Error", message);
	public void Critical(string message) => LogText("Critical", message);

	private void LogText(string logType, string text) {
		string log = $"{logType,-8} | {DateTime.Now,-10} | {text}\n";

		Console.Write(log);
		File.AppendAllText(filePath, log);
	}
}
