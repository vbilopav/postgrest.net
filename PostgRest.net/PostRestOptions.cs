namespace PostgRest.net
{
    public class PostRestOptions
    {
        public string ConnectionString { get;  set; }
        public string Prefix { get; set; } = "rest__";
        public string Schema { get; set; } = "public";
        public IRouteNameResolver RouteNameResolver { get; set; } = new KebabCaseRouteNameResolver();
        public string RouteNamePattern { get; set; } = "api/{0}";
    }
}