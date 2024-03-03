namespace Hive.Data.Entities;

public class HttpRequest
{
    public int Id { get; set; }

    public string Method { get; set; }

    public DateTime Date { get; set; }

    public ulong Size { get; set; }

    public Dictionary<string, string> Headers { get; set; }

    public Dictionary<string, string> QueryStrings { get; set; }

    public Dictionary<string, string> FormValues { get; set; }

}
