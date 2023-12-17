using Protocol.Models;
using Protocol.Packet;
using System.Collections.ObjectModel;

namespace GameClient;

public partial class ProcessPage : ContentPage, PageClient
{
	public XClient Client { get; set; }
    public ObservableCollection<UserInPage> Users { get; set; }
    public ObservableCollection<Message> Messages { get; set; }
    private bool IsRunning { get; set; }

    public string LabelRole { get; set; }
    public string PathImageRole { get; set; }

    public string LabelFirstCard { get; set; }
    public string PathFirstCard { get; set; }

    public string LabelSecondCard { get; set; }
    public string PathSecondCard { get; set; }

    public ObservableCollection<Arsenal> Guns { get; set; }

    public ProcessPage(XClient client, ObservableCollection<UserInPage> users, ObservableCollection<Arsenal> guns)
	{
		InitializeComponent();
		Client = client;
		Users = users;
        Guns = guns;
        Messages = new ObservableCollection<Message>();

        PathImageRole = Client.Person.Role.ResourceImage;
        LabelRole = Client.Person.Role.Name;
        if (Client.Person.Role.IsAlways)
            LabelRole += $"������ {Client.Person.Role.IsAlwaysWho}";

        PathSecondCard = Client.Person.SecondCard.PathImage;
        LabelSecondCard = Client.Person.SecondCard.Name;

        LabelFirstCard = Client.Person.FirstCard.Name;
        PathFirstCard = Client.Person.FirstCard.PathImage;

        Players.ItemsSource = Users.Where(x => x.Role.Name != LabelRole);
        

        BindingContext = this;
        IsRunning = true;
	}

    public async void FinishStepClicked(object sender, EventArgs e)
    {
        await Client.SendPacket(new XPacket
        {
            Action = XPacketActions.FinishStep,
        });
    }

