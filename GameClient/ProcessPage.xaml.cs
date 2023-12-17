using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls;
using Protocol.Models;
using Protocol.Packet;
using System.Collections.ObjectModel;

namespace GameClient;

public partial class ProcessPage : ContentPage, PageClient
{
	public XClient Client { get; set; }
    public ObservableCollection<UserInPage> Users { get; set; }
    public ObservableCollection<string> Messages { get; set; }
    private bool IsRunning { get; set; }

    public string LabelRole { get; set; }
    public string PathImageRole { get; set; }

    public string LabelFirstCard { get; set; }
    public string PathFirstCard { get; set; }

    public string LabelSecondCard { get; set; }
    public string PathSecondCard { get; set; }

    public ObservableCollection<Arsenal> Guns { get; set; }




    private Queue<XPacket> ReveicingPackets { get; set; }

    public ProcessPage(XClient client, ObservableCollection<UserInPage> users, ObservableCollection<Arsenal> guns)
	{
		InitializeComponent();
		Client = client;
		Users = users;
        Guns = guns;
        Messages = new ObservableCollection<string>();
        ReveicingPackets = new Queue<XPacket>();

        PathImageRole = Client.Person.Role.ResourceImage;
        LabelRole = Client.Person.Role.Name;
        if (Client.Person.Role.IsAlways)
            LabelRole += $"Всегда {Client.Person.Role.IsAlwaysWho}";

        PathSecondCard = Client.Person.SecondCard.PathImage;
        LabelSecondCard = Client.Person.SecondCard.Name;

        LabelFirstCard = Client.Person.FirstCard.Name;
        PathFirstCard = Client.Person.FirstCard.PathImage;

        //Guns = new ObservableCollection<Arsenal>();
        //foreach (var gun in Client.Guns)
        //    Guns.Add(gun);

        Players.ItemsSource = Users.Where(x => x.Role.Name != LabelRole);
        

        BindingContext = this;
        IsRunning = true;


        //Task.Run(ReveiceActions);
        //Task.Run(CompleteActions);
	}

    //public async Task CompleteActions()
    //{
    //    while (IsRunning)
    //    {
    //        if (ReveicingPackets.Count == 0)
    //            continue;

    //        ChangeData(ReveicingPackets.Dequeue());
    //        await Task.Delay(1000);
    //    }
    //}

	//public async Task ReveiceActions()
	//{
	//	while (IsRunning)   
	//	{
 //           ReveicingPackets.Enqueue(await Client.ReceivePacket());
 //           await Task.Delay(1000);
 //       }
	//}

    public async void ChangeData(XPacket packet)
    {
        switch (packet.Action)
        {
            //case XPacketActions.SetGuns:
            //    {
            //        MainThread.BeginInvokeOnMainThread(() =>
            //        {
            //            // Guns = (List<Arsenal>)packet.Content;
            //            Guns.Clear();
            //            foreach (var gun in (List<Arsenal>)packet.Content)
            //            {
            //                Guns.Add(gun);
            //            }
            //        });
            //        break;
            //    }
            case XPacketActions.SendMessage:
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Messages.Add(packet.Content.ToString());
                    });
                    break;
                }
            case XPacketActions.AddUser:
                {
                    var persons = (List<Player>)packet.Content;
                    Client.Person = persons.Where(p => p.ID == Client.Person.ID).FirstOrDefault();
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Users.Clear();
                        foreach (var person in persons)
                            Users.Add(UserInPage.ConverterUserForListView(person));

                    });
                    PathImageRole = Client.Person.Role.ResourceImage;
                    break;
                }
        }
    }

    public async void SendMessageClicked(object sender, EventArgs e)
    {
        var message = EntryMessage.Text;
        EntryMessage.Text = null;
        var packet = new XPacket()
        {
            Action = XPacketActions.SendMessage,
            Type = XPacketTypes.Message,
            Content = message
        };
        await Client.SendPacket(packet);
    }

    public async void MyRoleClicked(object sender, EventArgs e)
    {
        string description = "";
        if (Client.Person.Role.IsAlways)
            description += $"Всегда {Client.Person.Role.IsAlwaysWho}\n";
        description += $"{Client.Person.Role.Description}\n";
        if (Client.Person.Role.PermanentEffect is not null)
            description += $"Постоянный эффект: \n{Client.Person.Role.PermanentEffect}";
        DisplayAlert(Client.Person.Role.Name, description, "OK");
    }

    public async void RoleClicked(object sender, EventArgs e)
    {
        if (Client.Person.IsYourMove)
        {
            var image = (ImageButton)sender;
            var user = (UserInPage)image.BindingContext;

            string description = "";
            if (user.Role.IsAlways)
                description += $"Всегда {user.Role.IsAlwaysWho}\n";

            description += $"{user.Role.Description}\n";
            if (user.Role.PermanentEffect is not null)
                description += "Постоянный эффект: \n" + user.Role.PermanentEffect;

            DisplayAlert(user.Role.Name, description, "OK");
        }
    }

    public async void FirstCardClicked(object sender, EventArgs e)
    {
        if (Client.Person.IsYourMove)
        {
            var image = (ImageButton)sender;
            var user = (UserInPage)image.BindingContext;
            var description = $"{user.FirstCard.Name} X{user.FirstCard.Score}";

            DisplayAlert($"Карта верности игрока {user.Role.Name}", description, "OK");
        }
    }

    public async void SecondCardClicked(object sender, EventArgs e)
    {
        if (Client.Person.IsYourMove)
        {
            var image = (ImageButton)sender;
            var user = (UserInPage)image.BindingContext;
            var description = $"{user.SecondCard.Name} X{user.SecondCard.Score}";

            DisplayAlert($"Карта верности игрока {user.Role.Name}", description, "OK");
        }
    }
}