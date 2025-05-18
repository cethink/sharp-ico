# SharpIco

SharpIco是一个.NET命令行工具，用于生成和检查ICO图标文件。

## 功能

- 将PNG图像转换为多尺寸ICO图标
- 检查ICO文件的内部结构和信息

## 安装

```bash
dotnet build
```

## 使用方法

### 生成ICO图标

```bash
# 使用默认尺寸(16,32,48,64,128,256,512)
dotnet run -- generate -i input.png -o output.ico

# 指定自定义尺寸
dotnet run -- generate -i input.png -o output.ico -s 16,32,64,128
```

### 检查ICO文件

```bash
dotnet run -- inspect icon.ico
```

## 参数说明

### 生成命令(generate)

- `-i, --input`: (必需)输入PNG图像文件路径
- `-o, --output`: (必需)输出ICO文件路径
- `-s, --sizes`: (可选)图标尺寸列表，以逗号分隔，默认为16,32,48,64,128,256,512

### 检查命令(inspect)

- `ico-file`: (必需)要检查的ICO文件路径

## 示例

```bash
# 生成ICO图标
dotnet run -- generate -i logo.png -o logo.ico

# 使用自定义尺寸
dotnet run -- generate -i logo.png -o logo.ico -s 16,32,64

# 检查ICO文件
dotnet run -- inspect logo.ico
```

## 关于ICO格式的说明

ICO文件格式在表示图像尺寸时有一个限制：宽度和高度字段各只有一个字节，值范围是0-255。当这些字段为0时，按照规范表示256像素。对于大于256的尺寸（如512×512或1024×1024），在文件头中仍然会显示为0（即256），但实际图像数据可以包含更大尺寸的图像。

SharpIco的inspect命令现在会解析每个图像的实际数据，以获取其真实尺寸。这使得工具能够准确显示ICO文件中图像的实际分辨率，即使它们超过了ICO头部表示的范围限制。当头部信息与实际图像尺寸不一致时，SharpIco会显示相应的提示信息。 