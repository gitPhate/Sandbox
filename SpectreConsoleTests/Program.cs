// Styled text with markup
using Spectre.Console;

AnsiConsole.MarkupLine("[bold blue]Welcome[/] to [green]Spectre.Console[/]!");

// A simple table
var table = new Table()
    .AddColumn("Feature")
    .AddColumn("Description")
    .AddRow("[green]Markup[/]", "Rich text with colors and styles")
    .AddRow("[blue]Tables[/]", "Structured data display")
    .AddRow("[yellow]Progress[/]", "Spinners and progress bars");
AnsiConsole.Write(table);

// Status spinner for work
AnsiConsole.Status()
    .Start("Processing...", ctx =>
    {
        Thread.Sleep(2500);
    });

AnsiConsole.MarkupLine("[green]Done![/]");