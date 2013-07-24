
namespace Server.Models.Ajax
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// The structure used to send information back to the jquery
    /// terminal in the view.
    /// </summary>
    public class AjaxTerminalResponse
    {
        /// <summary>
        /// Gets or sets the action to evaluate on the client side
        /// </summary>
        /// <remarks>
        /// Must contain valid javascript
        /// </remarks>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the error message to display in the 
        /// terminal (if there is one)
        /// </summary>
        public string Error { get; set; }
    }
}