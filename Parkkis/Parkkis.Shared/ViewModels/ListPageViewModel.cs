using Caliburn.Micro;
using Parkkis.Models;
using Parkkis.Models.Oulu;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parkkis.ViewModels
{
    public class ListPageViewModel : Screen
    {
        private readonly INavigationService navigationService;
        private readonly IEventAggregator eventAggregator;

        public ListPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            this.navigationService = navigationService;
            this.eventAggregator = eventAggregator;
        }

        public ListPageViewModel()
        {
            // Add design time data
            if (Execute.InDesignMode)
            {
                this.ParkingFacilities = new List<OuluParkingFacility>
                {
                    new OuluParkingFacility
                    {
                        Name = "Parkkis eka",
                        Address = "Hämeenjoenkatu 4, 5466 Laaksolaaaa",
                        Location = null, 
                        FreeSpace = 53,
                        TotalSpace = 200,
                        Updated = DateTime.Now,
                        Status = ParkingFacilityStatus.SpacesAvailable,
                        Distance = 100,
                        IsSelected = false,
                    },

                    new OuluParkingFacility
                    {
                        Name = "Parkkis toka",
                        Address = "Murole 33, 5466 Laaksolaaaa",
                        Location = null, 
                        FreeSpace = 1,
                        TotalSpace = 50,
                        Updated = DateTime.Now,
                        Status = ParkingFacilityStatus.Closed,
                        Distance = 4366,
                        IsSelected = false,
                    }
                };


            }
        }


        // Navigation parameter
        public IReadOnlyCollection<IParkingFacility> Parameter { get; set; }


        // Parking facilities collection
        private IReadOnlyCollection<IParkingFacility> _parkingFacilities;
        public IReadOnlyCollection<IParkingFacility> ParkingFacilities
        {
            get { return _parkingFacilities; }
            set
            {
                _parkingFacilities = value;
                NotifyOfPropertyChange(() => ParkingFacilities);
            }
        }


        /// <summary>
        /// Page on active
        /// </summary>
        protected override void OnActivate()
        {
            ParkingFacilities = Parameter;
        }


    }
}
