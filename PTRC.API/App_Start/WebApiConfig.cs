using PTRC.API.Service;
using Swashbuckle.Application;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Unity;

namespace PTRC.API
{
    
    public static class WebApiConfig
    {
        public class Interceptor : DelegatingHandler
        {
            protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = await base.SendAsync(request, cancellationToken);
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "*");
                response.Headers.Add("Access-Control-Allow-Headers", "*");
                return response;
            }

        }
        public static void Register(HttpConfiguration config)
        {
            config.MessageHandlers.Add(new Interceptor());
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            var container = new UnityContainer();
            container.RegisterType<ILoggerManager, LoggerManager>();          
            config.DependencyResolver = new Service.UnityResolver(container);

            config.Routes.MapHttpRoute(
             name: "Swagger UI",
             routeTemplate: "",
             defaults: null,
             constraints: null,
             handler: new RedirectHandler(SwaggerDocsConfig.DefaultRootUrlResolver, "swagger/ui/index"));

        }
    }
}
