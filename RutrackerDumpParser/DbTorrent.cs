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
        OldHashes = string.Join(";", torrent.Old);
        TopicId = torrent.Id;
        RegistredAt = torrent.RegistredAt;
        Size = torrent.Size;
        Files = DirAndFilesToString(torrent);
    }

    public string Hash;
    public long TrackerId;
    public string Title;
    public long ForumId;
    public string ForumText;
    public string Content;
    public string OldHashes;
    public long TopicId;
    public string RegistredAt;
    public long Size;
    public string Files;

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
