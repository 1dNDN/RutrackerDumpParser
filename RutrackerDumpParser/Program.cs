// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Xml.Serialization;

using RutrackerDumpParser;

Console.WriteLine("Hello, World!");

var sw = Stopwatch.StartNew();
var serializer = new XmlSerializer(typeof(Torrents));

var reader = new StreamReader("C:\\Games\\rutracker-20240831.xml");
var data = (Torrents)serializer.Deserialize(reader);

Console.WriteLine($"Время на парсинг: {sw.Elapsed}");
Console.WriteLine($"Количество топиков: {data.TorrentsList.Count}");

sw.Restart();

var db = new DatabaseLowLvl();
db.LoadTopicsByChunksWithParameter(data.TorrentsList.Select(root => new DbTorrent(root)).ToArray());
db.Close();

Console.WriteLine($"Время на работу с SQLite: {sw.Elapsed}");
