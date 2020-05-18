namespace GR.Modules.Abstractions
{
    public abstract class GearModule : IModule
    {
        /// <summary>
        /// Name
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// System name
        /// </summary>
        public string SystemName => GetType().Assembly.GetName().Name;

        /// <summary>
        /// Version
        /// </summary>
        public string Version => GetType().Assembly.GetName().Version.ToString();
    }
}