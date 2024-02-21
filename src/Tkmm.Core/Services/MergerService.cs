﻿using Tkmm.Core.Components;
using Tkmm.Core.Components.Mergers;
using Tkmm.Core.Components.Mergers.Special;
using Tkmm.Core.Components.Models;
using Tkmm.Core.Models.Mods;

namespace Tkmm.Core.Services;

public class MergerService
{
    private static readonly string _ryujinxPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ryujinx", "sdcard", "atmosphere", "contents", "0100f2c0115b6000");

    private static readonly string _yuzuPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "yuzu", "load", "0100F2C0115B6000", "TKMM");

    private static readonly IMerger[] _mergers = [
        new ContentMerger(),
        new MalsMergerShell(),
        new RsdbMergerShell(),
        new SarcMergerShell()
    ];

    public static async Task Merge() => await Merge(ProfileManager.Shared.Current, Config.Shared.MergeOutput);
    public static async Task Merge(string output) => await Merge(ProfileManager.Shared.Current, output);
    public static async Task Merge(Profile profile) => await Merge(profile, Config.Shared.MergeOutput);
    public static async Task Merge(Profile profile, string output)
    {
        Mod[] mods = profile.Mods
            .Where(x => x.IsEnabled && x.Mod is not null)
            .Select(x => x.Mod!)
            .Reverse()
            .ToArray();

        if (mods.Length <= 0) {
            AppStatus.Set("Nothing to Merge", "fa-solid fa-code-merge",
                isWorkingStatus: false, temporaryStatusTime: 1.5,
                logLevel: LogLevel.Info);

            return;
        }

        AppStatus.Set($"Merging '{profile.Name}'", "fa-solid fa-code-merge");

        if (Directory.Exists(output)) {
            AppStatus.Set($"Clearing output", "fa-solid fa-code-merge");
            Directory.Delete(output, true);
        }

        Directory.CreateDirectory(output);
        await Task.Run(async () => {
            await MergeAsync(mods, output);
        });

        AppStatus.Set("Merge completed successfully", "fa-solid fa-list-check",
            isWorkingStatus: false, temporaryStatusTime: 1.5,
            logLevel: LogLevel.Info);
    }

    private static async Task MergeAsync(Mod[] mods, string output)
    {
        Task[] tasks = new Task[_mergers.Length];
        for (int i = 0; i < tasks.Length; i++) {
            tasks[i] = _mergers[i].Merge(mods, output);
        }

        await Task.WhenAll(tasks);
        await RstbMergerShell.Shared.Merge(mods, output);

        if (Config.Shared.UseRyu) {
            Directory.Delete(_ryujinxPath, true);
            Directory.CreateSymbolicLink(_ryujinxPath, output);
        }

        if (Config.Shared.UseYuzu) {
            Directory.Delete(_yuzuPath, true);
            Directory.CreateSymbolicLink(_yuzuPath, output);
        }
    }
}
