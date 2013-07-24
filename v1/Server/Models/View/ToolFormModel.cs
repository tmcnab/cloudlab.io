
namespace Server.Models.View
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using Server.Models;

    public class ToolFormModel
    {
        public ToolFormModel() { }

        public ToolFormModel(ToolObject toolObject)
        {
            this.Id = toolObject.Id;
            this.Title = toolObject.Title;
            this.Description = toolObject.Description;
            this.IsPublic = toolObject.IsPublic;
            this.Tool = toolObject.Tool;

            // Add the data tags
            if (toolObject.ToolObjectTags.Count != 0)
            {
                foreach (var tag in toolObject.ToolObjectTags.ToList())
                {
                    this.Tags += tag.Value + ",";
                }
                this.Tags = this.Tags.Remove(this.Tags.Length - 1);
            }
        }

        public ToolObject ToToolObject()
        {
            var toolObject = new ToolObject()
            {
                Id = this.Id,
                Created = DateTime.UtcNow,
                Description = this.Description,
                IsPublic = this.IsPublic,
                Title = this.Title,
                Tool = Yahoo.Yui.Compressor.JavaScriptCompressor.Compress(this.Tool, false, false, true, true, 0)
            };

            if (!string.IsNullOrWhiteSpace(this.Tags))
            {
                var tags = this.Tags.ToLowerInvariant().Trim().Split(new string[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var tag in tags)
                {
                    toolObject.ToolObjectTags.Add(new ToolObjectTag() { Value = tag });
                }
            }

            return toolObject;
        }

        public long Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Title { get; set; }

        [AllowHtml]
        [StringLength(4000)]
        public string Description { get; set; }

        [Required]
        public bool IsPublic { get; set; }

        [Required]
        public string Tool { get; set; }

        [StringLength(150)]
        public string Tags { get; set; }
    }
}