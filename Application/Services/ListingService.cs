using System.ComponentModel.DataAnnotations;
using Application.DTOs;
using Application.DTOs.Auth;
using Application.DTOs.Listings;
using Application.Enums;
using Application.Exceptions;
using Application.Interfaces.Persistence;
using Application.Interfaces.Persistence.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class ListingService(
    IVehicleListingRepository listings,
    IFileStorage fileStorage,
    IFileUploadStrategyFactory uploadStrategyFactory,
    IFileHashService hashService,
    IUnitOfWork unitOfWork
) : IListingService
{
    public async Task<int> CreateAndPublish(int userId, PublishedVehicleListingCommand dto)
    {
        var listing = new VehicleListing
        {
            UserId = userId,
            ModelId = dto.ModelId,
            BodyTypeId = dto.BodyTypeId,
            ConditionId = dto.ConditionId,
            ColorHex = dto.ColorHex,
            CityId = dto.CityId,
            Year = dto.Year,
            Description = dto.Description,
            Mileage = dto.Mileage,
            HasAccident = dto.HasAccident,
            Price = dto.Price,
            Number = dto.Number
        };

        if (dto.Photos != null)
            await AddPhotos(userId, listing, dto.Photos);

        listing.IsPublished = true;

        await listings.AddAsync(listing);
        await unitOfWork.SaveChangesAsync();

        return listing.Id;
    }

    public async Task UpdatePublished(int userId, int listingId, DraftVehicleListingCommand request)
    {
        await ApplyDraft(userId, listingId, request);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<int> CreateDraft(int userId, DraftVehicleListingCommand dto)
    {
        var listing = new VehicleListing
        {
            UserId = userId,
            ModelId = dto.ModelId,
            BodyTypeId = dto.BodyTypeId,
            ConditionId = dto.ConditionId,
            ColorHex = dto.ColorHex,
            CityId = dto.CityId,
            Year = dto.Year,
            Description = dto.Description,
            Mileage = dto.Mileage,
            HasAccident = dto.HasAccident,
            Price = dto.Price,
            Number = dto.Number
        };

        if (dto.Photos != null)
            await AddPhotos(userId, listing, dto.Photos);

        listing.IsPublished = false;

        await listings.AddAsync(listing);
        await unitOfWork.SaveChangesAsync();

        return listing.Id;
    }

    public async Task UpdateDraft(int userId, int listingId, DraftVehicleListingCommand dto)
    {
        await ApplyDraft(userId, listingId, dto);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task PublishDraft(int userId, int listingId, PublishedVehicleListingCommand request)
    {
        var listing = await GetUserListing(listingId, userId);

        if (listing.IsPublished)
            throw new ValidationException("Listing is already published");


        var draftCommand = new DraftVehicleListingCommand
        {
            ModelId = request.ModelId,
            BodyTypeId = request.BodyTypeId,
            ConditionId = request.ConditionId,
            CityId = request.CityId,
            Year = request.Year,
            Mileage = request.Mileage,
            Number = request.Number,
            ColorHex = request.ColorHex,
            Price = request.Price,
            Description = request.Description,
            HasAccident = request.HasAccident,
            Photos = request.Photos
        };

        await ApplyDraft(userId, listing, draftCommand);

        listing.IsPublished = true;
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteListing(int userId, int listingId)
    {
        var listing = await GetUserListing(listingId, userId);
        listings.Remove(listing);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<VehicleListingResponse>> GetPublishedListings()
    {
        return await listings.GetPublishedListingsAsync();
    }

    public async Task<IEnumerable<VehicleListingResponse>> GetUserListings(int userId)
    {
        return await listings.GetUserListingsAsync(userId);
    }

    public async Task<VehicleListingResponse> GetListingById(int id)
    {
        VehicleListing? listing = await listings.GetByIdAsync(id);

        if (listing == null)
            throw new NotFoundException("Listing not found");

        return VehicleListingMapper.ToResponseDto(listing);
    }

    private async Task ApplyDraft(int userId, int listingId, DraftVehicleListingCommand dto)
    {
        var listing = await GetUserListing(listingId, userId);
        await ApplyDraft(userId, listing, dto);
    }

    private async Task ApplyDraft(int userId, VehicleListing listing, DraftVehicleListingCommand dto)
    {
        if (dto.ModelId.HasValue) listing.ModelId = dto.ModelId;
        if (dto.BodyTypeId.HasValue) listing.BodyTypeId = dto.BodyTypeId;
        if (dto.ConditionId.HasValue) listing.ConditionId = dto.ConditionId;
        if (dto.CityId.HasValue) listing.CityId = dto.CityId;
        if (dto.Year.HasValue) listing.Year = dto.Year;
        if (dto.Mileage.HasValue) listing.Mileage = dto.Mileage;
        if (!string.IsNullOrEmpty(dto.Number)) listing.Number = dto.Number;
        if (!string.IsNullOrEmpty(dto.ColorHex)) listing.ColorHex = dto.ColorHex;
        if (dto.Price.HasValue) listing.Price = dto.Price;
        if (!string.IsNullOrEmpty(dto.Description)) listing.Description = dto.Description;
        if (dto.HasAccident.HasValue) listing.HasAccident = dto.HasAccident.Value;
        if (dto.Photos != null && dto.Photos.Count != 0) await AddPhotos(userId, listing, dto.Photos);
    }

    private async Task AddPhotos(int userId, VehicleListing listing, IEnumerable<FileDto> photos)
    {
        foreach (var photoDto in photos)
        {
            string hash = hashService.ComputeHash(photoDto.Stream);

            if (listing.Photos.Any(x => x.Hash == hash))
                continue;

            var uploadStrategy = uploadStrategyFactory.CreateFileUploadStrategy(PhotoCategory.Listing);
            string photoUrl = await fileStorage.Upload(uploadStrategy, photoDto.Stream, photoDto.Name, userId);

            listing.Photos.Add(new VehiclePhoto
            {
                PhotoUrl = photoUrl,
                VehicleListing = listing,
                Hash = hash
            });
        }
    }

    private async Task<VehicleListing> GetUserListing(int listingId, int userId)
    {
        var listing = await listings.GetByIdAsync(listingId);
        if (listing == null)
            throw new NotFoundException("Listing not found");

        if (listing.UserId != userId)
            throw new ValidationException("Listing does not belong to user");

        return listing;
    }
}