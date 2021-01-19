using GBotXamarinForms.Controls.Sat;
using GBotXamarinForms.Controls.Sat.ServicesSat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GBotXamarinForms.Sat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Ferramentas : ContentPage
    {
        // ObservableCollection notifica a listview toda vez que inserirmos um novo item
        ObservableCollection<string> linhas_retorno = new ObservableCollection<string>();
        public Ferramentas()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            // binding da fonte de dados da listview
            listResponta.ItemsSource = linhas_retorno;

            MessagingCenter.Subscribe<Xamarin.Forms.Application, string>(Xamarin.Forms.Application.Current, "SAT_Response_Notification", (n_sender, resposta) =>
            {
                // Insere um novo registro no inicio da lista que notificara a listview para atualizar
                linhas_retorno.Insert(0, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + $":\n {resposta}");
            });

            codAtivacao.Text = GlobalValuesSat.codigoAtivacao.ToString();
        }

        private void btnDesbloquerSat(object sender, EventArgs e)
        {
            if (!SatUteis.VerificaCodigoAtivacao(codAtivacao.Text))
            {
                DependencyService.Get<ISatFerramentas>().mostrarDialogo("Código de Ativação deve ter entre 8 a 32 caracteres!");
            }

            DependencyService.Get<ISatFerramentas>().desbloquearSat(codAtivacao.Text.ToString(), SatUteis.GerarNumeroSessao);
        }

        private void btnBloquearSat(object sender, EventArgs e)
        {
            if (!SatUteis.VerificaCodigoAtivacao(codAtivacao.Text))
            {
                DependencyService.Get<ISatFerramentas>().mostrarDialogo("Código de Ativação deve ter entre 8 a 32 caracteres!");
            }
            DependencyService.Get<ISatFerramentas>().bloquearSat(codAtivacao.Text.ToString(), SatUteis.GerarNumeroSessao);
        }

        private void btnExtrairLog(object sender, EventArgs e)
        {
            if (!SatUteis.VerificaCodigoAtivacao(codAtivacao.Text))
            {
                DependencyService.Get<ISatFerramentas>().mostrarDialogo("Código de Ativação deve ter entre 8 a 32 caracteres!");
            }
            DependencyService.Get<ISatFerramentas>().logSat(codAtivacao.Text.ToString(), SatUteis.GerarNumeroSessao);
        }

        private void btnAtualizar(object sender, EventArgs e)
        {
            if (!SatUteis.VerificaCodigoAtivacao(codAtivacao.Text))
            {
                DependencyService.Get<ISatFerramentas>().mostrarDialogo("Código de Ativação deve ter entre 8 a 32 caracteres!");
            }
            DependencyService.Get<ISatFerramentas>().atualizarSat(codAtivacao.Text.ToString(), SatUteis.GerarNumeroSessao);
        }

        private void btnVersao(object sender, EventArgs e)
        {
            DependencyService.Get<ISatFerramentas>().versaoSat(codAtivacao.Text.ToString(), SatUteis.GerarNumeroSessao);
        }
    }
}