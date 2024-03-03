namespace Hive.Data.Entities;

public class EmailRequest
{
    public int Id { get; set; }

    public string Host { get; set; }

    public ulong Size { get; set; }

    public string TextContent { get; set; }
}
