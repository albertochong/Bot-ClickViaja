using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using ClickViajaBot.Services;
using ClickViajaBot.DAL;
using MongoDB.Bson;
using ClickViajaBot.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Newtonsoft.Json;
using ClickViajaBot.Model;
using System.Linq;
using System.Net.Http;
using System.Text;
using ClickViajaBot.EuPagoService;
using AdaptiveCards;
using ClickViajaBot.Util;
using ClickViajaBot.Exceptions;
using System.IO;
using System.Web;

namespace ClickViajaBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [Serializable]
    [LuisModel("a1c0cfda-6583-4506-b82f-f0ffb25ae76f", "a83fa7be0115441e9012ef64e885fc38",
               domain: "westus.api.cognitive.microsoft.com", Staging = true)]
    public class LuisDialog : LuisDialog<object>
    {
        #region antigo
        //public LuisDialog() : base(new LuisService(new LuisModelAttribute(
        //    ConfigurationManager.AppSettings["LuisAppId"], 
        //    ConfigurationManager.AppSettings["LuisAPIKey"], 
        //    domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        //{
        //}

        //[LuisIntent("None")]
        //public async Task NoneIntent(IDialogContext context, LuisResult result)
        //{
        //    await this.ShowLuisResult(context, result);
        //}

        //// Go to https://luis.ai and create a new intent, then train/publish your luis app.
        //// Finally replace "Greeting" with the name of your newly created intent in the following handler
        //[LuisIntent("Greeting")]
        //public async Task GreetingIntent(IDialogContext context, LuisResult result)
        //{
        //    await this.ShowLuisResult(context, result);
        //}

        //[LuisIntent("Cancel")]
        //public async Task CancelIntent(IDialogContext context, LuisResult result)
        //{
        //    await this.ShowLuisResult(context, result);
        //}

        //[LuisIntent("Help")]
        //public async Task HelpIntent(IDialogContext context, LuisResult result)
        //{
        //    await this.ShowLuisResult(context, result);
        //}

        //private async Task ShowLuisResult(IDialogContext context, LuisResult result) 
        //{
        //    //result.Intents[0].Intent
        //    //result.Query

        //    await context.PostAsync($"ola");
        //    context.Wait(MessageReceived);
        //}

        //--------------------------------------------------------------------------------------------------------------

        #endregion

        #region Intents

        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            //TODO : Se nao responder deppois de 3 tentativas ver qual é o canal e reencaminhar para mim
            // TODO: DB.SaveMessage(result.Query);
            var cardActions = new List<CardAction>
            {
                //new CardAction
                //{
                //    Title = "Voos Barato",
                //    Type = ActionTypes.ImBack,
                //    Value = "Datas Voos Barato"
                //},
                //new CardAction
                //{
                //    Title = "Reservar Hotel",
                //    Type = ActionTypes.ImBack,
                //    Value = "Reservar Hotel"
                //},
                //new CardAction
                //{
                //    Title = "Reservar Carro",
                //    Type = ActionTypes.ImBack,
                //    Value = "Reservar Carro"
                //},
                //new CardAction
                //{
                //    Title = "Ver Orçamento",
                //    Type = ActionTypes.ImBack,
                //    Value = "Ver Orçamento"
                //},
                //new CardAction
                //{
                //    Title = "Consultar Reserva",
                //    Type = ActionTypes.ImBack,
                //    Value = "consultar Reserva"
                //},
                 new CardAction
                {
                    Title = "Pagar Reserva",
                    Type = ActionTypes.ImBack,
                    Value = "Pagar Reserva"
                },
                 new CardAction
                {
                    Title = "Ver Imagens Hotel",
                    Type = ActionTypes.ImBack,
                    Value = "Ver Imagens Hotel"
                },
                 new CardAction
                {
                    Title = "Solicitar Factura",
                    Type = ActionTypes.ImBack,
                    Value = "Solicitar Factura"
                },
                // new CardAction
                //{
                //    Title = "Últimas ofertas",
                //    Type = ActionTypes.ImBack,
                //    Value = "Últimas ofertas"
                //},
                 new CardAction
                {
                    Title = "Pedir Orçamento",
                    Type = ActionTypes.ImBack,
                    Value = "Pedir Orçamento"
                }
                //new CardAction
                //{
                //    Title = "Chamar assistente",
                //    Type = ActionTypes.ImBack,
                //    Value = "Chamar assistente"
                //}
            };

            var card = new HeroCard
            {
                Title = "Desculpe...",
                Text = "Ainda estou com algumas dúvidas, qual dessas opções representa o que deseja?",
                Buttons = cardActions
            };

            var activity = context.MakeMessage();
            activity.Id = new Random().Next().ToString();
            activity.Attachments.Add(card.ToAttachment());

            await context.PostAsync(activity);
            //context.Wait(MessageReceived);

            BsonDocument message = new BsonDocument {
                                                                    { "Message" , result.Query },
                                                                    { "Data" , DateTime.Now },
                                                                    { "Assunto" , "Unknow" },
                                                                    { "sentimento" , "" },
                                                                    { "Canal" , context.Activity.ChannelId },
                                                                    { "cliente" , context.Activity.From.Name},
                                                                    { "Idcliente" , context.Activity.From.Id},
                                                                    { "Agencia" , "ClickViaja - Estrada Luz"}
                                                                  };

            await ClickViajaDal.InsertDocument(message);

            string emailMessage = "Pergunta :" + result.Query + " do cliente "
                                  + context.Activity.From.Name + " a apartir do canal " + context.Activity.ChannelId;

            //Envia para meu email quando bot nao consegue responder
            Util.Util.SendEmail(emailMessage,"Click com dúvidas", "", null);

            await DespedidaAsync(context);
        }

        [LuisIntent("Saudacao")]
        public async Task Saudacao(IDialogContext context, LuisResult result)
        {
            //LearnLatestMessage(result.TopScoringIntent.Intent);

            var reply         = context.MakeMessage();
            reply.Recipient   = context.Activity.From;
            reply.Type        = "message";

            if (result.Query != "Sim" | result.Query != "sim")
            {
                reply.Attachments = new List<Attachment>();

                List<CardImage> cardImages = new List<CardImage>
                {
                    new CardImage(
                        url: "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSkg4OLKNa_d2H6G8LhiGbVRj5p26wn9WUdREu6StTb_kj-pych")
                };
                List<CardAction> cardButtons = new List<CardAction>();

                CardAction plButton = new CardAction()
                {
                    Value = "botao_iniciar_click",
                    Type = "postBack",
                    Title = "Clique aqui!",
                };

                cardButtons.Add(plButton);

                HeroCard plCard = new HeroCard()
                {
                    Title = "Olá eu sou o Click! 🙋‍♂️",
                    Subtitle = "Assistente Virtual Agência ClickViaja Estrada da Luz",///ver quem é o cliente para por o nome da agencia
                    Images = cardImages,
                    Text = "Bem vindo a plataforma digital de suporte ao cliente.",
                    Buttons = cardButtons
                };

                Attachment plAttachment = plCard.ToAttachment();
                reply.Attachments.Add(plAttachment);

                if (reply.From != null)
                {
                    reply.Text = "Olá " + context.Activity.From.Name;
                    await context.PostAsync(reply);
                    context.Wait(MessageReceived);

                    BsonDocument message = new BsonDocument {
                                                                    { "Message" , result.Query },
                                                                    { "Data" , DateTime.Now },
                                                                    { "Assunto" , "Saudacao" },
                                                                    { "sentimento" , "" },
                                                                    { "Canal" , context.Activity.ChannelId },
                                                                    { "cliente" , context.Activity.From.Name},
                                                                    { "Idcliente" , context.Activity.From.Id},
                                                                    { "Agencia" , "ClickViaja - Estrada Luz"}
                                                                  };

                    await ClickViajaDal.InsertDocument(message);
                }
            }
            else
            {
                reply.Text = "Em que posso ajudá-lo?";
                await context.PostAsync(reply);
            }          
        }

        [LuisIntent("Ajuda")]
        public async Task Ajuda(IDialogContext context, LuisResult result)
        {
            //LearnLatestMessage(result.TopScoringIntent.Intent);

            await context.PostAsync($"Sim posso ajudar nos seguintes tópicos abaixo 👇");
            //context.Wait(MessageReceived);

            var cardActions = new List<CardAction>
            {
                //new CardAction
                //{
                //    Title = "Voos Barato",
                //    Type = ActionTypes.ImBack,
                //    Value = "Datas Voos Barato"
                //},
                //new CardAction
                //{
                //    Title = "Reservar Hotel",
                //    Type = ActionTypes.ImBack,
                //    Value = "Reservar Hotel"
                    
                //},
                //new CardAction
                //{
                //    Title = "Reservar Carro",
                //    Type = ActionTypes.ImBack,
                //    Value = "Reservar Carro"
                //},
                // new CardAction
                //{
                //    Title = "Ver Orçamento",
                //    Type = ActionTypes.ImBack,
                //    Value = "Ver Orçamento"
                //},
                //new CardAction
                //{
                //    Title = "Consultar Reserva",
                //    Type = ActionTypes.ImBack,
                //    Value = "consultar Reserva"
                //},
                new CardAction
                {
                    Title = "Pagar Reserva",
                    Type = ActionTypes.ImBack,
                    Value = "Pagar Reserva"
                },
                new CardAction
                {
                    Title = "Ver Imagens Hotel",
                    Type = ActionTypes.ImBack,
                    Value = "Ver Imagens Hotel"
                },
                new CardAction
                {
                    Title = "Solicitar Factura",
                    Type = ActionTypes.ImBack,
                    Value = "Solicitar Factura"
                },
                //new CardAction
                //{
                //    Title = "Últimas ofertas",
                //    Type = ActionTypes.ImBack,
                //    Value = "Últimas ofertas"
                //},
                new CardAction
                {
                    Title = "Pedir Orçamento",
                    Type = ActionTypes.ImBack,
                    Value = "Pedir Orçamento"
                }
                //new CardAction
                //{
                //    Title = "Chamar assistente",
                //    Type = ActionTypes.ImBack,
                //    Value = "Chamar assistente"
                //}
            };

            var card = new HeroCard
            {
                //Title = "Lista das opçoes:",
                Buttons = cardActions
            };

            var activity = context.MakeMessage();
            activity.Id = new Random().Next().ToString();
            activity.Attachments.Add(card.ToAttachment());

            await context.PostAsync(activity);

            BsonDocument message = new BsonDocument {
                                                                    { "Message" , result.Query },
                                                                    { "Data" , DateTime.Now },
                                                                    { "Assunto" , "Ajuda" },
                                                                    { "sentimento" , "" },
                                                                    { "Canal" , context.Activity.ChannelId },
                                                                    { "cliente" , context.Activity.From.Name},
                                                                    { "Idcliente" , context.Activity.From.Id},
                                                                    { "Agencia" , "ClickViaja - Estrada Luz"}
                                                                  };

            await ClickViajaDal.InsertDocument(message);
        }


        //[LuisIntent("Reserva_Carro")]
        //public async Task Reserva_Carro(IDialogContext context, LuisResult result)
        //{
        //    //LearnLatestMessage(result.TopScoringIntent.Intent);

        //    await context.PostAsync($"Ótimo! Esses são os quartos que estão disponíveis...");
        //    context.Wait(MessageReceived);
        //}

        //[LuisIntent("Reserva_Hotel")]
        //public async Task Reserva_Hotel(IDialogContext context, LuisResult result)
        //{
        //    // LearnLatestMessage(result.TopScoringIntent.Intent);

        //    await context.PostAsync($"Entendi, iremos enviar nosso colaborador, qual é o número do seu quarto?");
        //    context.Wait(MessageReceived);
        //}      

        [LuisIntent("Reserva")]
        public async Task Reserva(IDialogContext context, LuisResult result)
        {

            await None(context, result);

            //LearnLatestMessage(result.TopScoringIntent.Intent);

            //await context.PostAsync($"Ok,o que deseja reservar?");
            //context.Wait(MessageReceived);

            //var cardActions = new List<CardAction>
            //{
            //    new CardAction
            //    {
            //        Title = "Datas Voos Barato",
            //        Type = ActionTypes.ImBack,
            //        Value = "Datas Voos Barato"
            //    },
            //    new CardAction
            //    {
            //        Title = "Reservar um hotel",
            //        Type = ActionTypes.ImBack,
            //        Value = "Reservar um hotel"
            //    },
            //    new CardAction
            //    {
            //        Title = "Reservar um carro",
            //        Type = ActionTypes.ImBack,
            //        Value = "Reservar um carro"
            //    }
            //};

            //var card = new HeroCard
            //{
            //    Title = "Ok...",
            //    Text = "O que deseja reservar?",
            //    Buttons = cardActions
            //};
            //var activity = context.MakeMessage();
            //activity.Id = new Random().Next().ToString();
            //activity.Attachments.Add(card.ToAttachment());

            //await context.PostAsync(activity);

            //BsonDocument message = new BsonDocument {
            //                                                        { "Message" , result.Query },
            //                                                        { "Data" , DateTime.Now },
            //                                                        { "Assunto" , "Reserva" },
            //                                                        { "sentimento" , "" },
            //                                                        { "Canal" , context.Activity.ChannelId },
            //                                                        { "cliente" , context.Activity.From.Name},
            //                                                        { "Idcliente" , context.Activity.From.Id},
            //                                                        { "Agencia" , "ClickViaja - Estrada Luz"}
            //                                                      };

            //await ClickViajaDal.InsertDocument(message);
        }


        [LuisIntent("Ver_Ofertas")]
        public async Task Ver_Ofertas(IDialogContext context, LuisResult result)
        {

            //todo inserir no mongo a message

            var reply            = context.MakeMessage();
            var images           = InstagramService.GetLastPostedImages();
            int numm_imagess     = 20;
           
            if (context.Activity.ChannelId == "skype")
                numm_imagess = 7;
            
            else if (context.Activity.ChannelId == "facebook")
                numm_imagess = 7;
            

            if (images != "")
            {
                var response = JsonConvert.DeserializeObject<Instagram>(images).data.Where(x => x.type == "carousel" || x.type == "image").Take(numm_imagess);

                if (response != null)
                {
                    await context.PostAsync($"Ok ,estas sáo apenas algumas das nossas ofertas em destaque.Pode acessar ao nosso site https://clickviajaestradadaluz.traveltool.pt/mshomett/Home de dispor de um leque grande de ofertas");

                    reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    reply.Attachments      = new List<Attachment>();

                    foreach (var item in response)
                    {
                        if (item.location != null && item.location.name == "Lisbon District")
                        {
                            if (item.carousel_media != null)
                            {
                                for (int i = 0; i < item.carousel_media.Count; i++)
                                {
                                    List<CardImage> cardImages = new List<CardImage>
                                    {
                                        new CardImage(url: item.carousel_media[i].images.standard_resolution.url)
                                    };

                                    List<CardAction> cardButtons = new List<CardAction>();

                                    CardAction plButton = new CardAction()
                                    {
                                        Value = item.carousel_media[i].images.standard_resolution.url,
                                        Type = "openUrl",
                                        Title = "Ver Oferta"
                                    };


                                    cardButtons.Add(plButton);

                                    CardAction plButton1 = new CardAction()
                                    {
                                        Value = "Pedir Orçamento",//+ item.carousel_media[i].images.standard_resolution.url,
                                        Type = "postBack",
                                        Title = "Pedir Orçamento"
                                    };

                                    cardButtons.Add(plButton1);

                                    HeroCard plCard = new HeroCard()
                                    {
                                        Title = item.tags.Count() > 0  ? item.tags[0] : string.Empty,
                                        Subtitle = item.tags.Count() > 1 ? item.tags[1] : string.Empty,
                                        Images = cardImages,
                                        Buttons = cardButtons
                                    };

                                    Attachment plAttachment = plCard.ToAttachment();
                                    reply.Attachments.Add(plAttachment);
                                }   
                            }
                            else
                            {
                                List<CardImage> cardImages = new List<CardImage>
                                {
                                    new CardImage(url: item.images.standard_resolution.url)
                                };

                                List<CardAction> cardButtons = new List<CardAction>();

                                CardAction plButton = new CardAction()
                                {
                                    Value = item.images.standard_resolution.url,
                                    Type = "openUrl",
                                    Title = "Ver Oferta"
                                };

                                cardButtons.Add(plButton);


                                CardAction plButton1 = new CardAction()
                                {
                                    Value = "Pedir Orçamento",// + item.images.standard_resolution.url,
                                    Type = "postBack",
                                    Title = "Pedir Orçamento"
                                };

                                cardButtons.Add(plButton1);

                                HeroCard plCard = new HeroCard()
                                {
                                    Title = item.tags.Count() > 0 ? item.tags[0] : string.Empty,
                                    Subtitle = item.tags.Count() > 0 ? item.tags[1] : string.Empty,
                                    Images = cardImages,
                                    Buttons = cardButtons
                                };

                                Attachment plAttachment = plCard.ToAttachment();
                                reply.Attachments.Add(plAttachment);

                            }
                        }                 
                    }

                    await context.PostAsync(reply);
                }
                else
                {

                }
            }
            else
            {
                reply.Text = "De momento nao temos nenhuma oferta disponível.🤧.Contacte-nos pelo número 212433527 ou pelo email lisboa.estradaluz@clickviaja.com";
            }          
        }


        #endregion

        #region Imagens e videos hotel

        [LuisIntent("Imagens_Hotel")]
        public async Task Imagens_Hotel(IDialogContext context, LuisResult result)
        {
            //LearnLatestMessage(result.TopScoringIntent.Intent);

            Activity typing = (Activity)context.MakeMessage();
            typing.Type     = ActivityTypes.Typing;
            await context.PostAsync(typing);


            if (result.Entities.Count == 1)
            {
                var form = new HotelDialog();
                var retrieveNomeHotel = new FormDialog<HotelDialog>(form, BuildRetrieveNomeHotel, FormOptions.PromptInStart);
                context.Call(retrieveNomeHotel, RetrieveCompleteNomeHotel);
            }
            else
            {
                try
                {
                    string hotel = String.Empty;

                    foreach (var item in result.Entities)
                    {
                        foreach (var et in result.Entities)
                        {
                            hotel += et.Entity + " ";
                        }

                        await context.PostAsync($"Ok aguarde um pouco.....");

                        List<string> urlList = BingSearchService.ImageSearch(hotel, context.Activity.ChannelId);
                        var reply = context.MakeMessage();

                        if (urlList.Count == 1)
                        {
                            reply.Text = "De momento nao foi possivel localizar este hotel.🤧";
                        }
                        else
                        {
                            #region imagens hotel

                            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                            reply.Attachments = new List<Attachment>();

                            foreach (string s in urlList)
                            {
                                List<CardImage> cardImages = new List<CardImage>
                            {
                                new CardImage(s)
                            };

                                List<CardAction> cardButtons = new List<CardAction>();

                                CardAction plButton = new CardAction()
                                {
                                    Value = s,
                                    Type = "openUrl",
                                    Title = "Ver Foto"
                                };

                                cardButtons.Add(plButton);

                                HeroCard plCard = new HeroCard()
                                {
                                    Title = string.Empty,
                                    Subtitle = string.Empty,
                                    Images = cardImages,
                                    Buttons = cardButtons
                                };

                                Attachment plAttachment = plCard.ToAttachment();
                                reply.Attachments.Add(plAttachment);
                            }

                            await context.PostAsync(reply);

                            BsonDocument message = new BsonDocument {
                                                                { "Message" , result.Query },
                                                                { "Data" , DateTime.Now },
                                                                { "Assunto" , "Imagens_Hotel" },
                                                                { "sentimento" , "" },
                                                                { "Canal" , context.Activity.ChannelId },
                                                                { "cliente" , context.Activity.From.Name},
                                                                { "Idcliente" , context.Activity.From.Id},
                                                                { "Agencia" , "ClickViaja - Estrada Luz"}
                                                              };

                            await ClickViajaDal.InsertDocument(message);

                            #endregion

                            //await context.PostAsync($"Um momento por favor vou ver se encontro alguns vídeos relacionados com este hotel também.");

                            //#region videos do hotel


                            //List<string> videoList = BingSearchService.VideoSearch(hotel, context.Activity.ChannelId);
                            //var replyVideo = context.MakeMessage();


                            //if (videoList.Count == 1)
                            //{
                            //    replyVideo.Text = "De momento nao foi possivel localizar nenhum video relacionado com este hotel.🤧";
                            //}
                            //else
                            //{
                            //    replyVideo.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                            //    replyVideo.Attachments = new List<Attachment>();

                            //    foreach (string s in videoList)
                            //    {
                            //       if (context.Activity.ChannelId != "facebook")
                            //       {
                            //            List<MediaUrl> urlsMedia = new List<MediaUrl>();
                            //            urlsMedia.Add(new MediaUrl(s));

                            //            List<CardAction> cardButtons = new List<CardAction>();

                            //            CardAction plButton = new CardAction()
                            //            {
                            //                Value = s,
                            //                Type = "playVideo",
                            //                Title = "",
                            //                Image = s
                            //            };

                            //            cardButtons.Add(plButton);

                            //            VideoCard vCard = new VideoCard()
                            //            {
                            //                Autoloop = true,
                            //                Shareable = true,
                            //                Media = urlsMedia,
                            //                Buttons = cardButtons,
                            //                Image = new ThumbnailUrl(s)
                            //            };

                            //            Attachment plAttachment = vCard.ToAttachment();
                            //            plAttachment.ContentUrl = s;
                            //            replyVideo.Attachments.Add(plAttachment);
                            //        }

                            //        //var videoObj = @" {
                            //        //            ""attachment"":{
                            //        //                ""type"":""video"",  
                            //        //                ""payload"":{
                            //        //                          ""url"":""http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4""}
                            //        //                           }
                            //        //               }";


                            //        //replyVideo.ChannelData = videoObj;


                            //        //if (s.Contains("youtube"))
                            //        //{
                            //        //    string url = "";

                            //        //    int index = s.IndexOf("=");
                            //        //    if (index > 0)
                            //        //        url = "https://youtu.be/" + s.Remove(0, index + 1);

                            //        //replyVideo.Attachments.Add(new VideoCard("My Video", "a subtitle", "some text", new ThumbnailUrl(), media: new[] { new MediaUrl(s) }).ToAttachment());

                            //        //}
                            //        //else
                            //        //{

                            //        //}

                            //        //if (s.Contains("youtube"))
                            //        //{
                            //        //    string url = "";

                            //        //    int index = s.IndexOf("=");
                            //        //    if (index > 0)
                            //        //        url = "https://youtu.be/" + s.Remove(0, index + 1) + ".mp4";

                            //        //replyVideo.Attachments.Add(new Attachment()
                            //        //{
                            //        //    ContentUrl = s,
                            //        //    ContentType = "video/mp4",
                            //        //ThumbnailUrl = s
                            //            //});

                            //        //}
                            //        //else
                            //        //{

                            //        //}                          
                            //    }

                            //    await context.PostAsync(replyVideo);

                            //    BsonDocument message1 = new BsonDocument {
                            //                                            { "Message" , "Vídeos de hotel " + hotel}, //result.Query },
                            //                                            { "Data" , DateTime.Now },
                            //                                            { "Assunto" , "Videos_Hotel" },
                            //                                            { "sentimento" , "" },
                            //                                            { "Canal" , context.Activity.ChannelId },
                            //                                            { "cliente" , context.Activity.From.Name},
                            //                                            { "Idcliente" , context.Activity.From.Id},
                            //                                          };

                            //    await ClickViajaDal.InsertDocument(message1);

                            //    var replyagain = context.MakeMessage();
                            //    replyagain.Text = $"Quer reservar este hotel " + context.Activity.From.Name + " ?";


                            //    replyagain.SuggestedActions = new SuggestedActions()
                            //    {
                            //        Actions = new List<CardAction>()
                            //                {
                            //                    new CardAction(){ Title = "Sim", Type=ActionTypes.PostBack, Value="Sim quero reservar hotel " + hotel, Image = "https://t4.ftcdn.net/jpg/00/30/84/05/240_F_30840579_2Z0YB8J9cPdp4pvQ0iShdBIQxAtHgkrY.jpg" },
                            //                    new CardAction(){ Title = "Nao", Type=ActionTypes.ImBack, Value="Nao quero reservar hotel", Image = "http://www.todaletra.com.br/wp-content/uploads/2012/10/duvidas-300x3001.jpg" }
                            //                }
                            //    };

                            //    await context.PostAsync(replyagain);


                            //    //context.Wait(MessageReceived);

                            //}

                            //#endregion

                            var replyagain = context.MakeMessage();
                            replyagain.Text = $"Quer reservar este hotel " + context.Activity.From.Name + " ?";


                            replyagain.SuggestedActions = new SuggestedActions()
                            {
                                Actions = new List<CardAction>()
                                        {
                                            new CardAction(){ Title = "Sim", Type=ActionTypes.PostBack, Value="Sim quero reservar hotel " + hotel, Image = "https://t4.ftcdn.net/jpg/00/30/84/05/240_F_30840579_2Z0YB8J9cPdp4pvQ0iShdBIQxAtHgkrY.jpg" },
                                            new CardAction(){ Title = "Nao", Type=ActionTypes.ImBack, Value="Nao quero reservar hotel", Image = "http://www.todaletra.com.br/wp-content/uploads/2012/10/duvidas-300x3001.jpg" }
                                        }
                            };

                            await context.PostAsync(replyagain);

                        }
                    }
                }
                catch (Exception)
                {
                    throw new ExceptionIntencao("Ocorreu um erro na intenção de ver imagens do hotel para ClickViaja - Estrada Luz e cliente " + context.Activity.From.Name + " no canal " + context.Activity.ChannelId);
                }             
            }
        }

        /// <summary>
        /// Método que constroi o formulario para pedir o nome do hotel
        /// </summary>
        /// <returns></returns>
        private static IForm<HotelDialog> BuildRetrieveNomeHotel()
        {
            var builder = new FormBuilder<HotelDialog>();
            return builder.AddRemainingFields().Build();

        }

        private async Task RetrieveCompleteNomeHotel(IDialogContext context, IAwaitable<HotelDialog> result)
        {
            HotelDialog appt = null;

            try
            {
                appt = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("Pedido abortado!");
                return;
            }

            if (appt != null)
            {
                String nif = appt.nomeHotel.ToString();

                await context.PostAsync($"Ok aguarde um pouco.....");

                List<string> urlList = BingSearchService.ImageSearch(appt.nomeHotel.ToString(), context.Activity.ChannelId);
                var reply            = context.MakeMessage();


                if (urlList.Count == 1)
                {
                    reply.Text = "De momento nao foi possivel localizar este hotel.🤧";
                }
                else
                {
                    #region imagens

                    reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    reply.Attachments = new List<Attachment>();

                    foreach (string s in urlList)
                    {
                        List<CardImage> cardImages = new List<CardImage>
                        {
                            new CardImage(s)
                        };

                        List<CardAction> cardButtons = new List<CardAction>();

                        CardAction plButton = new CardAction()
                        {
                            Value = s,
                            Type = "openUrl",
                            Title = "Ver Foto"
                        };

                        cardButtons.Add(plButton);

                        HeroCard plCard = new HeroCard()
                        {
                            Title = string.Empty,
                            Subtitle = string.Empty,
                            Images = cardImages,
                            Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        reply.Attachments.Add(plAttachment);

                    }

                    await context.PostAsync(reply);

                    BsonDocument message = new BsonDocument {
                                                                    { "Message" , "Vídeos de hotel " + appt.nomeHotel}, //result.Query },
                                                                    { "Data" , DateTime.Now },
                                                                    { "Assunto" , "Imagens_Hotel" },
                                                                    { "sentimento" , "" },
                                                                    { "Canal" , context.Activity.ChannelId },
                                                                    { "cliente" , context.Activity.From.Name},
                                                                    { "Idcliente" , context.Activity.From.Id},
                                                                  };

                    await ClickViajaDal.InsertDocument(message);

                    #endregion

                    #region videos do hotel

                    //await context.PostAsync($"Um momento por favor vou ver se encontro alguns vídeos relacionados com este hotel também.");

                    //List<string> videoList = BingSearchService.VideoSearch(appt.nomeHotel.ToString(), context.Activity.ChannelId);
                    //var replyVideo = context.MakeMessage();

                    //if (videoList.Count == 1)
                    //{
                    //    replyVideo.Text = "De momento nao foi possivel localizar nenhum video relacionado com este hotel.🤧";
                    //}
                    //else
                    //{
                    //    replyVideo.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    //    replyVideo.Attachments      = new List<Attachment>();

                    //    foreach (string s in videoList)
                    //    {
                    //        if (context.Activity.ChannelId != "facebook")
                    //        {
                    //            List<MediaUrl> urlsMedia = new List<MediaUrl>();
                    //            urlsMedia.Add(new MediaUrl(s));

                    //            List<CardAction> cardButtons = new List<CardAction>();

                    //            CardAction plButton = new CardAction()
                    //            {
                    //                Value = s,
                    //                Type = "playVideo",
                    //                Title = "",
                    //                Image = s
                    //            };

                    //            cardButtons.Add(plButton);

                    //            VideoCard vCard = new VideoCard()
                    //            {
                    //                Autoloop = true,
                    //                Shareable = true,
                    //                Media = urlsMedia,
                    //                Buttons = cardButtons,
                    //                Image = new ThumbnailUrl(s)
                    //            };

                    //            Attachment plAttachment = vCard.ToAttachment();
                    //            plAttachment.ContentUrl = s;
                    //            replyVideo.Attachments.Add(plAttachment);
                    //        }

                    //        //var videoObj = @" {
                    //        //                ""attachment"":{
                    //        //                    ""type"":""video"",  
                    //        //                    ""payload"":{
                    //        //                              ""url"":""http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4""}
                    //        //                               }
                    //        //                   }";


                    //        //replyVideo.ChannelData = videoObj;

                    //        //if (s.Contains("youtube"))
                    //        //{
                    //        //    string url = "";

                    //        //    int index = s.IndexOf("=");
                    //        //    if (index > 0)
                    //        //        url = "https://youtu.be/" + s.Remove(0, index + 1);

                    //           // replyVideo.Attachments.Add(new VideoCard("My Video", "a subtitle", "some text", new ThumbnailUrl() ,media: new[] { new MediaUrl(s) }).ToAttachment());

                    //        //}
                    //        //else
                    //        //{

                    //        //}



                    //        //if (s.Contains("youtube"))
                    //        //{
                    //        //    string url = "";

                    //        //    int index = s.IndexOf("=");
                    //        //    if (index > 0)
                    //        //        url = "https://youtu.be/" + s.Remove(0, index + 1) + ".mp4";

                    //           //replyVideo.Attachments.Add(new Attachment()
                    //           // {
                    //           //     ContentUrl = s,
                    //           //     ContentType = "video/mp4",
                    //           //     ThumbnailUrl = s
                    //           // });

                    //        //}
                    //        //else
                    //        //{

                    //        //}




                    //    }

                    //    await context.PostAsync(replyVideo);

                    //    BsonDocument message1 = new BsonDocument {
                    //                                                { "Message" , "Vídeos de hotel " + appt.nomeHotel}, //result.Query },
                    //                                                { "Data" , DateTime.Now },
                    //                                                { "Assunto" , "Videos_Hotel" },
                    //                                                { "sentimento" , "" },
                    //                                                { "Canal" , context.Activity.ChannelId },
                    //                                                { "cliente" , context.Activity.From.Name},
                    //                                                { "Idcliente" , context.Activity.From.Id},
                    //                                              };

                    //    await ClickViajaDal.InsertDocument(message1);


                    //    //context.Wait(MessageReceived);
                    //}

                    #endregion

                    var replyagain = context.MakeMessage();
                    replyagain.Text = $"Quer reservar este hotel " + context.Activity.From.Name + " ?";


                    replyagain.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                                        {
                                            new CardAction(){ Title = "Sim", Type=ActionTypes.PostBack, Value="Sim quero reservar hotel " + appt.nomeHotel.ToString(), Image = "https://t4.ftcdn.net/jpg/00/30/84/05/240_F_30840579_2Z0YB8J9cPdp4pvQ0iShdBIQxAtHgkrY.jpg" },
                                            new CardAction(){ Title = "Nao", Type=ActionTypes.ImBack, Value="Nao quero reservar hotel", Image = "http://www.todaletra.com.br/wp-content/uploads/2012/10/duvidas-300x3001.jpg" }
                                        }
                    };

                    await context.PostAsync(replyagain);
                }
            }
            else
            {
                await context.PostAsync("Nao digitou nenhuma informaçao!");
            }

            //context.Wait(MessageReceived);
        }

        #endregion

        #region Solicitar Facturas Intetnts

        [LuisIntent("Solicitar_Factura")]
        public async Task Solicitar_Factura(IDialogContext context, LuisResult result)
        {
            BsonDocument message = new BsonDocument {
                                                                        { "Message" , result.Query },
                                                                        { "Data" , DateTime.Now },
                                                                        { "Assunto" , "Solicitar_Factura" },
                                                                        { "sentimento" , "" },
                                                                        { "Canal" , context.Activity.ChannelId },
                                                                        { "cliente" , context.Activity.From.Name},
                                                                        { "Idcliente" , context.Activity.From.Id},
                                                                        { "Agencia" , "ClickViaja - Estrada Luz"}
                                                                      };

            await ClickViajaDal.InsertDocument(message);

            Activity typing = (Activity)context.MakeMessage();
            typing.Type     = ActivityTypes.Typing;
            await context.PostAsync(typing);

            var form             = new FacturaDialog();
            var retrievefactura  = new FormDialog<FacturaDialog>(form, BuildRetrieveForm, FormOptions.PromptInStart);
            context.Call(retrievefactura, RetrieveComplete);
        }

        /// <summary>
        /// Método que constroi o formulario pedido o numeor de contribuinte ou destino ou numero de telemovel
        /// </summary>
        /// <returns></returns>
        private static IForm<FacturaDialog> BuildRetrieveForm()
        {
            var builder = new FormBuilder<FacturaDialog>();
            return builder.AddRemainingFields().Build();
        }

        /// <summary>
        /// Método que pede o nif ou numer0 de telemovel correcto
        /// </summary>
        /// <returns></returns>
        private static IForm<ContribuinteDialog> BuildRetrieveNif()
        {
            var builder = new FormBuilder<ContribuinteDialog>();
            return builder.AddRemainingFields().Build();
        }

        private async Task RetrieveCompleteNif(IDialogContext context, IAwaitable<ContribuinteDialog> result)
        {
            ContribuinteDialog appt = null;

            try
            {
                appt = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("Pedido abortado!");
                return;
            }
            try
            {
                if (appt != null)
                {
                    String nif = appt.nif.ToString();
                    var list = await ClickViajaDal.GetInvoiceDocument(nif.ToString().Trim());

                    Activity typing = (Activity)context.MakeMessage();
                    typing.Type = ActivityTypes.Typing;
                    await context.PostAsync(typing);

                    if (list != null)
                    {
                        if (list.Count == 0)
                        {
                            BsonDocument message = new BsonDocument {
                                                                        { "Message" , "Não tem nenhuma factura disponível neste momento." },
                                                                        { "Data" , DateTime.Now },
                                                                        { "Assunto" , "Solicitar_Factura" },
                                                                        { "sentimento" , "" },
                                                                        { "Canal" , context.Activity.ChannelId },
                                                                        { "cliente" , context.Activity.From.Name},
                                                                        { "Idcliente" , context.Activity.From.Id},
                                                                        { "Nif" , nif.ToString()},
                                                                        {"Agência" , "ClicViaja - Estrada Luz"}
                                                                      };

                            await ClickViajaDal.InsertDocument(message);
                            await context.PostAsync("Não tem nenhuma factura disponível neste momento associada a este número de contribuinte.");
                        }
                        else
                        {
                            var reply           = context.MakeMessage();
                            reply.Recipient     = context.Activity.From;
                            reply.Type          = "message";
                            reply.Text          = "As suas facturas foram enviadas para o email.Obrigado";
                            string emailMessage = "As suas facturas relacionadas com as viagens para: ";
                            string destino      = " ";
                            List<System.Net.Mail.Attachment> invoiceList = new List<System.Net.Mail.Attachment>();
                            string emailCliente = ""; ;

                            for (int i = 0; i < list.Count; i++)
                            {
                                byte[] imagedata = list[i].Invoice;
                                //var image64 = "data:application/pdf;base64," + Convert.ToBase64String(imagedata);
                                
                                destino += list[i].Destino + ", ";
                                emailCliente = list[0].Email;

                                MemoryStream stream = new MemoryStream(imagedata);
                                System.Net.Mail.Attachment att = new System.Net.Mail.Attachment(stream, list[i].Destino + ".pdf");
                                invoiceList.Add(att);
                            }

                            Util.Util.SendEmail(emailMessage + " " + destino, "Facturas de Viagens", emailCliente, invoiceList);
                            await context.PostAsync(reply);

                            Activity typing1 = (Activity)context.MakeMessage();
                            typing1.Type = ActivityTypes.Typing;
                            await context.PostAsync(typing1);

                            //perguntar se desejaa mais alguma coisa se esta contente e apesentar o inquerito

                        }
                    }
                    else
                    {
                        BsonDocument message = new BsonDocument {
                                                                        { "Message" , "Não tem nenhuma factura disponível neste momento." },
                                                                        { "Data" , DateTime.Now },
                                                                        { "Assunto" , "Solicitar_Factura" },
                                                                        { "sentimento" , "" },
                                                                        { "Canal" , context.Activity.ChannelId },
                                                                        { "cliente" , context.Activity.From.Name},
                                                                        { "Idcliente" , context.Activity.From.Id},
                                                                        {"Nif" , nif.ToString()},
                                                                        {"Agência" , "ClicViaja - Estrada Luz"}
                                                                      };

                        await ClickViajaDal.InsertDocument(message);
                        await context.PostAsync("Não tem nenhuma factura disponível neste momento associada a este número de contribuinte.");
                    }
                }
                else
                {
                    await context.PostAsync("Nao digitou nenhuma informaçao!");
                }

                //context.Wait(MessageReceived);

                await DespedidaAsync(context);

            }
            catch (Exception)
            {
                throw new ExceptionIntencao("Ocorreu um erro na intenção de solicitação da factura para ClickViaja - Estrada Luz e cliente " + context.Activity.From.Name + " no canal " + context.Activity.ChannelId);
            }
        }

        private async Task RetrieveComplete(IDialogContext context, IAwaitable<FacturaDialog> result)
        {
            FacturaDialog appt = null;

            try
            {
                appt = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("Pedido abortado!");
                return;
            }
            try
            {
                if (appt != null)
                {
                    string nif = appt.nif.ToString();

                    int n = 0;

                    bool isNumeric = int.TryParse(nif, out n);

                    if (isNumeric | nif.Length == 9)
                    {
                        var list = await ClickViajaDal.GetInvoiceDocument(n.ToString().Trim());

                        Activity typing = (Activity)context.MakeMessage();
                        typing.Type = ActivityTypes.Typing;
                        await context.PostAsync(typing);

                        if (list != null)
                        {
                            if (list.Count == 0)
                            {
                                BsonDocument message = new BsonDocument {
                                                                        { "Message" , "Não tem nenhuma factura disponível neste momento." },
                                                                        { "Data" , DateTime.Now },
                                                                        { "Assunto" , "Solicitar_Factura" },
                                                                        { "sentimento" , "" },
                                                                        { "Canal" , context.Activity.ChannelId },
                                                                        { "cliente" , context.Activity.From.Name},
                                                                        { "Idcliente" , context.Activity.From.Id},
                                                                        {"Nif" , n.ToString()},
                                                                        {"Agência" , "ClicViaja - Estrada Luz"}
                                                                      };

                                await ClickViajaDal.InsertDocument(message);
                                await context.PostAsync("Não tem nenhuma factura disponível neste momento associada a este número de contribuinte.");
                            }
                            else
                            {
                                var reply           = context.MakeMessage();
                                reply.Recipient     = context.Activity.From;
                                reply.Type          = "message";
                                reply.Text          = "As suas facturas foram enviadas para o seu email.Obrigado";
                                string emailMessage = "As suas facturas relacionadas com as viagens para: ";
                                string destino      = " ";
                                List<System.Net.Mail.Attachment> invoiceList = new List<System.Net.Mail.Attachment>();
                                string emailCliente = ""; ;

                                for (int i = 0; i < list.Count; i++)
                                {
                                    byte[] imagedata = list[i].Invoice;
                                    //var image64 = "data:application/pdf;base64," + Convert.ToBase64String(imagedata);

                                    destino += list[i].Destino + ", ";
                                    emailCliente = list[0].Email;

                                    MemoryStream stream = new MemoryStream(imagedata);
                                    System.Net.Mail.Attachment att = new System.Net.Mail.Attachment(stream, list[i].Destino + ".pdf");
                                    invoiceList.Add(att);
                                }

                                Util.Util.SendEmail(emailMessage + " " + destino, "Facturas de Viagens", emailCliente, invoiceList);
                                await context.PostAsync(reply);


                                //perguntar se desejaa mais alguma coisa se esta contente e apesentar o inquerito
                            }

                            //context.Wait(MessageReceived);
                        }
                        else
                        {
                            BsonDocument message = new BsonDocument {
                                                                        { "Message" , "Não tem nenhuma factura disponível neste momento." },
                                                                        { "Data" , DateTime.Now },
                                                                        { "Assunto" , "Solicitar_Factura" },
                                                                        { "sentimento" , "" },
                                                                        { "Canal" , context.Activity.ChannelId },
                                                                        { "cliente" , context.Activity.From.Name},
                                                                        { "Idcliente" , context.Activity.From.Id},
                                                                        {"Nif" , n.ToString()},
                                                                        {"Agência" , "ClickViaja Estrada Luz"}
                                                                      };

                            await ClickViajaDal.InsertDocument(message);
                            await context.PostAsync("Não tem nenhuma factura disponível neste momento associada a este número de contribuinte.");
                        }
                    }
                    else
                    {
                        var form = new ContribuinteDialog();
                        var retrieveNif = new FormDialog<ContribuinteDialog>(form, BuildRetrieveNif, FormOptions.PromptInStart);
                        context.Call(retrieveNif, RetrieveCompleteNif);
                    }
                }
                else
                {
                    await context.PostAsync("Nao digitou nenhuma informaçao!");
                }
            }
            catch (Exception)
            {
                throw new ExceptionIntencao("Ocorreu um erro na intenção de solicitação da factura para ClickViaja - Estrada Luz e cliente " + context.Activity.From.Name + " no canal " + context.Activity.ChannelId);    
            }

            //context.Wait(MessageReceived);
            await DespedidaAsync(context);
        }

        #endregion

        #region Pagar reserva Intent

        [LuisIntent("Pagar_Reserva")]
        public async Task Pagar_Reserva(IDialogContext context, LuisResult result)
        {
            Activity typing = (Activity)context.MakeMessage();
            typing.Type     = ActivityTypes.Typing;
            await context.PostAsync(typing);

            BsonDocument message = new BsonDocument {
                                                                        { "Message" , result.Query },
                                                                        { "Data" , DateTime.Now },
                                                                        { "Assunto" , "Pagar_Reserva" },
                                                                        { "sentimento" , "" },
                                                                        { "Canal" , context.Activity.ChannelId },
                                                                        { "cliente" , context.Activity.From.Name},
                                                                        { "Idcliente" , context.Activity.From.Id},
                                                                        { "Agencia" , "ClickViaja - Estrada Luz"}
                                                                      };

            await ClickViajaDal.InsertDocument(message);

            var form = new ReservaDialog();
            var pagarReserva = new FormDialog<ReservaDialog>(form,  ReservaDialog.BuildPagarReservaForm, FormOptions.PromptInStart);
            context.Call(pagarReserva, PagarReservaComplete);
        }

        private async Task PagarReservaComplete(IDialogContext context, IAwaitable<ReservaDialog> result)
        {
            ReservaDialog appt = null;

            try
            {
                appt = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("Pedido abortado!");
                return;
            }
            try
            {
                if (appt != null)
                {
                    //getting user's input value e ----------Validar o email
                    String email = appt.email.ToString();

                    //vai ao mongo buscar a reserva
                    var reserva = await ClickViajaDal.GetReservation(email.ToString());

                    if (reserva != null)
                    {
                        if (appt.MeioPagamentoRequired.ToString() == "MBWay")
                        {
                            pedidoMBWRequest mbWay = new pedidoMBWRequest
                            {
                                valor = reserva.PrecoVenda,
                                id = reserva.Descricao,
                                alias = reserva.Telemovel_Principal
                            };

                            pedidoMBWResponse response = await Eu_PagoService.GeraPedidoMBWayAsync(mbWay);

                            if (response != null && response.estado == 0)
                            {
                                var reply = context.MakeMessage();
                                reply.Type = "message";
                                reply.Attachments = new List<Attachment>();

                                AdaptiveCard card = new AdaptiveCard
                                {

                                    // Specify speech for the card.  
                                    Speak = ""
                                };
                                // Body content  
                                card.Body.Add(new Image()
                                {
                                    Url = "https://lh6.ggpht.com/uFiCg8DkG7F2uW4jQSs7PzwZyxuxkyYUEfUAT6ju_qFmvk8WR1z6xG3HKMjWCLm-1rc",
                                    Size = ImageSize.Auto,
                                    Style = ImageStyle.Normal,
                                    AltText = "MBWay"

                                });

                                // Add text numero telemovel to the card.  
                                card.Body.Add(new TextBlock()
                                {
                                    Text = "Número Telemovél: " + response.alias,
                                    Size = TextSize.Medium,
                                    Weight = TextWeight.Bolder
                                });

                                // Add text referencia to the card.  
                                card.Body.Add(new TextBlock()
                                {
                                    Text = "Referência: " + response.referencia,
                                    Size = TextSize.Medium,
                                    Weight = TextWeight.Bolder
                                });

                                // Add text valor a pagar to the card.  
                                card.Body.Add(new TextBlock()
                                {
                                    Text = "Valor: " + response.valor + " €",
                                    Size = TextSize.Medium,
                                    Weight = TextWeight.Bolder
                                });

                                // Add text alerta to the card.  
                                card.Body.Add(new TextBlock()
                                {
                                    Text = "Por favor verifique na sua aplicação o pedido de pagamento.Receberá um pedido de pagamento em nome da EU PAGO que é nosso parceiro de pagamento.Assim que efectuar o pagamento enviaremos todos os documentos para sua viagem.Muito obrigado.Em caso de dúvida ligue 212433527",
                                    Size = TextSize.Normal,
                                    Weight = TextWeight.Lighter,
                                    Wrap = true,
                                    Color = TextColor.Good
                                });


                                // Create the attachment with adapative card.  
                                Attachment attachment = new Attachment()
                                {
                                    ContentType = AdaptiveCard.ContentType,
                                    Content = card
                                };

                                reply.Attachments.Add(attachment);

                                await context.PostAsync(reply);

                                //fazer update do campo na reserva Ref_MB_Gerada para 1
                                ClickViajaDal.UpdateReservationRef_MB_Gerada(reserva.Id, context.Activity.ChannelId, context.Activity.From.Name, reserva.Email_Principal,
                                                                             context.Activity.From.Id, "MBWay", response.referencia, response.valor);

                            }
                            else if (response.estado == -9)
                            {
                                await context.PostAsync("Não é possivel pagar esta reserva de momento, o pedido foi recusado, por favor verifique se não excedeu o limite diário.");
                            }
                            else
                                await context.PostAsync("Não é possivel pagar esta reserva de momento, por favor tente mais ou tarde ou ligue 212433527");

                        }
                        else
                        {
                            gerarReferenciaMBDLRequest obj = new gerarReferenciaMBDLRequest
                            {
                                valor = reserva.PrecoVenda,
                                id = reserva.Descricao,
                                data_inicio = DateTime.Now.Date.ToString("yyyy-MM-dd"),
                                data_fim = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd")
                            };

                            gerarReferenciaMBDLResponse response = await Eu_PagoService.GeraReferenciaMBDLAsync(obj);

                            if (response != null && response.estado == 0)
                            {
                                var reply = context.MakeMessage();
                                reply.Type = "message";
                                reply.Attachments = new List<Attachment>();


                                AdaptiveCard card = new AdaptiveCard
                                {

                                    // Specify speech for the card.  
                                    Speak = ""
                                };
                                // Body content  
                                card.Body.Add(new Image()
                                {
                                    Url = "https://www.vendus.pt/foto/referencia-multibanco1200-76_og.png",
                                    Size = ImageSize.Auto,
                                    Style = ImageStyle.Normal,
                                    AltText = "Entidade - Referência"

                                });

                                // Add text entidade to the card.  
                                card.Body.Add(new TextBlock()
                                {
                                    Text = "Entidade: " + response.entidade,
                                    Size = TextSize.Medium,
                                    Weight = TextWeight.Bolder
                                });

                                // Add text referencia to the card.  
                                card.Body.Add(new TextBlock()
                                {
                                    Text = "Referência: " + response.referencia,
                                    Size = TextSize.Medium,
                                    Weight = TextWeight.Bolder
                                });

                                // Add text valor a pagar to the card.  
                                card.Body.Add(new TextBlock()
                                {
                                    Text = "Valor: " + response.valor + " €",
                                    Size = TextSize.Medium,
                                    Weight = TextWeight.Bolder
                                });

                                // Add text alerta to the card.  
                                card.Body.Add(new TextBlock()
                                {
                                    Text = "O talão emitido pela caixa automática faz de prova de pagamento, conserve-o. Esta referência é válida até ao dia " + response.data_fim.ToString() + ". Assim que efectuar o pagamento enviaremos todos os documentos para sua viagem.Muito obrigado.Em caso de dúvida ligue 212433527",
                                    Size = TextSize.Normal,
                                    Weight = TextWeight.Lighter,
                                    Wrap = true,
                                    Color = TextColor.Attention
                                });

                                // Create the attachment with adapative card.  
                                Attachment attachment = new Attachment()
                                {
                                    ContentType = AdaptiveCard.ContentType,
                                    Content = card
                                };

                                reply.Attachments.Add(attachment);

                                await context.PostAsync(reply);

                                //fazer update do campo na reserva Ref_MB_Gerada para 1
                                ClickViajaDal.UpdateReservationRef_MB_Gerada(reserva.Id, context.Activity.ChannelId, context.Activity.From.Name, reserva.Email_Principal,
                                                                             context.Activity.From.Id, response.entidade, response.referencia, response.valor);

                            }
                            else
                                await context.PostAsync("Não é possivel pagar esta reserva de momento, por favor tente mais ou tarde ou ligue 212433527");
                        }
                    }
                    else
                        await context.PostAsync("Não encontrei nenhuma reserva associada a este email 😢.");

                    await DespedidaAsync(context);

                    context.Wait(MessageReceived);
                }
            }
            catch (Exception)
            {
                throw new ExceptionIntencao("Ocorreu um erro na intenção de pagamento da factura para ClickViaja - Estrada Luz e cliente " + context.Activity.From.Name + " no canal " + context.Activity.ChannelId);
            }
        }

        #endregion

        #region Pesquisa e reserva voo 

        [LuisIntent("Reserva_Voo")]
        public async Task Reserva_Voo(IDialogContext context, LuisResult result)
        {
            
        }


        private async Task PesquisaVooComplete(IDialogContext context, IAwaitable<PesquisaVooDialog> result)
        {
            PesquisaVooDialog appt = null;
            string token           = string.Empty;

            try
            {
                appt = await result;
                //aqui vamos já pedir o token

                if (appt != null)
                {
                    var reply              = context.MakeMessage();
                    reply.Type             = "message";
                    reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    reply.Attachments      = new List<Attachment>();
                    AdaptiveCard card      = null;
                    Attachment attachment  = null;
                    string originCity      = appt.OriginCity;
                    string destinationCity = appt.DestinationCity;
                    bool isFirtsSegment    = true;
                    int segment            = 1;

                    token = await AmadeuService.GetAmadeusAcessToken();

                    Activity typing = (Activity)context.MakeMessage();
                    typing.Type = ActivityTypes.Typing;
                    await context.PostAsync(typing);

                    
                    //traduzir a cidade de origem para EN
                    string OriginCityIataCode = await AmadeuService.GetAirportIataCodeAsync(appt.OriginCity, token);
                    appt.OriginCity = OriginCityIataCode;

                    Activity typing1 = (Activity)context.MakeMessage();
                    typing1.Type = ActivityTypes.Typing;
                    await context.PostAsync(typing1);


                    //traduzir a cidade de destino para EN
                    string DestinationCityIataCode = await AmadeuService.GetAirportIataCodeAsync(appt.DestinationCity, token);
                    appt.DestinationCity = DestinationCityIataCode;

                    await context.PostAsync($"Ok aguarde um pouco.....Apresentarei apenas os 5 preços mais baixos.Caso nao seja possível apresentar lhe os resultados agora enviaremos um email.");

                    Activity typingWait = (Activity)context.MakeMessage();
                    typingWait.Type = ActivityTypes.Typing;
                    await context.PostAsync(typingWait);

                    var search = AmadeuService.PesquisaVoo(appt, token);

                    if (search.Result != null)
                    {
                        foreach (var item in search.Result.data)
                        {
                            foreach (var offerItem in item.offerItems)
                            {
                                card = new AdaptiveCard
                                {
                                    Speak = "Reservar"
                                };

                                foreach (var itemService in offerItem.services)
                                {
                                    foreach (var seg in itemService.segments)
                                    {
                                        ///segmento ida
                                        if (isFirtsSegment)
                                        {
                                            if (itemService.segments.Count == 1)
                                                isFirtsSegment = false;

                                            // opcoes de ida 
                                            card.Body.Add(new TextBlock()
                                            {
                                                Text     = "Opçao" + segment + ": Ida",
                                                Size     = TextSize.Medium,
                                                Weight   = TextWeight.Bolder,
                                                IsSubtle = false
                                            });

                                            ///Data partida
                                            card.Body.Add(new TextBlock()
                                                {
                                                    Text = "Data Partida: " + seg.flightSegment.departure.at,
                                                    Size = TextSize.Medium,
                                                    Weight = TextWeight.Lighter,
                                                    Separation = SeparationStyle.None
                                                });

                                            //Terminal
                                            card.Body.Add(new TextBlock()
                                            {
                                                Text = "Terminal: " + seg.flightSegment.departure.terminal,
                                                Size = TextSize.Medium,
                                                Weight = TextWeight.Lighter,
                                                Separation = SeparationStyle.None
                                            });

                                            ColumnSet colset = new ColumnSet
                                            {
                                                Separation = SeparationStyle.Default
                                            };

                                            //// coluna 1
                                            Column col = new Column
                                            {
                                                Size = "1"
                                            };
                                            col.Items.Add(new TextBlock()
                                            {
                                                Text = originCity,
                                                IsSubtle = true

                                            });
                                            col.Items.Add(new TextBlock()
                                            {
                                                Text = seg.flightSegment.departure.iataCode,
                                                Size = TextSize.ExtraLarge,
                                                Color = TextColor.Accent,
                                                Separation = SeparationStyle.None
                                            });

                                            colset.Columns.Add(col);

                                            //// coluna 2 iamgem
                                            Column col1 = new Column
                                            {
                                                Size = "auto"
                                            };
                                            col1.Items.Add(new TextBlock()
                                            {
                                                Text = " "
                                            });
                                            col1.Items.Add(new Image()
                                            {
                                                Url = "http://adaptivecards.io/content/airplane.png",
                                                Size = ImageSize.Small,
                                                Separation = SeparationStyle.None

                                            });

                                            colset.Columns.Add(col1);

                                            //// coluna 3
                                            Column col2 = new Column
                                            {
                                                Size = "1"
                                            };
                                            col2.Items.Add(new TextBlock()
                                            {
                                                HorizontalAlignment = HorizontalAlignment.Right,
                                                Text = destinationCity,
                                                IsSubtle = true

                                            });
                                            col2.Items.Add(new TextBlock()
                                            {
                                                HorizontalAlignment = HorizontalAlignment.Right,
                                                Text = seg.flightSegment.arrival.iataCode,
                                                Size = TextSize.ExtraLarge,
                                                Color = TextColor.Accent,
                                                Separation = SeparationStyle.None
                                            });

                                            colset.Columns.Add(col2);

                                            card.Body.Add(colset);

                                            segment++;
                                        }

                                        #region else
                                        //else
                                        //{
                                        //    //Data regresso
                                        //    card.Body.Add(new TextBlock()
                                        //    {
                                        //        Text = "Data Regresso: " + seg.flightSegment.departure.at,
                                        //        Size = TextSize.Medium,
                                        //        Weight = TextWeight.Lighter,
                                        //        Separation = SeparationStyle.None
                                        //    });

                                        //    //Terminal
                                        //    card.Body.Add(new TextBlock()
                                        //    {
                                        //        Text = "Terminal: " + seg.flightSegment.departure.terminal,
                                        //        Size = TextSize.Medium,
                                        //        Weight = TextWeight.Lighter,
                                        //        Separation = SeparationStyle.None
                                        //    });

                                        //    ColumnSet colset = new ColumnSet();
                                        //    colset.Separation = SeparationStyle.Default;

                                        //    //// coluna 1
                                        //    Column col = new Column();
                                        //    col.Size = "1";
                                        //    col.Items.Add(new TextBlock()
                                        //    {
                                        //        Text = destinationCity,
                                        //        IsSubtle = true

                                        //    });
                                        //    col.Items.Add(new TextBlock()
                                        //    {
                                        //        Text = seg.flightSegment.departure.iataCode,
                                        //        Size = TextSize.ExtraLarge,
                                        //        Color = TextColor.Accent,
                                        //        Separation = SeparationStyle.None
                                        //    });

                                        //    colset.Columns.Add(col);

                                        //    //// coluna 2 iamgem
                                        //    Column col1 = new Column();
                                        //    col1.Size = "auto";
                                        //    col1.Items.Add(new TextBlock()
                                        //    {
                                        //        Text = " "
                                        //    });
                                        //    col1.Items.Add(new Image()
                                        //    {
                                        //        Url = "http://adaptivecards.io/content/airplane.png",
                                        //        Size = ImageSize.Small,
                                        //        Separation = SeparationStyle.None

                                        //    });

                                        //    colset.Columns.Add(col1);

                                        //    //// coluna 3
                                        //    Column col2 = new Column();
                                        //    col2.Size = "1";
                                        //    col2.Items.Add(new TextBlock()
                                        //    {
                                        //        HorizontalAlignment = HorizontalAlignment.Right,
                                        //        Text = originCity,
                                        //        IsSubtle = true

                                        //    });
                                        //    col2.Items.Add(new TextBlock()
                                        //    {
                                        //        HorizontalAlignment = HorizontalAlignment.Right,
                                        //        Text = seg.flightSegment.arrival.iataCode,
                                        //        Size = TextSize.ExtraLarge,
                                        //        Color = TextColor.Accent,
                                        //        Separation = SeparationStyle.None
                                        //    });

                                        //    colset.Columns.Add(col2);

                                        //    card.Body.Add(colset);

                                        //}

                                        #endregion
                                    }
                                }

                                attachment = new Attachment()
                                {
                                    ContentType = AdaptiveCard.ContentType,
                                    Content = card
                                };
                                reply.Attachments.Add(attachment);
                            }
                        }
                    }

                    await context.PostAsync(reply);
                    context.Wait(MessageReceived);
                }
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("Pedido abortado!");
                return;
            }
            catch (Exception)
            {
                throw new ExceptionIntencao("Ocorreu um erro na intenção de pesquisa dos voos para ClickViaja - Estrada Luz e cliente " + context.Activity.From.Name + " no canal " + context.Activity.ChannelId);
            }
        }

        #endregion

        #region pesquisa data voo barato

        [LuisIntent("Data_Voo_Barato")]
        public async Task Data_Voo_Barato(IDialogContext context, LuisResult result)
        {
            //LearnLatestMessage(result.TopScoringIntent.Intent);

            BsonDocument message = new BsonDocument {
                                                                    { "Message" , result.Query },
                                                                    { "Data" , DateTime.Now },
                                                                    { "Assunto" , "Data_Voo_Barato" },
                                                                    { "sentimento" , "" },
                                                                    { "Canal" , context.Activity.ChannelId },
                                                                    { "cliente" , context.Activity.From.Name},
                                                                    { "Idcliente" , context.Activity.From.Id},
                                                                    { "Agencia" , "ClickViaja - Estrada Luz"}
                                                                  };

            await ClickViajaDal.InsertDocument(message);

            Activity typing = (Activity)context.MakeMessage();
            typing.Type     = ActivityTypes.Typing;
            await context.PostAsync(typing);

            await context.PostAsync($"Ok vou pesquisar as datas com voos mais baratos que temos disponíveis.");

            string originCity      = string.Empty;
            string destinationCity = string.Empty;

            var form = new PesquisaVooDialog();

            if (result.Entities.Count >= 3)
            {
                foreach (var item in result.Entities)
                {
                    if (item.Type == "Lugar::LugarDe")
                    {
                        originCity = item.Entity;
                    }
                    if (item.Type == "Lugar::LugarPara")
                    {
                        destinationCity += item.Entity + " ";
                    } 
                }

                await PesquisaDataVooBarato(originCity, destinationCity, context);
            }
            else
            {
                var pesquisaVoo = new FormDialog<PesquisaVooDialog>(form, PesquisaVooDialog.BuildDataVooBaratoForm, FormOptions.PromptInStart);
                context.Call(pesquisaVoo, PesquisaDataVooBarato);
            }
        }

        private async Task PesquisaDataVooBarato(IDialogContext context, IAwaitable<PesquisaVooDialog> result)
        {
            PesquisaVooDialog appt = null;
            string token           = string.Empty;

            try
            {
                appt = await result;
          
                if (appt != null)
                {
                    var reply              = context.MakeMessage();
                    reply.Type             = "message";
                    reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    reply.Attachments      = new List<Attachment>();
                    AdaptiveCard card      = null;
                    Attachment attachment  = null;
                    string originCity      = appt.OriginCity;
                    string destinationCity = appt.DestinationCity;
                    bool direccao          = appt.Direccao.ToString() == "Directo" ? true : false;
                    

                    try
                    {
                        token = await AmadeuService.GetAmadeusAcessToken();

                        if (token != "")
                        {
                            Activity typing = (Activity)context.MakeMessage();
                            typing.Type = ActivityTypes.Typing;
                            await context.PostAsync(typing);

                            #region Get cidade origem

                            //traduzir a cidade de origem para EN
                            string OriginCityIataCode = await AmadeuService.GetAirportIataCodeAsync(appt.OriginCity, token);

                            //se nao encontrar cidade de origem sair e avisar e ir para outro fluxo
                            if (OriginCityIataCode == "")
                            {
                                await context.PostAsync($"Nao encontrei a cidade de origem.Por favor verifique se escreveu mal o nome da cidade.");
                                return;
                            }

                            #endregion

                            Activity typing1 = (Activity)context.MakeMessage();
                            typing1.Type = ActivityTypes.Typing;
                            await context.PostAsync(typing1);

                            #region Get cidade Destino

                            //traduzir a cidade de destino para EN
                            string DestinationCityIataCode = await AmadeuService.GetAirportIataCodeAsync(appt.DestinationCity, token);


                            //se nao encontrar cidade de origem sair e avisar e ir para outro fluxo
                            if (DestinationCityIataCode == "")
                            {
                                await context.PostAsync($"Nao encontrei a cidade de destino.Por favor verifique se escreveu mal o nome da cidade.");
                                return;
                            }

                            #endregion

                            await context.PostAsync($"Ok aguarde um pouco.....Apresentarei apenas os 5 preços mais baixos.Caso nao seja possível apresentar lhe os resultados agora enviaremos um email.");

                            Activity typingWait = (Activity)context.MakeMessage();
                            typingWait.Type = ActivityTypes.Typing;
                            await context.PostAsync(typingWait);

                            var search = AmadeuService.PesquisaDataVooBarato(OriginCityIataCode, DestinationCityIataCode, token, direccao);

                            if (search.Result != null)
                            {
                                //foreach (var item in search.Result.data)
                                //{
                                //    //foreach (var offerItem in item.offerItems)
                                //    //{
                                //    //    card = new AdaptiveCard
                                //    //    {
                                //    //        Speak = "Reservar"
                                //    //    };

                                //    //    foreach (var itemService in offerItem.services)
                                //    //    {
                                //    //        foreach (var seg in itemService.segments)
                                //    //        {
                                //    //            ///segmento ida
                                //    //            if (isFirtsSegment)
                                //    //            {
                                //    //                if (itemService.segments.Count == 1)
                                //    //                    isFirtsSegment = false;

                                //    //                // opcoes de ida 
                                //    //                card.Body.Add(new TextBlock()
                                //    //                {
                                //    //                    Text = "Opçao" + segment + ": Ida",
                                //    //                    Size = TextSize.Medium,
                                //    //                    Weight = TextWeight.Bolder,
                                //    //                    IsSubtle = false
                                //    //                });

                                //    //                ///Data partida
                                //    //                card.Body.Add(new TextBlock()
                                //    //                {
                                //    //                    Text = "Data Partida: " + seg.flightSegment.departure.at,
                                //    //                    Size = TextSize.Medium,
                                //    //                    Weight = TextWeight.Lighter,
                                //    //                    Separation = SeparationStyle.None
                                //    //                });

                                //    //                //Terminal
                                //    //                card.Body.Add(new TextBlock()
                                //    //                {
                                //    //                    Text = "Terminal: " + seg.flightSegment.departure.terminal,
                                //    //                    Size = TextSize.Medium,
                                //    //                    Weight = TextWeight.Lighter,
                                //    //                    Separation = SeparationStyle.None
                                //    //                });

                                //    //                ColumnSet colset = new ColumnSet
                                //    //                {
                                //    //                    Separation = SeparationStyle.Default
                                //    //                };

                                //    //                //// coluna 1
                                //    //                Column col = new Column
                                //    //                {
                                //    //                    Size = "1"
                                //    //                };
                                //    //                col.Items.Add(new TextBlock()
                                //    //                {
                                //    //                    Text = originCity,
                                //    //                    IsSubtle = true

                                //    //                });
                                //    //                col.Items.Add(new TextBlock()
                                //    //                {
                                //    //                    Text = seg.flightSegment.departure.iataCode,
                                //    //                    Size = TextSize.ExtraLarge,
                                //    //                    Color = TextColor.Accent,
                                //    //                    Separation = SeparationStyle.None
                                //    //                });

                                //    //                colset.Columns.Add(col);

                                //    //                //// coluna 2 iamgem
                                //    //                Column col1 = new Column
                                //    //                {
                                //    //                    Size = "auto"
                                //    //                };
                                //    //                col1.Items.Add(new TextBlock()
                                //    //                {
                                //    //                    Text = " "
                                //    //                });
                                //    //                col1.Items.Add(new Image()
                                //    //                {
                                //    //                    Url = "http://adaptivecards.io/content/airplane.png",
                                //    //                    Size = ImageSize.Small,
                                //    //                    Separation = SeparationStyle.None

                                //    //                });

                                //    //                colset.Columns.Add(col1);

                                //    //                //// coluna 3
                                //    //                Column col2 = new Column
                                //    //                {
                                //    //                    Size = "1"
                                //    //                };
                                //    //                col2.Items.Add(new TextBlock()
                                //    //                {
                                //    //                    HorizontalAlignment = HorizontalAlignment.Right,
                                //    //                    Text = destinationCity,
                                //    //                    IsSubtle = true

                                //    //                });
                                //    //                col2.Items.Add(new TextBlock()
                                //    //                {
                                //    //                    HorizontalAlignment = HorizontalAlignment.Right,
                                //    //                    Text = seg.flightSegment.arrival.iataCode,
                                //    //                    Size = TextSize.ExtraLarge,
                                //    //                    Color = TextColor.Accent,
                                //    //                    Separation = SeparationStyle.None
                                //    //                });

                                //    //                colset.Columns.Add(col2);

                                //    //                card.Body.Add(colset);

                                //    //                segment++;
                                //    //            }

                                //    //            #region else
                                //    //            //else
                                //    //            //{
                                //    //            //    //Data regresso
                                //    //            //    card.Body.Add(new TextBlock()
                                //    //            //    {
                                //    //            //        Text = "Data Regresso: " + seg.flightSegment.departure.at,
                                //    //            //        Size = TextSize.Medium,
                                //    //            //        Weight = TextWeight.Lighter,
                                //    //            //        Separation = SeparationStyle.None
                                //    //            //    });

                                //    //            //    //Terminal
                                //    //            //    card.Body.Add(new TextBlock()
                                //    //            //    {
                                //    //            //        Text = "Terminal: " + seg.flightSegment.departure.terminal,
                                //    //            //        Size = TextSize.Medium,
                                //    //            //        Weight = TextWeight.Lighter,
                                //    //            //        Separation = SeparationStyle.None
                                //    //            //    });

                                //    //            //    ColumnSet colset = new ColumnSet();
                                //    //            //    colset.Separation = SeparationStyle.Default;

                                //    //            //    //// coluna 1
                                //    //            //    Column col = new Column();
                                //    //            //    col.Size = "1";
                                //    //            //    col.Items.Add(new TextBlock()
                                //    //            //    {
                                //    //            //        Text = destinationCity,
                                //    //            //        IsSubtle = true

                                //    //            //    });
                                //    //            //    col.Items.Add(new TextBlock()
                                //    //            //    {
                                //    //            //        Text = seg.flightSegment.departure.iataCode,
                                //    //            //        Size = TextSize.ExtraLarge,
                                //    //            //        Color = TextColor.Accent,
                                //    //            //        Separation = SeparationStyle.None
                                //    //            //    });

                                //    //            //    colset.Columns.Add(col);

                                //    //            //    //// coluna 2 iamgem
                                //    //            //    Column col1 = new Column();
                                //    //            //    col1.Size = "auto";
                                //    //            //    col1.Items.Add(new TextBlock()
                                //    //            //    {
                                //    //            //        Text = " "
                                //    //            //    });
                                //    //            //    col1.Items.Add(new Image()
                                //    //            //    {
                                //    //            //        Url = "http://adaptivecards.io/content/airplane.png",
                                //    //            //        Size = ImageSize.Small,
                                //    //            //        Separation = SeparationStyle.None

                                //    //            //    });

                                //    //            //    colset.Columns.Add(col1);

                                //    //            //    //// coluna 3
                                //    //            //    Column col2 = new Column();
                                //    //            //    col2.Size = "1";
                                //    //            //    col2.Items.Add(new TextBlock()
                                //    //            //    {
                                //    //            //        HorizontalAlignment = HorizontalAlignment.Right,
                                //    //            //        Text = originCity,
                                //    //            //        IsSubtle = true

                                //    //            //    });
                                //    //            //    col2.Items.Add(new TextBlock()
                                //    //            //    {
                                //    //            //        HorizontalAlignment = HorizontalAlignment.Right,
                                //    //            //        Text = seg.flightSegment.arrival.iataCode,
                                //    //            //        Size = TextSize.ExtraLarge,
                                //    //            //        Color = TextColor.Accent,
                                //    //            //        Separation = SeparationStyle.None
                                //    //            //    });

                                //    //            //    colset.Columns.Add(col2);

                                //    //            //    card.Body.Add(colset);

                                //    //            //}

                                //    //            #endregion
                                //    //        }
                                //    //    }

                                //    //    attachment = new Attachment()
                                //    //    {
                                //    //        ContentType = AdaptiveCard.ContentType,
                                //    //        Content = card
                                //    //    };
                                //    //    reply.Attachments.Add(attachment);
                                //    //}
                                //}
                            }

                            await context.PostAsync(reply);
                            context.Wait(MessageReceived);
                        }
                        else
                            await context.PostAsync($"De momento não é possível satisfazer o seu pedido.");

                    }
                    catch (Exception)
                    {
                        throw new ExceptionIntencao("Ocorreu um erro na intenção de pesquisa de datas voo barato para ClickViaja - Estrada Luz e cliente " + context.Activity.From.Name + " no canal " + context.Activity.ChannelId);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("Pedido abortado!");
                return;
            }
        }

        private async Task PesquisaDataVooBarato(string originCity, string destinationCity, IDialogContext context)
        {
            string token = string.Empty;

            try
            {
                var reply              = context.MakeMessage();
                reply.Type             = "message";
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                reply.Attachments      = new List<Attachment>();
                //AdaptiveCard card = null;
                //Attachment attachment = null;

                token = await AmadeuService.GetAmadeusAcessToken();

                Activity typing = (Activity)context.MakeMessage();
                typing.Type = ActivityTypes.Typing;
                await context.PostAsync(typing);


                //traduzir a cidade de origem para EN
                string OriginCityIataCode = await AmadeuService.GetAirportIataCodeAsync(originCity, token);

                //se nao encontrar cidade de origem sair e avisar e ir para outro fluxo
                if (OriginCityIataCode == "")
                {
                    await context.PostAsync($"Nao encontrei a cidade de origem.Por favor verifique se escreveu mal o nome da cidade.");
                    return;
                }

                Activity typing1 = (Activity)context.MakeMessage();
                typing1.Type     = ActivityTypes.Typing;
                await context.PostAsync(typing1);


                //traduzir a cidade de destino para EN
                string DestinationCityIataCode = await AmadeuService.GetAirportIataCodeAsync(destinationCity, token);

                //se nao encontrar cidade de origem sair e avisar e ir para outro fluxo
                if (DestinationCityIataCode == "")
                {
                    await context.PostAsync($"Nao encontrei a cidade de destino.Por favor verifique se escreveu mal o nome da cidade.");
                    return;
                }


                await context.PostAsync($"Ok aguarde um pouco.....Apresentarei apenas os 5 preços mais baixos.Caso nao seja possível apresentar lhe os resultados agora enviaremos um email.");

                Activity typingWait = (Activity)context.MakeMessage();
                typingWait.Type = ActivityTypes.Typing;
                await context.PostAsync(typingWait);

                var search = AmadeuService.PesquisaDataVooBarato(OriginCityIataCode, DestinationCityIataCode, token,true);

                //if (search.Result != null)
                //{
                //    foreach (var item in search.Result.data)
                //    {
                //        foreach (var offerItem in item.offerItems)
                //        {
                //            card = new AdaptiveCard();
                //            card.Speak = "Reservar";

                //            foreach (var itemService in offerItem.services)
                //            {
                //                foreach (var seg in itemService.segments)
                //                {
                //                    ///segmento ida
                //                    if (isFirtsSegment)
                //                    {
                //                        if (itemService.segments.Count == 1)
                //                            isFirtsSegment = false;

                //                        // opcoes de ida 
                //                        card.Body.Add(new TextBlock()
                //                        {
                //                            Text = "Opçao" + segment + ": Ida",
                //                            Size = TextSize.Medium,
                //                            Weight = TextWeight.Bolder,
                //                            IsSubtle = false
                //                        });

                //                        ///Data partida
                //                        card.Body.Add(new TextBlock()
                //                        {
                //                            Text = "Data Partida: " + seg.flightSegment.departure.at,
                //                            Size = TextSize.Medium,
                //                            Weight = TextWeight.Lighter,
                //                            Separation = SeparationStyle.None
                //                        });

                //                        //Terminal
                //                        card.Body.Add(new TextBlock()
                //                        {
                //                            Text = "Terminal: " + seg.flightSegment.departure.terminal,
                //                            Size = TextSize.Medium,
                //                            Weight = TextWeight.Lighter,
                //                            Separation = SeparationStyle.None
                //                        });

                //                        ColumnSet colset = new ColumnSet();
                //                        colset.Separation = SeparationStyle.Default;

                //                        //// coluna 1
                //                        Column col = new Column();
                //                        col.Size = "1";
                //                        col.Items.Add(new TextBlock()
                //                        {
                //                            Text = originCity,
                //                            IsSubtle = true

                //                        });
                //                        col.Items.Add(new TextBlock()
                //                        {
                //                            Text = seg.flightSegment.departure.iataCode,
                //                            Size = TextSize.ExtraLarge,
                //                            Color = TextColor.Accent,
                //                            Separation = SeparationStyle.None
                //                        });

                //                        colset.Columns.Add(col);

                //                        //// coluna 2 iamgem
                //                        Column col1 = new Column();
                //                        col1.Size = "auto";
                //                        col1.Items.Add(new TextBlock()
                //                        {
                //                            Text = " "
                //                        });
                //                        col1.Items.Add(new Image()
                //                        {
                //                            Url = "http://adaptivecards.io/content/airplane.png",
                //                            Size = ImageSize.Small,
                //                            Separation = SeparationStyle.None

                //                        });

                //                        colset.Columns.Add(col1);

                //                        //// coluna 3
                //                        Column col2 = new Column();
                //                        col2.Size = "1";
                //                        col2.Items.Add(new TextBlock()
                //                        {
                //                            HorizontalAlignment = HorizontalAlignment.Right,
                //                            Text = destinationCity,
                //                            IsSubtle = true

                //                        });
                //                        col2.Items.Add(new TextBlock()
                //                        {
                //                            HorizontalAlignment = HorizontalAlignment.Right,
                //                            Text = seg.flightSegment.arrival.iataCode,
                //                            Size = TextSize.ExtraLarge,
                //                            Color = TextColor.Accent,
                //                            Separation = SeparationStyle.None
                //                        });

                //                        colset.Columns.Add(col2);

                //                        card.Body.Add(colset);

                //                        segment++;
                //                    }

                //                    #region else
                //                    //else
                //                    //{
                //                    //    //Data regresso
                //                    //    card.Body.Add(new TextBlock()
                //                    //    {
                //                    //        Text = "Data Regresso: " + seg.flightSegment.departure.at,
                //                    //        Size = TextSize.Medium,
                //                    //        Weight = TextWeight.Lighter,
                //                    //        Separation = SeparationStyle.None
                //                    //    });

                //                    //    //Terminal
                //                    //    card.Body.Add(new TextBlock()
                //                    //    {
                //                    //        Text = "Terminal: " + seg.flightSegment.departure.terminal,
                //                    //        Size = TextSize.Medium,
                //                    //        Weight = TextWeight.Lighter,
                //                    //        Separation = SeparationStyle.None
                //                    //    });

                //                    //    ColumnSet colset = new ColumnSet();
                //                    //    colset.Separation = SeparationStyle.Default;

                //                    //    //// coluna 1
                //                    //    Column col = new Column();
                //                    //    col.Size = "1";
                //                    //    col.Items.Add(new TextBlock()
                //                    //    {
                //                    //        Text = destinationCity,
                //                    //        IsSubtle = true

                //                    //    });
                //                    //    col.Items.Add(new TextBlock()
                //                    //    {
                //                    //        Text = seg.flightSegment.departure.iataCode,
                //                    //        Size = TextSize.ExtraLarge,
                //                    //        Color = TextColor.Accent,
                //                    //        Separation = SeparationStyle.None
                //                    //    });

                //                    //    colset.Columns.Add(col);

                //                    //    //// coluna 2 iamgem
                //                    //    Column col1 = new Column();
                //                    //    col1.Size = "auto";
                //                    //    col1.Items.Add(new TextBlock()
                //                    //    {
                //                    //        Text = " "
                //                    //    });
                //                    //    col1.Items.Add(new Image()
                //                    //    {
                //                    //        Url = "http://adaptivecards.io/content/airplane.png",
                //                    //        Size = ImageSize.Small,
                //                    //        Separation = SeparationStyle.None

                //                    //    });

                //                    //    colset.Columns.Add(col1);

                //                    //    //// coluna 3
                //                    //    Column col2 = new Column();
                //                    //    col2.Size = "1";
                //                    //    col2.Items.Add(new TextBlock()
                //                    //    {
                //                    //        HorizontalAlignment = HorizontalAlignment.Right,
                //                    //        Text = originCity,
                //                    //        IsSubtle = true

                //                    //    });
                //                    //    col2.Items.Add(new TextBlock()
                //                    //    {
                //                    //        HorizontalAlignment = HorizontalAlignment.Right,
                //                    //        Text = seg.flightSegment.arrival.iataCode,
                //                    //        Size = TextSize.ExtraLarge,
                //                    //        Color = TextColor.Accent,
                //                    //        Separation = SeparationStyle.None
                //                    //    });

                //                    //    colset.Columns.Add(col2);

                //                    //    card.Body.Add(colset);

                //                    //}

                //                    #endregion
                //                }
                //            }

                //            attachment = new Attachment()
                //            {
                //                ContentType = AdaptiveCard.ContentType,
                //                Content = card
                //            };
                //            reply.Attachments.Add(attachment);
                //        }
                //    }
                //}

                await context.PostAsync(reply);
                context.Wait(MessageReceived);
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("Pedido abortado!");
                return;
            }
        }

        #endregion

        #region pedido, consulta de orçamentos

        [LuisIntent("Orcamento")]
        public async Task Orcamento(IDialogContext context, LuisResult result)
        {
            Activity typing = (Activity)context.MakeMessage();
            typing.Type = ActivityTypes.Typing;
            await context.PostAsync(typing);

            var form = new OrcamentoDialog();

            



            var pedidoorcamento = new FormDialog<OrcamentoDialog>(form, OrcamentoDialog.BuildPedidoOrcamentoForm, FormOptions.PromptInStart);
            context.Call(pedidoorcamento, PedidoOrcamentoFormComplete);

        }

        private async Task PedidoOrcamentoFormComplete(IDialogContext context, IAwaitable<OrcamentoDialog> result)
        {
            OrcamentoDialog appt = null;

            try
            {
                appt = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("Pedido abortado!");
                return;
            }
            try
            {
                if (appt != null)
                {
                    StringBuilder message = new StringBuilder().Append("Nome: " + appt.Nome).AppendLine()
                                                               .Append("Email: " + appt.Email).AppendLine()
                                                               .Append("Contacto: " + appt.telemovel).AppendLine()
                                                               .Append("Pretendo um orçamento com as seguintes características:").AppendLine()
                                                               .Append(".Destino: " + appt.Destino).AppendLine()
                                                               .Append(".Finalidade: " + appt.FinalidadeViagemRequired.ToString()).AppendLine()
                                                               .Append(".Tipo de orçamento: " + appt.TipoOrcamentoRequired.ToString()).AppendLine()
                                                               .Append(".Observações: " + appt.OutrasInf).AppendLine();

                    Activity typing = (Activity)context.MakeMessage();
                    typing.Type = ActivityTypes.Typing;
                    await context.PostAsync(typing);

                    Util.Util.SendEmail(message.ToString(), "Pedido de Orçamento para " + appt.Destino, "lisboa.estradaluz@clickviaja.com", null);

                    await context.PostAsync("O seu pedido de orçamento foi registado, em breve um dos nossos consultores 👨‍🏫 entrarão em contacto consigo.Muito Obrigado 🙏");

                    BsonDocument msg = new BsonDocument {
                                                                    { "Message" , "O seu pedido de orçamento foi registado, em breve um dos nossos consultores entrarão em contacto consigo.Muito Obrigado" },
                                                                    { "Data" , DateTime.Now },
                                                                    { "Assunto" , "Pedir_Orcmento" },
                                                                    { "sentimento" , "" },
                                                                    { "Canal" , context.Activity.ChannelId },
                                                                    { "cliente" , context.Activity.From.Name},
                                                                    { "Idcliente" , context.Activity.From.Id},
                                                                    { "Agencia" , "ClickViaja - Estrada Luz"}
                                                                  };

                    await ClickViajaDal.InsertDocument(msg);

                    await DespedidaAsync(context);
                    context.Wait(MessageReceived);
                }
            }
            catch (Exception)
            {
                throw new ExceptionIntencao("Ocorreu um erro na intenção de pedido de orçamento para ClickViaja - Estrada Luz e cliente " + context.Activity.From.Name + " no canal " + context.Activity.ChannelId);

            }
        }

        #endregion

        /// <summary>
        /// Todo - falta reencaminha quando for despedida
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task DespedidaAsync(IDialogContext context)
        {
            var activity  = context.MakeMessage();
            activity.Text = "Será que respondi a sua pergunta ? A minha resposta foi útil?Posso ajudá-lo em mais alguma coisa?";

            activity.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){ Title = "Sim foi útil", Type=ActionTypes.PostBack, Value="Despedida", Image="https://t4.ftcdn.net/jpg/00/30/84/05/240_F_30840579_2Z0YB8J9cPdp4pvQ0iShdBIQxAtHgkrY.jpg" },
                    new CardAction(){ Title = "Ainda tenho dúvidas", Type=ActionTypes.ImBack, Value="Sim preciso de ajuda" , Image="http://www.todaletra.com.br/wp-content/uploads/2012/10/duvidas-300x3001.jpg" }
                }
            };

            await context.PostAsync(activity);

        }
    }
}