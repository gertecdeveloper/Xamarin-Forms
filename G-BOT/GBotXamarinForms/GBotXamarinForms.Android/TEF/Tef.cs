using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using System;
using GBotXamarinForms.Controls;
using Java.Util;
using System.Text;
using Java.Util.Regex;
using System.Collections.Generic;

[assembly: Xamarin.Forms.Dependency(typeof(GBotXamarinForms.Droid.TEF.Tef))]
namespace GBotXamarinForms.Droid.TEF
{
    [Activity(Label = "Tef")]
    public class Tef : Activity, ITef
    {               
        
        public Android.App.AlertDialog alerta;

        private Locale mLocale = new Locale("pt", "BR");

        ///  Defines mSitef
        private static int REQ_CODE_MSITEF = 4321;
        /// Fim Defines mSitef 

        /// Difines operação
        private static System.Random random = new System.Random();
        private string op = random.Next(99999).ToString();
        
        public Result RESULT_OK { get; private set; }

        //TEF
     
        public static string acao = "venda";

        public string DataAtual
        {
            get => DateTime.Now.ToString("dd/MM/yyyy");
        }

        public string HoraAtual
        {
            get => DateTime.Now.ToString("HHmmss");
        }


    protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);            
        }

        public void ResultadoTef(int requestCode, Result resultCode, Intent data)
        {
            
            if (requestCode == REQ_CODE_MSITEF)
            {
                if (requestCode == REQ_CODE_MSITEF && resultCode == Result.Ok)
                {
                    if (data.GetStringExtra("CODRESP").Equals("0"))
                    {
                        
                        string impressao = "";
                        // Verifica se tem algo pra imprimir
                        if (!string.IsNullOrEmpty(data.GetStringExtra("VIA_CLIENTE")))
                        {
                            impressao += data.GetStringExtra("VIA_CLIENTE");
                        }
                        if (!string.IsNullOrEmpty(data.GetStringExtra("VIA_ESTABELECIMENTO")))
                        {
                            impressao += "\n\n-----------------------------     \n";
                            impressao += data.GetStringExtra("VIA_ESTABELECIMENTO");
                        }
                        if (!string.IsNullOrEmpty(impressao))
                        {
                            dialogImpressaoGPOS(impressao, 17, "MSITEF");
                        }
                    }
                    // Verifica se ocorreu um erro durante venda ou cancelamento
                    if (acao.Equals("venda") || acao.Equals("cancelamento"))
                    {
                        if (string.IsNullOrEmpty(data.GetStringExtra("CODRESP")) || !data.GetStringExtra("CODRESP").Equals("0"))
                        {
                            dialodTransacaoNegadaMsitef(data);
                        }
                        else
                        {
                            dialodTransacaoAprovadaMsitef(data);
                        }
                    }
                }
                else
                {
                   if (resultCode == Result.Canceled && data == null)
                    {
                        Android.App.AlertDialog alertDialog = new Android.App.AlertDialog.Builder(MainActivity.mContext).Create();
                        alertDialog.SetTitle("Atenção");
                        alertDialog.SetMessage("Transação cancelada!");
                        alertDialog.SetButton("OK", delegate
                        {
                            alertDialog.Dismiss();
                        });
                        alertDialog.Show();
                    }
                    else
                    {
                        if (acao.Equals("venda") || acao.Equals("cancelamento"))
                        {
                            dialodTransacaoNegadaMsitef(data);
                        }
                    }
                    
                }
            }
            
        }
        
    

        public void execulteSTefReimpressao(string ip, int valor, bool impressao)
        {
            acao = "reimpressao";
            //REQ_CODE = 4321;
            Intent intentSitef = new Intent("br.com.softwareexpress.sitef.msitef.ACTIVITY_CLISITEF");

            intentSitef.PutExtra("empresaSitef", "00000000");
            intentSitef.PutExtra("enderecoSitef", ip);
            intentSitef.PutExtra("operador", "0001");
            intentSitef.PutExtra("numeroCupom", op);

            intentSitef.PutExtra("valor", valor.ToString().Replace(",", "").Replace(".", ""));
            intentSitef.PutExtra("CNPJ_CPF", "03654119000176");
            intentSitef.PutExtra("comExterna", "0");

            intentSitef.PutExtra("modalidade", "114");

            intentSitef.PutExtra("isDoubleValidation", "0");
            intentSitef.PutExtra("caminhoCertificadoCA", "ca_cert_perm");

            MainActivity.mContext.StartActivityForResult(intentSitef, REQ_CODE_MSITEF);
        }

    
        public void executeSTefCancelamento(string ip, int valor, bool impressao)
        {
            acao = "cancelamento";
            //REQ_CODE = 4321;
            Intent intentSitef = new Intent("br.com.softwareexpress.sitef.msitef.ACTIVITY_CLISITEF");

            intentSitef.PutExtra("empresaSitef", "00000000");
            intentSitef.PutExtra("enderecoSitef", ip);
            intentSitef.PutExtra("operador", "0001");
            intentSitef.PutExtra("data", DataAtual);
            intentSitef.PutExtra("hora", HoraAtual);
            intentSitef.PutExtra("numeroCupom", op);

            intentSitef.PutExtra("valor", valor.ToString().Replace(",", "").Replace(".", ""));
            intentSitef.PutExtra("CNPJ_CPF", "03654119000176");
            intentSitef.PutExtra("comExterna", "0");

            intentSitef.PutExtra("modalidade", "200");

            intentSitef.PutExtra("isDoubleValidation", "0");
            intentSitef.PutExtra("caminhoCertificadoCA", "ca_cert_perm");

            MainActivity.mContext.StartActivityForResult(intentSitef, REQ_CODE_MSITEF);
        }

        public void executeSTefFuncoes(string ip, int valor, bool impressao)
        {
            acao = "funcoes";
            //REQ_CODE = 4321;
            Intent intentSitef = new Intent("br.com.softwareexpress.sitef.msitef.ACTIVITY_CLISITEF");

            intentSitef.PutExtra("empresaSitef", "00000000");
            intentSitef.PutExtra("enderecoSitef", ip);
            intentSitef.PutExtra("operador", "0001");
            intentSitef.PutExtra("numeroCupom", op);

            intentSitef.PutExtra("valor", valor.ToString().Replace(",", "").Replace(".", ""));
            intentSitef.PutExtra("CNPJ_CPF", "03654119000176");
            intentSitef.PutExtra("comExterna", "0");

            intentSitef.PutExtra("isDoubleValidation", "0");
            intentSitef.PutExtra("caminhoCertificadoCA", "ca_cert_perm");

            intentSitef.PutExtra("modalidade", "110");
            intentSitef.PutExtra("restricoes", "transacoesHabilitadas=16;26;27");

            MainActivity.mContext.StartActivityForResult(intentSitef, REQ_CODE_MSITEF);


        }

        public void executeSTefVenda(string ip, int valor, string pagamento, int parcelas, string parcelamento, bool impressao)
        {
            acao = "venda";
            try
            {
                Intent intentSitef = new Intent("br.com.softwareexpress.sitef.msitef.ACTIVITY_CLISITEF");

                intentSitef.PutExtra("empresaSitef", "00000000");
                intentSitef.PutExtra("enderecoSitef", ip);
                intentSitef.PutExtra("operador", "0001");
                intentSitef.PutExtra("data", "20200324");
                intentSitef.PutExtra("hora", "130358");
                intentSitef.PutExtra("numeroCupom", op);

                intentSitef.PutExtra("valor", valor.ToString().Replace(",", "").Replace(".", ""));
                intentSitef.PutExtra("CNPJ_CPF", "03654119000176");
                intentSitef.PutExtra("comExterna", "0");

                if (pagamento.Equals("CREDITO"))
                {
                    intentSitef.PutExtra("modalidade", "3");
                    if (parcelas.ToString().Equals("0") || parcelas.ToString().Equals("1"))
                    {
                        intentSitef.PutExtra("transacoesHabilitadas", "26");
                    }
                    else if (parcelamento.Equals("Loja"))
                    {
                        // Essa informações habilida o parcelamento Loja
                        intentSitef.PutExtra("transacoesHabilitadas", "27");
                    }
                    else
                    {
                        // Essa informações habilida o parcelamento ADM
                        intentSitef.PutExtra("transacoesHabilitadas", "28");
                    }
                    intentSitef.PutExtra("numParcelas", parcelas.ToString());
                }else if (pagamento.Equals("DEBITO"))
                {
                    intentSitef.PutExtra("modalidade", "2");
                    intentSitef.PutExtra("transacoesHabilitadas", "16");
                }else if (pagamento.Equals("CARTEIRA_DIGITAL"))
                {
                    intentSitef.PutExtra("modalidade", "122");
                    intentSitef.PutExtra("transacoesHabilitadas", "7;8");
                }
                
                else 
                {
                    intentSitef.PutExtra("modalidade", "0");
                    intentSitef.PutExtra("restricoes", "transacoesHabilitadas=16");
                }

                intentSitef.PutExtra("isDoubleValidation", "0");
                intentSitef.PutExtra("caminhoCertificadoCA", "ca_cert_perm");


                MainActivity.mContext.StartActivityForResult(intentSitef, REQ_CODE_MSITEF);
            }
            catch (Exception e)
            {
                Toast.MakeText(MainActivity.mContext, e.Message, ToastLength.Long).Show();
            }
        }

        public void EnviarRespostaTEF(string texto)
        {
            Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "TEF_Response_Notification", texto);
        }

        public void dialogImpressaoGPOS(String texto, int size, string api)
        {
            EnviarRespostaTEF(texto);
        }

        public void dialodTransacaoNegadaMsitef(Intent data)
        {
            Android.App.AlertDialog alertDialog = new Android.App.AlertDialog.Builder(MainActivity.mContext).Create();

            StringBuilder cupom = new StringBuilder();

            cupom.Append("CODRESP: " + data.GetStringExtra("CODRESP") + "\n");

            EnviarRespostaTEF("Ocorreu um erro durante a realização da ação \n" + cupom.ToString());
        }

        public void dialodTransacaoAprovadaMsitef(Intent data)
        {
            Android.App.AlertDialog alertDialog = new Android.App.AlertDialog.Builder(MainActivity.mContext).Create();

            StringBuilder cupom = new StringBuilder();

            cupom.Append("CODRESP: " + data.GetStringExtra("CODRESP") + "\n");
            cupom.Append("COMP_DADOS_CONF: " + data.GetStringExtra("COMP_DADOS_CONF") + "\n");
            cupom.Append("CODTRANS: " + data.GetStringExtra("CODTRANS") + "\n");

            cupom.Append("CODTRANS: " + data.GetStringExtra("CODTRANS") + " " + Convert.ToString(retornaTipoParcelamento(Convert.ToInt32(data.GetStringExtra("TIPO_PARC")))) + "\n");

            cupom.Append("VLTROCO: " + data.GetStringExtra("VLTROCO") + "\n");
            cupom.Append("REDE_AUT: " + data.GetStringExtra("REDE_AUT") + "\n");
            cupom.Append("BANDEIRA: " + data.GetStringExtra("BANDEIRA") + "\n");

            cupom.Append("NSU_SITEF: " + data.GetStringExtra("NSU_SITEF") + "\n");
            cupom.Append("NSU_HOST: " + data.GetStringExtra("NSU_HOST") + "\n");
            cupom.Append("COD_AUTORIZACAO: " + data.GetStringExtra("COD_AUTORIZACAO") + "\n");
            cupom.Append("NUM_PARC: " + data.GetStringExtra("NUM_PARC") + "\n");

            alertDialog.SetTitle("Ação executada com sucesso");
            alertDialog.SetMessage(cupom.ToString());
            alertDialog.SetButton("OK", delegate
            {
                alertDialog.Dismiss();
            });
            alertDialog.Show();
        }

        private string retornaTipoParcelamento(int op)
        {
            string retorno = "Valor invalido";

            switch (op)
            {
                case 0:
                    retorno = "A vista";
                    break;
                case 1:
                    retorno = "Pré-Datado";
                    break;
                case 2:
                    retorno = "Parcelado Loja";
                    break;
                case 3:
                    retorno = "Parcelado Adm";
                    break;
            }
            return retorno;
        }

        public bool validaIp(string ipserver)
        {            
            Java.Util.Regex.Pattern p = Java.Util.Regex.Pattern.Compile("^([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\." +
                "([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\." +
                "([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\." +
                "([01]?\\d\\d?|2[0-4]\\d|25[0-5])$");
            Matcher m = p.Matcher(ipserver);
            bool b = m.Matches();
            Console.WriteLine(b);
            return b;
        }
                
    }
}