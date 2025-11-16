using Application.Interfaces.Services; 
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Auth;

namespace Application.Services
{
    public class ListingService : IListingService
    {
        public Task<VehicleListingResponse> CreateListingAsync(CreateVehicleListingRequest request, int userId)
        {
            throw new NotImplementedException(); 
        }

        public Task DeleteListingAsync(int listingId, int userId)
        {
            throw new NotImplementedException();
        }
        public Task<VehicleListingResponse> GetListingByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<VehicleListingResponse>> GetListingsAsync()
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<VehicleListingResponse>> GetUserListingsAsync(int userId)
        {
            throw new NotImplementedException();
        }
        public Task UpdateListingAsync(int listingId, UpdateVehicleListingRequest request, int userId)
        {
            throw new NotImplementedException();
        }
    }
}