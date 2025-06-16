using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAOCr.Services;

public class DialogService
{
    public event EventHandler? ShowDialogSettingsRequested;

    public void OpenDialogSettings()
    {
        ShowDialogSettingsRequested?.Invoke(this, EventArgs.Empty);
    }
}
