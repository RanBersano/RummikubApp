using CommunityToolkit.Mvvm.Messaging.Messages;

namespace RummikubApp.Models
{
    public class AppMessage<T>(T msg) : ValueChangedMessage<T>(msg)
    {
    }
}
