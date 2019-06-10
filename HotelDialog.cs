using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ClickViajaBot.Dialogs
{
    [Serializable]
    public class HotelDialog
    {
        [Prompt("Ótimo!Qual é o nome do hotel e em que cidade ou país?")]
        public string nomeHotel
        {
            get; set;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(nomeHotel);
            return builder.ToString();
        }
    }
}