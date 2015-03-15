using Parkkis.Models;
using Parkkis.Models.Oulu;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;
using Windows.Web.Http;
using Windows.Devices.Geolocation;
using System.Collections.ObjectModel;
using Parkkis.Services.Utils;
using Caliburn.Micro;
using System.Text.RegularExpressions;

namespace Parkkis.Services.Oulu
{
    public class OuluParkingFacilityService : IParkingFacilityService
    {
        private readonly string baseUrl = "http://www.oulunliikenne.fi/rss/parking/parking.xml";

        XNamespace dcNs = "http://purl.org/dc/elements/1.1/";
        XNamespace georssNs = "http://www.georss.org/georss";

        /// <summary>
        /// Get Oulu parking facilities
        /// 
        /// Details http://www.ouka.fi/oulu/oulu-tietoa/avoin-data/-/asset_publisher/Wz43/content/pysakointitalojen-tiedot
        /// Facility locations http://www.infotripla.fi/oulunliikenne/rajapinnat/parking.csv
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<IParkingFacility>> GetParkingFacilitiesAsync()
        {
            InfoUtilService infoUtil = IoC.Get<InfoUtilService>();

            try
            {
                // Load data from web api
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", infoUtil.UserAgent);

                var response = await client.GetAsync(new Uri(this.baseUrl));
                response.EnsureSuccessStatusCode();

                string xmlContent = await response.Content.ReadAsStringAsync();

                return new ReadOnlyCollection<IParkingFacility>(ParseXMLContent(xmlContent));
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Oulu GetParkingFacilitiesAsync Error - " + ex.ToString());
                return null;
            }
        }


        /// <summary>
        /// Parse xml content to objects
        /// </summary>
        /// <param name="xmlContent"></param>
        /// <returns></returns>
        private List<IParkingFacility> ParseXMLContent(string xmlContent)
        {
            List<IParkingFacility> newParkingFacilities = new List<IParkingFacility>();
            List<XElement> parkingItems = new List<XElement>();

            // Parse xml document
            XDocument xmlDoc = XDocument.Parse(xmlContent);
            parkingItems = xmlDoc.Descendants("item").ToList();
            
            // Loop all parking houses and build list of parking house objects
            foreach (var parkItem in parkingItems)
            {
                try
                {
                    OuluParkingFacility newPark = new OuluParkingFacility()
                    {
                        Id = ParseId(parkItem),
                        Name = ParseName(parkItem),
                        Address = ParseAddress(parkItem),
                        Location = ParseLocation(parkItem),
                        FreeSpace = ParseFreeSpaces(parkItem),
                        TotalSpace = ParseTotalSpaces(parkItem),
                        Updated = ParseDate(parkItem),
                    };
                    newParkingFacilities.Add(newPark);
                }

                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Oulu GetParkingFacilitiesAsync ParseXMLContent - " + ex.ToString());
                }
            }

            return newParkingFacilities;
        }


        /// <summary>
        /// Get updated time
        /// </summary>
        /// <param name="parkItem"></param>
        /// <returns></returns>
        private string ParseId(XElement parkItem)
        {
            // Use title as id
            return parkItem.Element("title").Value;
        }


        /// <summary>
        /// Get updated time
        /// </summary>
        /// <param name="parkItem"></param>
        /// <returns></returns>
        private DateTime ParseDate(XElement parkItem)
        {
            return DateTime.Parse(parkItem.Element(this.dcNs + "date").Value);
        }


        /// <summary>
        /// Get parking facility name
        /// </summary>
        /// <param name="parkItem"></param>
        /// <returns></returns>
        private string ParseName(XElement parkItem)
        {
            string title = parkItem.Element("title").Value;

            if(String.IsNullOrWhiteSpace(title))
            {
                // We need to get the name
                throw new ArgumentNullException("title");
            }

            string[] parts = title.Split(new string[] { " - " }, StringSplitOptions.None);

            return parts[0].Trim();
        }


        /// <summary>
        /// Get address
        /// </summary>
        /// <param name="parkItem"></param>
        /// <returns></returns>
        private string ParseAddress(XElement parkItem)
        {
            string title = parkItem.Element("title").Value;

            if (String.IsNullOrWhiteSpace(title))
            {
                return String.Empty;
            }

            string[] parts = title.Split(new string[] { " - " }, StringSplitOptions.None);

            return parts[1].Trim();
        }


        /// <summary>
        /// Get Geopoint location
        /// </summary>
        /// <param name="parkItem"></param>
        /// <returns></returns>
        private Geopoint ParseLocation(XElement parkItem)
        {
            string location = parkItem.Element( this.georssNs + "point").Value;
            string[] locationParts = location.Split(' ');

            double latitude = double.Parse(locationParts[1], System.Globalization.CultureInfo.InvariantCulture);
            double longitude = double.Parse(locationParts[0], System.Globalization.CultureInfo.InvariantCulture);

            Geopoint locatoin = new Geopoint(
                            new BasicGeoposition()
                            {
                                Latitude = latitude,
                                Longitude = longitude
                            });

            return locatoin;
        }


        /// <summary>
        /// Get free space, if available
        /// </summary>
        /// <param name="parkItem"></param>
        /// <returns></returns>
        private int? ParseFreeSpaces(XElement parkItem)
        {
            try
            {
                string description = parkItem.Element("description").Value;
                string[] descriptionParts = description.Split(',');
                string[] freeSpaceParts = descriptionParts[0].Split(':');

                if (freeSpaceParts[1].Trim() == "ei tietoa.")
                    return null;

                else
                    return Convert.ToInt32(freeSpaceParts[1].Trim());
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Oulu GetParkingFacilitiesAsync ParseFreeSpace - " + ex.ToString());
                return null;
            }
        }


        /// <summary>
        /// Get total space, if available
        /// </summary>
        /// <param name="parkItem"></param>
        /// <returns></returns>
        private int? ParseTotalSpaces(XElement parkItem)
        {
            try
            {
                string description = parkItem.Element("description").Value;
                string[] descriptionParts = description.Split(',');
                string[] freeSpaceParts = descriptionParts[1].Split(':');

                if (freeSpaceParts[1].Trim() == "ei tietoa.")
                    return null;

                else
                    return Convert.ToInt32(freeSpaceParts[1].Trim());
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Oulu GetParkingFacilitiesAsync ParseTotalSpace - " + ex.ToString());
                return null;
            }
        }



    }
}
