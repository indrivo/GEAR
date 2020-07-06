using System;

namespace GR.Core.Attributes
{
    /// <summary>
    /// This attribute aims to exclude from DbContext for automatic migration in search of pending migrations
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class IgnoreContextAutoMigrationsAttribute : Attribute
    {
    }
}