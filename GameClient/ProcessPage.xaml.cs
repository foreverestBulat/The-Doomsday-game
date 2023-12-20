using Protocol.Models;
using Protocol.Packet;
using System.Collections.ObjectModel;
using ApplyProgramSend = Protocol.Models.AppleyProgramSend;
using ApplyPrograms = Protocol.Models.Programs;

namespace GameClient;

public partial class ProcessPage : ContentPage, PageClient
{
	public XClient Client { get; set; }
    public ObservableCollection<UserInPage> Users { get; set; }
    public Queue<Protocol.Models.Program> Programs { get; set; }
    public ObservableCollection<Message> Messages { get; set; }

    private Protocol.Models.Program ApplyCurrentProgram { get; set; }

    private bool IsFinishProgram = true;
    private bool IsRoleExchange = false;
    private List<int> RolesExchange = new List<int>();
    public string LabelRole { get; set; }
    public string PathImageRole { get; set; }

    public string LabelFirstCard { get; set; }
    public string PathFirstCard { get; set; }

    public string LabelSecondCard { get; set; }
    public string PathSecondCard { get; set; }

    public ObservableCollection<Arsenal> Guns { get; set; }
    public ObservableCollection<Protocol.Models.Program> MyPrograms { get; set; }

    public ProcessPage(XClient client, 
        ObservableCollection<UserInPage> users, 
        ObservableCollection<Arsenal> guns, 
        Queue<Protocol.Models.Program> programs)
	{
		InitializeComponent();
		Client = client;
		Users = users;
        Programs = programs;
        Guns = guns;
        MyPrograms = new ObservableCollection<Protocol.Models.Program>();
        Messages = new ObservableCollection<Message>();

        PathImageRole = Client.Person.Role.ResourceImage;
        LabelRole = Client.Person.Role.Name;
        if (Client.Person.Role.IsAlways)
            LabelRole += $"Всегда {Client.Person.Role.IsAlwaysWho}";

        PathSecondCard = Client.Person.SecondCard.PathImage;
        LabelSecondCard = Client.Person.SecondCard.Name;

        LabelFirstCard = Client.Person.FirstCard.Name;
        PathFirstCard = Client.Person.FirstCard.PathImage;

        Players.ItemsSource = Users.Where(x => x.Role.Name != LabelRole);
        
        BindingContext = this;
	}

