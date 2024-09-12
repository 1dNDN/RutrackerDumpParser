using System.ComponentModel;
using System.Xml.Serialization;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace RutrackerDumpParser;


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
    public string Title { get; set; }

    [XmlElement(ElementName = "torrent")]
    public TorrentMeta Torrent { get; set; }

    [XmlElement(ElementName = "forum")]
    public Forum Forum { get; set; }

    [XmlElement(ElementName = "content")]
    public string Content { get; set; }

    [XmlElement(ElementName = "file")]
    public List<TorrentFile> File { get; set; }
    
    [XmlElement(ElementName = "dir")]
    public List<TorrentDir> Dir { get; set; }

    [XmlElement(ElementName = "old")]
    public List<Old> Old { get; set; }

    [XmlAttribute(AttributeName = "id")]
    public long Id { get; set; }

    [XmlAttribute(AttributeName = "registred_at")]
    public string RegistredAt { get; set; }

    [XmlAttribute(AttributeName = "size")]
    public long Size { get; set; }

    [XmlText]
    public string Text { get; set; }
    
    [XmlElement("del", IsNullable = true)]
    public string? Del { get; set; }
}

[XmlRoot(ElementName = "torrent")]
public class TorrentMeta
{
    [XmlAttribute(AttributeName = "hash")]
    public string Hash { get; set; }

    [XmlAttribute(AttributeName = "tracker_id")]
    public long TrackerId { get; set; }
}

[XmlRoot(ElementName = "forum")]
public class Forum
{
    [XmlAttribute(AttributeName = "id")]
    public long Id { get; set; }

    [XmlText]
    public string Text { get; set; }
}

[XmlRoot(ElementName = "file")]
public class TorrentFile
{
    [XmlAttribute(AttributeName = "size")]
    public long Size { get; set; }

    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }
}

[XmlRoot(ElementName = "dir")]
public class TorrentDir
{
    [XmlElement(ElementName = "file")]
    public List<TorrentFile> Files { get; set; }

    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }

    [XmlElement(ElementName = "dir")]
    public List<TorrentDir> InnerDir { get; set; }
}

[XmlRoot(ElementName = "old")]
public class Old
{
    [XmlAttribute(AttributeName = "hash")]
    public string Hash { get; set; }

    [XmlAttribute(AttributeName = "time")]
    public string Time { get; set; }

    [XmlText]
    public string Text { get; set; }
}
