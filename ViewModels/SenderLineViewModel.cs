using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MailSweeper.Models;

namespace MailSweeper.ViewModels;

/// <summary>
/// This is a ViewModel which represents a <see cref="Models.SenderStats"/> 
/// </summary>
public partial class SenderStatsViewModel : ViewModelBase
{
    /// <summary>
    /// Creates a new blank SenderStatsViewModel
    /// </summary>
    public SenderStatsViewModel()
    {
        // empty
    }

    /// <summary>
    /// Creates a new SenderStatsViewModel for the given <see cref="Models.SenderStats"/> 
    /// </summary>
    /// <param name="line">The SenderStats to load</param>
    public SenderStatsViewModel(SenderStats line)
    {
        // Init the properties with the given values
        Name = line.Name;
        Count = line.Count;
        TotalSize = line.TotalSize;
        MostRecentDate = line.MostRecentDate;
    }

    /// <summary>
    /// Gets or sets the Name of the SenderStats
    /// </summary>
    [ObservableProperty]
    private string? _name;

    /// <summary>
    /// Gets or sets the number of messages from the sender with this name
    /// </summary>
    [ObservableProperty]
    private int _count;

    /// <summary>
    /// The total size of all messages for the sender
    /// </summary>
    [ObservableProperty]
    private long _totalSize;

    /// <summary>
    /// The most recent date a message for the sender was received
    /// </summary>
    [ObservableProperty]
    private DateTimeOffset _mostRecentDate;

    /// <summary>
    /// Gets a SenderStats of this ViewModel
    /// </summary>
    /// <returns>The SenderStats</returns>
    public SenderStats GetSenderStats()
    {
        return new SenderStats()
        {
            Name = this.Name,
            Count = this.Count,
            TotalSize = this.TotalSize,
            MostRecentDate = this.MostRecentDate
        };
    }
}