# SharpIco

<div style="display: flex; justify-content: center; align-items: center; gap: 1.25rem;">
  <img src="docs/images/logo.png" alt="SharpIco Logo" width="100" height="100" />
  <h1>SharpIco</h1>
</div>

![License](https://img.shields.io/badge/license-MIT-blue)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)

SharpIco是一个纯 C# AOT 实现的轻量级图标生成工具，用于生成和检查ICO图标文件。可将一张高分辨率 PNG 图片一键生成标准的 Windows .ico 图标文件，内含多种尺寸（16x16 到 512x512），还可以自定义尺寸。

通过SharpIco，您可以轻松将PNG图像转换为包含多种尺寸的ICO图标，完全摆脱命令行依赖，无需 ImageMagick、Node.js 或 Python，适合在 .NET 项目中内嵌、分发或集成自动打包流程中使用。

除了图标生成，SharpIco 还内置图标结构分析功能，助你轻松验证 .ico 文件中包含的图层与尺寸。

🚀 跨平台、零依赖、极速生成，一切尽在 SharpIco！


## ✨ 功能特点

- 🖼️ 将PNG图像转换为多尺寸ICO图标
- 🔍 支持生成包含自定义尺寸的ICO图标（最高支持1024×1024）
- 🧐 检查ICO文件的内部结构和信息
- 📏 准确识别并显示超大尺寸图标（如512×512、1024×1024）的实际尺寸

## 📦 安装

### 从源码构建

```bash
git clone https://github.com/yourusername/SharpIco.git
cd SharpIco
dotnet build -c Release
```

### 发布为独立应用程序

```bash
dotnet publish -c Release -r win-x64 --self-contained
```

## 🚀 使用方法

### 生成ICO图标

```bash
# 使用默认尺寸(16,32,48,64,128,256,512,1024)
SharpIco generate -i input.png -o output.ico

# 指定自定义尺寸
SharpIco generate -i input.png -o output.ico -s 16,32,64,128
```

### 检查ICO文件

```bash
SharpIco inspect icon.ico
```

## 📋 参数说明

### 生成命令(generate)

| 参数 | 缩写 | 说明 | 是否必需 |
| --- | --- | --- | --- |
| `--input` | `-i` | 输入PNG图像文件路径 | 是 |
| `--output` | `-o` | 输出ICO文件路径 | 是 |
| `--sizes` | `-s` | 图标尺寸列表，以逗号分隔 | 否，默认为16,32,48,64,128,256,512,1024 |

### 检查命令(inspect)

| 参数 | 说明 | 是否必需 |
| --- | --- | --- |
| `ico-file` | 要检查的ICO文件路径 | 是 |

## 📝 示例输出

### 生成ICO图标

```
正在将 logo.png 转换为 logo.ico...
生成尺寸: 16, 32, 48, 64, 128, 256, 512, 1024
ICO 文件已生成: logo.ico
```

### 检查ICO文件

```
正在检查ICO文件: logo.ico
图标数量: 8
- 第1张图像: 16x16, 32bpp, 大小: 840字节, 偏移: 150
- 第2张图像: 32x32, 32bpp, 大小: 1939字节, 偏移: 990
- 第3张图像: 48x48, 32bpp, 大小: 3375字节, 偏移: 2929
- 第4张图像: 64x64, 32bpp, 大小: 4951字节, 偏移: 6304
- 第5张图像: 128x128, 32bpp, 大小: 13782字节, 偏移: 11255
- 第6张图像: 256x256, 32bpp, 大小: 37823字节, 偏移: 25037
- 第7张图像: 512x512, 32bpp, 大小: 114655字节, 偏移: 62860
  注意: 文件头中指定的尺寸为256x256，但实际图像尺寸为512x512
- 第8张图像: 1024x1024, 32bpp, 大小: 248965字节, 偏移: 177515
  注意: 文件头中指定的尺寸为256x256，但实际图像尺寸为1024x1024
```

## 🔍 关于ICO格式的说明

ICO文件格式在表示图像尺寸时有一个限制：宽度和高度字段各只有一个字节，值范围是0-255。当这些字段为0时，按照规范表示256像素。对于大于256的尺寸（如512×512或1024×1024），在文件头中仍然会显示为0（即256），但实际图像数据可以包含更大尺寸的图像。

SharpIco的inspect命令会解析每个图像的实际数据，以获取其真实尺寸。这使得工具能够准确显示ICO文件中图像的实际分辨率，即使它们超过了ICO头部表示的范围限制。当头部信息与实际图像尺寸不一致时，SharpIco会显示相应的提示信息。

## 🛠️ 技术实现

SharpIco使用以下技术：

- .NET 9.0
- [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp) - 用于图像处理
- [System.CommandLine](https://github.com/dotnet/command-line-api) - 用于命令行参数解析

## 📄 许可证

MIT License
