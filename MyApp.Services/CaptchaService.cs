using MyApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyApp.Services
{
    public class CaptchaService : ICaptchaService
    {
        public bool Validate(string encodedResponse)
        {
            if (string.IsNullOrEmpty(encodedResponse)) return false;

            var secret = "6LfpR3YeAAAAAPxlS8B53r37UZDvkEkjJ2j5cKpb";
            if (string.IsNullOrEmpty(secret)) return false;

            var client = new System.Net.WebClient();

            var googleReply = client.DownloadString(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={encodedResponse}");

            RecaptchaResponse result = JsonConvert.DeserializeObject<RecaptchaResponse>(googleReply)!;
            return result.Success;
        }
    }
}
