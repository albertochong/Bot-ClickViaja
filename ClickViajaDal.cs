using System.Linq;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Linq   ;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System.Configuration;
using System.Threading.Tasks;
using ClickViajaBot.Model;
using System;

namespace ClickViajaBot.DAL
{
    public static class ClickViajaDal
    {
        private static readonly string uri         = ConfigurationManager.AppSettings["MongoDbUri"].ToString();
        private static MongoClient client = new MongoClient(uri);

        /// <summary>
        /// Insert documents new
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async static Task InsertDocument(BsonDocument message)
        {
            BsonDocument msg = message;
            var db           = client.GetDatabase("clickviajadb");
            var botMessage   = db.GetCollection<BsonDocument>("BotMessage");

            await botMessage.InsertOneAsync(msg);
            
        }

        /// <summary>
        /// Get invoice by nif or phoneNumber
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public async static Task<List<InvoiceClient>> GetInvoiceDocument(string document)
        {
            var db          = client.GetDatabase("clickviajadb");
            var collection  = db.GetCollection<InvoiceClient>("Invoice");
            //var filter     = Builders<InvoiceClient>.Filter.Eq("NIF", document);
            //filter         = filter | (Builders<InvoiceClient>.Filter.Eq(x => x.Telemovel, document));
            
            string email = await GetClienteByNif(document);


            var query = from p in collection.AsQueryable()
                        where p.NIF == document
                        select new InvoiceClient()
                        {
                            ClientName = p.ClientName,
                            Destino = p.Destino,
                            Invoice = p.Invoice,
                            Email = email
                        };

            //return await collection.Find(filter).ToListAsync();

            return query.ToList();

        }

        /// <summary>
        /// Get last Reservation by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async static Task<Reserva> GetReservation(string email)
        {
            var db = client.GetDatabase("clickviajadb");
            var collection = db.GetCollection<Reserva>("reserva");

            var query = (from e in collection.AsQueryable()
                         where e.Email_Principal == email && e.Ref_MB_Gerada == 0
                         //orderby e.DataPartida descending
                         select e).FirstOrDefault();

            return query;

        }

        /// <summary>
        /// Get Cliente by nif
        /// </summary>
        /// <param name="nif"></param>
        /// <returns></returns>
        public async static Task<string> GetClienteByNif(string nif)
        {
            var db         = client.GetDatabase("clickviajadb");
            var collection = db.GetCollection<Cliente>("Cliente");
            string email   = "";

            try
            {
                email = (from e in collection.AsQueryable()
                         where e.NIF == nif
                         select e.Email).FirstOrDefault().ToString();
            }
            catch
            {
            }

            return email;

        }


        public async static void UpdateReservationRef_MB_Gerada(ObjectId idReserva, string canal, string cliente, string email,
                                                                string idCliente, string entidade, string referencia, float valor)
        {
            var db         = client.GetDatabase("clickviajadb");
            var collection = db.GetCollection<Reserva>("reserva");
            var result     = await collection.FindOneAndUpdateAsync(
                                Builders<Reserva>.Filter.Eq("Id", idReserva),
                                Builders<Reserva>.Update.Set("Ref_MB_Gerada", 1)
                                );

            BsonDocument message = new BsonDocument {
                                                                        { "Message" , "Gerou uma referencia multibanco." },
                                                                        { "Data" , DateTime.Now },
                                                                        { "Assunto" , "Referencia_Multibanco" },
                                                                        { "sentimento" , "" },
                                                                        { "Canal" , canal },
                                                                        { "cliente" , cliente},
                                                                        { "email" , email},
                                                                        { "Idcliente" , idCliente},
                                                                        {"entidade" , entidade},
                                                                        {"referencia" , referencia},
                                                                        {"valor" , valor},
                                                                        //por nome da agncia
                                                                      };

            await InsertDocument(message);
        }
    }
}