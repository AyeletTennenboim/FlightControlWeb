using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTestFlightControl
{
    // This class can be injected into the HttpClient which let override any request.
    public class HttpMessageHandlerStub : HttpMessageHandler
    {
        // When any method is called on the httpClient (like GetAsync), it will return a
        // 200 response status code with the same flight plan json string content.
        // This new stub is invoked by injecting it into the HttpClient and calling a method on it.
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            string flightPlanJson = @"{
            'passengers': '160',
            'company_name': 'TestAir',
            'initial_location': {
                'longitude': '34.95',
                'latitude': '29.55',
                'date_time': '2020-06-02T12:12:00Z'
            },
            'segments': [{
                'longitude': '34.21',
                'latitude': '32.76',
                'timespan_seconds': '1000'
            }]}";

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(flightPlanJson)
            };
            return await Task.FromResult(responseMessage);
        }
    }
}
