using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProductService.Facebook
{
    public class FacebookBackChannelHandler : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //This was added to adhere to facebook's breaking change to their re-direction parameter changes.
            if (!request.RequestUri.AbsolutePath.Contains("/oauth"))
            {
                request.RequestUri = new System.Uri(request.RequestUri.AbsoluteUri.Replace("?access_token","&access_token"));
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}