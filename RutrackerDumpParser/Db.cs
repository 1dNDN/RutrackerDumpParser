using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using SQLitePCL;

namespace TloSql;

public class DatabaseLowLvl
{
    private sqlite3 _connection;

    private Stopwatch _stopwatch = new Stopwatch();

    private void StartTimer()
    {
        _stopwatch.Reset();
        _stopwatch.Start();
    }

    private void StopTimer(string text)
    {
        _stopwatch.Stop();
        Console.WriteLine($"{text} {_stopwatch.Elapsed}");
    }

    public DatabaseLowLvl()
    {
        SQLitePCL.raw.SetProvider(new SQLite3Provider_e_sqlite3());
        
        OpenDb();

        SetPragmas();

        BeginTransaction();

        StartTimer();
        
        Truncate("main.Topics");
        
        StopTimer("Clear all table");
    }

    private void ExecuteSql(string sql)
    {
        var rc = raw.sqlite3_exec(_connection, sql);

        CheckRc(rc);
    }

    private sqlite3_stmt PrepareStatement(string sql)
    {
        var rc = raw.sqlite3_prepare_v2(_connection, sql, out var stmt);
        
        CheckRc(rc);

        return stmt;
    }

    private void Bind(sqlite3_stmt stmt, int index, int value)
    {
        var rc = raw.sqlite3_bind_int(stmt, index, value);
        CheckRc(rc);
    }
    
    private void Bind(sqlite3_stmt stmt, int index, long value)
    {
        var rc = raw.sqlite3_bind_int64(stmt, index, value);
        CheckRc(rc);
    }
    
    private void Bind(sqlite3_stmt stmt, int index, string value)
    {
        var rc = raw.sqlite3_bind_text(stmt, index, value);
        CheckRc(rc);
    }

    private void OpenDb()
    {
        var rc = raw.sqlite3_open("rutrackerdb.db", out _connection);
        
        CheckRc(rc);
    }
    
    private void CheckRc(int rc)
    {
        if (rc != raw.SQLITE_OK && rc != raw.SQLITE_DONE)
            throw new ProviderException(rc.ToString() + raw.sqlite3_errmsg(_connection).utf8_to_string());
    }
    
    private void ExecBindedStatement(sqlite3_stmt stmt)
    {
        var rc = raw.sqlite3_step(stmt);
        CheckRc(rc);
        rc = raw.sqlite3_clear_bindings(stmt);
        CheckRc(rc);
        rc =  raw.sqlite3_reset(stmt);
        CheckRc(rc);
    }

    public void LoadTopicsByChunksWithParameter(DbTorrent[] torrents)
    {
        StartTimer();
        
        var chunkSize = 32766 / 11;
        
        var chunks = ChunkArray(torrents, chunkSize);
    
        var sql = "INSERT INTO Topics (Hash, TrackerId, Title, ForumId, ForumText, Content, OldHashes, TopicId, RegistredAt, Size, Files) VALUES ";
        var paramsPart = $"(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?),";
        
        ConstructStatement(chunkSize, sql, paramsPart, out var stmt);

        for (var i = 0; i < chunks.Count; i++)
        {
            if (i == chunks.Count - 1)
            {
                ConstructStatement(chunks[i].Length, sql, paramsPart, out var stmtLast);
                
                BindKeeperParams(chunks, i, stmtLast);

                ExecBindedStatement(stmtLast);
            }
            else
            {
                BindKeeperParams(chunks, i, stmt);

                ExecBindedStatement(stmt);
            }
        }
        
        StopTimer($"Insert all topics with chunk size {chunkSize}");
    }

    private void BindKeeperParams(List<DbTorrent[]> chunks, int ci, sqlite3_stmt stmt)
    {
        for (var i = 0; i < chunks[ci].Length; i++)
        {
            Bind(stmt, 11 * i + 1, chunks[ci][i].Hash);
            Bind(stmt, 11 * i + 2, chunks[ci][i].TrackerId);
            Bind(stmt, 11 * i + 3, chunks[ci][i].Title);
            Bind(stmt, 11 * i + 4, chunks[ci][i].ForumId);
            Bind(stmt, 11 * i + 5, chunks[ci][i].ForumText);
            Bind(stmt, 11 * i + 6, chunks[ci][i].Content);
            Bind(stmt, 11 * i + 7, chunks[ci][i].OldHashes);
            Bind(stmt, 11 * i + 8, chunks[ci][i].TopicId);
            Bind(stmt, 11 * i + 9, chunks[ci][i].RegistredAt);
            Bind(stmt, 11 * i + 10, chunks[ci][i].Size);
            Bind(stmt, 11 * i + 11, chunks[ci][i].Files);
        }
    }

    private void ConstructStatement(int chunkSize, string sql, string paramsPart, out sqlite3_stmt stmt)
    {
        var sb = new StringBuilder();
            
        sb.Append(sql);
        
        for(var i = 0; i < chunkSize; i++)
            sb.Append(paramsPart);

        sb.Remove(sb.Length - 1, 1);
        sb.Append(';');

        var rc = raw.sqlite3_prepare_v2(_connection, sb.ToString(), out stmt);
        CheckRc(rc);
    }

    private void SetPragmas()
    {

        ExecuteSql("PRAGMA synchronous=OFF;");
        ExecuteSql("PRAGMA journal_mode=OFF;");
        ExecuteSql("PRAGMA count_changes=OFF;");
        ExecuteSql("PRAGMA temp_store=OFF;");
        ExecuteSql("PRAGMA page_size=65536;");
        ExecuteSql("PRAGMA cache_size=-16777216;");
        ExecuteSql("PRAGMA locking_mode = EXCLUSIVE;");
    }
    
    public void Close()
    {
        CommitTransaction();
        Optimize();
    }
    
    private void Truncate(string table) =>
        ExecuteSql($"delete from {table}");

    private void BeginTransaction() =>
        ExecuteSql("begin;");

    private void CommitTransaction() =>
        ExecuteSql("commit;");
    
    private void Optimize() =>
        ExecuteSql("PRAGMA optimize;");

    public static List<T[]> ChunkArray<T>(T[] array, int chunkSize)
    {
        var chunks = new List<T[]>();

        for (int i = 0; i < array.Length; i += chunkSize)
        {
            var chunk = new T[Math.Min(chunkSize, array.Length - i)];
            Array.Copy(array, i, chunk, 0, chunk.Length);
            chunks.Add(chunk);
        }

        return chunks;
    }
}
