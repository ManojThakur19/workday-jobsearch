using System.Text.Json;
using System.Text.Json.Serialization;

namespace JobListingsTable;

public class JobListing
{
    [JsonPropertyName("date_posted")]
    public DateTime? DatePosted { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

public static class Program
{
    public static void Main(string[] args)
    {
        // Path to the JSON file containing the array of job objects.
        // Defaults to "jobs.json" in the current directory, or pass a path as the first argument.
        string jsonPath = args.Length > 0 ? args[0] : "jobs.json";

        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"File not found: {jsonPath}");
            Console.WriteLine("Usage: JobListingsTable <path-to-json-file>");
            return;
        }

        string jsonContent;
        try
        {
            jsonContent = File.ReadAllText(jsonPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
            return;
        }

        List<JobListing>? jobs;
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        try
        {
            jobs = JsonSerializer.Deserialize<List<JobListing>>(jsonContent, options);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
            return;
        }

        if (jobs == null || jobs.Count == 0)
        {
            Console.WriteLine("No job listings found.");
            return;
        }

        PrintTable(jobs);
    }

    private static void PrintTable(List<JobListing> jobs)
    {
        const int dateWidth = 12;
        const int titleWidth = 45;
        const int urlWidth = 70;

        string header = $"{Pad("Date Posted", dateWidth)} | {Pad("Title", titleWidth)} | {Pad("URL", urlWidth)}";
        Console.WriteLine(header);
        Console.WriteLine(new string('-', header.Length));

        foreach (var job in jobs)
        {
            string date = job.DatePosted.HasValue ? job.DatePosted.Value.ToString("yyyy-MM-dd") : "N/A";
            string title = Truncate(job.Title ?? "N/A", titleWidth);
            string url = job.Url ?? "N/A";

            Console.WriteLine($"{Pad(date, dateWidth)} | {Pad(title, titleWidth)} | {url}");
        }

        Console.WriteLine();
        Console.WriteLine($"Total listings: {jobs.Count}");
    }

    private static string Pad(string value, int width)
    {
        return value.Length >= width ? value : value.PadRight(width);
    }

    private static string Truncate(string value, int maxWidth)
    {
        if (value.Length <= maxWidth)
        {
            return value;
        }

        return value.Substring(0, maxWidth - 3) + "...";
    }
}
