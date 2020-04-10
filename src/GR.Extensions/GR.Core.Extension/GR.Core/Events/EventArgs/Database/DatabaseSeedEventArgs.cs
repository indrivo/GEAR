using Microsoft.EntityFrameworkCore;

namespace GR.Core.Events.EventArgs.Database
{
    public class DatabaseSeedEventArgs : System.EventArgs
    {
        public virtual DbContext DbContext { get; set; }

        public virtual string ContextName { get; set; }
    }
}