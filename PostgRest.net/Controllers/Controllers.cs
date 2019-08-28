using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;


namespace PostgRest.net
{
    public class GetController<T> : ControllerBase<T>
    {
        public GetController(IStringContentService stringContentService, IOptions<PostgRestConfig> options) : base(stringContentService, options) { }

        [HttpGet] public async Task<ContentResult> Get() => await GetContentAsync();
    }

    public class PostController<T> : ControllerBase<T>
    {
        public PostController(IStringContentService stringContentService, IOptions<PostgRestConfig> options) : base(stringContentService, options) { }

        [HttpPost] public async Task<ContentResult> Post() => await GetContentAsync();
    }

    public class PutController<T> : ControllerBase<T>
    {
        public PutController(IStringContentService stringContentService, IOptions<PostgRestConfig> options) : base(stringContentService, options) { }

        [HttpPut] public async Task<ContentResult> Put() => await GetContentAsync();
    }

    public class DeleteController<T> : ControllerBase<T>
    {
        public DeleteController(IStringContentService stringContentService, IOptions<PostgRestConfig> options) : base(stringContentService, options) { }

        [HttpDelete] public async Task<ContentResult> Delete() => await GetContentAsync();
    }
}
