using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Runtime;
using GertecXamarinForms.Droid;

using Plugin.DeviceInfo;


using System;
using GertecXamarinForms.Controls;
using Xamarin.Forms;
using GertecXamarinForms.Droid.Impressao;
using GertecXamarinForms.Droid.Services;
using System.Linq;
using System.IO;

[assembly: Android.App.UsesPermission(Android.Manifest.Permission.Flashlight)]
[assembly: Xamarin.Forms.Dependency(typeof(MainActivity))]
namespace GertecXamarinForms.Droid
{
    [Activity(Label = "GertecXamarinForms", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : 
        global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity,
        IImpressao, INfc
    {
        public static ConfigPrint configPrint;
        public static GertecPrinter printer;
        public static string modelo;
        private Activity mainActivity;
        public Context context;

        public MainActivity()
        {
            this.mainActivity = this;
            this.context = Android.App.Application.Context;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            modelo = CrossDeviceInfo.Current.Model;
            printer = new GertecPrinter(mainActivity);
            configPrint = new ConfigPrint();
            printer.setConfigImpressao(configPrint);

            // ZXing Inicialização
            global::ZXing.Net.Mobile.Forms.Android.Platform.Init();

            //LoadApplication is a Xamarin.Forms method
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public string iniLoadModelo()
        {
            if (modelo.Equals("Smart G800"))
            {
                return "Funções Impressão TSG800";
            }
            else
            {
                return "Funções Impressão GPOS700";
            }
        }

        public void BtnStatusImpressora_Click()
        {
            string statusImpressora;

            try
            {
                statusImpressora = printer.getStatusImpressora();
                Toast.MakeText(context, statusImpressora, ToastLength.Long).Show();
            }
            catch (Exception e)
            {
                Toast.MakeText(context, e.Message, ToastLength.Long).Show();
            }
        }

        public void BtnImprimirMensagem_Click(string mensagem, string alinhamento, bool btnNegrito, bool btnItalico, bool btnSublinhado, string fonte, int tamanho)
        {
            if (String.IsNullOrWhiteSpace(mensagem))
            {
                Toast.MakeText(context, "Escreva uma mensagem", ToastLength.Long).Show();
            } else if (String.IsNullOrWhiteSpace(alinhamento))
            {
                Toast.MakeText(context, "Selecione um alinhamento", ToastLength.Long).Show();
            }
            else {
                try
                {
                    configPrint.Alinhamento = alinhamento;
                    configPrint.Negrito = btnNegrito;
                    configPrint.Italico = btnItalico;
                    configPrint.SubLinhado = btnSublinhado;
                    configPrint.Fonte = fonte;
                    configPrint.Tamanho = tamanho;

                    printer.setConfigImpressao(configPrint);
                    printer.ImprimeTexto(mensagem);
                    printer.AvancaLinha(100);
                    printer.ImpressoraOutput();
                }
                catch (IOException e)
                {
                    Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                }
            }
        }

        public void BtnImprimirImagem_Click()
        {
            configPrint.IWidth = 430;
            configPrint.IHeight = 700;
            printer.setConfigImpressao(configPrint);
            printer.ImprimeImagem("invoice");
            printer.AvancaLinha(100);
            printer.ImpressoraOutput();
        }

        public void BtnImprimirBarCode_Click(string mensagem, int height, int width, string tipoCode)
        {
            if (String.IsNullOrWhiteSpace(mensagem))
            {
                Toast.MakeText(context, "Escreva uma mensagem", ToastLength.Long).Show();
            }
            else if (tipoCode.Equals("EAN_8") || tipoCode.Equals("EAN_13"))
            {
                if(mensagem.All(char.IsDigit))
                {
                    if ((tipoCode.Equals("EAN_8") && mensagem.Length == 7) || (tipoCode.Equals("EAN_13") && mensagem.Length == 12))
                    {
                        printer.ImprimeBarCode(
                        mensagem,
                        height,
                        width,
                        tipoCode);
                        printer.AvancaLinha(100);
                        printer.ImpressoraOutput();
                    }
                    else
                    {
                        Toast.MakeText(context, "", ToastLength.Long).Show();
                    }
                }
                else
                {
                    Toast.MakeText(context, "", ToastLength.Long).Show();
                }
            }
            else
            {
                printer.ImprimeBarCode(
                    mensagem,
                    height,
                    width,
                    tipoCode);
                printer.AvancaLinha(100);
                printer.ImpressoraOutput();
            }
        }

        public void BtnImprimirTodasFunc_Click()
        {
            configPrint.Italico = false;
            configPrint.Negrito = true;
            configPrint.Tamanho = 20;
            configPrint.Fonte = "MONOSPACE";
            printer.setConfigImpressao(configPrint);

            try
            {
                printer.getStatusImpressora();
                // Imprimindo imagem
                configPrint.IWidth = 300;
                configPrint.IHeight = 130;
                configPrint.Alinhamento = "CENTER";
                printer.setConfigImpressao(configPrint);
                printer.ImprimeTexto("==[Iniciando Impressao Imagem]==");
                printer.ImprimeImagem("gertec_2");
                printer.AvancaLinha(10);
                printer.ImprimeTexto("====[Fim Impressão Imagem]====");
                printer.AvancaLinha(10);
                // Fim Imagem

                // Impressão Centralizada
                configPrint.Alinhamento = "CENTER";
                configPrint.Tamanho = 30;
                printer.setConfigImpressao(configPrint);
                printer.ImprimeTexto("CENTRALIZADO");
                printer.AvancaLinha(10);
                // Fim Impressão Centralizada

                // Impressão Esquerda
                configPrint.Alinhamento = "LEFT";
                configPrint.Tamanho = 40;
                printer.setConfigImpressao(configPrint);
                printer.ImprimeTexto("ESQUERDA");
                printer.AvancaLinha(10);
                // Fim Impressão Esquerda

                // Impressão Direita
                configPrint.Alinhamento = "RIGHT";
                configPrint.Tamanho = 20;
                printer.setConfigImpressao(configPrint);
                printer.ImprimeTexto("DIREITA");
                printer.AvancaLinha(10);
                // Fim Impressão Direita

                // Impressão Negrito
                configPrint.Negrito = true;
                configPrint.Alinhamento = "LEFT";
                configPrint.Tamanho = 20;
                printer.setConfigImpressao(configPrint);
                printer.ImprimeTexto("=======[Escrita Netrigo]=======");
                printer.AvancaLinha(10);
                // Fim Impressão Negrito

                // Impressão Italico
                configPrint.Negrito = false;
                configPrint.Italico = true;
                configPrint.Alinhamento = "LEFT";
                configPrint.Tamanho = 20;
                printer.setConfigImpressao(configPrint);
                printer.ImprimeTexto("=======[Escrita Italico]=======");
                printer.AvancaLinha(10);
                // Fim Impressão Italico

                // Impressão Italico
                configPrint.Negrito = false;
                configPrint.Italico = false;
                configPrint.SubLinhado = true;
                configPrint.Alinhamento = "LEFT";
                configPrint.Tamanho = 20;
                printer.setConfigImpressao(configPrint);
                printer.ImprimeTexto("======[Escrita Sublinhado]=====");
                printer.AvancaLinha(10);
                // Fim Impressão Italico

                // Impressão BarCode 128
                configPrint.Negrito = false;
                configPrint.Italico = false;
                configPrint.SubLinhado = false;
                configPrint.Alinhamento = "CENTER";
                configPrint.Tamanho = 20;
                printer.setConfigImpressao(configPrint);
                printer.ImprimeTexto("====[Codigo Barras CODE 128]====");
                printer.ImprimeBarCode("12345678901234567890", 120, 120, "CODE_128");
                printer.AvancaLinha(10);
                // Fim Impressão BarCode 128

                // Impressão Normal
                configPrint.Negrito = false;
                configPrint.Italico = false;
                configPrint.SubLinhado = true;
                configPrint.Alinhamento = "LEFT";
                configPrint.Tamanho = 20;
                printer.setConfigImpressao(configPrint);
                printer.ImprimeTexto("=======[Escrita Normal]=======");
                printer.AvancaLinha(10);
                // Fim Impressão Normal

                // Impressão Normal
                configPrint.Negrito = false;
                configPrint.Italico = false;
                configPrint.SubLinhado = true;
                configPrint.Alinhamento = "LEFT";
                configPrint.Tamanho = 20;
                printer.setConfigImpressao(configPrint);
                printer.ImprimeTexto("=========[BlankLine 50]=========");
                printer.AvancaLinha(50);
                printer.ImprimeTexto("=======[Fim BlankLine 50]=======");
                printer.AvancaLinha(10);
                // Fim Impressão Normal

                // Impressão BarCode 13
                configPrint.Negrito = false;
                configPrint.Italico = false;
                configPrint.SubLinhado = false;
                configPrint.Alinhamento = "CENTER";
                configPrint.Tamanho = 20;
                printer.setConfigImpressao(configPrint);
                printer.ImprimeTexto("=====[Codigo Barras EAN13]=====");
                printer.ImprimeBarCode("7891234567895", 120, 120, "EAN_13");
                printer.AvancaLinha(10);
                // Fim Impressão BarCode 128

                // Impressão BarCode 13
                printer.setConfigImpressao(configPrint);
                printer.ImprimeTexto("===[Codigo QrCode Gertec LIB]==");
                printer.AvancaLinha(10);
                printer.ImprimeBarCode("Gertec Developer Partner LIB", 240, 240, "QR_CODE");

                configPrint.Negrito = false;
                configPrint.Italico = false;
                configPrint.SubLinhado = false;
                configPrint.Alinhamento = "CENTER";
                configPrint.Tamanho = 20;
                printer.ImprimeTexto("===[Codigo QrCode Gertec IMG]==");
                printer.ImprimeBarCodeIMG("Gertec Developer Partner IMG", 240, 240, "QR_CODE");

                printer.AvancaLinha(100);
                // Fim Imagem
            }
            catch (IOException e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Long).Show();
            }
            finally
            {
                try
                {
                    printer.ImpressoraOutput();
                }
                catch (IOException e)
                {
                    Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                }
            }
        }

        public void Nfc()
        {
            Intent myIntent = new Intent(this.context, typeof(Nfc));
            myIntent.AddFlags(ActivityFlags.NewTask);
            this.context.StartActivity(myIntent);
        }
    }
}
