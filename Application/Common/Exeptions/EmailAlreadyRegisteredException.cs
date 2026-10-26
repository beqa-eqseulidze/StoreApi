
namespace Application.Common.Exeptions
{
    public sealed class EmailAlreadyRegisteredException(string message) : Exception(message)
    {       
    }
}
