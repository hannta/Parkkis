using Parkkis.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;
using Parkkis.Models.Tampere;
using Windows.Devices.Geolocation;
using Parkkis.Services.Utils;
using Caliburn.Micro;

namespace Parkkis.Services.Tampere
{
    /// <summary>
    /// Tampere Parking Facility Service
    /// </summary>
    public class TampereParkingFacilityService : IParkingFacilityService
    {
        private readonly string baseUrl = "http://parkingdata.finnpark.fi:8080/Datex2/OpenData";

        // Namespaces
        XNamespace xsiNs = "http://www.w3.org/2001/XMLSchema-instance";
        XNamespace ns = "http://datex2.eu/schema/2/2_0";


        /// <summary>
        /// Get parking facilities
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<IParkingFacility>> GetParkingFacilitiesAsync()
        {
            try
            {
                InfoUtilService infoUtil = IoC.Get<InfoUtilService>();

                // Load data from web api
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", infoUtil.UserAgent);

                var response = await client.GetAsync(new Uri(this.baseUrl));
                response.EnsureSuccessStatusCode();

                string xmlContent = await response.Content.ReadAsStringAsync();

                return new ReadOnlyCollection<TampereParkingFacility>(ParseXMLContent(xmlContent));
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Tampere GetParkingFacilitiesAsync Error - " + ex.ToString());
                return null;
            }
        }


        /// <summary>
        /// Parse xml content
        /// </summary>
        /// <param name="xmlContent"></param>
        /// <returns></returns>
        private IList<TampereParkingFacility> ParseXMLContent(string xmlContent)
        {
            IList<TampereParkingFacility> newParkingFacilities = new List<TampereParkingFacility>();

            XDocument xmlDoc = XDocument.Parse(xmlContent);

            foreach (var parkingFacility in xmlDoc.Descendants(this.ns + "parkingFacility").ToList())
            {
                try
                {
                    TampereParkingFacility newFacility = new TampereParkingFacility()
                    {
                        Id = parkingFacility.Attribute("id").Value.ToString(),
                        Name = parkingFacility.Element(this.ns + "parkingFacilityName").Value.ToString(),
                        Address = ParseFacilityAddress(parkingFacility),
                        Location = ParseFacilityLocation(parkingFacility),
                    };

                    newParkingFacilities.Add(newFacility);
                }

                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Tampere GetParkingFacilitiesAsync ParseXMLContent - " + ex.ToString());
                }
            }

            return ParseFacilitiesStats(newParkingFacilities, xmlDoc);
        }


        /// <summary>
        /// Get parking facility address
        /// </summary>
        /// <param name="parkItem"></param>
        /// <returns></returns>
        private string ParseFacilityAddress(XElement parkItem)
        {
            string address = String.Empty;

            // <xs:element name="carParkDetails" type="D2LogicalModel:TaggedValue" minOccurs="0" maxOccurs="unbounded"> 
            foreach (var details in parkItem.Elements(this.ns + "carParkDetails"))
            {
                if (String.Equals(details.Element(this.ns + "tag").Value.ToString(), "Address", StringComparison.OrdinalIgnoreCase))
                {
                    address = details.Element(this.ns + "value").Value.ToString();
                }
            }

            return address;
        }


        /// <summary>
        /// Get parking facility location
        /// </summary>
        /// <param name="parkItem"></param>
        /// <returns></returns>
        private Geopoint ParseFacilityLocation(XElement parkItem)
        {
            var entrance = parkItem.Element(this.ns + "entranceLocation");

            if(entrance != null)
            {
                // Just take first point
                var entranceCoor = entrance.Descendants(this.ns + "pointCoordinates").FirstOrDefault();

                string latString = entranceCoor.Element(this.ns + "latitude").Value.ToString();
                double latitude = double.Parse(latString, System.Globalization.CultureInfo.InvariantCulture);

                string lonString = entranceCoor.Element(this.ns + "longitude").Value.ToString();
                double longitude = double.Parse(lonString, System.Globalization.CultureInfo.InvariantCulture);

                Geopoint locatoin = new Geopoint(
                    new BasicGeoposition()
                    {
                        Latitude = latitude,
                        Longitude = longitude
                    });

                return locatoin;
            }

            return null;
        }


        /// <summary>
        /// Add facility status data to parking facilities
        /// </summary>
        /// <param name="ParkingFacilities"></param>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        private IList<TampereParkingFacility> ParseFacilitiesStats(IList<TampereParkingFacility> parkingFacilities, XDocument xmlDoc)
        {
            var statusTable = xmlDoc.Descendants(this.ns + "parkingFacilityTableStatusPublication").FirstOrDefault();

            // Just to make sure we have parkingFacilityTableStatusPublication
            if (statusTable == null)
                return parkingFacilities;


            foreach (var facilityStatus in statusTable.Elements(this.ns + "parkingFacilityStatus").ToList())
            {
                try
                {
                    // Get parking facility object, to append data
                    string id = facilityStatus.Element(this.ns + "parkingFacilityReference").Attribute("id").Value.ToString();
                    TampereParkingFacility tampereFacility = parkingFacilities.Where(f => f.Id == id).FirstOrDefault();

                    if (tampereFacility != null)
                    {
                        tampereFacility.Updated = ParseUpdatedTime(facilityStatus);
                        tampereFacility.Status = ParseStatus(facilityStatus);
                    }
                }

                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Tampere GetParkingFacilitiesAsync ParseFacilitiesStats - " + ex.ToString());
                }
            }

            return parkingFacilities;

        }


        /// <summary>
        /// Parse updated time
        /// </summary>
        /// <param name="facilityStatus"></param>
        /// <returns></returns>
        private DateTime ParseUpdatedTime(XElement facilityStatus)
        {
            string timeString = facilityStatus.Element(this.ns + "parkingFacilityStatusTime").Value.ToString();

            return DateTime.Parse(timeString);
        }


        /// <summary>
        /// Parse facility stats
        /// </summary>
        /// <param name="facilityStatus"></param>
        /// <returns></returns>
        private ParkingFacilityStatus ParseStatus(XElement facilityStatus)
        {
            // Check if facility is closed
            if (facilityStatus.Elements(this.ns + "parkingFacilityStatus").Where(x => x.Value.ToString() == "closed").Any())
            {
                return ParkingFacilityStatus.Closed;
            }

            foreach (var status in facilityStatus.Elements(this.ns + "parkingFacilityStatus").ToList())
            {
                if (status.Value.ToString() == "spacesAvailable")
                {
                    return ParkingFacilityStatus.SpacesAvailable;
                }

                else if (status.Value.ToString() == "full")
                {
                    return ParkingFacilityStatus.Full;
                }

                else if (status.Value.ToString() == "almostFull")
                {
                    return ParkingFacilityStatus.AlmostFull;
                }
            }

            // If we still do not have status, check if it is just "Open"
            if (facilityStatus.Elements(this.ns + "parkingFacilityStatus").Where(x => x.Value.ToString() == "open").Any())
            {
                return ParkingFacilityStatus.Open;
            }

            else
            {
                return ParkingFacilityStatus.Unknown;
            }
        }



    }
}
