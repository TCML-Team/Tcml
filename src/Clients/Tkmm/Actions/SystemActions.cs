using System.Diagnostics;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using Humanizer;
using Markdown.Avalonia.Full;
using Microsoft.Extensions.Logging;
using Tkmm.Core;
using Tkmm.Dialogs;

namespace Tkmm.Actions;

public sealed partial class SystemActions : GuardedActionGroup<SystemActions>
{
    protected override string ActionGroupName { get; } = nameof(SystemActions).Humanize();

    [RelayCommand]
    public async Task ShowAboutDialog()
    {
        await CanActionRun(showError: false);
        
        await using Stream aboutFileStream = AssetLoader.Open(new Uri("avares://Tkmm/Assets/About.md"));
        string contents = await new StreamReader(aboutFileStream).ReadToEndAsync();

        contents = contents.Replace("@@version@@", App.Version);

        Uri markdownStylePath = new("avares://Tkmm/Styles/Markdown.axaml");

        TaskDialog dialog = new() {
            XamlRoot = App.XamlRoot,
            Title = "About",
            Content = new MarkdownScrollViewer {
                Markdown = contents,
                Styles = {
                    new StyleInclude(markdownStylePath) {
                        Source = markdownStylePath
                    }
                }
            },
            Buttons = [
                TaskDialogButton.OKButton
            ]
        };

        await dialog.ShowAsync();
    }

    [RelayCommand]
    public async Task OpenDocumentationWebsite()
    {
        await CanActionRun(showError: false);

        try {
            Process.Start(new ProcessStartInfo("https://tkmm.org/docs/using-mods/") {
                UseShellExecute = true
            });
        }
        catch (Exception ex) {
            TKMM.Logger.LogError(ex, "An error occured while trying to open the documentation website.");
            await ErrorDialog.ShowAsync(ex);
        }
    }
    
    [RelayCommand]
    public async Task CheckForUpdates(CancellationToken ct = default)
    {
        await CanActionRun(showError: false);

        // TODO: Check for updates
        
        if (!(await AppManager.HasUpdate()).Result) {
            await new ContentDialog {
                Title = "Check for Updates",
                Content = "Software up to date.",
                PrimaryButtonText = "OK"
            }.ShowAsync();

            return;
        }

        await RequestUpdate(ct);
    }

    [RelayCommand]
    public async Task RequestUpdate(CancellationToken ct = default)
    {
        await CanActionRun(showError: false);
        
        ContentDialog dialog = new() {
            Title = "Proceed with update?",
            Content = "Your current session will be saved and closed, are you sure you wish to proceed?",
            PrimaryButtonText = "Yes",
            SecondaryButtonText = "Cancel"
        };

        if (await dialog.ShowAsync() is not ContentDialogResult.Primary) {
            return;
        }
        
        // TODO: Update
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
            await TKMM.ModManager.Save();
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