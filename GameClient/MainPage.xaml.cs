﻿using Protocol.Models;
using System.Net;

namespace GameClient
{
    public partial class MainPage : ContentPage
    {
        IPEndPoint IPEndPoint = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 8888);
        public XClient Client { get; set; }

        public MainPage()
        {
            InitializeComponent();
        }

       
        public async void SignInClicked(object sender, EventArgs e)
        {
            Client = new XClient(IPEndPoint);
            try
            {
                Client.Connect();
                var rand = new Random();
                var person = new Player()
                {
                    Name = EntryName.Text,
                    Color = System.Drawing.Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256)),
                    IsReady = false,
                    IsMyMove = false,
                    HealthPoints = 2,
                    IsWatchCard = false,
                    MyPrograms = new List<Protocol.Models.Program>(),
                    GunsIsPointedMe = new List<Arsenal>()
                };
                Client.CurrentPage = new GamePage(Client);
                await Client.SignIn(person);
                await Navigation.PushAsync(Client.CurrentPage);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
                Client.Close();
            }
        }

        private void MainPage_Disappearing(object sender, EventArgs e)
        {
            if (Client is not null)
                Client.Close();
        }

        private async void ShowMyPopup(object sender, EventArgs e)
        {
            var popupPage = new MyDisplayAlert();
            await Navigation.PushModalAsync(popupPage);
        }
    }
}
