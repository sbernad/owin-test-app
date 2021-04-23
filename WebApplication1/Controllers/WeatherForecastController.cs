using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    [RoutePrefix("api")]
    public class WeatherForecastController : ApiController
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public WeatherForecastController()
        {
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            Trace.TraceWarning("WeatherForecast endpoint reached...");

            var rng = new Random();
            var firstTemp = rng.Next(-20, 35);
            var avgTemp = firstTemp;
            var res = Enumerable.Range(1, 5).Select(index => {

                int thisTemp = firstTemp;
                if (index > 1)
                {
                    thisTemp = Math.Min(Math.Max(avgTemp + rng.Next(-10, 10), MinTemp), MaxTemp);

                    var beStrict = (rng.NextDouble() > 0.7);

                   if(beStrict && (thisTemp == MinTemp || thisTemp == MaxTemp))
                   {
                        throw new ArgumentOutOfRangeException();
                   }

                    avgTemp = (index * avgTemp + thisTemp) / (index + 1);
                }

                var summaryIndex = (int)Math.Floor((thisTemp - MinTemp) / (float)(MaxTemp - MinTemp) * Summaries.Length);
                if (summaryIndex == Summaries.Length)
                {
                    summaryIndex--;
                }

                return new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = thisTemp,
                    Summary = Summaries[summaryIndex]
                };
            })
            .ToArray();

            Trace.TraceInformation($"Forecast: {JsonConvert.SerializeObject(res)}");

            return res;
        }

        private const int MinTemp = -20;
        private const int MaxTemp = 35;
    }
}
