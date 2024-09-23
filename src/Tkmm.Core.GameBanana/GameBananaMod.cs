﻿using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Tkmm.Core.GameBanana;

internal sealed partial class GameBananaMod : ObservableObject
{
    private const int GB_TOTK_ID = 7617;
    
    [JsonPropertyName("_idRow")]
    public ulong Id { get; set; }

    [JsonPropertyName("_sName")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("_aPreviewMedia")]
    public GameBananaMedia Media { get; set; } = new();

    [JsonPropertyName("_sVersion")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("_bIsPrivate")]
    public bool IsPrivate { get; set; }

    [JsonPropertyName("_bIsFlagged")]
    public bool IsFlagged { get; set; }

    [JsonPropertyName("_bIsTrashed")]
    public bool IsTrashed { get; set; }

    [JsonPropertyName("_aFiles")]
    public List<GameBananaFile> Files { get; set; } = [];

    [JsonPropertyName("_sText")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("_aSubmitter")]
    public GameBananaSubmitter Submitter { get; set; } = new();

    [JsonPropertyName("_aGame")]
    public GameBananaGame Game { get; set; } = new();

    [JsonPropertyName("_aCredits")]
    public List<GameBananaCreditGroup> Credits { get; set; } = [];

//     public static async Task<ITkMod> FromId(string id)
//     {
//         GameBananaMod gamebananaMod = await DownloadMetaData(id);
//
//         if (gamebananaMod.Game.Id != GB_TOTK_ID) {
//             throw new InvalidDataException($"""
//                 The mod '{gamebananaMod.Name}' is not for TotK
//                 """);
//         }
//
//         if (gamebananaMod.Files.FirstOrDefault(x => Path.GetExtension(x.Name) is ".tkcl" or ".zip" or ".rar" or ".7z") is not GameBananaFile file) {
//             throw new InvalidDataException($"""
//                 The mod '{gamebananaMod.Name}' has no valid archive (tkcl, zip, rar, 7z) file
//                 """);
//         }
//
//         return await gamebananaMod.FromFile(file);
//     }
//
//     public async Task<Mod> FromFile(GameBananaFile file)
//     {
//         byte[] data = await DownloadOperations.DownloadAndVerify(file.DownloadUrl, Convert.FromHexString(file.Checksum));
//         using MemoryStream ms = new(data);
//         Mod mod = await Mod.FromStream(ms, file.Name, GetTkModId(file));
//
//         ObservableCollection<ModContributor> contributors = [];
//         foreach (var group in Credits) {
//             foreach (var author in group.Authors) {
//                 contributors.Add(new() {
//                     Name = author.Name,
//                     Contributions = [author.Role]
//                 });
//             }
//         }
//
//         string thumbnailUrl = $"{Media.Images[0].BaseUrl}/{Media.Images[0].File}";
//         using Stream thumbnailHttpStream = await DownloadOperations.Client.GetStreamAsync(thumbnailUrl);
//
//         string thumbnailPath = Path.Combine(mod.SourceFolder, PackageBuilder.THUMBNAIL);
//         using (FileStream thumbnailFileStream = File.Create(thumbnailPath)) {
//             await thumbnailHttpStream.CopyToAsync(thumbnailFileStream);
//         }
//
//         mod.Name = Name;
//         mod.Author = Submitter.Name;
//         mod.Description = new ReverseMarkdown.Converter(new ReverseMarkdown.Config {
//             GithubFlavored = true,
//             ListBulletChar = '*',
//             UnknownTags = ReverseMarkdown.Config.UnknownTagsOption.Bypass
//         }).Convert(Text);
//         mod.ThumbnailUri = PackageBuilder.THUMBNAIL;
//         mod.Contributors = contributors;
//         mod.Version = Version;
//
//         PackageBuilder.CreateMetaData(mod, mod.SourceFolder);
//
//         return mod;
//     }

    private Ulid GetTkModId(GameBananaFile gameBananaFile)
    {
        Span<byte> guidRawBuffer = stackalloc byte[16];
        MemoryMarshal.Write(guidRawBuffer, Id);
        MemoryMarshal.Write(guidRawBuffer[8..], gameBananaFile.Id);

        return MemoryMarshal.Read<Ulid>(guidRawBuffer);
    }
}

[JsonSerializable(typeof(GameBananaMod))]
internal partial class GameBananaModJsonContext : JsonSerializerContext;
