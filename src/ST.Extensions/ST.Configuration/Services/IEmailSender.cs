using System.Threading.Tasks;

namespace ST.Configuration.Services
{
	public interface IEmailSender
	{
		Task SendEmailAsync(string email, string subject, string message);
	}
}