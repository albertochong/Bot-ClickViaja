using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClickViajaBot.Dialogs
{
    [Serializable]
    public class OrcamentoDialog
    {
        [Prompt("Em que nome ficará o orçamento?")]
        public string Nome
        {
            get; set;
        }

        [Prompt("Para que email devo enviar o orçamento?")]
        public string Email
        {
            get; set;
        }

        [Prompt("O seu telemovél?📱")]
        public string telemovel
        {
            get; set;
        }

        [Prompt("Qual é o destino? ")]
        public string Destino
        {
            get; set;
        }

        [Prompt("Ok 👍,Qual é a finalidade da sua viagem?🏊{||}")]
        public FinalidadeViagem? FinalidadeViagemRequired
        {
            get; set;
        }

        public enum FinalidadeViagem
        {
            [Prompt("Lua de Mel")]
            LuaDeMel,
            [Describe("Férias")]
            [Terms(".*Férias", "Férias")]
            Ferias,
            [Describe("Negócios")]
            [Terms(".*Negócios", "Negócios")]
            Negocios,
            [Describe("Trabalho")]
            [Terms(".*Trabalho", "Trabalho")]
            Trabalho,
            [Describe("Estudos")]
            [Terms(".*Estudos", "Estudos")]
            Estudos,
            [Describe("Outro")]
            [Terms(".*Outro", "Outro")]
            Outro
        }

        [Prompt("Ok 👍,Qual é o tipo de orçamento pretende?{||}")]
        public TipoOrcamento TipoOrcamentoRequired
        {
            get; set;
        }

        public enum TipoOrcamento
        {

            [Prompt("Apenas Hotel")]
            ApenasHotel,
            [Describe("Voo e Hotel")]
            [Terms(".*Voo e Hotel", "Voo e Hotel")]
            VooHotel,
            [Describe("Cruzeiro")]
            [Terms(".*Crueiro", "Cruzeiro")]
            Cruzeiro,
            [Describe("Pacote Turístico")]
            [Terms(".*Pacote Turístico", "Pacote Turístico")]
            PacoteTuristico,
            [Describe("Circuito")]
            [Terms(".*Circuito", "Circuito")]
            Circuito,
            [Describe("Apenas Voo")]
            [Terms(".*Apenas Voo", "Apenas Voo")]
            ApenasVoo,
            [Describe("Outro")]
            [Terms(".*Outro", "Outro")]
            Outro
        }

        [Prompt("Outras informaçoes:Data de partida e regresso?Quantas pessoas?Tem crianças?De que idade?Pretende Hotel,resort, hostel,apartamentos?Quer receber orçamentos até ao máximo de que valor?")]
        public string OutrasInf
        {
            get; set;
        }

        public static IForm<OrcamentoDialog> BuildPedidoOrcamentoForm()
        {
            return new FormBuilder<OrcamentoDialog>()
                //.Field("MeioPagamentoRequired")
                //.Field("ServiceRequired")
                .Message("Claro que sim, mas preciso de algumas respostas suas se possível!")
                .AddRemainingFields()
                .Build();
        }
    }
}