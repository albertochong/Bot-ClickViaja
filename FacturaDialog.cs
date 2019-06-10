
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Text;

namespace ClickViajaBot.Dialogs
{
    [Serializable]
    public class FacturaDialog 
    {
        [Prompt("Ok 👍,preciso de saber o seu número de contribuinte.👇")]
        public string nif
        {
            get; set;
        }

      
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(nif);
            return builder.ToString();
        }
    }

    [Serializable]
    public class ContribuinteDialog
    {
        [Prompt("Introduza correctamente o número de contribuinte.Apenas são aceites números de 9 dígitos 👇")]
        public string nif
        {
            get; set;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(nif);
            return builder.ToString();
        }
    }
}