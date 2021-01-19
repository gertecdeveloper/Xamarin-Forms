using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GBotXamarinForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Kiosk : ContentPage
    {
        private string labelStatusKioskMode;
        public string LabelStatusKioskMode
        {
            get { return labelStatusKioskMode; }
            set
            {
                labelStatusKioskMode = value;
                OnPropertyChanged(nameof(LabelStatusKioskMode)); // Notifica quando houver mudança
            }
        }
        public Kiosk()
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            LabelStatusKioskMode = "Modo Kiosk Desativado";
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IKioskMode>().StartKioskMode();
            LabelStatusKioskMode = "Modo Kiosk Ativado";
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            DependencyService.Get<IKioskMode>().StopKioskMode();
            LabelStatusKioskMode = "Modo Kiosk Desativado";
        }
    }
}