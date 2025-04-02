namespace Secure_P_Backend.Helpers.Extensions;

public static class MvcOptionsExtensions
{
    public static void UseRoutePrefix(this MvcOptions options, string prefix)
    {
        options.UseRoutePrefix(new RouteAttribute(prefix));
    }

    public static void UseRoutePrefix(this MvcOptions options, RouteAttribute prefix)
    {
        options.Conventions.Add(new RoutePrefixConvention(prefix));
    }
}