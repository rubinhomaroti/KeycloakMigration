using System.Data;

namespace KeycloakMigration
{
    internal static class Extensions
    {
        public static string GetExceptionMessages(this Exception exception)
        {
            return GetExceptionMessage(exception, string.Empty);
        }

        private static string GetExceptionMessage(Exception exception, string message)
        {
            if (exception.InnerException != null)
            {
                message = message + "\n" + GetExceptionMessage(exception.InnerException, exception.Message);
                return message;
            }
            else
            {
                return message + "\n" + exception.Message;
            }
        }
    }
}
