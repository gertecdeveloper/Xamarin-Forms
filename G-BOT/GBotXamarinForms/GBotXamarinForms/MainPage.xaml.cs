using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using GBotXamarinForms.Views;

namespace GBotXamarinForms
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }


        private void ViewCell_Tapped(object sender, EventArgs e)
        {
            // DisplayAlert("Alert", "You have been alerted", "OK");
            Navigation.PushAsync(new CodigoDeBarras());
        }

        private void ViewCell_Tapped_1(object sender, EventArgs e)
        {
            Navigation.PushAsync(new NFC());
        }

        private void ViewCell_Tapped_2(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Sensor());
        }

        private void ViewCell_Tapped_3(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Fala());
        }

        private void ViewCell_Tapped_4(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Kiosk());
        }

        private void ViewCell_Tapped_5(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Tef());
        }

        private void ViewCell_Tapped_6(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MenuSat());
        }
    }
}
