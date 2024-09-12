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

    [XmlIgnore]
    public bool Del
    {
        get
        {
            return this.TempProperty != null;
        }
    }
    
    [XmlElement("del", IsNullable = true)]
    public string TempProperty { get; set; }

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
