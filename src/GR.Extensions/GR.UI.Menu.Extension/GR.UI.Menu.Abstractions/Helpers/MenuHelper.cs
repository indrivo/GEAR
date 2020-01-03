namespace GR.UI.Menu.Abstractions.Helpers
{
    public static class MenuHelper
    {
        /// <summary>
        /// Get menu cache key
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public static string GetCacheKey(string menuId)
        {
            return $"_menu_{menuId}";
        }

        /// <summary>
        /// Cache key
        /// </summary>
        /// <param name="menuBlockId"></param>
        /// <returns></returns>
        public static string GetBlockCacheKey(string menuBlockId)
        {
            return $"_menu_block_{menuBlockId}";
        }
    }
}
