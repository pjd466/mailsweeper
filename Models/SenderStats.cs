using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MailSweeper.Models;

/// <summary>
/// Model for a single sender's report
/// </summary>
public class SenderStats
{
    /// <summary>
    /// Gets or sets the name of the sender
    /// </summary>
    public string? Name {get; set; } 

    /// <summary>
    /// Gets or sets the count of messages for the sender
    /// </summary>
    public int Count {get; set; }

    /// <summary>
    /// The total size of all messages for the sender
    /// </summary>
    public long TotalSize { get; set; }

    /// <summary>
    /// The most recent date a message for the sender was received
    /// </summary>
    public DateTimeOffset MostRecentDate { get; set; }

}