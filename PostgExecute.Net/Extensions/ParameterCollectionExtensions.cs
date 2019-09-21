using Npgsql;

namespace PostgExecute.Net
{
    public static class ParameterCollectionExtensions
    {
        public static NpgsqlParameterCollection Add(this NpgsqlParameterCollection collection, string parameterName, object value)
        {
            collection.AddWithValue(parameterName, value);
            return collection;
        }

        public static NpgsqlParameterCollection _(this NpgsqlParameterCollection collection, string parameterName,
            object value)
            => collection.Add(parameterName, value);
    }
}
