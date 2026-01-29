using ElectricFox.BdfSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Threading.Tasks;

namespace ElectricFox.CobraUi
{
    public interface IResourceProvider
    {
        Task LoadAsync();
        BdfFont GetFont(string fontName);
        Image<Rgba32> GetImage(string imageName);
    }
}
