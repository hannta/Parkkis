using Parkkis.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Parkkis.Services
{
    public interface IParkingFacilityService
    {
        /// <summary>
        /// Get parking facilities
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyCollection<IParkingFacility>> GetParkingFacilitiesAsync();

    }
}
