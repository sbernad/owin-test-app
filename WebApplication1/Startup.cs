using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using WebApplication1.OwinInfrastructure;

[assembly: OwinStartup(typeof(WebApplication1.Startup))]
namespace WebApplication1
{
    public class Startup
    {
        /// <summary> Configurations the specified application. </summary>
        /// <param name="app">The application.</param>
        public static void Configuration(IAppBuilder app)
        {
            var httpConfiguration = CreateHttpConfiguration();
            httpConfiguration.Routes.MapHttpRoute(
                name: "Default",
                routeTemplate: "{prefix}/{controller}");

            httpConfiguration.Services.Replace(typeof(IExceptionHandler), new PassthroughExceptionHandler());

            app.UseOwinExceptionHandler().UseWebApi(httpConfiguration);

        }

        /// <summary> Creates the HTTP configuration. </summary>
        /// <returns> An <see cref="HttpConfiguration"/> to bootstrap the hosted API </returns>
        public static HttpConfiguration CreateHttpConfiguration()
        {
            var httpConfiguration = new HttpConfiguration();

            httpConfiguration.MapHttpAttributeRoutes();

            httpConfiguration.Formatters.Clear();
            httpConfiguration.Formatters.Add(new JsonMediaTypeFormatter());
            httpConfiguration.Formatters.JsonFormatter.SerializerSettings =
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return httpConfiguration;
        }

        public class PassthroughExceptionHandler : IExceptionHandler
        {
            public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
            {
                var info = ExceptionDispatchInfo.Capture(context.Exception);
                info.Throw();

                return Task.CompletedTask;
            }
        }
    }
}