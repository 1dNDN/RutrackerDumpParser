// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text.Json;
using System.Xml.Serialization;

using Microsoft.EntityFrameworkCore;

using TloSql;

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
    [XmlElement(ElementName = "title")]
    public string Title;

    [XmlElement(ElementName = "torrent")]
    public TorrentMeta Torrent;

    [XmlElement(ElementName = "forum")]
    public Forum Forum;

    [XmlElement(ElementName = "content")]
    public string Content;

    [XmlElement(ElementName = "file")]
    public List<TorrentFile> File;
    
    [XmlElement(ElementName = "dir")]
    public List<TorrentDir> Dir;

    [XmlElement(ElementName = "old")]
    public List<Old> Old;

    [XmlAttribute(AttributeName = "id")]
    public long Id;

    [XmlAttribute(AttributeName = "registred_at")]
    public string RegistredAt;

    [XmlAttribute(AttributeName = "size")]
    public long Size;

    [XmlText]
    public string Text;

    [XmlElement(ElementName = "del")]
    public string Del;
}

[XmlRoot(ElementName = "torrent")]
public class TorrentMeta
{
    [XmlAttribute(AttributeName = "hash")]
    public string Hash;

    [XmlAttribute(AttributeName = "tracker_id")]
    public long TrackerId;
}

[XmlRoot(ElementName = "forum")]
public class Forum
{
    [XmlAttribute(AttributeName = "id")]
    public long Id;

    [XmlText]
    public string Text;
}

[XmlRoot(ElementName = "file")]
public class TorrentFile
{
    [XmlAttribute(AttributeName = "size")]
    public long Size;

    [XmlAttribute(AttributeName = "name")]
    public string Name;
}

[XmlRoot(ElementName = "dir")]
public class TorrentDir
{
    [XmlElement(ElementName = "file")]
    public List<TorrentFile> Files;

    [XmlAttribute(AttributeName = "name")]
    public string Name;

    [XmlElement(ElementName = "dir")]
    public List<TorrentDir> InnerDir;
}

[XmlRoot(ElementName = "old")]
public class Old
{
    [XmlAttribute(AttributeName = "hash")]
    public string Hash;

    [XmlAttribute(AttributeName = "time")]
    public string Time;

    [XmlText]
    public string Text;
}
