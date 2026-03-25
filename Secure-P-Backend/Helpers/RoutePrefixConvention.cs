namespace Secure_P_Backend.Helpers;

public class RoutePrefixConvention(IRouteTemplateProvider route) : IApplicationModelConvention
{
    private readonly AttributeRouteModel _routePrefix = new(route);

    // Somehow the file which I F12-ed into doesn't show that RouteAttribute implement IRouteTemplateProvider
    public void Apply(ApplicationModel application)
    {
        foreach (var selector in application.Controllers.SelectMany(c => c.Selectors))
        {
            selector.AttributeRouteModel = selector.AttributeRouteModel != null ? AttributeRouteModel.CombineAttributeRouteModel(_routePrefix, selector.AttributeRouteModel) : _routePrefix;
        }
    }
}