namespace MyApp.Services
{
    public interface ICaptchaService
    {
        bool Validate(string encodedResponse);
    }
}