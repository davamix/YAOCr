using System;
using System.Collections.Generic;

namespace YAOCr.Core.Models;

//public class Message {
//    public Guid Id { get; set; } = Guid.NewGuid();
//    public string Content { get; set; }
//    public SenderEnum Sender { get; set; }
//    public DateTime CreatedAt { get; set; } = DateTime.Now;
//    public DateTime UpdatedAt { get; set; } = DateTime.Now;
//    public List<(string Path, string Content)> FilesContent { get; set; } = new();

//    public Message(Guid Id, string Content, SenderEnum Sender, DateTime CreatedAt, DateTime UpdatedAt,
//        List<(string Path, string Content)> FilesContent) {
        
//    }
//}

public record Message(
    Guid Id,
    string Content,
    SenderEnum Sender,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<(string Path, string Content)> FilesContent);
