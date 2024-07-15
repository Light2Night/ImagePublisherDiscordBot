namespace Bot;

public class AppConfigurations(
	string botToken,
	int publicationIntervalSeconds,
	string imagesDirName,
	bool publishOnLaunch
) {

	public readonly string BotToken = botToken;
	public readonly int PublicationIntervalSeconds = publicationIntervalSeconds;
	public readonly string ImagesDirName = imagesDirName;
	public readonly bool PublishOnLaunch = publishOnLaunch;
}
