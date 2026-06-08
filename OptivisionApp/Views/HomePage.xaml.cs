using System;
using Microsoft.Maui.Controls;

namespace OptivisionApp.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var loginPage = Handler.MauiContext.Services.GetService<LoginPage>();
            loginPage.StartInRegisterMode = "false";
            await Navigation.PushAsync(loginPage);
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            var loginPage = Handler.MauiContext.Services.GetService<LoginPage>();
            loginPage.StartInRegisterMode = "true";
            await Navigation.PushAsync(loginPage);
        }
    }
}
