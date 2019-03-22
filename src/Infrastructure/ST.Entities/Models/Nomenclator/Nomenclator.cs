using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ST.Audit.Models;

namespace ST.Entities.Models.Nomenclator
{
  
    public class NomenclatorViewModel : NomenclatorItem
    {
        public NomenclatorViewModel[] SubItems { get; set; }
    }

    public class Nomenclator : ExtendedModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string MachineName { get; set; }

        public string Description { get; set; }
    }
    public class NomenclatorItem : ExtendedModel
    {
        [Required]
        public string Name { get; set; }
        public Guid NomenclatorId { get; set; }
        public Guid?  ParentId { get; set; }
        public Guid? DependencyId { get; set; } // ref to other nomenc. Used for view in one dropbox elements depending from result in other dropbox 
        public Guid? RefId { get; set; } // ref to other nomenc. used for show value from other nomenc. 
    }

    public class NomKPICategory : ExtendedModel 
    {
        [Required]
        public string Name { get; set; }
        public Guid NomenclatorId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? DependencyId { get; set; } // ref to other nomenc. Used for view in one dropbox elements depending from result in other dropbox 
        public Guid? RefId { get; set; } // ref to other nomenc. used for show value from other nomenc. 
    }

    public class NomMeasurement : NomenclatorItem
    {}
    public class NomType : NomenclatorItem
    { }
    public class NomPeriod : NomenclatorItem
    { }
    public class NomFulfillment : NomenclatorItem
    { }
    public class NomGoal : NomenclatorItem
    { }
    //public class NomRequirement : NomenclatorItem
    //{ }
    public class NomInterestedPartyType : NomenclatorItem
    { }
    public class NomInterestedParty : NomenclatorItem
    { }
    public class NomRequirement : NomenclatorItem
    { }


}
