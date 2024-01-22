﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tkmm.Core.Helpers;

namespace Tkmm.Core.Models.GameBanana;

public partial class GameBananaModInfo : ObservableObject
{
    private const string ENDPOINT = $"/Mod/{{0}}/ProfilePage";

    [JsonPropertyName("_idRow")]
    public int Id { get; set; }

    [JsonPropertyName("_sName")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("_bHasContentRatings")]
    public bool IsContentRated { get; set; }

    [JsonPropertyName("_bIsObsolete")]
    public bool IsObsolete { get; set; }

    [JsonPropertyName("_sProfileUrl")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("_aPreviewMedia")]
    public GameBananaMedia Media { get; set; } = new();

    [JsonPropertyName("_aSubmitter")]
    public GameBananaSubmitter Submitter { get; set; } = new();

    [JsonPropertyName("_sVersion")]
    public string Version { get; set; } = string.Empty;

    [ObservableProperty]
    private object? _thumbnail = null;

    [JsonIgnore]
    public GameBananaMod Info { get; set; } = new();

    [RelayCommand]
    public Task Install(GameBananaFile file)
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    public async Task DownloadMod()
    {
        using Stream stream = await GameBananaHelper.Get(string.Format(ENDPOINT, Id.ToString()));
        Info = JsonSerializer.Deserialize<GameBananaMod>(stream)
            ?? throw new InvalidOperationException("""
                Could not deserialize GameBananaMod
                """);
    }
}