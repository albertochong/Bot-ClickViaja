using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClickViajaBot.Model
{
    public class Cliente
    {
        public ObjectId Id { get; set; }

        public string faceAccount { get; set; }

        public string NIF
        {
            get; set;
        }

        public string Nome
        {
            get; set;
        }

        public string Email
        {
            get; set;
        }

        public string Telemovel { get; set; }

        public DateTime Dob { get; set; }

        public string BI { get; set; }

        public string Numero_Passaporte { get; set; }

        public string Nacionalidade { get; set; }

        public string Data_emissao_Passaporte { get; set; }

        public DateTime Data_validade_Passaporte { get; set; }

        public string Sexo { get; set; }

        public float Altura { get; set; }
    }
}