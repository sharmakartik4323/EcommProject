using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace EcommProject.Utility
{
    public class SMSService : ISMSService
    {
        private readonly SMSSettings _sMSSettings;
        public SMSService(IOptions<SMSSettings> sMSSettings)
        {
            _sMSSettings = sMSSettings.Value;
        }
        public async Task<MessageResource> SendAsync(string to, string message)
        {
            TwilioClient.Init(_sMSSettings.AccountSID, _sMSSettings.AuthToken);

           var result = await MessageResource.CreateAsync(
                body: message,
                from: new PhoneNumber(_sMSSettings.PhoneNumber),
                to: new PhoneNumber(to)
                );
            return result;
        }

        public async Task<CallResource> SendCallAsync(string to, string message)
        {
            TwilioClient.Init(_sMSSettings.AccountSID, _sMSSettings.AuthToken);

            var result = await CallResource.CreateAsync(
                 from: new PhoneNumber(_sMSSettings.PhoneNumber),
                 to: new PhoneNumber(to),
                 twiml: new Twiml($"<Response><Say>{message}</Say></Response>")
                 );
            return result;
        }
    }
}