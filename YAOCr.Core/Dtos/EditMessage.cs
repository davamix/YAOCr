using YAOCr.Core.Models;

namespace YAOCr.Core.Dtos;
public class EditMessage {
    public Message Message { get; set; }
    public string NewContent { get; set; }
}
