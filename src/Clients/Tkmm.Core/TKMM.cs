using Microsoft.Extensions.Logging;
using Tkmm.Abstractions;
using Tkmm.Abstractions.IO;
using Tkmm.Abstractions.Providers;
using Tkmm.Common;
using Tkmm.Common.Providers;
using Tkmm.Core.Logging;
using Tkmm.Core.Providers;
using Tkmm.Core.Studio;

namespace Tkmm.Core;

// ReSharper disable once InconsistentNaming
public static class TKMM
{
    private static readonly Lazy<ILogger> _logger = new(() => {
        using ILoggerFactory factor = LoggerFactory.Create(
            builder => builder
                .AddProvider(new DesktopLoggerProvider())
                .AddProvider(new EventLoggerProvider())
        );

        return factor.CreateLogger(nameof(TKMM));
    });

    private static readonly Lazy<IRomfsProvider> _romfsProvider = new(() => new RomfsProvider());

    public static ILogger Logger => _logger.Value;

    public static IRomfs Romfs => _romfsProvider.Value.GetRomfs();

    public static Config Config => Config.Shared;

    public static IModManager ModManager { get; }

    public static ITkShopManager ShopManager { get; } = new TkShopManager();
    
    public static TkProjectManager ProjectManager { get; } = new();

    public static IMergerProvider MergerProvider { get; } = new TkMergerProvider();

    public static TkMergerMarshal MergerMarshal { get; }

    public static IChangelogBuilderProvider ChangelogBuilderProvider { get; } = new TkChangelogBuilderProvider(Romfs);

    public static TkChangelogBuilderMarshal ChangelogBuilderMarshal { get; } = new(Romfs.Zstd, ChangelogBuilderProvider);

    static TKMM()
    {
        ModManager = new ModManager(new ModReaderProvider(ChangelogBuilderMarshal));
        MergerMarshal = new TkMergerMarshal(ModManager, MergerProvider, Romfs);
    }
}