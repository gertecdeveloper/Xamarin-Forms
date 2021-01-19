using GBotXamarinForms.Controls;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GBotXamarinForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Tef : ContentPage
    {
        // ObservableCollection notifica a listview toda vez que inserirmos um novo item
        ObservableCollection<string> linhas_retorno = new ObservableCollection<string>();

        public Tef()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            valPag.Text = "1000";
            //ipEdit.Text = "192.168.0.5";
            qtdeParcelas.Text = "1";

            // binding da fonte de dados da listview
            listResponta.ItemsSource = linhas_retorno;

            MessagingCenter.Subscribe<Xamarin.Forms.Application,string>(Xamarin.Forms.Application.Current, "TEF_Response_Notification", (n_sender, resposta) =>
            {
                // Insere um novo registro no inicio da lista que notificara a listview para atualizar
                linhas_retorno.Insert(0, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + $":\n {resposta}");
            });

        }

        private void checkBoxCreditoChanged(object sender, CheckedChangedEventArgs e)
        {
            if (checkBoxCredito.IsChecked == true)
            {
                checkBoxDebito.IsChecked = false;
                checkBoxTodos.IsChecked = false;
                checkBoxCarteira.IsChecked = false;
            }
        }

        private void checkBoxDebitoChanged(object sender, CheckedChangedEventArgs e)
        {
            if (checkBoxTodos.IsChecked || checkBoxDebito.IsChecked)
            {
                qtdeParcelas.Text = "1";
                qtdeParcelas.IsEnabled = false;
            }
            else
            {
                qtdeParcelas.IsEnabled = true;
            }

            if (checkBoxDebito.IsChecked == true)
            {
                checkBoxTodos.IsChecked = false;
                checkBoxCredito.IsChecked = false;
                checkBoxCarteira.IsChecked = false;
            }
        }

        private void checkBoxTodosChanged(object sender, CheckedChangedEventArgs e)
        {
            if (checkBoxTodos.IsChecked || checkBoxDebito.IsChecked)
            {
                qtdeParcelas.Text = "1";
                qtdeParcelas.IsEnabled = false;
            }
            else
            {
                qtdeParcelas.IsEnabled = true;
            }

            if (checkBoxTodos.IsChecked == true)
            {
                checkBoxDebito.IsChecked = false;
                checkBoxCredito.IsChecked = false;
                checkBoxCarteira.IsChecked = false;
            }
        }

        private void checkParcLojaChanged(object sender, CheckedChangedEventArgs e)
        {
            if (checkParcLoja.IsChecked == true)
            {
                checkParcAdm.IsChecked = false;
            }
        }

        private void checkParcAdmChanged(object sender, CheckedChangedEventArgs e)
        {
            if (checkParcAdm.IsChecked == true)
            {
                checkParcLoja.IsChecked = false;
            }
        }

   

        private void btnEnviaTrasacao(object sender, EventArgs e)
        {
            
            string pagamento = "";
            string parcelamento = "Adm";

            if (checkBoxDebito.IsChecked)
            {
                pagamento = "DEBITO";

            }
            else if (checkBoxCredito.IsChecked)
            {
                pagamento = "CREDITO";
            }else if (checkBoxCarteira.IsChecked)
            {
                pagamento = "CARTEIRA_DIGITAL";
            }
            
            

            if (checkParcAdm.IsChecked)
            {
                parcelamento = "Adm";
            }else 
            {
                parcelamento = "Loja";
            }

            bool validaIp = DependencyService.Get<ITef>().validaIp(ipEdit.Text.ToString());            
            if (valPag.Text.ToString().Equals(0) || String.IsNullOrEmpty(valPag.Text.ToString()))
            {
                DisplayAlert("Erro ao executar função", "O valor de venda digitado deve ser maior que 0", "OK");
            }
            else if ( !validaIp)
            {
                DisplayAlert("Erro ao executar função", "Digite um IP válido", "OK");
            }
            else
            {
                if (checkBoxCredito.IsChecked && String.IsNullOrEmpty(qtdeParcelas.Text))
                {
                    DisplayAlert("Erro ao executar função", "É necessário colocar o número de parcelas desejadas (obs.: Opção de compra por crédito marcada)", "OK");
                }
                else
                {                                                            
                        
                        DependencyService.Get<ITef>().executeSTefVenda(ipEdit.Text, RemoveFormatacao(valPag.Text), pagamento, int.Parse(qtdeParcelas.Text), parcelamento, false);                    
                }                
            }
           
        }

        private void btnCancelaTransacao(object sender, EventArgs e)
        {
            
            bool validaIp = DependencyService.Get<ITef>().validaIp(ipEdit.Text.ToString());
            
            if (valPag.Text.ToString().Equals(0) || String.IsNullOrEmpty(valPag.Text.ToString()))
            {
                DisplayAlert("Erro ao executar função", "O valor de venda digitado deve ser maior que 0", "OK");
            }
            else if (!validaIp)
            {
                DisplayAlert("Erro ao executar função", "Digite um IP válido", "OK");
            }
            else
            {      
                DependencyService.Get<ITef>().executeSTefCancelamento(ipEdit.Text, RemoveFormatacao(valPag.Text), false);                
            }
           
        }

        private void btnFuncoes(object sender, EventArgs e)
        {
            
            bool validaIp = DependencyService.Get<ITef>().validaIp(ipEdit.Text.ToString());
            
            if (valPag.Text.ToString().Equals(0) || String.IsNullOrEmpty(valPag.Text.ToString()))
            {
                DisplayAlert("Erro ao executar função", "O valor de venda digitado deve ser maior que 0", "OK");
            }
            else if (!validaIp)
            {
                DisplayAlert("Erro ao executar função", "Digite um IP válido", "OK");
            }
            else
            {
                DependencyService.Get<ITef>().executeSTefFuncoes(ipEdit.Text, RemoveFormatacao(valPag.Text), false);
                
            }
            
        }

        private void btnReimpressao(object sender, EventArgs e)
        {
            
            bool validaIp = DependencyService.Get<ITef>().validaIp(ipEdit.Text.ToString());
            
            if (valPag.Text.ToString().Equals(0) || String.IsNullOrEmpty(valPag.Text.ToString()))
            {
                DisplayAlert("Erro ao executar função", "O valor de venda digitado deve ser maior que 0", "OK");
            }else if ( !validaIp )
            {
                DisplayAlert("Erro ao executar função", "Digite um IP válido", "OK");
            }
            else
            {
                                      
                DependencyService.Get<ITef>().execulteSTefReimpressao(ipEdit.Text, RemoveFormatacao(valPag.Text), false);
                
            }
            
        }
        
        private int RemoveFormatacao(string valor)
        {
            return int.Parse(valor.Replace("R$ ", "").Replace(",", "").Replace(".", ""));
        }

        private void checkBoxCarteira_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (checkBoxCarteira.IsChecked == true)
            {
                checkBoxDebito.IsChecked = false;
                checkBoxTodos.IsChecked = false;
                checkBoxCredito.IsChecked = false;
            }
        }
    }
}