using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Core;
using GR.Entities.Abstractions.Models.Tables;

namespace GR.PageRender.Abstractions.Models.ViewModels
{
    public class ViewModel : BaseModel
    {
        /// <summary>
        /// Reference to entity
        /// </summary>
        public virtual TableModel TableModel { get; set; }
        public virtual Guid TableModelId { get; set; }
        /// <summary>
        /// View model name
        /// </summary>
        [Required]
        public virtual string Name { get; set; }
        /// <summary>
        /// One to many for View model and view model fields
        /// </summary>
        public virtual IEnumerable<ViewModelFields> ViewModelFields { get; set; }
    }

    public class ViewModelFields : BaseModel
    {
        /// <summary>
        /// Field name
        /// </summary>
        [Required]
        public virtual string Name { get; set; }
        /// <summary>
        /// Reference to view model
        /// </summary>
        public virtual ViewModel ViewModel { get; set; }
        public virtual Guid ViewModelId { get; set; }
        /// <summary>
        /// Reference to table model field
        /// </summary>
        public virtual TableModelField TableModelFields { get; set; }
        public virtual Guid? TableModelFieldsId { get; set; }
        /// <summary>
        /// Translate key
        /// </summary>
        public virtual string Translate { get; set; }
        /// <summary>
        /// Template to render in ui
        /// </summary>
        public virtual string Template { get; set; }
        /// <summary>
        /// Number on list
        /// </summary>
        public virtual int Order { get; set; }

        /// <summary>
        /// Represent mode to get data
        /// </summary>
        [Required]
        public ViewModelVirtualDataType VirtualDataType { get; set; } = ViewModelVirtualDataType.SelfReference;

        /// <summary>
        /// View model field configurations
        /// </summary>
        public virtual IEnumerable<ViewModelFieldConfiguration> Configurations { get; set; }
    }

    public class ViewModelFieldCode
    {
        /// <summary>
        /// View model config code
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// Name of code
        /// </summary>
        [Required]
        public string Name { get; set; }
    }

    public class ViewModelFieldConfiguration
    {
        /// <summary>
        /// View model field reference
        /// </summary>
        public ViewModelFields ViewModelField { get; set; }
        public Guid ViewModelFieldId { get; set; }
        /// <summary>
        /// Reference to field code
        /// </summary>
        [Required]
        public ViewModelFieldCode ViewModelFieldCode { get; set; }
        public int ViewModelFieldCodeId { get; set; }

        /// <summary>
        /// Represent data
        /// </summary>
        [Required]
        public string Value { get; set; }
    }

    public enum ViewModelVirtualDataType
    {
        SelfReference, OneToOne, OneToMany, ManyToMany
    }
}
