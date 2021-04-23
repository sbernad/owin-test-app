using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Microsoft.ApplicationInsights;
using Microsoft.Owin;
using Owin;

namespace WebApplication1.OwinInfrastructure
{
    public class ExceptionHandlerMiddleware : OwinMiddleware
    {
        private static void HandleException(IOwinContext context, Exception ex)
        {
            switch (ex)
            {
                default:
                    var tc = new TelemetryClient();
                    tc.TrackException(ex);
                    break;
            }
        }
        public ExceptionHandlerMiddleware(OwinMiddleware next) : base(next)
        {
        }
        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                if (Next != null)
                {
                    await Next?.Invoke(context);
                }

                var anyException = HttpContext.Current?.AllErrors?.FirstOrDefault();
                if (anyException != null)
                {
                    HandleException(context, anyException);
                }
            }
            catch (Exception ex)
            {
                HandleException(context, ex);
                // Log EXception TODO
            }
        }
    }

    public static class OwinExceptionHandlerMiddlewareAppBuilderExtensions
    {
        public static IAppBuilder UseOwinExceptionHandler(this IAppBuilder app)
        {
            return app.Use<ExceptionHandlerMiddleware>();
        }
    }
}