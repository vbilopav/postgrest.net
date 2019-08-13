using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PostgRest.net
{
    public interface IRouteNameResolver
    {
        string ResolveRouteName(string routineName, string candidateRaw, string candidateLowerNoVerb, string verb);
    }

    public class KebabCaseRouteNameResolver : IRouteNameResolver
    {
        public string ResolveRouteName(string routineName, string candidateRaw, string candidateLowerNoVerb, string verb)
        {
            var snaked = string.Concat(candidateLowerNoVerb.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));
            return snaked.Trim('_').Replace("_", "-");
        }
    }
}
