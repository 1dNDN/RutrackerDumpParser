// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text.Json;
using System.Xml.Serialization;

using Microsoft.EntityFrameworkCore;

using TloSql;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

Console.WriteLine("Hello, World!");

var sw = Stopwatch.StartNew();
var serializer = new XmlSerializer(typeof(Torrents));

var reader = new StreamReader("C:\\Games\\rutracker-20240831.xml");
var test = (Torrents)serializer.Deserialize(reader);

Console.WriteLine($"Elaped: {sw.Elapsed}");
Console.WriteLine($"Hui {test.TorrentsList.Count}");

sw.Restart();

var db = new DatabaseLowLvl();
db.LoadTopicsByChunksWithParameter(test.TorrentsList.Select(root => new DbTorrent(root)).ToArray());
db.Close();

Console.WriteLine($"Elaped: {sw.Elapsed}");


public class RutrackerContext : DbContext
{
    public static RutrackerContext CreateContext()
    {
        var dbContext = new RutrackerContext();
        dbContext.Database.EnsureCreated();
        return dbContext;
    }

    public DbSet<TorrentRoot> Torrents { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite("Data Source=rutracker.db");
    }
}

[XmlRoot(ElementName = "torrents")]
public class Torrents
{
    [XmlElement(ElementName = "torrent")]
    public List<TorrentRoot> TorrentsList;
}


[XmlRoot(ElementName = "torrent")]
public class TorrentRoot
{
    [field: XmlElement(ElementName = "title")]
    public string Title { get; }

    [XmlElement(ElementName = "torrent")]
    public TorrentMeta Torrent { get; }

    [XmlElement(ElementName = "forum")]
    public Forum Forum { get; }

    [XmlElement(ElementName = "content")]
    public string Content { get; }

    [XmlElement(ElementName = "file")]
    public List<TorrentFile> File { get; }
    
    [XmlElement(ElementName = "dir")]
    public List<TorrentDir> Dir { get; }

    [XmlElement(ElementName = "old")]
    public List<Old> Old { get; }

    [XmlAttribute(AttributeName = "id")]
    public long Id { get; }

    [XmlAttribute(AttributeName = "registred_at")]
    public string RegistredAt { get; }

    [XmlAttribute(AttributeName = "size")]
    public long Size { get; }

    [XmlText]
    public string Text { get; }

    [XmlElement(ElementName = "del")]
    public string Del { get; }
}

[XmlRoot(ElementName = "torrent")]
public class TorrentMeta
{
    [XmlAttribute(AttributeName = "hash")]
    public string Hash { get; }

    [XmlAttribute(AttributeName = "tracker_id")]
    public long TrackerId { get; }
}

[XmlRoot(ElementName = "forum")]
public class Forum
{
    [XmlAttribute(AttributeName = "id")]
    public long Id { get; }

    [XmlText]
    public string Text { get; }
}

[XmlRoot(ElementName = "file")]
public class TorrentFile
{
    [XmlAttribute(AttributeName = "size")]
    public long Size { get; }

    [XmlAttribute(AttributeName = "name")]
    public string Name { get; }
}

[XmlRoot(ElementName = "dir")]
public class TorrentDir
{
    [XmlElement(ElementName = "file")]
    public List<TorrentFile> Files { get; }

    [XmlAttribute(AttributeName = "name")]
    public string Name { get; }

    [XmlElement(ElementName = "dir")]
    public List<TorrentDir> InnerDir { get; }
}

[XmlRoot(ElementName = "old")]
public class Old
{
    [XmlAttribute(AttributeName = "hash")]
    public string Hash { get; }

    [XmlAttribute(AttributeName = "time")]
    public string Time { get; }

    [XmlText]
    public string Text { get; }
}
