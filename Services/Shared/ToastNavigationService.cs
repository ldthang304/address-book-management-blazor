
using Blazored.Toast.Services;

namespace AddressBookManagement.Services.Shared
{
    public class ToastNavigationService
    {
        public string? Message { get; private set; }
        public ToastLevel Level { get; private set; } = ToastLevel.Info;

        public void SetMessage(string message, ToastLevel level = ToastLevel.Info)
        {
            Message = message;
            Level = level;
        }

        public (string? Message, ToastLevel Level) ConsumeMessage()
        {
            var temp = (Message, Level);
            Message = null; // Clear after use
            return temp;
        }
    }
}
