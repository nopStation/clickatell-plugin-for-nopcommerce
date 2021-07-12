using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Sms.Clickatell.Models;
using Nop.Plugin.SMS.Clickatell;
using Nop.Plugin.SMS.Clickatell.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Sms.Clickatell.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class SmsClickatellController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly ISerderService _serderService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor

        public SmsClickatellController(ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            ISerderService serderService,
            INotificationService notificationService)

        {
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _serderService = serderService;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var clickatellSettings = await _settingService.LoadSettingAsync<ClickatellSettings>(storeScope);

            var model = new SmsClickatellModel
            {
                Enabled = clickatellSettings.Enabled,
                ApiId = clickatellSettings.ApiId,
                Password = clickatellSettings.Password,
                Username = clickatellSettings.Username,
                PhoneNumber = clickatellSettings.PhoneNumber,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(clickatellSettings, x => x.Enabled, storeScope);
                model.PhoneNumber_OverrideForStore = await _settingService.SettingExistsAsync(clickatellSettings, x => x.PhoneNumber, storeScope);
            }

            return View("~/Plugins/SMS.Clickatell/Views/Configure.cshtml", model);
        }


        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public async Task<IActionResult> Configure(SmsClickatellModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var clickatellSettings = await _settingService.LoadSettingAsync<ClickatellSettings>(storeScope);

            //save settings
            clickatellSettings.Enabled = model.Enabled;
            clickatellSettings.ApiId = model.ApiId;
            clickatellSettings.Username = model.Username;
            clickatellSettings.Password = model.Password;
            clickatellSettings.PhoneNumber = model.PhoneNumber;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingAsync(clickatellSettings, x => x.ApiId, storeScope, false);
            await _settingService.SaveSettingAsync(clickatellSettings, x => x.Username, storeScope, false);
            await _settingService.SaveSettingAsync(clickatellSettings, x => x.Password, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(clickatellSettings, x => x.Enabled, model.Enabled_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(clickatellSettings, x => x.PhoneNumber, model.PhoneNumber_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return RedirectToAction("Configure");
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("test")]
        public async Task<IActionResult> TestSms(SmsClickatellModel model)
        {
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var clickatellSettings = await _settingService.LoadSettingAsync<ClickatellSettings>(storeScope);

            //test SMS send
            if (await _serderService.SendSmsAsync(model.TestMessage, 0, clickatellSettings))
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Sms.Clickatell.TestSuccess"));
            else
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Sms.Clickatell.TestFailed"));

            return RedirectToAction("Configure");
        }

        #endregion
    }
}