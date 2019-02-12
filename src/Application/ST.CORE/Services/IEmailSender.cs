using System.Threading.Tasks;

namespace ST.CORE.Services
{
	public interface IEmailSender
	{
		Task SendEmailAsync(string email, string subject, string message);
	}
}