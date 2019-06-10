using Microsoft.Bot.Connector;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace ClickViajaBot.Util
{
    public class Util
    {
        private static string azureBlobConnection = ConfigurationManager.AppSettings["AzureBlobConnection"];

        public Microsoft.Bot.Connector.Attachment GetHeroCard()
        {
            var heroCard = new HeroCard()
            {
                Title = "Rivelino",
                Subtitle = "Roberto Rivelino",
                Images = new List<CardImage>
                {
                    new CardImage("https://www.imortaisdofutebol.com/wp-content/uploads/2012/09/roberto-rivelino2.jpg", "Roberto Rivelino")
                },
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.OpenUrl, "Wikipedia", null, "https://pt.wikipedia.org/wiki/Roberto_Rivellino")
                }
            };

            return heroCard.ToAttachment();
        }

        public static void SendEmail(string message, string subject, string emailTo,List<System.Net.Mail.Attachment> invoiceList)
        {
            string msg = "Agência ClickViaja - Estrada Luz :" + message;

            if (emailTo == "")
                emailTo = "alberto.chong @clickviaja.com";

            SmtpClient client = new SmtpClient
            {
                Port = 587,
                Host = "mail.clickviaja.com",
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential("alberto.chong@clickviaja.com", "AbNg1452")
            };

            MailMessage mm = new MailMessage("alberto.chong@clickviaja.com", emailTo, "Mensagem bot Agência Estrada Luz " + subject, msg)
            {
                BodyEncoding = UTF8Encoding.UTF8
            };

            if (invoiceList != null)
            {
                foreach (var item in invoiceList)
                {
                    mm.Attachments.Add(item);
                }
            }

            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);
        }

        //public System.Net.Mail.Attachment UploadFacturaToAzure(byte[] imagedata, string fileName)
        //{
        //    CloudStorageAccount account = CloudStorageAccount.Parse(azureBlobConnection);
        //    CloudFileClient client      = account.CreateCloudFileClient();

        //    foreach (var item in client.ListShares())
        //    {
        //        var root = item.GetRootDirectoryReference();

        //        CloudFileDirectory dir = root.GetDirectoryReference("FACTURAS_DOWNLOAD");
        //        var list               = dir.ListFilesAndDirectories();

        //        foreach (CloudFile file in dir.ListFilesAndDirectories())
        //        {
        //            byte[] fich = new byte[file.StreamWriteSizeInBytes];
        //            file.UploadFromByteArray(imagedata, 0, imagedata.Length);

        //            Attachment att = new System.Net.Mail.Attachment()





        //            file.Delete();

        //        }
        //    }

        //}
    }
}