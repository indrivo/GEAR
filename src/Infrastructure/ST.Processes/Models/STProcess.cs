using ST.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ST.Procesess.Models
{
    public class STProcess : ExtendedModel
    {
        /// <summary>
        /// Name of process
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Reference to schema
        /// </summary>
        public STProcessSchema ProcessSchema { get; set; }
        public Guid ProcessSchemaId { get; set; }

        /// <summary>
        /// Current version of process
        /// </summary>
        [DefaultValue(1)]
        [Required]
        public int Version { get; set; }

        /// <summary>
        /// Initial Process with version 1
        /// </summary>
        public STProcess IntitialProcess { get; set; }
        public Guid? IntitialProcessId { get; set; }

        /// <summary>
        /// Started instances of this process
        /// </summary>
        public IEnumerable<STProcessInstance> ProcessInstances { get; set; }

        /// <summary>
        /// Defined process transitions
        /// </summary>
        public IList<STProcessTransition> ProcessTransitions { get; set; }
        /// <summary>
        /// Store author settings of process
        /// </summary>
        public string ProcessSettings { get; set; }
    }
}
