using Caliburn.Micro;
using Parkkis.Services.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Store;

namespace Parkkis.ViewModels
{

    /// <summary>
    /// About Page View Model 
    /// </summary>
    public class AboutPageViewModel : Screen
    {
        private readonly InfoUtilService infoUtilService;


        public AboutPageViewModel(InfoUtilService infoUtilService)
        {
            this.infoUtilService = infoUtilService;

            this.AppVersion = infoUtilService.AppVersion;
        }


        public AboutPageViewModel()
        {
            // Empty constructor for design time support
        }


        /// <summary>
        /// App version
        /// </summary>
        private string _appVersion;
        public string AppVersion
        {
            get { return _appVersion; }
            set
            {
                _appVersion = value;
                NotifyOfPropertyChange(() => AppVersion);
            }
        }


        /// <summary>
        /// Leave feedback, compose email
        /// </summary>
        public async void LeaveFeedback()
        {
            ResourceLoader resLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            EmailRecipient sendTo = new EmailRecipient()
            {
                Address = resLoader.GetString("ContactEmailAddress"),
            };
            
            EmailMessage mail = new EmailMessage();
            mail.Subject = resLoader.GetString("ContactEmailSubject");
            mail.To.Add(sendTo);

            await EmailManager.ShowComposeNewEmailAsync(mail);
        }


        /// <summary>
        /// Rate and review app
        /// </summary>
        public async void RateAndReview()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
        }

    }
}
