using Protocol;
using Protocol.Models;
using Protocol.Packet;
using System.Collections.ObjectModel;
using System.Net.Sockets;

namespace GameClient;

public partial class GamePage : ContentPage, PageClient
{
	public XClient Client { get; set; }
	public ObservableCollection<UserInPage> Users { get; set; }
    public bool IsRunning { get; set; }

	public ObservableCollection<Message> Messages { get; set; }

    public GamePage(XClient client)
	{
		InitializeComponent();
		Client = client;
		Users = new ObservableCollection<UserInPage>();
		Messages = new ObservableCollection<Message>();
		BindingContext = this;
		//IsRunning = true;
		//Task.Run(Client.ReceivePackets);
		//Task.Run(Client.CompleteReceivingPackets);
        //Task.Run(CompleteActions);
	}

    public void ChangeData(XPacket packet)
	{
		switch (packet.Action)
		{
			//case XPacketActions.SetGuns:
			//	{
			//		Client.Guns = (List<Arsenal>)packet.Content;
			//		break;
			//	}
			case XPacketActions.AddUser:
				{
                    var persons = (List<Player>)packet.Content;
					//Client.Players = persons;
					MainThread.BeginInvokeOnMainThread(() =>
					{
						Users.Clear();
						foreach (var person in persons)
							Users.Add(UserInPage.ConverterUserForListView(person));
					});
					break;
                }

            case XPacketActions.SendMessage:
				{
					MainThread.BeginInvokeOnMainThread(() =>
					{
						Messages.Add(new Message
						{
							Color = Colors.White,
							MessageText = packet.Content.ToString()
                        });
					});
					break;
				}

			case XPacketActions.StartGame:
				{
					(List<Player> persons, List<Arsenal> guns) 
						= ((List<Player>, List<Arsenal>))packet.Content;

					Client.Person = persons.Where(player => player.ID == Client.Person.ID).FirstOrDefault();
                    MainThread.BeginInvokeOnMainThread(() =>
					{
                        Users.Clear();
                        foreach (var person in persons)
                            Users.Add(UserInPage.ConverterUserForListView(person));

						var newGuns = new ObservableCollection<Arsenal>();
						foreach (var gun in guns)
							newGuns.Add(gun);
						Client.CurrentPage = new ProcessPage(Client, Users, newGuns);
						Navigation.PushAsync(Client.CurrentPage);
					});
                    break;
				}

		}
	}

	public async void ClickedReady(object sender, EventArgs e)
	{
		Client.Person.IsReady = !Client.Person.IsReady;

		if (Client.Person.IsReady)
			Readiness.Text = "ÕÂ „ÓÚÓ‚";
		else
            Readiness.Text = "√ÓÚÓ‚";
        

		var packet = new XPacket()
		{
			Type = XPacketTypes.Person,
			Action = XPacketActions.Readiness,
			Content = Client.Person
		};

		await Client.SendPacket(packet);
	}

	//public async void CompleteActions()
	//{
 //       while (IsRunning)
	//	{
	//		var packet = await Client.ReceivePacket();
 //           ChangeData(packet);
	//		await Task.Delay(1000);
	//	}
	//}


    private void MainPage_Disappearing(object sender, EventArgs e)
    {
        if (Client is not null)
            Client.Close();
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

}

public class UserInPage
{
	public int ID { get; set; }
    public string ItemName { get; set; }
    public Color ItemColor { get; set; }
	public string ItemIsReady { get; set; }
	public string ItemIsYouMove { get; set; }
	public string —urrentImageRole { get; set; }
	public string CurrentImageFirstCard { get; set; }
	public string CurrentImageSecondCard { get; set; }

	public Role Role { get; set; }
	public Loyalty FirstCard { get; set; }
	public Loyalty SecondCard { get; set; }


	public List<Arsenal> Guns { get; set; }
    public static UserInPage ConverterUserForListView(Player user)
    {
		return new UserInPage()
		{
			ID = user.ID,
			ItemName = user.Name,
			ItemColor = new Color(user.Color.R, user.Color.G, user.Color.B),
			ItemIsReady = user.IsReady ? "√ÓÚÓ‚" : "ÕÂ „ÓÚÓ‚",
			ItemIsYouMove = user.IsYourMove ? "’Ó‰ËÚ" : null,
			—urrentImageRole = Role.DefaultImage,
			CurrentImageFirstCard = user.FirstCard != null ? user.FirstCard.DefaultImage : null,
			CurrentImageSecondCard = user.SecondCard != null ? user.SecondCard.DefaultImage : null,
            Role = user.Role,
            FirstCard = user.FirstCard,
            SecondCard = user.SecondCard,
        };
    }
}



//if (XPacketTypes.Person == packet.Type)
//{
//    MainThread.BeginInvokeOnMainThread(() =>
//    {
//        Client.Players.Add((Player)packet.Content);
//        Users.Add
//            (UserInPage.ConverterUserForListView((Player)packet.Content));
//    });
//}
//else if (XPacketTypes.Persons == packet.Type)
//{
//    //((PageClient)CurrentPage).Users = new ObservableCollection<UserInPage>();

//    var persons = (List<Player>)packet.Content;
//    MainThread.BeginInvokeOnMainThread(() =>
//    {
//        foreach (var player in persons)
//        {
//            Client.Players.Add(player);
//            Users.Add
//                (UserInPage.ConverterUserForListView(player));
//        }
//    });
//}

//            case XPacketActions.RemoveUser:
//                {
//                    int id = (int)packet.Content;
//MainThread.BeginInvokeOnMainThread(() =>
//                    {
//                        Users.RemoveAt(id);
//                    });
//break;
//                }