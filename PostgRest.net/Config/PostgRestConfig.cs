namespace PostgRest.Net.Config
{
    public class PostgRestConfig
    {
        public string Connection { get; set; }
        public string RoutinePrefix { get;set; } = "rest__";
        public string DatabaseSchema { get; set; } = "public";
        public string RouteNamePattern = "api/{0}";
        public string[] JsonTypes { get; set; } = { "json", "jsonb" };
        public string[] RecordSetTypes { get; set; } = { "USER-DEFINED", "record" };
        public string VoidType { get; set; } = "void";
        public string QueryStringParamNameRegex { get; set; } = "query";
        public string BodyParamNameRegex { get; set; } = "body";
        public string JsonContentType { get; set; } = "application/json; charset=utf-8";
        public string TextContentType { get; set; } = "text/plain; charset=utf-8";
        public string JsonDefaultValue { get; set; } = "{}";
        public string NonJsonDefaultValue { get; set; } = null;
        public string GetRouteRegex { get; set; } = "^get";
        public string PostRouteRegex { get; set; } = "^post";
        public string PutRouteRegex { get; set; } = "^put";
        public string DeleteRouteRegex { get; set; } = "^delete";
        public string ReadPgRoutinesCommand { get; set; } =
            @"select
                r.routine_name,
                r.data_type as ""return_type"",
                coalesce(json_agg(
                    json_build_object(
                        'ParamName', p.parameter_name,
                        'ParamType', p.data_type,
                        'Position', p.ordinal_position,
                        'HaveDefault', p.parameter_default is not null,
                        'Direction', p.parameter_mode)
                )  filter (where p.parameter_name is not null), '[]') as ""parameters""
            from information_schema.routines r
            left outer join information_schema.parameters p on r.specific_name = p.specific_name
            where
                r.routine_type = 'FUNCTION'
                and r.specific_schema = @schema
            group by
                r.routine_name, r.data_type";
    }
}
