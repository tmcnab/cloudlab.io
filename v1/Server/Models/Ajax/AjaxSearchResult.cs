
namespace Server.Models.Ajax
{
    using System.Collections.Generic;

    /// <summary>
    /// Data structure used in search result serialization.
    /// </summary>
    /// <remarks>
    /// Properties are in lowercase because they're made lowercase once
    /// serialized into JSON. It is guaranteed that either dataid or toolid
    /// will be nonzero for any given instance.
    /// </remarks>
    public class AjaxSearchResult
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the AjaxSearchResult class using 
        /// a DataObject entity as the source.
        /// </summary>
        /// <param name="dataObject">The object to fill-out the instance with</param>
        public AjaxSearchResult(DataObject dataObject)
        {
            this.title = dataObject.Title;
            this.dataid = dataObject.Id;
            this.tags = new List<string>();
            foreach (var tag in dataObject.DataObjectTags)
            {
                this.tags.Add(tag.Value);
            }
        }

        public AjaxSearchResult(ToolObject toolObject)
        {
            this.title = toolObject.Title;
            this.toolid = toolObject.Id;
            this.tags = new List<string>();
            foreach (var tag in toolObject.ToolObjectTags) {
                this.tags.Add(tag.Value);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the title of the item
        /// </summary>
        public string title { get; protected set; }

        /// <summary>
        /// Gets or sets the id of the item (if it's a data object)
        /// </summary>
        public long dataid { get; protected set; }

        /// <summary>
        /// Gets or sets the id of the item (if it's a tool object)
        /// </summary>
        public long toolid { get; protected set; }

        /// <summary>
        /// Gets or sets a list of tags that can be used to refine searches
        /// </summary>
        public List<string> tags { get; protected set; }

        #endregion
    }
}