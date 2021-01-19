using System;
using System.Collections.Generic;
using System.Text;

namespace GBotXamarinForms.Sat
{
    public class UteisSat
    {
        public static string SomenteNumeros(string texto)
        {
            return texto.Replace(".", "").Replace("/", "").Replace("-", "");
        }
    }
}