    public async void FinishStepClicked(object sender, EventArgs e)
    {
        if (Client.Person.IsMyMove)
        {
            await Client.SendPacket(new XPacket
            {
                Action = XPacketActions.FinishStep,
            });
        }
        else
        {
            DisplayAlert("Вы сейчас не ходите", "Дождитесь своего хода", "ОК");
        }
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
                            if (!Client.Person.IsMyMove)
                                Programs.Dequeue();
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
                    Client.Players = persons;
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
                    var program = Programs.Dequeue();

                    
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("Вы берете программу", $"Вы не можете не взять программу\n{program}", "ОК");
                        Client.Person.MyPrograms.Add(program);
                        MyPrograms.Add(program);
                    });

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
                            user.СurrentImageRole = user.Role.ResourceImage;
                        });
                    }
                    break;
                }
        }
    }

    public async void ProgramClicked(object sender, EventArgs e)
    {
        var image = (Button)sender;
        var program = (Protocol.Models.Program)image.BindingContext;
        if (Client.Person.IsMyMove)
        {
            if (await DisplayAlert("Вы взяли программу", $"{program}", "Использовать", "Назад"))
            {
                await ApplyProgram(program);
            }
        }
        else
        {
            DisplayAlert("Вы взяли программу", $"{program}", "Назад");
        }
    }

    public async void TakeProgram(object sender, EventArgs e)
    {
        var image = (Button)sender;
        var program = (Protocol.Models.Program)image.BindingContext;

        if (await DisplayAlert("Программа", $"{program}", "Использовать", "Назад"))
        {

        }
    }

    private async Task ApplyProgram(Protocol.Models.Program program)
    {
        ApplyCurrentProgram = program;
        switch (program.ProgramAction)
        {
            case ApplyPrograms.RoleExchange:
                {
                    IsRoleExchange = true;
                    IsFinishProgram = false;
                    await DisplayAlert("Вы используете программу", "Выберите из списка две роли", "ОК");
                    break;
                }

            case ApplyPrograms.OpenAccessNewWeapons:
                {
                    foreach (var gun in Guns)
                        if (gun is FlareGun || gun is Laser)
                            gun.IsAvailable = true;

                    await Client.SendPacket(new XPacket()
                    {
                        Action = XPacketActions.SendMessage,
                        Type = XPacketTypes.Program,
                        Content = $"{Client.Person.Name} применил программу:\n{ApplyCurrentProgram}"
                    });
                    IsFinishProgram = true;
                    break;
                }

            case ApplyPrograms.ChangeDirectionEveryoneWeapons:
                {
                    await Client.SendPacket(new XPacket()
                    {
                        Action = XPacketActions.ApplyProgram,
                        Type = XPacketTypes.Program,
                        Content = new ApplyProgramSend() 
                        {
                            Program = ApplyCurrentProgram
                        }
                    });
                    IsFinishProgram = true;
                    break;
                }

            case ApplyPrograms.GeneralPain:
                {
                    
                    break;
                }
        }
    }

    public async void FinishProgramClicked(object sender, EventArgs e)
    {
        if (IsFinishProgram) return;
        switch (ApplyCurrentProgram.ProgramAction)
        {
            case ApplyPrograms.RoleExchange:
                {
                    if (RolesExchange.Count() == 2)
                        await Client.SendPacket(new XPacket
                        {
                            Action = XPacketActions.ApplyProgram,
                            Type = XPacketTypes.Program,
                            Content = new ApplyProgramSend()
                            {
                                Program = ApplyCurrentProgram,
                                Data = RolesExchange
                            }
                        });
                    RolesExchange.Clear();
                    break;
                }
        }
    }

    private async Task OpenCardOrLostHealth(XPacketActions action, int lostHealthPoints)
    {
        
        if (action == XPacketActions.OpenCard)
        {
            string description = "Вы должны открыть одну из карт верности или потерять одно очко здоровья.";
            var result = await DisplayAlert("Вас выстрелили!", description, "Открыть карту верности", "Потерять очки");

            if (result)
            {
                result = await DisplayAlert("Карты верности!", "Выберите одну карту верности (все игроки смогут видеть эту карту).", Client.Person.FirstCard.Name, Client.Person.SecondCard.Name);
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
                    Action = XPacketActions.LostHealthPoints,
                    Type = XPacketTypes.HealthPoints,
                    Content = lostHealthPoints
                });
            }
        }
        else if (action == XPacketActions.OpenRole)
        {
            string description = "Вы должны открыть карту роли или потерять два очка здоровья.";
            var result = await DisplayAlert("Вас выстрелили!", description, "Открыть карту роли", "Потерять очки");
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
                    Action = XPacketActions.LostHealthPoints,
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
            description += $"Всегда {Client.Person.Role.IsAlwaysWho}\n";
        description += $"{Client.Person.Role.Description}\n";
        if (Client.Person.Role.PermanentEffect is not null)
            description += $"Постоянный эффект: \n{Client.Person.Role.PermanentEffect}";
        DisplayAlert(Client.Person.Role.Name, description, "OK");
    }

    public async void RoleClicked(object sender, EventArgs e)
    {
        var image = (ImageButton)sender;
        var user = (UserInPage)image.BindingContext;

        if (Client.Person.IsMyMove || user.Role.IsAvailable)
        {
            if (IsRoleExchange)
            {
                if (RolesExchange.Count() == 2)
                    await DisplayAlert("Карта роли", "Вы больше не можете использовать карту замены ролей", "Назад");
                
                if (await DisplayAlert("Карта роли", "Поменять взять эту карту для замены ролей", "Да", "Нет"))
                {
                    RolesExchange.Add(user.ID);
                    return;
                }
            }

            if (!await DisplayAlert("Карта роли", "Вы можете посмотреть одну карту, но больше нельзя", "Смотреть", "Назад"))
                return;

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
        var image = (ImageButton)sender;
        var user = (UserInPage)image.BindingContext;

        if (Client.Person.IsMyMove || user.FirstCard.IsAvailable)
        {
            var description = $"{user.FirstCard.Name} X{user.FirstCard.Score}";
            DisplayAlert($"Карта верности игрока {user.Role.Name}", description, "OK");
        }
    }

    public async void SecondCardClicked(object sender, EventArgs e)
    {
        var image = (ImageButton)sender;
        var user = (UserInPage)image.BindingContext;

        if (Client.Person.IsMyMove || user.SecondCard.IsAvailable)
        {
            var description = $"{user.SecondCard.Name} X{user.SecondCard.Score}";
            DisplayAlert($"Карта верности игрока {user.Role.Name}", description, "OK");
        }
    }

    public async void GunClicked(object sender, EventArgs e)
    {
        var image = (ImageButton)sender;
        var gun = (Arsenal)image.BindingContext;
        var description = $"\n{gun.Description}\nВыстрел\n{gun.Shot}";

        bool result;
        if (Client.Person.IsMyMove && gun.IsAvailable)
            result = await DisplayAlert($"Оружие {gun.Name}", description, "Взять", "Не брать");
        else
        {
            DisplayAlert($"Оружие {gun.Name}", description, "Назад");
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
        if (Client.Person.IsMyMove)
        {
            var image = (Button)sender;
            var user = (UserInPage)image.BindingContext;

            if (Client.Person.Gun != null)
            {
                if (Client.Person.Gun.IsReadyShoot)
                {
                    if (await DisplayAlert
                        ("Действие",
                        $"Вы собираетесь стрелять в игрока {user.ItemName}",
                        "Стрелять",
                        "Назад"))
                    {
                        await Client.SendPacket(new XPacket()
                        {
                            Action = XPacketActions.Shot,
                            Type = XPacketTypes.Person,
                            Content = user.ID
                        });
                        Client.Person.Gun = null;
                    }
                }
                else
                {
                    if (await DisplayAlert
                       ("Действие",
                       $"Вы собираетесь направить оружие в игрока {user.ItemName}",
                       "Направить",
                       "Назад"))
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
                DisplayAlert("Действие", "Вы ничего не можете сделать", "OK");
        }
    }
}