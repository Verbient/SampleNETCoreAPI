

using MyApp.Util;

namespace MyApp.Models
{
    public class JWTConfig
    {
        private string secret = null!;

        public string Secret
        {
            get { return AesEncrypter.CustomDecryptString(secret); }
            set { secret = value; }
        }
    }
}
