namespace YY.Ocelot.HttpApi.Handler
{
    public class AddHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            // 添加自定义头信息
            request.Headers.Add("X-Custom-Header", "HeaderValue");

            var response = await base.SendAsync(request, token);

            // 记录响应日志
            Console.WriteLine($"Response: {response.StatusCode}");

            return response;
        }
    }
}
