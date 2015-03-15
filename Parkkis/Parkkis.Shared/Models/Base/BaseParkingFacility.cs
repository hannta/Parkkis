using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace Parkkis.Models.Base
{
    public class BaseParkingFacility : PropertyChangedBase, IParkingFacility
    {
        public virtual string Id
        {
            get;
            set;
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual string Address
        {
            get;
            set;
        }

        public virtual Geopoint Location
        {
            get;
            set;
        }

        public virtual int? FreeSpace
        {
            get;
            set;
        }

        public virtual int? TotalSpace
        {
            get;
            set;
        }


        public virtual DateTime Updated
        {
            get;
            set;
        }


        public virtual ParkingFacilityStatus Status
        {
            get;
            set;
        }


        private double? _distance;
        public virtual double? Distance
        {
            get { return _distance; }
            set
            {
                _distance = value;
                NotifyOfPropertyChange(() => Distance);
            }
        }


        private bool _isSelected;
        public virtual bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }


        public virtual Point NormalizedAnchorPoint
        {
            get
            {
                return new Point(0.5, 1);
            }
        }

    }
}
