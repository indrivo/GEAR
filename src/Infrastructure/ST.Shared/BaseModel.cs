using System;

namespace ST.Core
{
    /// <summary>
    /// Base Proprieties for every Entity. Every model that inherits from Base Model can be manipulated with CRUD operations form our generic repository without additional requirements.
    /// @date 2017/05/19
    /// </summary>
    public abstract class BaseModel
    {
        /// <summary>
        /// Constructor. Initialize object with default values. A unique Id, Creation time and set IsDeleted to false
        /// </summary>
        protected BaseModel()
        {
            Id = Guid.NewGuid();
            Created = DateTime.UtcNow;
            Changed = DateTime.UtcNow;
        }

        /// <summary>Stores Id of the Object</summary>
        public Guid Id { get; set; }

        /// <summary>Stores Id of the User that created the object</summary>
        public virtual string Author { get; set; }

        /// <summary>Stores the time when object was created</summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Stores the Id of the User that modified the object. Nullable
        /// </summary>
        public virtual string ModifiedBy { get; set; }

        /// <summary>Stores the time when object was modified. Nullable</summary>
        public DateTime Changed { get; set; }

        /// <summary>
        /// Stores state of the Object. True if object is deleted and false otherwise
        /// </summary>
        public virtual bool IsDeleted { get; set; }

        /// <summary>
        /// Version of data
        /// </summary>
        public virtual int Version { get; set; }

        /// <summary>
        /// Tenant id
        /// </summary>
        public virtual Guid? TenantId { get; set; }
    }
}
