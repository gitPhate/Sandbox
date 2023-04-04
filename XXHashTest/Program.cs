// See https://aka.ms/new-console-template for more information
using K4os.Hash.xxHash;

Console.WriteLine("Hello, World!");

unsafe
{
    byte[] b = File.ReadAllBytes(@"C:\Temp\APEEvo\Data\Attachments\defect_2_2.jpg");
    ulong hash1 = 0;
    fixed (byte* bp = b)
    {
        hash1 = XXH64.DigestOf(bp, b.Length);
    }

    ulong hash2 = XXH64.DigestOf(b, 0, b.Length);
}