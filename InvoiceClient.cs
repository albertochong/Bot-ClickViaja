using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClickViajaBot.Model
{
    public class InvoiceClient
    {
        [BsonId]
        public ObjectId _id
        {
            get; set;
        }

        public string ClientName
        {
            get; set;
        }

        public string Destino
        {
            get; set;
        }

        public string NIF
        {
            get; set;
        }

        public string Telemovel
        {
            get; set;
        }

        public string Email
        {
            get; set;
        }

        public byte[] Invoice
        {
            get; set;
        }

        public Cliente Cliente
        {
            get; set;
        }
    }
}