using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAOCr.Core.Models;

public enum SenderEnum{
    User,
    Assistant,
    System
}

public class Conversation {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ObservableCollection<Message> Messages { get; set; } = new();
}
