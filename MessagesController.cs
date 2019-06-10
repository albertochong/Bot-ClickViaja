using System;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Web.Http.Description;
using System.Net.Http;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using ClickViajaBot.DAL;
using ClickViajaBot.Util;
using ClickViajaBot.Services;
using ClickViajaBot.Model;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace ClickViajaBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            try
            {
               
                // check if activity is of type message
                if (activity.GetActivityType() == ActivityTypes.Message)
                {

                    if ((activity.ChannelId == "facebook" && activity.Text == "comecar") ||
                        (activity.ChannelId == "webchat" && activity.Text == "") ||
                        (activity.ChannelId == "skype" && activity.Text == ""))
                        Gretting(connector, activity);
                    else if (activity.Text != "Chamar assistente")
                    {
                        Activity typing = activity.CreateReply();
                        typing.Type = ActivityTypes.Typing;
                        await connector.Conversations.ReplyToActivityAsync(typing);

                        var audioAttachment = activity.Attachments?.FirstOrDefault(a => a.ContentType.Equals("audio/wav") || a.ContentType.Equals("application/octet-stream"));

                        if (audioAttachment != null)
                        {

                            var stream = new Audio().GetAudioStream(connector, audioAttachment).Result;
                            activity.Text = new SpeechService().GetTextFromAudioAsync(stream).Result;
                            SendMessage(activity);

                        }
                        else
                            await Conversation.SendAsync(activity, () => new LuisDialog());
                    }
                    else
                        await SendMessageToAssistenteAsync(activity);
              
                }
                else if (activity.GetActivityType() == ActivityTypes.ConversationUpdate)
                    Gretting(connector, activity);
                else
                    await HandleSystemMessageAsync(activity, connector);
            }
            catch (Exception ex)
            {
                BsonDocument message = new BsonDocument {
                                                                    { "Message" , ex.Message },
                                                                    { "Data" , DateTime.Now },
                                                                    { "Assunto" , "Erro" },
                                                                    { "sentimento" , "" },
                                                                    { "Canal" , activity.ChannelId },
                                                                    { "cliente" , activity.From.Name},
                                                                    { "Idcliente" , activity.From.Id},
                                                                    { "Agência" , "ClicViaja - Estrada Luz"},
                                                                  };

                Util.Util.SendEmail(ex.Message, "Ocorreu uma excepção no bot", "", null);
                await ClickViajaDal.InsertDocument(message);

                Activity msg = activity.CreateReply("Ocorreu um erro,de momento não é possivel contactar me");
                msg.Type = ActivityTypes.Message;
                await connector.Conversations.ReplyToActivityAsync(msg);
            }
            //finally
            //{

            //    //Activity msg = activity.CreateReply("Ocorreu um erro,de momento não é possivel contactar me");
            //    //msg.Type = ActivityTypes.Message;
            //    //await connector.Conversations.ReplyToActivityAsync(msg);
            //}

            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private async void Gretting(ConnectorClient connector, Activity activity)
        {
            #region antigo
            //if (activity.ChannelId == "webchat" || activity.ChannelId == "skype")
            //{
            //IConversationUpdateActivity update = activity;
            // if (update.MembersAdded != null && update.MembersAdded.Any())
            // {
            //foreach (var newMember in update.MembersAdded)
            //{
            //if (newMember.Id != activity.Recipient.Id)
            //{
            //var reply = activity.CreateReply();
            //reply.Recipient = activity.From;
            //reply.Type = "message";
            //reply.Attachments = new List<Attachment>();

            //List<CardImage> cardImages = new List<CardImage>
            //                    {
            //                        new CardImage(url: "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSkg4OLKNa_d2H6G8LhiGbVRj5p26wn9WUdREu6StTb_kj-pych")
            //                    };

            //List<CardAction> cardButtons = new List<CardAction>();

            //CardAction plButton = new CardAction()
            //{
            //    Value = "botao_iniciar_click",
            //    Type = "postBack",
            //    Title = "Vamos começar a falar?Clique aqui.",
            //};

            //cardButtons.Add(plButton);

            //HeroCard plCard = new HeroCard()
            //{
            //   Title = "Olá eu sou o Click!",
            //   Subtitle = "Assistente Virtual da Agência ClickViaja Estrada da Luz",
            //  Images = cardImages,
            //  Text = "Bem vindo a nossa plataforma digital de suporte ao cliente.Estamos aqui para o ajudar a obter informaçao " +
            //          "sobre os nossos produtos e serviços ,poderá também consultar voo, hotéis, aluguer de carros, consultar suas reservas, orçamentos, e muito mais.",
            //Buttons = cardButtons
            //};

            //Attachment plAttachment = plCard.ToAttachment();
            //reply.Attachments.Add(plAttachment);

            ////if (update.From != null)
            //{
            //    reply.Text = "Olá " + activity.From.Name;
            //    await connector.Conversations.ReplyToActivityAsync(reply);
            //}
            //}
            //  }
            //  }
            //}
            //else
            //{
            #endregion

            var reply = activity.CreateReply();
                reply.Recipient = activity.From;
                reply.Type = "message";
                reply.Attachments = new List<Attachment>();

                List<CardImage> cardImages = new List<CardImage>
                                    {
                                        new CardImage(url: "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSkg4OLKNa_d2H6G8LhiGbVRj5p26wn9WUdREu6StTb_kj-pych")
                                    };

                List<CardAction> cardButtons = new List<CardAction>();

                CardAction plButton = new CardAction()
                {
                    Value = "botao_iniciar_click",
                    Type = "postBack",
                    Title = "Clique aqui",
                };

                cardButtons.Add(plButton);

            HeroCard plCard = new HeroCard()
            {
                Title = "Olá eu sou o Click!",
                Subtitle = "Assistente Virtual da Agência ClickViaja Estrada da Luz",
                Images = cardImages,
                Text = "Bem vindo a nossa plataforma digital de suporte ao cliente.",
                    //.Estamos aqui para o ajudar a obter informaçao " +
                      //         "sobre os nossos produtos e serviços ,poderá também consultar voo, hotéis, aluguer de carros, consultar suas reservas, orçamentos, e muito mais.",
                Buttons = cardButtons
            };

                Attachment plAttachment = plCard.ToAttachment();
                reply.Attachments.Add(plAttachment);

               
                reply.Text = "Olá " + activity.From.Name;
                await connector.Conversations.ReplyToActivityAsync(reply);
               
            //}
        }

        private async void SendMessage(Activity activity)
        {
            //await Task.Run(() =>
            // {
            await Conversation.SendAsync(activity, () => new LuisDialog());
             //});
        }

        private async Task<Activity> HandleSystemMessageAsync(Activity message, ConnectorClient connector)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                if (message.ChannelId == "skype")
                {

                    var reply = message.CreateReply();
                    reply.Recipient = message.From;
                    reply.Type = "message";
                    reply.Attachments = new List<Attachment>();

                    List<CardImage> cardImages = new List<CardImage>
                                    {
                                        new CardImage(url: "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSkg4OLKNa_d2H6G8LhiGbVRj5p26wn9WUdREu6StTb_kj-pych")
                                    };

                    List<CardAction> cardButtons = new List<CardAction>();

                    CardAction plButton = new CardAction()
                    {
                        Value = "botao_iniciar_click",
                        Type = "postBack",
                        Title = "Clique aqui",
                    };

                    cardButtons.Add(plButton);

                    HeroCard plCard = new HeroCard()
                    {
                        Title = "Olá eu sou o Click!",
                        Subtitle = "Assistente Virtual da Agência ClickViaja Estrada da Luz",
                        Images = cardImages,
                        Text = "Bem vindo a nossa plataforma digital de suporte ao cliente.Estamos aqui para o ajudar a obter informaçao " +
                                   "sobre os nossos produtos e serviços ,poderá também consultar voo, hotéis, aluguer de carros, consultar suas reservas, orçamentos, e muito mais.",
                        Buttons = cardButtons
                    };

                    Attachment plAttachment = plCard.ToAttachment();
                    reply.Attachments.Add(plAttachment);


                    reply.Text = "Olá " + message.From.Name;
                    await connector.Conversations.ReplyToActivityAsync(reply);




                }
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        private async Task SendMessageToAssistenteAsync(Activity activity)
        {
            ChannelAccount assistantContact = null;
            ChannelAccount contaBot = new ChannelAccount(activity.From.Id);

            if (activity.ChannelId == "facebook")
            {
                assistantContact = new ChannelAccount("2088567844548008", "Alberto Chong");
            }
            else if (activity.ChannelId == "skype")
            {
                assistantContact = new ChannelAccount("Beto.Chong");
            }

            var connector      = new ConnectorClient(new Uri(activity.ServiceUrl));
            var conversationId = await connector.Conversations.CreateDirectConversationAsync(contaBot, assistantContact);

            //IMessageActivity mensagem = Activity.CreateMessageActivity();
            //mensagem.From = botAccount;
            //mensagem.Recipient = contaUsuario;

            //mensagem.Conversation = new ConversationAccount(id: conversationId.Id);
            //mensagem.Text = "Olá";
            //mensagem.Locale = "pt-BR";

            //await connector.Conversations.SendToConversation((Activity)mensagem);
        }

        //private async Task<HttpResponseMessage> SendFBGreeting()
        //{
        //    var payload = @"{
        //    ""setting_type"": ""greeting"",
        //    ""greeting"": {
        //        ""text"": ""Timeless apparel for the masses.""
        //    }
        //}";
        //    var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(payload));
        //    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
        //    using (var client = new HttpClient())
        //    {
        //        // Do the actual request and await the response
        //        var httpResponse = await client.PostAsync("https://graph.facebook.com/v2.6/me/thread_settings?access_token=xxx", httpContent);

        //        // If the response contains content we want to read it!
        //        if (httpResponse.Content != null)
        //        {
        //            var responseContent = await httpResponse.Content.ReadAsStringAsync();

        //        }
        //        var response = Request.CreateResponse(HttpStatusCode.OK);
        //        return response;
        //    }
        //}

    }
}