using Protocol.Packet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameClient;

interface PageClient
{
    XClient Client { get; set; }
    ObservableCollection<UserInPage> Users { get; set; }
    ObservableCollection<string> Messages { get; set; }
    void ChangeData(XPacket packet);
    
}