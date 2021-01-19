using System;
using System.Collections.Generic;
using System.Text;

namespace GBotXamarinForms.Controls.Sat
{
    public interface ISatAssociar
    {
        void mostrarDialogo(string mensagem);
        void associarSat(string cnpjContribuinte, string cnpjSH, string codAtivacao, string assinatura, int numeroSessao);
    }
}
