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

var sizesOption = new Option<int[]>(
    new[] { "--sizes", "-s" },
    description: "图标尺寸列表，以逗号分隔",
    getDefaultValue: () => new[] { 16, 32, 48, 64, 128, 256, 512, 1024 }
);
sizesOption.AllowMultipleArgumentsPerToken = true;

generateCommand.AddOption(inputOption);
generateCommand.AddOption(outputOption);
generateCommand.AddOption(sizesOption);

generateCommand.SetHandler((input, output, sizes) => {
    try {
        Console.WriteLine($"正在将 {input.FullName} 转换为 {output.FullName}...");
        Console.WriteLine($"生成尺寸: {string.Join(", ", sizes)}");
        IcoGenerator.GenerateIcon(input.FullName, output.FullName, sizes);
    }
    catch (Exception ex) {
        Console.WriteLine($"错误: {ex.Message}");
    }
}, inputOption, outputOption, sizesOption);

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