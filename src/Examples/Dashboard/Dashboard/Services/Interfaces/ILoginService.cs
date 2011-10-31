using Dashboard.DomainModel;

namespace Dashboard.Services.Interfaces
{
	public interface ILoginService
	{
		User AttemptLogin(string username, string password);
	}
}