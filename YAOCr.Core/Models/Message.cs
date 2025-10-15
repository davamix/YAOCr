using System;

namespace YAOCr.Core.Models;

//public class Message {
//    public Guid Id { get; set; } = Guid.NewGuid();
//    public DateTime CreatedAt { get; set; } = DateTime.Now;
//    public DateTime UpdatedAt { get; set; } = DateTime.Now;
//    public string Content { get; set; }
//    public SenderEnum Sender { get; set; }
//}

public record Message(
    Guid Id,
    string Content,
    SenderEnum Sender,
    DateTime CreatedAt,
    DateTime UpdatedAt);
