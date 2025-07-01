using System;
using System.IO;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SharpIco;

public static class IcoGenerator {
    // 默认尺寸数组
    private static readonly int[] DefaultSizes = new[] { 16, 32, 48, 64, 128, 256, 512 };

    public static void GenerateIcon(string sourcePng, string outputIco) {
        GenerateIcon(sourcePng, outputIco, DefaultSizes);
    }

    public static void GenerateIcon(string sourcePng, string outputIco, int[] sizes) {
        var images = new List<byte[]>();

        using var original = Image.Load(sourcePng);

        foreach (var size in sizes) {
            using var clone = original.Clone(ctx => ctx.Resize(size, size));
            using var ms = new MemoryStream();
            // 强制图像为 Rgba32 格式，确保是 32-bit
            var rgbaImage = clone.CloneAs<Rgba32>();
            rgbaImage.SaveAsPng(ms);
            images.Add(ms.ToArray());
        }

        using var fs = new FileStream(outputIco, FileMode.Create);
        using var writer = new BinaryWriter(fs);

        // ICONDIR (6 字节)
        writer.Write((ushort)0); // reserved
        writer.Write((ushort)1); // image type: 1 = icon, 2 = cursor
        writer.Write((ushort)images.Count); // number of images

        var offset = 6 + (16 * images.Count);
        
        // ICONDIRENTRY (16 字节 × 图像数)
        foreach (var image in images) {
            using var ms = new MemoryStream(image);
            using var img = Image.Load(ms);

            writer.Write((byte)(img.Width == 256 ? 0 : img.Width)); // width, 图标宽度（像素），256 写 0
            writer.Write((byte)(img.Height == 256 ? 0 : img.Height)); // height, 图标高度（像素），256 写 0
            writer.Write((byte)0); // colors in palette, 调色板颜色数，0 = 真彩色
            writer.Write((byte)0); // reserved, 必须为 0
            writer.Write((ushort)1); // color planes, 总是写 1
            writer.Write((ushort)32); // bits per pixel, 位深度，比如 32
            writer.Write(image.Length); // size of image data, 图像数据大小（单位：字节）
            writer.Write(offset); // offset of image data, 数据在文件中的偏移量

            offset += image.Length;
        }

        // 写入图片数据
        foreach (var image in images) {
            writer.Write(image);
        }

        Console.WriteLine($"ICO 文件已生成: {outputIco}");
    }
}