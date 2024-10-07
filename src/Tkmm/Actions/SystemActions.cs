using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using Humanizer;
using Microsoft.Extensions.Logging;
using Tkmm.Core;
using Tkmm.Dialogs;

namespace Tkmm.Actions;

public sealed partial class SystemActions : ActionsBase<SystemActions>
{
    protected override string ActionGroupName { get; } = nameof(SystemActions).Humanize();
    
    [RelayCommand]
    public async Task CheckForUpdates()
    {
        await CanActionRun(showError: false);

        throw new NotImplementedException();
    }

    [RelayCommand]
    public async Task RequestUpdate()
    {
        await CanActionRun(showError: false);
        
        throw new NotImplementedException();
    }

    [RelayCommand]
    public async Task CleanupTempFolder()
    {
        await CanActionRun(showError: false);
        
        try {
            string tempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".temp");
            
            if (!Directory.Exists(tempFolder)) {
                return;
            }

            Directory.Delete(tempFolder, recursive: true);
            Directory.CreateDirectory(tempFolder);
            
            App.Toast("The temporary folder was succesfully deleted.",
                "Temporary Files Cleared", NotificationType.Success, TimeSpan.FromSeconds(3));
        }
        catch (Exception ex) {
            TKMM.Logger.LogError(ex, "An error occured while trying to cleanup the temp folder.");
            await ErrorDialog.ShowAsync(ex);
        }
    }

    [RelayCommand]
    public async Task SoftClose()
    {
        await CanActionRun(showError: false);
        
        try {
            Config.Shared.Save();
            await TKMM.ModManager.SaveState();
            Environment.Exit(0);
        }
        catch (Exception ex) {
            TKMM.Logger.LogError(ex, "An error occured while saving the mod manager state.");

            object errorReportResult = await ErrorDialog.ShowAsync(ex, TaskDialogButton.CloseButton, TaskDialogButton.CancelButton);
            if (Equals(errorReportResult, TaskDialogButton.CloseButton)) {
                Environment.Exit(-1);
            }
        }
    }
}