    public async void ChangeData(XPacket packet)
    {
        switch (packet.Action)
        {
            case XPacketActions.SendMessage:
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (packet.Type == XPacketTypes.Gun)
                        {
                            Messages.Add(new Message()
                            {
                                Color = Colors.Yellow,
                                MessageText = packet.Content.ToString()
                            });
                        }
                        else if (packet.Type == XPacketTypes.Program)
                        {
                            Messages.Add(new Message()
                            {
                                Color = Colors.Aqua,
                                MessageText = packet.Content.ToString()
                            });
                        }
                        else
                        {
                            Messages.Add(new Message()
                            {
                                Color = Colors.White,
                                MessageText = packet.Content.ToString()
                            });
                        }
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

            case XPacketActions.Shot:
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        (int lostHealthPoints, XPacketActions action) =
                            ((int lostHealthPoints, XPacketActions action))packet.Content;

                        await OpenCardOrLostHealth(action, lostHealthPoints);
                    });
                    break;
                }

            case XPacketActions.TakeProgram:
                {
                    // ����� ���������
                    await Client.SendPacket(new XPacket()
                    {
                        Action = XPacketActions.TakeProgram,
                    });
                    break;
                }

            case XPacketActions.DropGun:
                {
                    Client.Person.Gun = null;
                    break;
                }

            case XPacketActions.Nothing:
                {
                    break;
                }

            case XPacketActions.OpenFirstCard:
                {
                    var id = (int)packet.Content;
                    var user = Users.Where(user => user.ID == id).FirstOrDefault();
                    user.FirstCard.IsAvailable = true;
                    if (user != null)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            user.CurrentImageFirstCard = user.FirstCard.PathImage;
                        });
                    }
                    break;
                }

            case XPacketActions.OpenSecondCard:
                {
                    var id = (int)packet.Content;
                    var user = Users.Where(user => user.ID == id).FirstOrDefault();
                    user.FirstCard.IsAvailable = true;
                    if (user != null)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            user.CurrentImageSecondCard = user.SecondCard.PathImage;
                        });
                    }
                    break;
                }

            case XPacketActions.OpenRole:
                {
                    var id = (int)packet.Content;
                    var user = Users.Where(user => user.ID == id).FirstOrDefault();
                    user.Role.IsAvailable = true;
                    if (user != null)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            user.�urrentImageRole = user.Role.ResourceImage;
                        });
                    }
                    break;
                }
        }
    }

    private async Task OpenCardOrLostHealth(XPacketActions action, int lostHealthPoints)
    {
        
        if (action == XPacketActions.OpenCard)
        {
            string description = "�� ������ ������� ���� �� ���� �������� ��� �������� ���� ���� ��������.";
            var result = await DisplayAlert("��� ����������!", description, "������� ����� ��������", "�������� ����");

            if (result)
            {
                result = await DisplayAlert("����� ��������!", "�������� ���� ����� �������� (��� ������ ������ ������ ��� �����).", Client.Person.FirstCard.Name, Client.Person.SecondCard.Name);
                XPacketActions actionCard;
                if (result) actionCard = XPacketActions.OpenFirstCard;
                else actionCard = XPacketActions.OpenSecondCard;
                
                await Client.SendPacket(new XPacket()
                {
                    Action = actionCard,
                    Type = XPacketTypes.Card,
                });
            }
            else
            {
                await Client.SendPacket(new XPacket()
                {
                    Action = XPacketActions.LostHealtPoints,
                    Type = XPacketTypes.HealthPoints,
                    Content = lostHealthPoints
                });
            }
        }
        else if (action == XPacketActions.OpenRole)
        {
            string description = "�� ������ ������� ����� ���� ��� �������� ��� ���� ��������.";
            var result = await DisplayAlert("��� ����������!", description, "������� ����� ����", "�������� ����");
            if (result)
            {
                await Client.SendPacket(new XPacket()
                {
                    Action = XPacketActions.OpenRole,
                    Type = XPacketTypes.Role,
                });
            }
            else
            {
                await Client.SendPacket(new XPacket()
                {
                    Action = XPacketActions.LostHealtPoints,
                    Type = XPacketTypes.HealthPoints,
                    Content = lostHealthPoints
                });
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
            description += $"������ {Client.Person.Role.IsAlwaysWho}\n";
        description += $"{Client.Person.Role.Description}\n";
        if (Client.Person.Role.PermanentEffect is not null)
            description += $"���������� ������: \n{Client.Person.Role.PermanentEffect}";
        DisplayAlert(Client.Person.Role.Name, description, "OK");
    }

    public async void RoleClicked(object sender, EventArgs e)
    {
        var image = (ImageButton)sender;
        var user = (UserInPage)image.BindingContext;

        if (Client.Person.IsYourMove || user.Role.IsAvailable)
        {
            if (!await DisplayAlert("����� ����", "�� ������ ���������� ���� �����, �� ������ ������", "��������", "�����"))
                return;

            string description = "";
            if (user.Role.IsAlways)
                description += $"������ {user.Role.IsAlwaysWho}\n";

            description += $"{user.Role.Description}\n";
            if (user.Role.PermanentEffect is not null)
                description += "���������� ������: \n" + user.Role.PermanentEffect;

            DisplayAlert(user.Role.Name, description, "OK");
        }
    }

    public async void FirstCardClicked(object sender, EventArgs e)
    {
        var image = (ImageButton)sender;
        var user = (UserInPage)image.BindingContext;

        if (Client.Person.IsYourMove || user.FirstCard.IsAvailable)
        {
            var description = $"{user.FirstCard.Name} X{user.FirstCard.Score}";
            DisplayAlert($"����� �������� ������ {user.Role.Name}", description, "OK");
        }
    }

    public async void SecondCardClicked(object sender, EventArgs e)
    {
        var image = (ImageButton)sender;
        var user = (UserInPage)image.BindingContext;

        if (Client.Person.IsYourMove || user.SecondCard.IsAvailable)
        {
            var description = $"{user.SecondCard.Name} X{user.SecondCard.Score}";
            DisplayAlert($"����� �������� ������ {user.Role.Name}", description, "OK");
        }
    }

    public async void GunClicked(object sender, EventArgs e)
    {
        var image = (ImageButton)sender;
        var gun = (Arsenal)image.BindingContext;
        var description = $"\n{gun.Description}\n�������\n{gun.Shot}";

        bool result;
        if (Client.Person.IsYourMove && gun.IsAvailable)
            result = await DisplayAlert($"������ {gun.Name}", description, "�����", "�� �����");
        else
        {
            DisplayAlert($"������ {gun.Name}", description, "�����");
            return;
        }

        if (result)
        {
            Client.Person.Gun = gun;
            await Client.SendPacket(new XPacket()
            {
                Action = XPacketActions.ArmYourself,
                Type = XPacketTypes.Gun,
                Content = gun
            });
        }
    }

    public async void NameClicked(object sender, EventArgs e)
    {
        if (Client.Person.IsYourMove)
        {
            var image = (Button)sender;
            var user = (UserInPage)image.BindingContext;

            if (Client.Person.Gun != null)
            {
                if (Client.Person.Gun.IsReadyShoot)
                {
                    if (await DisplayAlert
                        ("��������",
                        $"�� ����������� �������� � ������ {user.ItemName}",
                        "��������",
                        "�����"))
                    {
                        await Client.SendPacket(new XPacket()
                        {
                            Action = XPacketActions.Shot,
                            Type = XPacketTypes.Person,
                            Content = user.ID
                        });
                    }
                }
                else
                {
                    if (await DisplayAlert
                       ("��������",
                       $"�� ����������� ��������� ������ � ������ {user.ItemName}",
                       "���������",
                       "�����"))
                    {
                        Client.Person.Gun.IsReadyShoot = true;
                        await Client.SendPacket(new XPacket()
                        {
                            Action = XPacketActions.AimWeapon,
                            Type = XPacketTypes.Person,
                            Content = user.ID
                        });
                    }
                }
            }
            else
                DisplayAlert("��������", "�� ������ �� ������ �������", "OK");
        }
    }
}