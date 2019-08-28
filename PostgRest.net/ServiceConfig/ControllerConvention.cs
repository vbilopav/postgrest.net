using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;


namespace PostgRest.net
{
    public class ControllerConvention : IControllerModelConvention
    {
        private readonly PostgRestOptions options;

        public ControllerConvention(PostgRestOptions options)
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
            info.RouteName = options.ApplyRouteName(info.RouteName, info.RoutineName);
            
            /*
            if (info.RoutineName == "rest__get_values" && info.ReturnType == "text")
            {
                var r = string.Concat(info.RouteName, "/{1}");
                controller.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(r))
                });
                return;
            }
            else
            {
                controller.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(info.RouteName))
                });
            }
            */
            controller.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(info.RouteName))
            });

            options.ApplyFilters(controller.Filters, info.RouteName, info.RoutineName);
            options.ApplyControllerConvention(controller, info);
        }
    }
}
