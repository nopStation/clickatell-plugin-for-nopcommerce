using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Plugin.SMS.Clickatell.Services;
using Nop.Services.Events;

namespace Nop.Plugin.SMS.Clickatell.Infrastructure.Cache
{
    public class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
    {
        #region Fields

        private readonly ISerderService _serderService;

        #endregion

        #region Ctor

        public OrderPlacedEventConsumer(ISerderService  serderService)
        {
            _serderService = serderService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            await _serderService.SendSmsAsync(string.Empty, eventMessage.Order.Id);
        }

        #endregion
    }
}