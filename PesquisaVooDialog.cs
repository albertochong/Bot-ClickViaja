using ClickViajaBot.Services;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ClickViajaBot.Dialogs
{
    [Serializable]
    public class PesquisaVooDialog
    {
        //private string _OriginCity;

        [Prompt("De que cidade vai iniciar a sua viagem?")]
        public string OriginCity
        {
            get;
            set;
        }

        [Prompt("Para que cidade deseja viajar?")]
        public string DestinationCity
        {
            get; set;
        }

        [Prompt("Ok 👍,Deseja com escala ou directo?{||}")]
        public TipoDireccao Direccao
        {
            get; set;
        }

        public enum TipoDireccao
        {
            [Prompt("Refencia Multibanco")]
            Entidade,
            [Describe("Directo")]
            [Terms(".*Directo", "Directo")]
            Directo,
            [Describe("Com Escala")]
            [Terms(".*Com escala", "Com Escala")]
            Escala
        }

        [Prompt("Qual é a data de Partida?")]
        public string DepartDate
        {
            get; set;
        }

        [Prompt("Qual é a data de regresso?")]
        [Optional]
        public string ReturnDate
        {
            get; set;
        }

        [Prompt("Quantos adultos?{||}")]
        public Adultos NumeroAdultosRequired
        {
            get; set;
        }

        public enum Adultos
        {
           Um     = 1,
           Dois   = 2,
           Tres   = 3,
           Quatro = 4,
           Cinco  = 5,
           Seis   = 6,
           Sete   = 7,
           Oito   = 8,
           Nove   = 9
        }

        [Prompt("Tem bebés?Quantos?Sáo considerados bebés até aos 2 anos.{||}")]
        public Bebe NumeroBebeRequired
        {
            get; set;
        }

        public enum Bebe
        {
            Um = 1,
            Dois = 2,
            Tres = 3,
            Quatro = 4,
            Cinco = 5,
            Seis = 6,
            Sete = 7,
            Oito = 8,
            Nove = 9
        }

        [Prompt("Tem crianças?Quantas?Sâo consideradas crianças de 2-12 anos.{||}")]
        public Crianca NumeroCriancaRequired
        {
            get; set;
        }

        public enum Crianca
        {
            Um = 1,
            Dois = 2,
            Tres = 3,
            Quatro = 4,
            Cinco = 5,
            Seis = 6,
            Sete = 7,
            Oito = 8,
            Nove = 9,
            Dez = 10
        }

        [Prompt("Tem idosos?Quantos?Sâo considerados idosos apartir 65 anos.{||}")]
        public Crianca NumeroIdosoRequired
        {
            get; set;
        }

        public enum Idoso
        {
            Um = 1,
            Dois = 2,
            Tres = 3,
            Quatro = 4,
            Cinco = 5,
            Seis = 6,
            Sete = 7,
            Oito = 8,
            Nove = 9,
            Dez = 10
        }

        //travelClass ecomony, premium_economy, business, first
        //nonStop - sim ou nao

        public static IForm<PesquisaVooDialog> BuildPesquisaVooForm()
        {
            return new FormBuilder<PesquisaVooDialog>()
                //.Field("MeioPagamentoRequired")
                //.Field("ServiceRequired")
                .Message("Ok para que eu possa entender melhor a sua necessidade vamos ver alguns pontos do seu pedido!")
                .AddRemainingFields()
                .Build();
        }

        public static IForm<PesquisaVooDialog> BuildDataVooBaratoForm()
        {
            return new FormBuilder<PesquisaVooDialog>()
                .Message("Diga me por favor o seguinte:")
                .Field("OriginCity")
                .Field("DestinationCity")
                .Field("Direccao")
                .Build();
        }
    }

}
