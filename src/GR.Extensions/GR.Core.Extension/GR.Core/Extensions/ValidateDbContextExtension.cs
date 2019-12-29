using Microsoft.EntityFrameworkCore;
using GR.Core.Exceptions;

namespace GR.Core.Extensions
{
    public static class ValidateDbContextExtension
    {
        /// <summary>
        /// Validate context usage
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="context"></param>
        public static void Validate<TContext>(this TContext context) where TContext : DbContext
        {
            if (context == null)
            {
                throw new DbContextNotRegisteredException($"{nameof(TContext)} is not registered or is disposed!");
            }
        }

        /// <summary>
        /// Validate context usage
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="context"></param>
        public static void ValidateNullAbstractionContext<TContext>(this TContext context)
        {
            if (context == null)
            {
                throw new DbContextNotRegisteredException($"{nameof(TContext)} is not registered or is disposed!");
            }
        }
    }
}
