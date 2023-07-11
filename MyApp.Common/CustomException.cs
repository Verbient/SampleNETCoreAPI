namespace MyApp.Common
{
    public class CustomException:Exception
    {
        public CustomException(string ErrorMessage):base(ErrorMessage)
        {

        }

    }
}
