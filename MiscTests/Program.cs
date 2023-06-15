// See https://aka.ms/new-console-template for more information
using Kyklos.Kernel.Core.KLinq;
using Kyklos.Kernel.Core.Strings;

List<TestRecord> list = new();

foreach (var x in Enumerable.Range(1, 3))
{
    list.Add(new(x, Guid.NewGuid().ToString()));
}

var rows = list.ToArray();
var batches = rows.Batch(320);

foreach(var batch in batches)
{
    var xx = batch.Select(x => x.Name).Distinct().ToArray();
    var yy = batch.Select(x => x.Name).Distinct().ToArray();
    var zz = batch.Select(x => x.Name).Distinct().ToArray();
    Console.WriteLine(xx.StringJoin(", "));
}

Console.ReadLine();


record TestRecord(long Id, string Name);