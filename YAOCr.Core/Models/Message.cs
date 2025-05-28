using System;

namespace YAOCr.Core.Models;

public class Message {
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Content { get; set; }
    public SenderEnum Sender { get; set; }
}
