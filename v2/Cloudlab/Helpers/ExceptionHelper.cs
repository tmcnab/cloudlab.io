using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cloudlab.Helpers
{
    public static class ExceptionHelper
    {
        public static Exception SendToACP(this Exception exception)
        {
            Cloudlab.Controllers.AdministrationController.Exceptions.Add(exception);
            return exception;
        }
    }
}