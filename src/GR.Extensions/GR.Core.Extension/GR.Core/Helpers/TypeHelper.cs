
namespace GR.Core.Helpers
{
    public class TypeHelper
    {
        public static object GetPropertyValue(object obj, string name)
        {
            return obj?.GetType()
                .GetProperty(name)
                .GetValue(obj, null);
        }
    }
}
