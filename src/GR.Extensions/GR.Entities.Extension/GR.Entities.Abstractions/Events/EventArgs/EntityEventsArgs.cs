using System;

namespace GR.Entities.Abstractions.Events.EventArgs
{
    public class EntityBaseEventArgs : System.EventArgs
    {
        public Guid EntityId { get; set; }
        public string EntityName { get; set; }
    }

    #region Entities
    public class EntityCreatedEventArgs : EntityBaseEventArgs
    {

    }

    public class EntityDeleteEventArgs : EntityBaseEventArgs
    {

    }

    public class EntityUpdateEventArgs : EntityBaseEventArgs
    {

    }
    #endregion

    #region Entity Fields

    public class EntityFieldBaseEventArgs : EntityBaseEventArgs
    {
        public Guid FieldId { get; set; }
        public string FieldName { get; set; }
    }

    public class EntityAddNewFieldEventArgs : EntityFieldBaseEventArgs
    {

    }

    public class EntityDeleteFieldEventArgs : EntityFieldBaseEventArgs
    {

    }

    public class EntityUpdateFieldEventArgs : EntityFieldBaseEventArgs
    {

    }

    #endregion
}
