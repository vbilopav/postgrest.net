using System;

namespace PostgRest.net
{
    public class ReferenceValueType
    {
        private object value = DBNull.Value;
        public object Value { get => value; set => this.value = value ?? DBNull.Value; }
    }
}