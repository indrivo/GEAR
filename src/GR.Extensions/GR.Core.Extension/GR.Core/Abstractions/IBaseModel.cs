using System;

namespace GR.Core.Abstractions
{
    /// <summary>
    /// Base Proprieties for every Entity. Every model that inherits from Base Model can be manipulated with CRUD operations form our generic repository without additional requirements.
    /// @date 2017/05/19
    /// </summary>
    public interface IBaseModel
    {
        /// <summary>Stores Id of the User that created the object</summary>
        string Author { get; set; }

        /// <summary>Stores the time when object was created</summary>
        DateTime Created { get; set; }

        /// <summary>
        /// Stores the Id of the User that modified the object. Nullable
        /// </summary>
        string ModifiedBy { get; set; }

        /// <summary>Stores the time when object was modified. Nullable</summary>
        DateTime Changed { get; set; }

        /// <summary>
        /// Stores state of the Object. True if object is deleted and false otherwise
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// Version of data
        /// </summary>
        int Version { get; set; }

        /// <summary>
        /// Tenant id
        /// </summary>
        Guid? TenantId { get; set; }
    }
}
