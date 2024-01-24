using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.TorchSharp;
using Microsoft.ML.TorchSharp.NasBert;
using Tensorboard;

//string directory = @"C:\Temp\CostsAnalyzer";
//var names = Enum.GetNames(typeof(Labels)).OrderBy(x => x);
//var namesQueue = new Queue<string>(names);
//var files = Directory.GetFiles(directory).OrderBy(x => Path.GetFileNameWithoutExtension(x));

//foreach(var x in files)
//{
//    string name = namesQueue.Dequeue();
//    File.Move(x, Path.Combine(directory, name + ".txt"));
//}


// Initialize MLContext
MLContext mlContext = new()
{
    GpuDeviceId = 0,
    FallbackToCpu = true
};

ModelInput[] initialData =
    Directory
        .GetFiles(@"C:\Temp\CostsAnalyzer")
        .Select(x =>
        {
            string labelName = Path.GetFileNameWithoutExtension(x);
            string[] lines = File.ReadAllLines(x);
            Labels label = Enum.Parse<Labels>(labelName);
            if (label != Labels.RistorantiBar && label != Labels.GeneriAlimentariSupermercato)
            {
                return Array.Empty<ModelInput>();
            }
            var models = lines.Select(x => new ModelInput { Label = Convert.ToSingle(Enum.Parse<Labels2>(labelName)), Sentence = x });
            return models;
        })
        .Where(x => x.Any())
        .SelectMany(x => x)
        .ToArray();

Random rng = new();
initialData =
    initialData
        .OrderBy(a => rng.Next())
        .ToArray();

string[] lines =
    initialData
    .Select(x => $"{x.Sentence}\t{x.Label}")
    .ToArray();

File.WriteAllLines("data.tsv", lines);

// Load the data source
Console.WriteLine("Loading data...");
IDataView dataView = mlContext.Data.LoadFromEnumerable(initialData);
//IDataView dataView = mlContext.Data.LoadFromTextFile<ModelInput>(
//    @"C:\Users\gabriele.ricci\Downloads\Turtles.tsv",
//    separatorChar: '\t',
//    hasHeader: false
//);

// To evaluate the effectiveness of machine learning models we split them into a training set for fitting
// and a testing set to evaluate that trained model against unknown data
DataOperationsCatalog.TrainTestData dataSplit = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.1, seed: 1234);
IDataView trainData = dataSplit.TrainSet;
IDataView testData = dataSplit.TestSet;

// Create a pipeline for training the model
var pipeline = mlContext.Transforms.Conversion.MapValueToKey(
                            outputColumnName: "Label",
                            inputColumnName: "Label")
                        .Append(mlContext.MulticlassClassification.Trainers.TextClassification(
                            labelColumnName: "Label",
                            sentence1ColumnName: "Sentence",
                            architecture: BertArchitecture.Roberta))
                        .Append(mlContext.Transforms.Conversion.MapKeyToValue(
                            outputColumnName: "PredictedLabel",
                            inputColumnName: "PredictedLabel"));

// Train the model using the pipeline
Console.WriteLine("Training model...");
ITransformer model = pipeline.Fit(trainData);

// Evaluate the model's performance against the TEST data set
Console.WriteLine("Evaluating model performance...");

// We need to apply the same transformations to our test set so it can be evaluated via the resulting model
IDataView transformedTest = model.Transform(testData);
MulticlassClassificationMetrics metrics = mlContext.MulticlassClassification.Evaluate(transformedTest);

// Display Metrics
Console.WriteLine($"Macro Accuracy: {metrics.MacroAccuracy}");
Console.WriteLine($"Micro Accuracy: {metrics.MicroAccuracy}");
Console.WriteLine($"Log Loss: {metrics.LogLoss}");
Console.WriteLine();

// List different intents with their class number
Console.WriteLine("Classes:");
foreach (Labels2 value in Enum.GetValues<Labels2>())
{
    Console.WriteLine($"{((int)value)}: {value}");
}

// Generate the table for diagnostics
Console.WriteLine(metrics.ConfusionMatrix.GetFormattedConfusionTable());

// Generate a prediction engine
Console.WriteLine("Creating prediction engine...");
PredictionEngine<ModelInput, ModelOutput> engine =
    mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);

Console.WriteLine("Ready to generate predictions.");

// Generate a series of predictions based on user input
string input;
do
{
    Console.WriteLine();
    Console.WriteLine("What do you want to say about turtles? (Type Q to Quit)");
    input = Console.ReadLine()!;

    // Get a prediction
    ModelInput sampleData = new() { Sentence = input };
    ModelOutput result = engine.Predict(sampleData);

    // Print classification
    float maxScore = result.Score[(uint)result.PredictedLabel];
    Console.WriteLine($"Matched intent {(Labels2)result.PredictedLabel} with score of {maxScore:f2}");
    Console.WriteLine();
}
while (!string.IsNullOrWhiteSpace(input) && input.ToLowerInvariant() != "q");



#nullable disable

class ModelInput
{
    [LoadColumn(0)]
    [ColumnName(@"Sentence")]
    public string Sentence { get; set; }

    [LoadColumn(1)]
    [ColumnName(@"Label")]
    public float Label { get; set; }
}

public class ModelOutput
{
    [ColumnName(@"Sentence")]
    public string Sentence { get; set; }

    [ColumnName(@"Label")]
    public uint Label { get; set; }

    [ColumnName(@"PredictedLabel")]
    public float PredictedLabel { get; set; }

    [ColumnName(@"Score")]
    public float[] Score { get; set; }

}

enum Labels2
{
    RistorantiBar = 0,
    GeneriAlimentariSupermercato
}


enum Labels
{
    AbbigliamentoAccessori,
    AddebitiVari,
    AltreUscite,
    Associazioni,
    BonificiRicevuti,
    BonificiUscita,
    Carburanti,
    Cellulare,
    CorsiSport,
    CuraPersona,
    DivertimentiLocali,
    DomiciliazioniUtenze,
    ElettrodomesticiArredamentoGiardino,
    EntrateVarie,
    Farmacia,
    GasEnergia,
    GeneriAlimentariSupermercato,
    HiTechInformatica,
    ImposteBolliCommissioni,
    LibriFilmMusica,
    ManutenzioneVeicoli,
    PedaggiTelepass,
    Prelievi,
    ProdottiVistaUdito,
    Regali,
    RimborsiSpeseStorni,
    RistorantiBar,
    SpeseMediche,
    StipendiPensioni,
    TabaccaiSimili,
    TempoLibero,
    TrasportiNoleggiTaxiParcheggi,
    TVInternetTelefono,
    ViaggiVacanze
}


enum TurtleIntents
{
    EatTurtle = 0,
    LikeTurtle = 1,
    Unknown = 2,
    Ninjitsu = 3,
    TurtleCare = 4,
}