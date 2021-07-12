using Nop.Core;
using Nop.Services.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using System.Threading.Tasks;

namespace Nop.Plugin.SMS.Clickatell
{
    /// <summary>
    /// Represents the Clickatell SMS provider
    /// </summary>
    public class ClickatellSmsProvider : BasePlugin, IMiscPlugin
    {
        #region Fields

        private readonly ClickatellSettings _clickatellSettings;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public ClickatellSmsProvider(ClickatellSettings clickatellSettings,
            ILogger logger,
            IOrderService orderService,
            ISettingService settingService,
            IWebHelper webHelper,
            ILocalizationService localizationService)
        {
            _clickatellSettings = clickatellSettings;
            _logger = logger;
            _orderService = orderService;
            _settingService = settingService;
            _webHelper = webHelper;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/SmsClickatell/Configure";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new ClickatellSettings());

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.ApiId", "API ID");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.ApiId.Hint", "Specify Clickatell API ID.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.Enabled", "Enabled");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.Enabled.Hint", "Check to enable SMS provider.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.Password", "Password");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.Password.Hint", "Specify Clickatell API password.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.PhoneNumber", "Phone number");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.PhoneNumber.Hint", "Enter your phone number.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.TestMessage", "Message text");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.TestMessage.Hint", "Enter text of the test message.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.Username", "Username");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.Username.Hint", "Specify Clickatell API username.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Clickatell.SendTest", "Send");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Clickatell.SendTest.Hint", "Send test message");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Clickatell.TestFailed", "Test message sending failed");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Sms.Clickatell.TestSuccess", "Test message was sent");

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<ClickatellSettings>();

            //locales
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.ApiId");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.ApiId.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.Enabled");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.Enabled.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.Password");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.Password.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.PhoneNumber");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.PhoneNumber.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.TestMessage");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.TestMessage.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.Username");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Clickatell.Fields.Username.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Clickatell.SendTest");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Clickatell.SendTest.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Clickatell.TestFailed");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Sms.Clickatell.TestSuccess");

            await base.UninstallAsync();
        }

        #endregion
    }
}
