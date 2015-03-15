using Caliburn.Micro;
using Parkkis.Models;
using Parkkis.Models.Base;
using Parkkis.Services;
using Parkkis.Services.Utils;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Input;

namespace Parkkis.ViewModels
{
    public class MapPageViewModel : Screen
    {

        private readonly INavigationService navigationService;
        private readonly IEventAggregator eventAggregator;
        private readonly DialogService dialogService;
        private readonly AppSettingsService appSettingsService;
        private readonly RegionService regionService;
        private readonly LocationUtilService locationUtilService;

        private Geolocator geolocator;
        private DateTime parkingFacilitiesUpdated; // Last parking facilities updat time

        public MapPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator, DialogService dialogService, AppSettingsService appSettingsService, RegionService regionService, LocationUtilService locationUtilService)
        {
            this.navigationService = navigationService;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;
            this.appSettingsService = appSettingsService;
            this.regionService = regionService;
            this.locationUtilService = locationUtilService;

            this.SelectedRegion = this.regionService.CurrentRegion;

            this.MapZoomLevel = this.SelectedRegion.MapZoomLevelDefault;
            this.MapCenter = this.SelectedRegion.MapCenterDefault;
            this.IsCurrentLocationVisible = false;
            this.NoParkingFacilities = false;
        }

        /// <summary>
        /// Public empty constructor, set design time data
        /// </summary>
        public MapPageViewModel()
        {
            // Add design time data
            if (Execute.InDesignMode)
            {
                this.SelectedParkingFacility = new BaseParkingFacility
                    {
                        Name = "Parkkis eka",
                        Address = "Hämeenjoenkatu 4, 5466 Laaksolaaaa",
                        Location = null,
                        FreeSpace = 53,
                        TotalSpace = 200,
                        Updated = DateTime.Now,
                        Status = ParkingFacilityStatus.SpacesAvailable,
                        Distance = 100,
                        IsSelected = true,
                    };
            }
        }


        /// <summary>
        /// Map zoom level
        /// </summary>
        private Region _selectedRegion;
        public Region SelectedRegion
        {
            get { return _selectedRegion; }
            set
            {
                _selectedRegion = value;
                NotifyOfPropertyChange(() => SelectedRegion);
            }
        }


        /// <summary>
        /// Map zoom level
        /// </summary>
        private double _mapZoomLevel;
        public double MapZoomLevel
        {
            get { return _mapZoomLevel; }
            set
            {
                _mapZoomLevel = value;
                NotifyOfPropertyChange(() => MapZoomLevel);
            }
        }


        /// <summary>
        /// Map center
        /// </summary>
        private Geopoint _mapCenter;
        public Geopoint MapCenter
        {
            get { return _mapCenter; }
            set
            {
                _mapCenter = value;
                NotifyOfPropertyChange(() => MapCenter);
            }
        }


