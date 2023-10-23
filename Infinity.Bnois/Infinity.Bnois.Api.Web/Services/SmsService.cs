using Microsoft.AspNet.Identity;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Infinity.Ers.IdentityServer.Services
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SmsService'
    public class SmsService : IIdentityMessageService
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SmsService'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SmsService.SendAsync(IdentityMessage)'
        public Task SendAsync(IdentityMessage message)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SmsService.SendAsync(IdentityMessage)'
        {
            string smsBaisUrl = "https://api.mobireach.com.bd/SendTextMultiMessage?Username=infinittech&Password=Infinity%40123&From=MobiReach";
            using (HttpClient client = new HttpClient())
            {
            
                var url = smsBaisUrl + "&To=" + message.Destination + "&Message=" + message.Body;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync(url).Result;
                return Task.FromResult(response.StatusCode);
            }

        }
    }
}