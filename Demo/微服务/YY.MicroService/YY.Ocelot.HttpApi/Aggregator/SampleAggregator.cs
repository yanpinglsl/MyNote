using System.Net;
using Newtonsoft.Json;
using Ocelot.Middleware;
using Ocelot.Multiplexer;

namespace YY.Ocelot.HttpApi.Aggregator
{
    public class SampleAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responseHttpContexts)
        {
            var responses = responseHttpContexts.Select(x => x.Items.DownstreamResponse()).ToArray();
            var contentList = new List<string>();

            foreach (var response in responses)
            {
                var content = await response.Content.ReadAsStringAsync();
                contentList.Add(content);
            }

            return new DownstreamResponse(
                new StringContent(JsonConvert.SerializeObject(contentList)),
                HttpStatusCode.OK,
                responses.SelectMany(x => x.Headers).ToList(),
                "reason");
        }
    }


}
