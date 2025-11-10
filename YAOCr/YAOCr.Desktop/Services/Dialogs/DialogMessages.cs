namespace YAOCr.Services.Dialogs;

public class DialogContent {
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public static class DialogMessages {
    public static DialogContent ConfirmDeleteConversationMessage(string conversationName) =>
        new DialogContent {
            Title = "Delete Conversation",
            Message = $"""
                        Are you sure you want to delete this conversation?
        
                        {conversationName}
        
                        This action cannot be undone.
                        """
        };
}
