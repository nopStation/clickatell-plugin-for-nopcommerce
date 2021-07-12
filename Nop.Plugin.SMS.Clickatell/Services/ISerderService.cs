using System.Threading.Tasks;

namespace Nop.Plugin.SMS.Clickatell.Services
{
    public interface ISerderService
    {
        Task<bool> SendSmsAsync(string text, int orderId, ClickatellSettings settings = null);
    }
}