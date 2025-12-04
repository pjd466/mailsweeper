using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using MailSweeper.Models;
using MailSweeper.ViewModels;
using MailSweeper.Services;

namespace MailSweeper.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Gets a collection of <see cref="Models.SenderStats"/> which allows adding and removing lines
    /// </summary>
    public ObservableCollection<SenderStatsViewModel> SenderLines {get;} = new ObservableCollection<SenderStatsViewModel>();

    private string _password = "";
    public string password
    {
        get { return _password; }
        set 
        { 
            SetProperty(ref _password, value); 
            MailServerService.password = value;
        }
    }

    [RelayCommand]
    private async Task FetchSenderLines()
    {
        var LinesLoaded = await MailServerService.LoadSenderLinesAsync();

        if (LinesLoaded is not null)
        {
            foreach (var line in LinesLoaded)
            {
                SenderLines.Add(new SenderStatsViewModel(line));
            }
        }
    }
}
