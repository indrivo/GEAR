using System.Collections.Generic;
using System.Threading.Tasks;

namespace GR.Email.Abstractions.Helpers
{
    public class BaseMailTemplateGenerator
    {
        /// <summary>
        /// Subject
        /// </summary>
        public virtual string Subject { get; set; }

        /// <summary>
        /// Emails
        /// </summary>
        public virtual IEnumerable<string> Emails { get; set; } = new List<string>();

        /// <summary>
        /// Content
        /// </summary>
        public virtual string Content { get; set; }

        /// <summary>
        /// Generate
        /// </summary>
        /// <returns></returns>
        public virtual Task<string> GenerateAsync()
            => Task.Run(() => Content);
    }
}