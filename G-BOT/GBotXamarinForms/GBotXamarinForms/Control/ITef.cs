using Xamarin.Forms;

namespace GBotXamarinForms.Controls
{
    public interface ITef
    {
        void executeSTefVenda(string ip, int valor, string pagamento, int parcelas, string parcelamento, bool impressao);
             
        void executeSTefCancelamento(string ip, int valor, bool impressao);
        
        void executeSTefFuncoes(string ip, int valor, bool impressao);
        
        void execulteSTefReimpressao(string ip, int valor, bool impressao);
        bool validaIp(string ipserver);
    }
}