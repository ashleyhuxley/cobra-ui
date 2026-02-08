using ElectricFox.BdfSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ElectricFox.CobraUi.Demo;

public sealed class ResourceManager : IResourceProvider
{
    public const string AppName = "DemoApp";
    private const string NotLoadedError = "Resources not loaded";

    public static class BdfFonts
    {
        public static string Tamzen8x15b => "ElectricFox.CobraUi.Demo.Resources.Tamzen8x15b.bdf";
    }

    public static class Images
    {
        public static string CobraUi => "ElectricFox.CobraUi.Demo.Resources.cobra-ui.png";
    }

    private readonly Dictionary<string, BdfFont> _fontCache = new();
    private readonly Dictionary<string, Image<Rgba32>> _imageCache = new();

    public async Task LoadAsync()
    {
        await AddFont(BdfFonts.Tamzen8x15b);
        await AddImage(Images.CobraUi);
    }

    private async Task AddFont(string font)
    {
        var assembly = typeof(ResourceManager).Assembly;

        var bdfFont = await BdfFont.LoadFromEmbeddedResourceAsync(
            font,
            assembly
        );

        _fontCache[font] = bdfFont;
    }

    private async Task AddImage(string imageName)
    {
        var assembly = typeof(ResourceManager).Assembly;

        var imageData = await Image.LoadAsync<Rgba32>(
            assembly.GetManifestResourceStream(imageName)!
        );

        _imageCache[imageName] = imageData;
    }

    public BdfFont GetFont(string fontName)
    {
        return _fontCache.TryGetValue(fontName, out var value) ? value : throw new InvalidOperationException(NotLoadedError);
    }

    public Image<Rgba32> GetImage(string imageName)
    {
        return _imageCache.TryGetValue(imageName, out var value) ? value : throw new InvalidOperationException(NotLoadedError);
    }
}
