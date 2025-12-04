using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MailSweeper.Models;

public class SweepRule
{
    private uint _minItemsToKeep;
    private uint _maxItemsToKeep;

    /// <summary>
    /// Gets or sets the name of the sender
    /// </summary>
    public string? Name {get; set; } 

    public uint MinItemsToKeep
    {
        get => _minItemsToKeep;
        set
        {
            if (value > _maxItemsToKeep)
                throw new ArgumentException("MinItemsToKeep cannot be greater than MaxItemsToKeep.");
            _minItemsToKeep = value;
        }
    }

    public uint MaxItemsToKeep
    {
        get => _maxItemsToKeep;
        set
        {
            if (value < _minItemsToKeep)
                throw new ArgumentException("MaxItemsToKeep cannot be less than MinItemsToKeep.");
            _maxItemsToKeep = value;
        }
    }

    public uint DaysToKeep {get; set; }
}