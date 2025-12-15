using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MailSweeper.Models;

namespace MailSweeper.Messages;

public class SenderStatsRequestMessage : AsyncRequestMessage<IEnumerable<SenderStats>>
{
    
}