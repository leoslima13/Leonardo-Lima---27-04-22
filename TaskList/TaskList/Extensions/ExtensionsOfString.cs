using System;

namespace TaskList.Extensions
{
    public static class ExtensionsOfString
    {
        public static string AsNavigation(this string pageName)
        {
            return $"NavigationPage/{pageName}";
        }

        public static Uri AsNavigationAbsolute(this string pageName)
        {
            return new Uri($"http://beautyportionadmin.com/NavigationPage/{pageName}", UriKind.Absolute);
        }
    }
}