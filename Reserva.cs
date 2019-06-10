using System;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace ClickViajaBot.Model
{
    public class Reserva
    {
        public ObjectId Id { get; set; }
        //public string faceAccount { get; set; }
        public string Descricao
        {
            get; set;
        }
        public string Email_Principal
        {
            get; set;
        }
        public string Telemovel_Principal
        {
            get; set;
        }
        public int Ref_MB_Gerada
        {
            get; set;
        }
        //public DateTime DataReserva { get; set; }
        //public List<Passageiro> Passageiro { get; set; }
        //public Itenerario Itenerario { get; set; }
        //public string TipoReserva { get; set; }
        //public List<Hotel> Hotel { get; set; }
        //public string Id_Orcamento { get; set; }
        //public string Operador { get; set; }
        //public string Nº_ref_Operador { get; set; }
        //public float PrecoCompra { get; set; }
        public float PrecoVenda { get; set; }
        //public float Comissao { get; set; }
        //public DateTime DataPartida { get; set; }
        //public DateTime DataRegresso { get; set; }
        //public string Observacoes { get; set; }
        //public string ContactosUrgentes { get; set; }
        //public string Lembrete { get; set; }   
        //public int NumHorasAntesViagem { get; set; }
    }

    public class Passageiro
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public List<string>Telemovel { get; set; }
        public DateTime Dob { get; set; }
        public string BI { get; set; }
        public string Numero_Passaporte { get; set; }
        public string Nacionalidade { get; set; }
        public string Data_emissao_Passaporte { get; set; }
        public DateTime Data_validade_Passaporte { get; set; }
        public DateTime Data_validade_BI { get; set; }
        public string Sexo { get; set; }
        public double? Altura { get; set; }
        public bool LembreteEnviado { get; set; }
        public string Id_Cliente
        { get; set; }
        public string Bagagem
        {
            get; set;
        }

   
    }

    public class Itenerario
    {
        public string De { get; set; }
        public string Para { get; set; }
        public List<string> Cidade { get; set; }
        public string Tipo { get; set; }
        public string Nº_Voo_Ida { get; set; }
        public string Nº_Voo_Regresso { get; set; }
        public List<string> Escala { get; set; } 
    }

    public class Hotel
    {
        public string Nome { get; set; }
        public string Morada
        {
            get; set;
        }
        public string Categoria
        { get; set; }
        public string quarto { get; set; }
    }
    
}
