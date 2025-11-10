using System;

namespace YAOCr.Services.Dialogs;

public class DialogService {
    public event EventHandler? ShowDialogSettingsRequested;
    public event EventHandler<YesNoDialogArgs>? ShowYesNoDialogRequested;

    public void OpenDialogSettings() {
        ShowDialogSettingsRequested?.Invoke(this, EventArgs.Empty);
    }

    public void OpenYesNoDialog(YesNoDialogArgs args) {
        ShowYesNoDialogRequested?.Invoke(this, args);
    }
}
