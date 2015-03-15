using Caliburn.Micro;
using Parkkis.Services;
using Parkkis.Services.Oulu;
using Parkkis.Services.Tampere;
using Parkkis.Services.Utils;
using Parkkis.ViewModels;
using Parkkis.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Parkkis
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App
    {
        public WinRTContainer container { get; private set; }

        public App()
        {
            InitializeComponent();
        }

        protected override void Configure()
        {
            container = new WinRTContainer();

            container.RegisterWinRTServices();

            // View models
            container.PerRequest<MapPageViewModel>();
            container.PerRequest<ListPageViewModel>();
            container.PerRequest<SettingsPageViewModel>();
            container.PerRequest<AboutPageViewModel>();
            
            // Utils etc
            container.Singleton<InfoUtilService>();
            container.Singleton<AppSettingsService>();
            container.Singleton<DialogService>();
            container.Singleton<LocationUtilService>();
            container.Singleton<RegionService>();

            //Parking facility services
            container.RegisterPerRequest(typeof(IParkingFacilityService), "tampere", typeof(TampereParkingFacilityService));
            container.RegisterPerRequest(typeof(IParkingFacilityService), "oulu", typeof(OuluParkingFacilityService));

        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            container.RegisterNavigationService(rootFrame);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            DisplayRootView<MapPageView>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }
    }
}