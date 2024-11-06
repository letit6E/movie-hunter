using System;
using System.IO;

static class SeparatedFileProcessor
{
    public static void Process(string filePath, char sep, Action<string[]> mapFunction)
    {
        using (var reader = new StreamReader(filePath))
        {
            reader.ReadLine();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var fields = line.Split(sep);
                mapFunction(fields);
            }
        }
    }
}
