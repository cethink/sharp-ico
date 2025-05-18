using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace SharpIco;

public static class IcoInspector {
    public static void Inspect(string icoPath) {
        using var fs = new FileStream(icoPath, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(fs);

        ushort reserved = reader.ReadUInt16(); // 0
        ushort type = reader.ReadUInt16(); // 1 = icon
        ushort count = reader.ReadUInt16(); // image count

        Console.WriteLine($"图标数量: {count}");

        // 存储所有图像条目
        var imageEntries = new List<ImageEntry>();
        
        // 读取所有图像条目信息
        for (int i = 0; i < count; i++) {
            byte width = reader.ReadByte();
            byte height = reader.ReadByte();
            byte colorCount = reader.ReadByte();
            byte reservedByte = reader.ReadByte();
            ushort planes = reader.ReadUInt16();
            ushort bitCount = reader.ReadUInt16();
            int sizeInBytes = reader.ReadInt32();
            int imageOffset = reader.ReadInt32();

            imageEntries.Add(new ImageEntry {
                Index = i,
                Width = width,
                Height = height,
                BitCount = bitCount,
                SizeInBytes = sizeInBytes,
                ImageOffset = imageOffset
            });
        }
        
        // 排序条目，确保按照偏移量顺序处理
        imageEntries = imageEntries.OrderBy(e => e.ImageOffset).ToList();
        
        // 读取并解析每个图像的数据
        for (int i = 0; i < imageEntries.Count; i++) {
            var entry = imageEntries[i];
            
            // 保存当前位置
            long currentPosition = fs.Position;
            
            // 跳转到图像数据位置
            fs.Seek(entry.ImageOffset, SeekOrigin.Begin);
            
            // 确定图像数据大小
            int dataSize = entry.SizeInBytes;
            byte[] imageData = new byte[dataSize];
            
            // 读取图像数据
            fs.Read(imageData, 0, dataSize);
            
            // 使用ImageSharp解析图像数据获取真实尺寸
            (int actualWidth, int actualHeight) = GetImageDimensions(imageData);
            
            // 恢复文件读取位置
            fs.Seek(currentPosition, SeekOrigin.Begin);
            
            // 更新图像条目的实际尺寸
            entry.ActualWidth = actualWidth;
            entry.ActualHeight = actualHeight;
        }
        
        // 输出图像信息
        DisplayImageInfo(imageEntries);
    }
    
    private static (int width, int height) GetImageDimensions(byte[] imageData) {
        try {
            // PNG图像以字节序列89 50 4E 47 0D 0A 1A 0A开头
            bool isPng = imageData.Length > 8 && 
                         imageData[0] == 0x89 && 
                         imageData[1] == 0x50 && 
                         imageData[2] == 0x4E && 
                         imageData[3] == 0x47;
                         
            if (isPng) {
                using var ms = new MemoryStream(imageData);
                using var image = Image.Load(ms);
                return (image.Width, image.Height);
            }
            
            // 对于不是PNG格式的，尝试通过通用方法读取
            using var ms2 = new MemoryStream(imageData);
            try {
                using var image = Image.Load(ms2);
                return (image.Width, image.Height);
            }
            catch {
                // 如果无法解析，返回0表示未知
                return (0, 0);
            }
        }
        catch (Exception) {
            // 读取失败则返回0表示未知
            return (0, 0);
        }
    }
    
    private static void DisplayImageInfo(List<ImageEntry> entries) {
        // 按索引排序，保持原始顺序
        var sortedEntries = entries.OrderBy(e => e.Index).ToList();
        
        foreach (var entry in sortedEntries) {
            // 文件头中的尺寸信息
            int headerWidth = entry.Width == 0 ? 256 : entry.Width;
            int headerHeight = entry.Height == 0 ? 256 : entry.Height;
            
            // 实际图像尺寸
            int actualWidth = entry.ActualWidth > 0 ? entry.ActualWidth : headerWidth;
            int actualHeight = entry.ActualHeight > 0 ? entry.ActualHeight : headerHeight;
            
            // 显示信息
            Console.WriteLine(
                $"- 第{entry.Index + 1}张图像: {actualWidth}x{actualHeight}, {entry.BitCount}bpp, 大小: {entry.SizeInBytes}字节, 偏移: {entry.ImageOffset}");
            
            // 当实际尺寸与头部信息不一致时，显示提示
            if (actualWidth != headerWidth || actualHeight != headerHeight) {
                Console.WriteLine($"  注意: 文件头中指定的尺寸为{headerWidth}x{headerHeight}，但实际图像尺寸为{actualWidth}x{actualHeight}");
            }
        }
    }

    private class ImageEntry {
        public int Index { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public ushort BitCount { get; set; }
        public int SizeInBytes { get; set; }
        public int ImageOffset { get; set; }
        public int ActualWidth { get; set; }
        public int ActualHeight { get; set; }
    }
}