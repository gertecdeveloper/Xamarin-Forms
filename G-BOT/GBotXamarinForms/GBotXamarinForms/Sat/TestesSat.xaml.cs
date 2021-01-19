using GBotXamarinForms.Controls.Sat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GBotXamarinForms.Sat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestesSat : ContentPage
    {
        private string arquivoXmlVendas = "arq_venda.xml"; // Armazena o nome do arquivo
        private string arquivoXmlCancelamento = "arq_cancelamento.xml"; // Armazena o nome do arquivo

        // ObservableCollection notifica a listview toda vez que inserirmos um novo item
        ObservableCollection<string> linhas_retorno = new ObservableCollection<string>();

        public TestesSat()
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

        private void btnContultarSat(object sender, EventArgs e)
        {
            DependencyService.Get<ISatTestes>().consultarSat();
        }

        private void btnStatusSat(object sender, EventArgs e)
        {
            DependencyService.Get<ISatTestes>().statusSat(codAtivacao.Text);
        }

        private void btnTesteSat(object sender, EventArgs e)
        {
            string dadosVenda = CarregarArquivo(arquivoXmlVendas);
            DependencyService.Get<ISatTestes>().testeSat(codAtivacao.Text, dadosVenda);
        }

        private void btnEnviarVenda(object sender, EventArgs e)
        {
            string dadosVenda = CarregarArquivo(arquivoXmlVendas);
            DependencyService.Get<ISatTestes>().enviarVenda(codAtivacao.Text, dadosVenda);
        }

        private void btnCancelarVenda(object sender, EventArgs e)
        {
            string dadosVenda = CarregarArquivo(arquivoXmlCancelamento);
            DependencyService.Get<ISatTestes>().cancelarVenda(codAtivacao.Text, dadosVenda);
        }

        private void btnConsultarSessao(object sender, EventArgs e)
        {

            DependencyService.Get<ISatTestes>().secaoSat(codAtivacao.Text);
        }

        private string CarregarArquivo(string arquivo)
        {
            var assembly = Assembly.GetExecutingAssembly();
            // O caminho da pasta onde vai ficar os arquivos, no caso deste exemplo, na pasta Files na raiz do projeto forms
            // namespace.pasta.nomedoarquivo
            // o arquivo a ser carregado precisa estar com a propriedade "Ação de compilação" como "Recurso Inserido"
            string resourceName = "GBotXamarinForms.Files." + arquivo;

            string texto = "";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                texto = reader.ReadToEnd();
            }

            return texto;
        }
    }
}