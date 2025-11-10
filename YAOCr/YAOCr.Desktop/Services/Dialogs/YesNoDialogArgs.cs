using System;

namespace YAOCr.Services.Dialogs;

public class YesNoDialogArgs : DialogArgs {
    public required Action YesAction { get; set; }
    public Action? NoAction { get; set; }
}
