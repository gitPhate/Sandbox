using Newtonsoft.Json;
using SevenZip;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using CompressionMode = System.IO.Compression.CompressionMode;

namespace DataSetCompression
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestDS ds1 = new TestDS();

            //for(int i = 0; i < 15000; ++i)
            //{
            //    var row11 = ds1.TestTable.NewTestTableRow();
            //    row11.Id = i;
            //    row11.Name = Guid.NewGuid().ToString();
            //    ds1.TestTable.AddTestTableRow(row11);
            //}

            var sevenZpath = Path.Combine(@"C:\development\dotnet\SandBox\DataSetCompression", $"7z_{(Environment.Is64BitProcess ? "x64" : "x86")}.dll");
            SevenZipBase.SetLibraryPath(sevenZpath);

            //CompressionBenchmarkWithKb(1);
            //CompressionBenchmarkWithKb(10);
            //CompressionBenchmarkWithKb(100);
            //CompressionBenchmarkWithKb(1024); //1 MB
            //CompressionBenchmarkWithKb(5120); //5 MB
            //CompressionBenchmarkWithKb(10240); //10 MB

            //< 1000 kb: GZIP
            //1000 kb - 5000 kb: Normal
            // > 5000 kb: none
            CompressionBenchmarkWithKb(5000);

            Console.ReadLine();
        }

        static void CompressionBenchmarkWithKb(int kb)
        {
            decimal kbRatio = 0.1657M;
            int lines = (int)Math.Round(kb / kbRatio);

            using (TestDS ds = new TestDS())
            {
                foreach (var x in Enumerable.Range(1, lines))
                {
                    var row1 = ds.TestTable.NewTestTableRow();
                    row1.Id = x;
                    row1.Name = Guid.NewGuid().ToString();
                    ds.TestTable.AddTestTableRow(row1);
                }

                //byte[] bytes = SerializeDataSet(ds);
                //TestDS ds2 = DeserializeDataSet<TestDS>(bytes);

                using (Stream stringStream = SerializeDataSetWithDataContractSerializer(ds))
                {
                    Console.WriteLine($"DataTable rows {ds.TestTable.Count}");
                    Console.WriteLine($"Uncompressed kb: {(stringStream.Length / 1024M).ToString("0.00")}");
                    Console.WriteLine();

                    TestGZipCompression(stringStream);

                    Test7ZipCompressionLevel(stringStream, SevenZip.CompressionLevel.Fast);
                    Test7ZipCompressionLevel(stringStream, SevenZip.CompressionLevel.High);
                    Test7ZipCompressionLevel(stringStream, SevenZip.CompressionLevel.Low);
                    Test7ZipCompressionLevel(stringStream, SevenZip.CompressionLevel.None);
                    Test7ZipCompressionLevel(stringStream, SevenZip.CompressionLevel.Normal);
                    Test7ZipCompressionLevel(stringStream, SevenZip.CompressionLevel.Ultra);
                }
            }
        }

        static void TestGZipCompression(Stream stringStream)
        {
            byte[] gzipCompressed = null;
            decimal gzipRatio = 0;

            Stopwatch stopwatch = new Stopwatch();
            using (MemoryStream ms = new MemoryStream())
            using (GZipStream zipStream = new GZipStream(ms, CompressionMode.Compress, false))
            {
                stopwatch.Start();
                stringStream.CopyTo(zipStream);
                gzipCompressed = ms.ToArray();
                stopwatch.Stop();
                gzipRatio = (decimal)gzipCompressed.Count() / (decimal)stringStream.Length * 100;
            }

            Console.WriteLine($"GZIP Compression");
            Console.WriteLine($"Ratio: {gzipRatio.ToString("0.00")} %");
            Console.WriteLine($"Elapsed: {stopwatch.Elapsed}");
            Console.WriteLine($"Compressed kb: {(gzipCompressed.Length / 1024M).ToString("0.00")}");
            Console.WriteLine();
        }

        static void Test7ZipCompressionLevel(Stream stringStream, SevenZip.CompressionLevel level)
        {
            stringStream.Position = 0;
            decimal compressionRatio = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            byte[] result = SevenZipCompress(stringStream, level, out compressionRatio);
            stopwatch.Stop();
            Console.WriteLine($"CompressionLevel: {level}");
            Console.WriteLine($"Ratio: {compressionRatio.ToString("0.00")} %");
            Console.WriteLine($"Elapsed: {stopwatch.Elapsed}");
            Console.WriteLine($"Compressed kb: {(result.Length / 1024M).ToString("0.00")}");
            Console.WriteLine();
        }

        static byte[] SevenZipCompress(Stream stringStream, SevenZip.CompressionLevel level, out decimal sevenZipRatio)
        {
            byte[] sevenZipCompressed = null;
            sevenZipRatio = 0;

            using (MemoryStream resultStream = new MemoryStream())
            {
                SevenZipCompressor compressor =
                    new SevenZipCompressor
                    {
                        CompressionMethod = CompressionMethod.Lzma2,
                        CompressionLevel = level
                    };

                compressor.CompressStream(stringStream, resultStream);
                sevenZipCompressed = resultStream.ToArray();
                sevenZipRatio = sevenZipCompressed.Count() / (decimal)stringStream.Length * 100;
                return sevenZipCompressed;
            }
        }

        static byte[] SerializeDataSet<T>(T ds)
            where T : DataSet
        {
            using (Stream stringStream = SerializeDataSetWithDataContractSerializer(ds))
            using (MemoryStream resultStream = new MemoryStream())
            {
                SevenZipCompressor compressor =
                    new SevenZipCompressor
                    {
                        CompressionMethod = CompressionMethod.Lzma2,
                        CompressionLevel = SevenZip.CompressionLevel.Normal
                    };

                compressor.CompressStream(stringStream, resultStream);
                byte[] byteArray = resultStream.ToArray();
                return byteArray;
            }
        }

        static T DeserializeDataSet<T>(byte[] bytes)
            where T : DataSet, new()
        {
            using (MemoryStream inputStream = new MemoryStream(bytes))
            using (MemoryStream outputStream = new MemoryStream())
            {
                SevenZipExtractor extractor = new SevenZipExtractor(inputStream);
                extractor.ExtractFile(0, outputStream);
                outputStream.Position = 0;

                byte[] b = outputStream.ToArray();
                string xml = Encoding.UTF8.GetString(b);

                T dataSet = DeserializeDataSetWithDataContractSerializer<T>(outputStream);
                return dataSet;
            }
        }

        static Stream SerializeDataSetWithDataContractSerializer<T>(T dataSet)
            where T : DataSet
        {
            MemoryStream ms = new MemoryStream();
            DataContractSerializer ser = new DataContractSerializer(typeof(T));
            ser.WriteObject(ms, dataSet);
            ms.Position = 0;
            return ms;
        }

        static T DeserializeDataSetWithDataContractSerializer<T>(Stream stream)
            where T : DataSet
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(T));
            T dataSet = (T)ser.ReadObject(stream);
            return dataSet;
        }
    }
}
