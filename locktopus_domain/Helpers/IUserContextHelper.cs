namespace locktopus_domain.Helpers;

public interface IUserContextHelper
{
    string UserId { get; }
    bool IsAuthenticated { get; }
}