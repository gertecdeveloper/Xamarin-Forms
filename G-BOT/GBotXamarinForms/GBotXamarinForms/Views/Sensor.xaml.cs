using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GBotXamarinForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Sensor : ContentPage
    {
        // ObservableCollection notifica a listview toda vez que inserirmos um novo item
        ObservableCollection<string> linhas_retorno = new ObservableCollection<string>();
        public Sensor()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            // binding da fonte de dados da listview
            listResponta.ItemsSource = linhas_retorno;           
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            // subscribe no canal "Sensor_Notification" para recebermos as notificacoes do sensor quando uma pessoa for detectada 
            MessagingCenter.Subscribe<Xamarin.Forms.Application>(Xamarin.Forms.Application.Current, "Sensor_Notification",  (n_sender) =>
            {
                // Insere um novo registro no inicio da lista que notificara a listview para atualizar
                linhas_retorno.Insert(0, DateTime.Now.ToLongTimeString() + ": Pessoa identificada à frente do equipamento");
            });
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {   
            // parar de receber notificacoes
            MessagingCenter.Unsubscribe<Xamarin.Forms.Application>(Xamarin.Forms.Application.Current, "Sensor_Notification");
        }
    }
}