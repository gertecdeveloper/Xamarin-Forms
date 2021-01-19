using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Wangpos.Sdk4.Libbasebinder;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;

namespace GBotXamarinForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CodigoDeBarras : ContentPage
    {
        Scaner scaner;
        Boolean run_scaner;
        // ObservableCollection notifica a listview toda vez que inserirmos um novo item
        ObservableCollection<string> linhas_retorno = new ObservableCollection<string>();
        public CodigoDeBarras()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            // binding da fonte de dados da listview
            listResponta.ItemsSource = linhas_retorno;
            run_scaner = false;
            
        }

        public void NovaLeitura(String codigo)
        {
            // Insere um novo registro no inicio da lista que notificara a listview para atualizar
            linhas_retorno.Insert(0, codigo);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            run_scaner = true;
            Byte[] leitura = new Byte[2048];
            int[] tamanho = new int[1];

            Task.Run(async () =>
            {
                //inicia o scaner
                scaner = new Scaner(Android.App.Application.Context);

                while (run_scaner)
                {
                    // inicia o scan por um codigo de barras
                    int result = scaner.ScanSingle(leitura,tamanho);

                    if(result == 0)
                    {
                        //converte o array de bytes para string
                        string s = System.Text.Encoding.UTF8.GetString(leitura, 2, leitura.Length - 2);

                        // necessario para atualizar a thread principal
                        Device.BeginInvokeOnMainThread(() =>
                        {                            
                            NovaLeitura(s);
                        });
                    }
                    // reduz o processamento
                    Thread.Sleep(1000);
                }
            });
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            run_scaner = false;
        }

        private void Button_Clicked_2(object sender, EventArgs e)
        {
            LedUtil.SetRedLed();
        }

        private void Button_Clicked_3(object sender, EventArgs e)
        {
            LedUtil.SetOffLed();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            LedUtil.SetOffLed();
            run_scaner = false;
        }
    }
}