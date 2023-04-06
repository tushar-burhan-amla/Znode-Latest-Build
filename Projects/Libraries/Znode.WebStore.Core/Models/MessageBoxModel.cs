namespace Znode.Engine.WebStore.Models
{
    /// <summary>
    /// This is the model for the Message Box operations
    /// </summary>
    public class MessageBoxModel
    {
        public string Message { get; set; }
        public string Type { get; set; }
        public bool IsFadeOut { get; set; }
        public int FadeOutMilliSeconds { get; set; }
    }

    /// <summary>
    /// This is enum for the Message Box type 
    /// </summary>
    public enum NotificationType
    {
        error,
        success,
        info
    }
}