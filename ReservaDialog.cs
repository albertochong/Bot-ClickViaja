using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ClickViajaBot.Dialogs
{
    [Serializable]
    public class ReservaDialog
    {
       
        [Prompt("Ok 👍,preciso de saber o seu email.👇")]
        public string email
        {
            get; set;
        }

        [Prompt("Ok 👍,Qual é o meio de pagamento que prentende?{||}")]
        public MeiosPagamentos MeioPagamentoRequired
        {
            get; set;
        }

        public enum MeiosPagamentos
        {
            [Prompt("Refencia Multibanco")]
            Entidade,
            [Prompt("MB Way")]
            MBWay,
            [Describe("Entidade e Referencia")]
            [Terms(".*Entidade e Referencia", "Entidade")]
            EntidadeReferencia
        }
        

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(email);
            return builder.ToString();
        }

        public static IForm<ReservaDialog> BuildPagarReservaForm()
        {
            return new FormBuilder<ReservaDialog>()
                .Field("MeioPagamentoRequired")
                //.Field("ServiceRequired")
                .AddRemainingFields()
                .Build();
        }

        private static IForm<ReservaDialog> BuilFormPagarReserva()
        {
            var builder = new FormBuilder<ReservaDialog>();
            return builder.AddRemainingFields().Build();


        }
    }
}