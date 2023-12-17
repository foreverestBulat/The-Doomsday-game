namespace GameClient;

public class MyDisplayAlert : ContentPage
{
	public MyDisplayAlert()
	{
        var layout = new StackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        Label titleLabel = new Label
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Start,
            TextColor = Colors.Coral,
            Text = "Title",
            FontSize = 20,
            FontAttributes = FontAttributes.Bold
        };

        Label descriptionLabel = new Label
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Start,
            TextColor = Colors.Red,
            Text = "Description"
        };

        // Добавьте кнопки
        var closeButton = new Button
        {
            Text = "Close" // Можете заменить на OK или что-то еще.
        };
        closeButton.Clicked += (sender, e) => { ClosePopup(); };

        layout.Children.Add(titleLabel);
        layout.Children.Add(descriptionLabel);
        layout.Children.Add(closeButton);

        this.Content = new Frame
        {
            BackgroundColor = Color.FromArgb("99000000"), // Полупрозрачный черный фон
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand,
            Content = layout
        };

        //Content = new VerticalStackLayout
        //{
        //	Children = {
        //		new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!"
        //		}
        //	}
        //};
    }

    private async void ClosePopup()
    {
        await Navigation.PopModalAsync(); // Закрывает модальное окно.
    }
}