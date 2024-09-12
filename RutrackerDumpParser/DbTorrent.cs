namespace RutrackerDumpParser;

public class DbTorrent
{
    public DbTorrent(TorrentRoot torrent)
    {
        Hash = torrent.Torrent.Hash;
        TrackerId = torrent.Torrent.TrackerId;
        Title = torrent.Title;
        ForumId = torrent.Forum.Id;
        ForumText = torrent.Forum.Text;
        Content = torrent.Content;
        OldHashes = string.Join(";", torrent.Old.Select(old => $"{old.Hash}|{old.Text}"));
        TopicId = torrent.Id;
        RegistredAt = torrent.RegistredAt;
        Size = torrent.Size;
        Files = DirAndFilesToString(torrent);
        Del = torrent.Del != null;
    }

    public string Hash { get; set; }
    public long TrackerId { get; set; }
    public string Title { get; set; }
    public long ForumId { get; set; }
    public string ForumText { get; set; }
    public string Content { get; set; }
    public string OldHashes { get; set; }
    public long TopicId { get; set; }
    public string RegistredAt { get; set; }
    public long Size { get; set; }
    public string Files { get; set; }
    public bool Del { get; set; }
    
    private string DirAndFilesToString(TorrentRoot torrentRoot)
    {
        var result = "";
        result += string.Join("", GetFilePaths(torrentRoot.Dir));
        result += string.Join("", GetFilePaths(torrentRoot.File));

        return result;
    }

    public static string[] GetFilePaths(List<TorrentFile> files)
    {
        var filePaths = new List<string>();

        foreach (var file in files)
            filePaths.Add($"{file.Name}|{file.Size};");

        return filePaths.ToArray();
    }

    public static string[] GetFilePaths(List<TorrentDir> dirs)
    {
        var filePaths = new List<string>();

        foreach (var dir in dirs)
            GetFilePathsRecursive(dir, "", filePaths);

        return filePaths.ToArray();
    }

    private static void GetFilePathsRecursive(TorrentDir dir, string currentPath, List<string> filePaths)
    {
        foreach (var file in dir.Files)
            filePaths.Add($"{Path.Combine(currentPath, dir.Name, file.Name)}|{file.Size};");

        foreach (var innerDir in dir.InnerDir)
            GetFilePathsRecursive(innerDir, Path.Combine(currentPath, dir.Name), filePaths);
    }
}
