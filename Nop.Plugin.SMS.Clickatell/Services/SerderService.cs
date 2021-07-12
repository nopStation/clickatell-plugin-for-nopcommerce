using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Plugin.SMS.Clickatell.Clickatell;
using Nop.Services.Logging;
using Nop.Services.Orders;

namespace Nop.Plugin.SMS.Clickatell.Services
{
    public class SerderService : ISerderService
    {
        #region Fields

        private readonly ClickatellSettings _clickatellSettings;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public SerderService(ClickatellSettings clickatellSettings,
            IOrderService orderService,
            ILogger logger)
        {
            _clickatellSettings = clickatellSettings;
            _orderService = orderService;
            _logger = logger;
        }

        #endregion

        /// <summary>
        /// Send SMS 
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="orderId">Order id</param>
        /// <param name="settings">Clickatell settings</param>
        /// <returns>True if SMS was successfully sent; otherwise false</returns>
        public async Task<bool> SendSmsAsync(string text, int orderId, ClickatellSettings settings = null)
        {
            var clickatellSettings = settings ?? _clickatellSettings;
            if (!clickatellSettings.Enabled)
                return false;

            //change text
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order != null)
                text = $"New order #{order.Id} was placed for the total amount {order.OrderTotal:0.00}";

            using (var smsClient = new ClickatellSmsClient(new BasicHttpBinding(), new EndpointAddress("http://api.clickatell.com/soap/document_literal/webservice")))
            {
                //check credentials
                var authentication = smsClient.auth(int.Parse(clickatellSettings.ApiId), clickatellSettings.Username, clickatellSettings.Password);
                if (!authentication.ToUpperInvariant().StartsWith("OK"))
                {
                    await _logger.ErrorAsync($"Clickatell SMS error: {authentication}");
                    return false;
                }

                //send SMS
                var sessionId = authentication.Substring(4);
                var result = smsClient.sendmsg(sessionId, int.Parse(clickatellSettings.ApiId), clickatellSettings.Username, clickatellSettings.Password,
                    text, new[] { clickatellSettings.PhoneNumber }, string.Empty, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    string.Empty, 0, string.Empty, string.Empty, string.Empty, 0).FirstOrDefault();

                if (result == null || !result.ToUpperInvariant().StartsWith("ID"))
                {
                    await _logger.ErrorAsync($"Clickatell SMS error: {result}");
                    return false;
                }
            }

            //order note
            if (order != null)
            {
                await _orderService.InsertOrderNoteAsync(new OrderNote()
                {
                    Note = "\"Order placed\" SMS alert (to store owner) has been sent",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
            }

            return true;
        }
    }
}
