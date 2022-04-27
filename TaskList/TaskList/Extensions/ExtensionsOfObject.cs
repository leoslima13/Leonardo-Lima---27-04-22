using Prism.Navigation;

namespace TaskList.Extensions
{
    public static class ExtensionsOfObject
    {
        public static NavigationParameters ToNavigationParameters(this object value)
        {
            return new NavigationParameters {{value.GetType().FullName, value}};
        }
    }
}