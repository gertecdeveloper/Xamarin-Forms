﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GBotXamarinForms.Controls;
using GBotXamarinForms.Droid.Sat;
using GBotXamarinForms.Droid.SAT;
using GBotXamarinForms.Droid.SAT.ServiceSat;

[assembly: Xamarin.Forms.Dependency(typeof(Ativacao))]
namespace GBotXamarinForms.Droid.SAT
{
    [Activity(Label = "Ativacao")]
    public class Ativacao : Activity, ISatAtivacao
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }
        public void ativacaoSat(string txtCodAtivacao, string txtCNPJContribuinte, string txtCodConfirmacao, int numSessao)
        {

            string resp = MainActivity.satFunctions.AtivarSat(txtCodAtivacao.ToString(),
                                                txtCNPJContribuinte.ToString(),
                                                numSessao);

            RetornoSat retornoSat = OperacaoSat.invocarOperacaoSat("AtivarSAT", resp);

            //* Está função [OperacaoSat.formataRetornoSat] recebe como parâmetro a operação realizada e um objeto do tipo RetornoSat
            //* Retorna uma String com os valores obtidos do retorno da Operação já formatados e prontos para serem exibidos na tela
            // Recomenda-se acessar a função e entender como ela funciona
            string retornoFormatado = OperacaoSat.formataRetornoSat(retornoSat);

            SatUtils.DialogoRetorno(MainActivity.mContext, retornoFormatado);
        }

        public void mostrarDialogo(string mensagem)
        {
            SatUtils.MostrarToast(MainActivity.mContext, mensagem);
            return;
        }

    }
}