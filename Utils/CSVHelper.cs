public class CSVHelper
{
    public static List<string[]> ReadCSV(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"The file '{filePath}' was not found.");
            throw new FileNotFoundException($"The file '{filePath}' was not found.");

        }
        var lines = File.ReadAllLines(filePath);

        List<string[]> result = new List<string[]>();
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].Trim();
            result.Add(lines[i].Split(','));
        }
        return result;
    }

    public static void WriteCSV(string filePath, List<string[]> data)
    {
        var lines = data.Select(fields => string.Join(",", fields)).ToArray();
        File.WriteAllLines(filePath, lines);
    }
}