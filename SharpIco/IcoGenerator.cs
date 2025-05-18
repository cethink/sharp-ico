using System;
using System.IO;
using System.Collections.Generic;
using SixLabors.ImageSharp;
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
            clone.SaveAsPng(ms);
            images.Add(ms.ToArray());
        }

        using var fs = new FileStream(outputIco, FileMode.Create);
        using var writer = new BinaryWriter(fs);

        // ICONDIR
        writer.Write((ushort)0); // reserved
        writer.Write((ushort)1); // image type: 1 = icon
        writer.Write((ushort)images.Count); // number of images

        int offset = 6 + (16 * images.Count);

        foreach (var image in images) {
            using var ms = new MemoryStream(image);
            using var img = Image.Load(ms);

            writer.Write((byte)(img.Width == 256 ? 0 : img.Width)); // width
            writer.Write((byte)(img.Height == 256 ? 0 : img.Height)); // height
            writer.Write((byte)0); // colors in palette
            writer.Write((byte)0); // reserved
            writer.Write((ushort)1); // color planes
            writer.Write((ushort)32); // bits per pixel
            writer.Write(image.Length); // size of image data
            writer.Write(offset); // offset of image data

            offset += image.Length;
        }

        // 写入图片数据
        foreach (var image in images) {
            writer.Write(image);
        }

        Console.WriteLine($"ICO 文件已生成: {outputIco}");
    }
}