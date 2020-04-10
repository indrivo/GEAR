using Microsoft.EntityFrameworkCore;

namespace GR.Core.Events.EventArgs.Database
{
    public class DatabaseMigrateEventArgs : System.EventArgs
    {
        public virtual DbContext DbContext { get; set; }

        public virtual string ContextName { get; set; }

        public virtual long ElapsedMilliseconds { get; set; }
    }
}
