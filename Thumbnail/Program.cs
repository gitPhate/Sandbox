// See https://aka.ms/new-console-template for more information
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

await ImgSharp.CreateThumbnailAsync(30);



static class ImgSharp
{
    public static async Task CreateThumbnailAsync(int scale)
    {
        int getPercScale(int v) => (int)Math.Round(v * scale / 100F, MidpointRounding.AwayFromZero);

        using Image image = await Image.LoadAsync(@"C:\temp\hd_image.png");
        int width = 165;
        int height = 165;
        image.Mutate(x => x.Resize(width, height));
        await image.SaveAsJpegAsync(@"C:\temp\hd_image_thumb.jpg");
    }
}