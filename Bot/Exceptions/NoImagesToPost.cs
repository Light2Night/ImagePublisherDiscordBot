namespace Bot.Exceptions;

public class NoImagesToPost : Exception {
	public NoImagesToPost(string message) : base(message) { }
	public NoImagesToPost() : this("There are no images to post") { }
}
