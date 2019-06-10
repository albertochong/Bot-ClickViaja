using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClickViajaBot.Exceptions
{
    public class ExceptionIntencao : Exception
    {
        public ExceptionIntencao(string message) : base(message)
        {
        }
    }
}