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
        Del = torrent.Del;
    }

    public string Hash { get; }
    public long TrackerId { get; }
    public string Title { get; }
    public long ForumId { get; }
    public string ForumText { get; }
    public string Content { get; }
    public string OldHashes { get; }
    public long TopicId { get; }
    public string RegistredAt { get; }
    public long Size { get; }
    public string Files { get; }
    public bool Del { get; }
    
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
