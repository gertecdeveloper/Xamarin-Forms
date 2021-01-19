using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GBotXamarinForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Fala : ContentPage
    {
        public Fala()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            // metodo do Xamarin.Essentials usado para text to speech
            await TextToSpeech.SpeakAsync("Sejam todos bem-vindos à aprensentação  do G-Bot");
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await TextToSpeech.SpeakAsync("Possui reconhecimento facial");
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
           await TextToSpeech.SpeakAsync("Microfone e leitor NFC");
        }

        private async void Button_Clicked_3(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(input_frase.Text))
            {
                await DisplayAlert("Atenção", "Você digitou uma frase vazia!", "OK");
                return;
            }
                
            await TextToSpeech.SpeakAsync(input_frase.Text);
        }
    }
}