        /// <summary>
        /// User current location
        /// </summary>
        private Geopoint _currentLocation;
        public Geopoint CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                NotifyOfPropertyChange(() => CurrentLocation);
            }
        }


        /// <summary>
        /// Is user location visible
        /// </summary>
        private bool _isCurrentLocationVisible;
        public bool IsCurrentLocationVisible
        {
            get { return _isCurrentLocationVisible; }
            set
            {
                _isCurrentLocationVisible = value;
                NotifyOfPropertyChange(() => IsCurrentLocationVisible);
            }
        }


        /// <summary>
        /// Is page loading, display loading indicator
        /// </summary>
        private bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
                NotifyOfPropertyChange(() => CanRefreshParkingFacilities);
            }
        }


        /// <summary>
        /// Parking facilities collection
        /// </summary>
        private IReadOnlyCollection<IParkingFacility> _parkingFacilities;
        public IReadOnlyCollection<IParkingFacility> ParkingFacilities
        {
            get { return _parkingFacilities; }
            set
            {
                _parkingFacilities = value;
                NotifyOfPropertyChange(() => ParkingFacilities);
                NotifyOfPropertyChange(() => CanOpenListPage);
            }
        }


        /// <summary>
        /// Selected parking facility
        /// </summary>
        private IParkingFacility _selectedParkingFacility;
        public IParkingFacility SelectedParkingFacility
        {
            get { return _selectedParkingFacility; }
            set
            {
                _selectedParkingFacility = value;
                NotifyOfPropertyChange(() => SelectedParkingFacility);

                if (value != null)
                    ShowSelectedParkingFacility = true;

                else
                    ShowSelectedParkingFacility = false;
            }
        }


        /// <summary>
        /// Show selected parking facility
        /// </summary>
        private bool _showSelectedParkingFacility;
        public bool ShowSelectedParkingFacility
        {
            get { return _showSelectedParkingFacility; }
            set
            {
                _showSelectedParkingFacility = value;
                NotifyOfPropertyChange(() => ShowSelectedParkingFacility);
            }
        }


        /// <summary>
        /// No parking facilities found, display error massage in view
        /// </summary>
        private bool _noParkingFacilities;
        public bool NoParkingFacilities
        {
            get { return _noParkingFacilities; }
            set
            {
                _noParkingFacilities = value;
                NotifyOfPropertyChange(() => NoParkingFacilities);
            }
        }


        /// <summary>
        /// Can we open list page, is there parking facilities available
        /// </summary>
        public bool CanOpenListPage
        {
            get
            {
                if (ParkingFacilities != null && ParkingFacilities.Count > 0)
                    return true;

                else
                    return false;
            }
        }


        /// <summary>
        /// Can we refresh parking facilities, not loading
        /// </summary>
        public bool CanRefreshParkingFacilities
        {
            get
            {
                return !IsLoading;
            }
        }


        /// <summary>
        /// OnInitialize
        /// </summary>
        protected override async void OnInitialize()
        {
            base.OnInitialize();

            // Check do we allow location usage
            if(!this.appSettingsService.AllowLocationUsage)
            {
                ResourceLoader resLoader = new Windows.ApplicationModel.Resources.ResourceLoader();
                bool dialogRes = await this.dialogService.ShowMessageDialog(resLoader.GetString("DialogAllowLocationUsageMessage"), resLoader.GetString("DialogAllowLocationUsageTitle"), resLoader.GetString("ButtonYes"), resLoader.GetString("ButtonNo"));

                if(dialogRes)
                {
                    this.appSettingsService.AllowLocationUsage = true;
                }
            }
        }


        /// <summary>
        /// OnActivate
        /// Start location tracking and get parkign facilities
        /// </summary>
        protected override void OnActivate()
        {
            base.OnActivate();

            // Region changed
            if (this.SelectedRegion.Key != this.regionService.CurrentRegion.Key)
            {
                this.MapZoomLevel = this.regionService.CurrentRegion.MapZoomLevelDefault;
                this.MapCenter = this.regionService.CurrentRegion.MapCenterDefault;
                this.SelectedRegion = this.regionService.CurrentRegion;

                this.ParkingFacilities = new ReadOnlyCollection<IParkingFacility>(new IParkingFacility[] { });
                this.parkingFacilitiesUpdated = DateTime.MinValue;
                this.SelectedParkingFacility = null;
            }

            // Start location traking only if it is allowed from settings
            if (this.appSettingsService.AllowLocationUsage)
            {
                StartLocationTracking();
            }

            GetParkingFacilities();
        }


        /// <summary>
        /// OnDeactivate
        /// Stop location tracking
        /// </summary>
        /// <param name="close"></param>
        protected override void OnDeactivate(bool close)
        {
            StoptLocationTracking();

            base.OnDeactivate(close);
        }


        /// <summary>
        /// Get parkign facilities
        /// </summary>
        private void GetParkingFacilities()
        {
            if (this.parkingFacilitiesUpdated == null || this.parkingFacilitiesUpdated < DateTime.Now.AddMinutes(-5))
            {
                RefreshParkingFacilities();
            }
        }


        /// <summary>
        /// Refresh parking facilites
        /// </summary>
        public async void RefreshParkingFacilities()
        {
            this.IsLoading = true;

            this.ParkingFacilities = await this.regionService.GetParkingFacilityService().GetParkingFacilitiesAsync();

            this.IsLoading = false;
            this.parkingFacilitiesUpdated = DateTime.Now;

            if(this.ParkingFacilities == null || this.ParkingFacilities.Count < 1)
            {
                this.NoParkingFacilities = true;
            }

            else
            {
                this.NoParkingFacilities = false;
            }
        }


        /// <summary>
        /// Start location tracking
        /// </summary>
        private void StartLocationTracking()
        {
            System.Diagnostics.Debug.WriteLine("MapPageViewModel, StartLocationTracking");

            if (this.geolocator != null)
            {
                return;
            }

            this.geolocator = new Geolocator();
            this.geolocator.DesiredAccuracy = PositionAccuracy.High;
            this.geolocator.MovementThreshold = 30; // Meters
            this.geolocator.PositionChanged += GeolocatorPositionChanged;
        }


        /// <summary>
        /// Stop location tracking
        /// </summary>
        private void StoptLocationTracking()
        {
            System.Diagnostics.Debug.WriteLine("MapPageViewModel, StoptLocationTracking");

            if (this.geolocator == null)
            {
                return;
            }

            this.IsCurrentLocationVisible = false;
            this.geolocator.PositionChanged -= GeolocatorPositionChanged;
            this.geolocator = null;
        }


        /// <summary>
        /// Geolocator position changed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void GeolocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("MapPageViewModel, GeolocatorPositionChanged");

            this.CurrentLocation = args.Position.Coordinate.Point;
            this.IsCurrentLocationVisible = true;

            UpdateFacilityDistances(args.Position.Coordinate.Point);

        }


        /// <summary>
        /// Update parking facilities collection, set distances
        /// </summary>
        /// <param name="currentLocation"></param>
        public void UpdateFacilityDistances(Geopoint currentLocation)
        {
            if (this.ParkingFacilities == null || this.ParkingFacilities.Count < 1)
                return;

            foreach(IParkingFacility facility in this.ParkingFacilities)
            {
                if (facility.Location != null)
                {
                    facility.Distance = this.locationUtilService.Distance(facility.Location, currentLocation);
                }
            }
        }


        /// <summary>
        /// Select parking facility
        /// </summary>
        /// <param name="parkingFacility"></param>
        public void SelectParkingFacility(IParkingFacility parkingFacility, TappedRoutedEventArgs eventArgs)
        {
            System.Diagnostics.Debug.WriteLine("MapPageViewModel, SelectParkingFacility");

            if (parkingFacility != null)
            {
                if (eventArgs != null)
                {
                    eventArgs.Handled = true;
                }

                this.SelectedParkingFacility = parkingFacility;

                foreach (IParkingFacility facility in this.ParkingFacilities)
                {
                    if (facility.Id == this.SelectedParkingFacility.Id)
                    {
                        facility.IsSelected = true;
                    }

                    else
                    {
                        facility.IsSelected = false;
                    }
                }
            }

            //this.ParkingFacilities.Select(f => { f.IsSelected = false; return f; }).ToList();
            //this.ParkingFacilities.FirstOrDefault(f => f.Id == parkingFacility.Id).IsSelected = true;
        }


        /// <summary>
        /// Unselect parking facility, facility = null
        /// </summary>
        public void UnselectParkingFacility()
        {
            System.Diagnostics.Debug.WriteLine("MapPageViewModel, UnselectParkingFacility");

            this.ParkingFacilities.Select(f => { f.IsSelected = false; return f; }).ToList();
            this.SelectedParkingFacility = null;
        }


        /// <summary>
        /// Navigate to selected parking facility, SelectedParkingFacility need to be set first
        /// </summary>
        public async void NavigateToSelectedParkingFacility()
        {
            if(this.SelectedParkingFacility != null)
            {
                ResourceLoader resLoader = new Windows.ApplicationModel.Resources.ResourceLoader();
                bool dialogRes = await this.dialogService.ShowMessageDialog(resLoader.GetString("DialogStartNavigationMessage") + " " + this.SelectedParkingFacility.Name + "?", resLoader.GetString("DialogStartNavigationTitle"), resLoader.GetString("ButtonYes"), resLoader.GetString("ButtonNo"));

                if (dialogRes)
                {
                    string latitude = this.SelectedParkingFacility.Location.Position.Latitude.ToString(CultureInfo.InvariantCulture);
                    string longitude = this.SelectedParkingFacility.Location.Position.Longitude.ToString(CultureInfo.InvariantCulture);
                    string name = this.SelectedParkingFacility.Name;

                    Uri uri = new Uri("ms-drive-to:?destination.latitude=" + latitude + "&destination.longitude=" + longitude + "&destination.name=" + name);

                    var success = await Windows.System.Launcher.LaunchUriAsync(uri);

                    if (!success)
                    {
                        System.Diagnostics.Debug.WriteLine("MapPageViewModel, Failed to get driving directions: " + uri);
                    }
                }
            }
        }


        /// <summary>
        /// Open parking facilities list page
        /// </summary>
        public void OpenListPage()
        {
            this.navigationService.NavigateToViewModel<ListPageViewModel>(this.ParkingFacilities);
        }


        /// <summary>
        /// Open about and settings
        /// </summary>
        public void OpenAboutPage()
        {
            this.navigationService.NavigateToViewModel<AboutPageViewModel>();
        }


        /// <summary>
        /// Open settings page
        /// </summary>
        public void OpenSettingsPage()
        {
            this.navigationService.NavigateToViewModel<SettingsPageViewModel>();
        }

    }
}
