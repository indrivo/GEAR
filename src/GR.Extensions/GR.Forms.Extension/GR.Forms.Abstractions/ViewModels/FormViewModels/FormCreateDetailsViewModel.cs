using System;

namespace GR.Forms.Abstractions.ViewModels.FormViewModels
{
    public class FormCreateDetailsViewModel : Core.BaseModel
    {
        /// <summary>
        /// Form name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Form description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Url to redirect after submit form
        /// </summary>
        public string RedirectUrl { get; set; }
        /// <summary>
        /// Url to call on submit form
        /// </summary>
        public string PostUrl { get; set; }
        /// <summary>
        /// Form data deserialized from json
        /// </summary>
        public FormViewModel Model { get; set; }
        /// <summary>
        /// Entity reference
        /// </summary>
        public Guid TableId { get; set; }
        /// <summary>
        /// Form Type reference
        /// </summary>
        public Guid FormTypeId { get; set; }
        public bool EditMode { get; set; }
    }
}
