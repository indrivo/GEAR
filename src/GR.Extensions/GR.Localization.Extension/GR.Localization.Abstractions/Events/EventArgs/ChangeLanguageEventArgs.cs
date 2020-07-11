using System;

namespace GR.Localization.Abstractions.Events.EventArgs
{
    public class ChangeLanguageEventArgs : System.EventArgs
    {
        public virtual Guid UserId { get; set; }
        public virtual string Identifier { get; set; }
    }
}