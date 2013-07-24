
namespace Server.Models.View
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using Server.Models;

    public class DataFormModel
    {
        public DataFormModel() { }

        public DataFormModel(DataObject dataObject)
        {
            this.Id = dataObject.Id;
            this.Title = dataObject.Title;
            this.Description = dataObject.Description;
            this.IsPublic = dataObject.IsPublic;
            this.Data = dataObject.Data;
            
            // Add the data tags
            if (dataObject.DataObjectTags.Count != 0) {
                foreach (var tag in dataObject.DataObjectTags.ToList()) {
                    this.Tags += tag.Value + ",";
                }
                this.Tags = this.Tags.Remove(this.Tags.Length - 1);
            }
        }

        public DataObject ToDataObject()
        {
            var dataObject = new DataObject()
            {
                Id = this.Id,
                Created = DateTime.UtcNow,
                Description = this.Description,
                IsPublic = this.IsPublic,
                Title = this.Title,
                Data = Regex.Replace(this.Data, @"\s+", string.Empty)
            };

            if (!string.IsNullOrWhiteSpace(this.Tags))
            {
                var tags = this.Tags.ToLowerInvariant().Trim().Split(new string[] { " ", "," },StringSplitOptions.RemoveEmptyEntries);
                foreach (var tag in tags)
                {
                    dataObject.DataObjectTags.Add(new DataObjectTag() { Value = tag });
                }
            }

            return dataObject;
        }
        
        public long Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength=1)]
        public string Title { get; set; }

        [AllowHtml]
        [StringLength(4000)]
        public string Description { get; set; }

        [Required]
        public bool IsPublic { get; set; }

        [Required]
        public string Data { get; set; }

        [StringLength(150)]
        public string Tags { get; set; }
    }
}