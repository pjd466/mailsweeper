using System.Collections.Generic;
using System.Threading.Tasks;
using MailSweeper.Models;
using MailKit.Net.Imap;
using MailKit;
using MailKit.Search;
using System;
using System.Linq;
using System.Data;

namespace MailSweeper.Services;

public static class MailServerService
{
    private static string _host = "imap.one.com";
    private static int _port = 993;
    private static bool _useSSL = true;
    private static string _username = "pieter@xyrinx.com";
    private static string _folderName = "INBOX";
    private static string _password = "";

    public static string password
    {
        set
        {
            _password = value;
        }
    }

    public static async Task<IEnumerable<SenderStats>> LoadSenderLinesAsync()
    {
        var result = new Dictionary<string, SenderStats>(StringComparer.OrdinalIgnoreCase);
        
        using var client = new ImapClient();
        await client.ConnectAsync(_host, _port, _useSSL);
        await client.AuthenticateAsync(_username, _password);

        var folder = client.GetFolder(_folderName);
        await folder.OpenAsync(FolderAccess.ReadOnly);

        // Get all UIDs
        var uids = await folder.SearchAsync(SearchQuery.All);

        // Fetch summaries in one go (or in chunks for very large folders)
        var summaries = await folder.FetchAsync(uids, MessageSummaryItems.Envelope | MessageSummaryItems.Size);

        foreach (var summary in summaries)
        {
            var sender = summary.Envelope?.From?.Mailboxes.FirstOrDefault()?.Address ?? "Unknown";
            var size = summary.Size ?? 0;
            var date = summary.Envelope?.Date ?? DateTimeOffset.MinValue;

            if (!result.ContainsKey(sender))
            {
                result[sender] = new SenderStats
                {
                    Name = sender,
                    Count = 0,
                    TotalSize = 0,
                    MostRecentDate = DateTimeOffset.MinValue
                };
            }

            var stats = result[sender];
            stats.Count++;
            stats.TotalSize += size;
            if (date > stats.MostRecentDate)
                stats.MostRecentDate = date;
        }

        await client.DisconnectAsync(true);
        return result.Values.OrderByDescending(s => s.Count);
    }

    public static async Task Sweep(string FolderName, string DestinationFolderName, IList<SweepRule> Rules)
    {
        using var client = new ImapClient();
        await client.ConnectAsync(_host, _port, _useSSL);
        await client.AuthenticateAsync(_username, _password);

        var folder = await client.GetFolderAsync(FolderName);
        await folder.OpenAsync(FolderAccess.ReadWrite);

        List<UniqueId> ids = [];

        foreach (SweepRule Rule in Rules)
        {
            ids.AddRange(await GetUIDsToSweep(folder, Rule));
        }

        IMailFolder DestinationFolder = await client.GetFolderAsync(DestinationFolderName);
        await folder.MoveToAsync(ids, DestinationFolder);
        await client.DisconnectAsync(true);
    }

    public static async Task<IList<UniqueId>> GetUIDsToSweep(IMailFolder folder, SweepRule Rule)
    {
        // Get all UIDs
        var uids = await folder.SearchAsync(SearchQuery.FromContains(Rule.Name));

        // Fetch summaries in one go (or in chunks for very large folders)
        var summaries = await folder.FetchAsync(uids, MessageSummaryItems.Envelope | MessageSummaryItems.Flags);

        int FlaggedCount = summaries.Count( s => (s.Flags & MessageFlags.Flagged) == MessageFlags.Flagged);

        int MinUnflaggedToKeep = (int)Math.Max(0, Rule.MinItemsToKeep - FlaggedCount);
        int MaxUnflaggedToKeep = (int)Math.Max(0, Rule.MaxItemsToKeep - FlaggedCount);
        DateTime cutoffDate = DateTime.UtcNow.AddDays(Rule.DaysToKeep);

        return summaries
                    .Where(s => (s.Flags & MessageFlags.Flagged) == MessageFlags.None)
                    .OrderByDescending(s => s.Envelope?.Date ?? DateTimeOffset.MaxValue)
                    .Skip(MinUnflaggedToKeep)
                    .Where((s, index) => 
                                    index >= MaxUnflaggedToKeep - 1
                                    || (s.Envelope?.Date ?? DateTimeOffset.MaxValue) < cutoffDate)
                    .Select(s => s.UniqueId)
                    .ToList();
     }
}