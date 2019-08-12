using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;


namespace PostgRest.net
{
    public class PostgRestRouteConvention : IControllerModelConvention
    {
        private readonly PostgRestOptions options;

        public PostgRestRouteConvention(PostgRestOptions options)
        {
            this.options = options;
        }

        public void Apply(ControllerModel controller)
        {
            if (!controller.ControllerType.IsGenericType)
            {
                return;
            }
            var genericType = controller.ControllerType.GenericTypeArguments[0];
            if (!ControllerData.Data.TryGetValue(genericType.Name, out var info))
            {
                return;
            }
            controller.Selectors.Clear();
            controller.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(options.ApplyRouteName(info.RouteName, info.RoutineName))),
            });
            options.ApplyFilters(controller.Filters, info.RouteName, info.RoutineName);
        }
    }
}
