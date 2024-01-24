// Antivirus Performance Test
//
// This query generates a tiny (2KB) DLL and times how long it takes to load.
//
// Normal results on a healthy computer:
//  - 1 millisecond or less with antivirus software disabled
//  - 30 milliseconds or less with antivirus software enabled
//
// If it takes over 100ms, consider adding antivirus exclusions for these process names:
//  - LINQPad8.exe
//  - LINQPad8-x64.exe
//  - devenv.exe (if you use Visual Studio)

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Diagnostics;
using System.Reflection;

Console.WriteLine($"Process Name {Path.GetFileName(Environment.ProcessPath)}");
for (int i = 1; i < 11; i++)
{
    string path = $"test{i}.dll";
    CreateTinyAssembly(path);

    var sw = Stopwatch.StartNew();
    Assembly.LoadFile(Path.GetFullPath(path));    // This should not take long!
    sw.Stop();

    Console.WriteLine($"Test {i} - {Math.Round(sw.Elapsed.TotalMilliseconds, 3)} milliseconds");
}
Environment.Exit(0);   // End the process right away to release locks on the assemblies.

void CreateTinyAssembly(string path)
{
    Random _random = new();
    // Use the Microsoft Roslyn API to compile a DLL from a really simple class.
    // Start with a SyntaxTree containing the source code:
    var tree = CSharpSyntaxTree.ParseText($"class Test{_random.Next()} {{}}");

    var compilation = CSharpCompilation    // Create a compilation
        .Create("test")
        .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
        .AddSyntaxTrees(tree)
        .AddReferences(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));

    EmitResult result = compilation.Emit(path);   // Compile to disk
    if (!result.Success)
    {
        Console.WriteLine(result.Diagnostics);
        throw new Exception("Could not compile code");
    }
}