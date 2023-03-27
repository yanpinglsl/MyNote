using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.DiagnosticAdapter;
using zipkin4net;
using zipkin4net.Propagation;
using zipkin4net.Tracers.Zipkin.Thrift;

namespace YY.MicroService.Framework.ZipkinExtend
{
    /// <summary>
    /// 自定义埋点诊断监听类
    /// </summary>
    public class HttpDiagnosticListener : ITraceDiagnosticListener
    {
        public string DiagnosticName => "HttpHandlerDiagnosticListener";

        private ClientTrace? clientTrace;
        private readonly IInjector<HttpHeaders> _injector = 
            Propagations.B3String.Injector<HttpHeaders>((carrier, key, value) => carrier.Add(key, value));

        [DiagnosticName("System.Net.Http.Request")]
        public void HttpRequest(HttpRequestMessage request)
        {
            clientTrace = new ClientTrace("Gateway", request.Method.Method);
            if (clientTrace.Trace != null)
            {
                _injector.Inject(clientTrace.Trace.CurrentSpan, request.Headers);
            }
        }

        [DiagnosticName("System.Net.Http.Response")]
        public void HttpResponse(HttpResponseMessage response)
        {
            if (clientTrace!.Trace != null)
            {
                Console.WriteLine("=========================>自定义埋点");
                // 标记请求的路径
                clientTrace.AddAnnotation(Annotations.Tag(zipkinCoreConstants.HTTP_PATH, response.RequestMessage!.RequestUri!.LocalPath));
                // 标记请求的方法
                clientTrace.AddAnnotation(Annotations.Tag(zipkinCoreConstants.HTTP_METHOD, response.RequestMessage.Method.Method));
                // 标记请求的主机
                clientTrace.AddAnnotation(Annotations.Tag(zipkinCoreConstants.HTTP_HOST, response.RequestMessage.RequestUri.Host));
                if (!response.IsSuccessStatusCode)
                {
                    clientTrace.AddAnnotation(Annotations.Tag(zipkinCoreConstants.HTTP_STATUS_CODE, ((int)response.StatusCode).ToString()));
                }
            }
        }

        [DiagnosticName("System.Net.Http.Exception")]
        public void HttpException(HttpRequestMessage request, Exception exception)
        {
        }
    }
}
