using Caliburn.Micro;
using Parkkis.Models;
using Parkkis.Services.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Store;

namespace Parkkis.ViewModels
{
    public class SettingsPageViewModel : Screen
    {
        private readonly RegionService regionService;
        private readonly AppSettingsService appSettingsService;

        public SettingsPageViewModel(RegionService regionService, AppSettingsService appSettingsService)
        {
            this.regionService = regionService;
            this.appSettingsService = appSettingsService;

            this.Regions = this.regionService.Regions;

        }


        public SettingsPageViewModel()
        {
            // Empty constructor for design time support
        }


        /// <summary>
        /// Available regions
        /// </summary>
        private ObservableCollection<Region> _regions;
        public ObservableCollection<Region> Regions
        {
            get { return _regions; }
            set
            {
                _regions = value;
                NotifyOfPropertyChange(() => Regions);
            }
        }


        /// <summary>
        /// Selected region
        /// </summary>
        public Region SelectedRegion
        {
            get
            {
                return this.regionService.CurrentRegion;
            }

            set
            {
                this.regionService.CurrentRegion = value;
            }
        }


        /// <summary>
        /// Is location usage allowed
        /// </summary>
        public bool IsLocationAllowed
        {
            get
            {
                return this.appSettingsService.AllowLocationUsage;
            }

            set
            {
                this.appSettingsService.AllowLocationUsage = value;

            }
        }


    }
}
