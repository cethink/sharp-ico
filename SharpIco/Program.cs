// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using SharpIco;

// 定义根命令
var rootCommand = new RootCommand("SharpIco - ICO图标生成和检查工具");

// 生成命令
var generateCommand = new Command("generate", "将PNG图像转换为ICO图标");
var inputOption = new Option<FileInfo>(
    new[] { "--input", "-i" },
    "输入PNG图像文件路径"
);
inputOption.IsRequired = true;

var outputOption = new Option<FileInfo>(
    new[] { "--output", "-o" },
    "输出ICO文件路径"
);
outputOption.IsRequired = true;

// 支持多分隔符（逗号、分号、空格）的尺寸输入
var sizesOptionRaw = new Option<string[]>(
    new[] { "--sizes", "-s" },
    getDefaultValue: () => Array.Empty<string>(),
    description: "图标尺寸列表，支持逗号、分号或空格分隔"
);
sizesOptionRaw.AllowMultipleArgumentsPerToken = true;

generateCommand.AddOption(inputOption);
generateCommand.AddOption(outputOption);
generateCommand.AddOption(sizesOptionRaw);

generateCommand.SetHandler((input, output, sizesTokens) => {
    try {
        // 如果未提供尺寸，使用默认尺寸
        var sizes = (sizesTokens is null || sizesTokens.Length == 0)
            ? new[] { 16, 32, 48, 64, 128, 256, 512 }
            : ParseSizes(sizesTokens);

        Console.WriteLine($"正在将 {input.FullName} 转换为 {output.FullName}...");
        Console.WriteLine($"生成尺寸: {string.Join(", ", sizes)}");
        IcoGenerator.GenerateIcon(input.FullName, output.FullName, sizes);
    }
    catch (Exception ex) {
        Console.WriteLine($"错误: {ex.Message}");
    }
}, inputOption, outputOption, sizesOptionRaw);

// 检查命令
var inspectCommand = new Command("inspect", "检查ICO文件结构");
var icoFileArgument = new Argument<FileInfo>(
    "ico-file",
    "要检查的ICO文件路径"
);

inspectCommand.AddArgument(icoFileArgument);

inspectCommand.SetHandler((icoFile) => {
    try {
        Console.WriteLine($"正在检查ICO文件: {icoFile.FullName}");
        IcoInspector.Inspect(icoFile.FullName);
    }
    catch (Exception ex) {
        Console.WriteLine($"错误: {ex.Message}");
    }
}, icoFileArgument);

// 将命令添加到根命令
rootCommand.AddCommand(generateCommand);
rootCommand.AddCommand(inspectCommand);

// 执行命令
return await rootCommand.InvokeAsync(args);

// 工具方法
/// <summary>
/// 解析尺寸参数：支持使用逗号、分号或空格作为分隔符，将字符串数组解析为去重且有效的整数数组。
/// 允许的取值范围为 1 到 1024；遇到非法值会抛出异常并终止处理。
/// </summary>
/// <param name="tokens">尺寸参数的原始字符串标记集合，例如 ["16,32", "64", "128;256"]。</param>
/// <returns>解析后的整型尺寸数组，例如 [16, 32, 64, 128, 256]。</returns>
static int[] ParseSizes(IEnumerable<string> tokens) {
    var result = new List<int>();
    foreach (var token in tokens) {
        var parts = token
            .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var part in parts) {
            if (!int.TryParse(part, out var value)) {
                throw new ArgumentException($"无法将 '{part}' 解析为整数。");
            }
            if (value < 1 || value > 1024) {
                throw new ArgumentOutOfRangeException(nameof(tokens), value, "尺寸必须在 1 到 1024 之间。");
            }
            result.Add(value);
        }
    }

    // 去重并保持原始出现顺序
    var seen = new HashSet<int>();
    var deduped = new List<int>();
    foreach (var v in result) {
        if (seen.Add(v)) deduped.Add(v);
    }
    return deduped.ToArray();
}