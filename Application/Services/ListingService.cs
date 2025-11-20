using System.ComponentModel.DataAnnotations;
using Application.DTOs;
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
    IUnitOfWork unitOfWork,
    IVehiclePhotoRepository photos
) : IListingService
{
    public async Task<int> CreateAndPublish(int userId, PublishedVehicleListingCommand dto)
    {
        bool isValid = await listings.IsBodyTypeValidForModel(dto.ModelId, dto.BodyTypeId);
        if (!isValid)
            throw new ValidationException("Selected body type is not compatible with the chosen model.");

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
            Number = dto.Number,
            GearTypeId = dto.GearTypeId,
            FuelTypeId = dto.FuelTypeId,
            IsPublished = true
        };

        await listings.AddAsync(listing);
        await unitOfWork.SaveChangesAsync();
        
        await UpdatePhotos(listing, userId, newPhotos: dto.NewPhotos);

        await unitOfWork.SaveChangesAsync();

        return listing.Id;
    }

    public async Task UpdatePublished(int userId, int listingId, DraftVehicleListingCommand request)
    {
        if (request is { ModelId: not null, BodyTypeId: not null })
        {
            bool isValid = await listings.IsBodyTypeValidForModel(request.ModelId.Value, request.BodyTypeId.Value);
            if (!isValid)
                throw new ValidationException("Selected body type is not compatible with the chosen model.");
        }

        await ApplyDraft(userId, listingId, request);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<int> CreateDraft(int userId, DraftVehicleListingCommand dto)
    {
        if (dto is { ModelId: not null, BodyTypeId: not null })
        {
            bool isValid = await listings.IsBodyTypeValidForModel(dto.ModelId.Value, dto.BodyTypeId.Value);
            if (!isValid)
                throw new ValidationException("Selected body type is not compatible with the chosen model.");
        }

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
            FuelTypeId = dto.FuelTypeId,
            GearTypeId = dto.GearTypeId,
            Number = dto.Number
        };

        if (dto.NewPhotos != null)
            await AddPhotos(userId, listing, dto.NewPhotos);

        listing.IsPublished = false;

        await listings.AddAsync(listing);
        await unitOfWork.SaveChangesAsync();

        return listing.Id;
    }

    public async Task UpdateDraft(int userId, int listingId, DraftVehicleListingCommand dto)
    {
        if (dto is { ModelId: not null, BodyTypeId: not null })
        {
            bool isValid = await listings.IsBodyTypeValidForModel(dto.ModelId.Value, dto.BodyTypeId.Value);
            if (!isValid)
                throw new ValidationException("Selected body type is not compatible with the chosen model.");
        }

        await ApplyDraft(userId, listingId, dto);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task PublishDraft(int userId, int listingId, PublishedVehicleListingCommand request)
    {
        bool isValid = await listings.IsBodyTypeValidForModel(request.ModelId, request.BodyTypeId);

        if (!isValid)
            throw new ValidationException("Selected body type is not compatible with the chosen model.");

        var listing = await GetUserListing(listingId, userId);

        if (listing.IsPublished)
            throw new ValidationException("Listing is already published");

        // var newPhotos = request.Photos?
        //     .Where(x => listing.Photos.All(y =>
        //         y.Hash != hashService.ComputeHash(x.Stream)));
        //
        // var photosToRemove = listing.Photos
        //     .Where(x => request.Photos != null && request.Photos.All(y => x.Hash != hashService.ComputeHash(y.Stream)))
        //     .Select(x => x.Id);

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
            NewPhotos = request.NewPhotos,
            PhotosToRemove = request.PhotosToRemove,
            UpdatedPhotoSortOrder = request.UpdatedPhotoSortOrder,
            GearTypeId = request.GearTypeId,
            FuelTypeId = request.FuelTypeId,
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

    public async Task<IEnumerable<VehicleListingResponse>> GetPublishedListings(VehicleListingFilter? filter = null)
    {
        List<VehicleListing> filtered = await listings.GetPublishedListingsAsync(filter);
        return filtered.Select(VehicleListingMapper.ToResponseDto);
    }

    public async Task<IEnumerable<VehicleListingResponse>> GetUserListings(int userId)
    {
        return (await listings.GetUserListingsAsync(userId)).Select(VehicleListingMapper.ToResponseDto);
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
        if (dto.Number != null) listing.Number = dto.Number;
        if (dto.ColorHex != null) listing.ColorHex = dto.ColorHex;
        if (dto.Price.HasValue) listing.Price = dto.Price;
        if (dto.Description != null) listing.Description = dto.Description;
        if (dto.HasAccident.HasValue) listing.HasAccident = dto.HasAccident.Value;
        if (dto.GearTypeId.HasValue) listing.GearTypeId = dto.GearTypeId;
        if (dto.FuelTypeId.HasValue) listing.FuelTypeId = dto.FuelTypeId;

        await UpdatePhotos(listing, userId, dto.NewPhotos, dto.PhotosToRemove, dto.UpdatedPhotoSortOrder);
    }

    private async Task UpdatePhotos(VehicleListing listing, int userId, IEnumerable<OrderedFileDto>? newPhotos = null,
        IEnumerable<int>? photosToRemove = null, IEnumerable<ListingPhotoSortOrder>? updatedPhotoSortOrder = null)
    {
        if (photosToRemove != null) await DeletePhotos(listing, photosToRemove);
        if (newPhotos != null) await AddPhotos(userId, listing, newPhotos);
        if (updatedPhotoSortOrder != null) await UpdateSortOrder(listing, updatedPhotoSortOrder);

        listing.Photos = listing.Photos
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .ToList();
    }

    private async Task AddPhotos(int userId, VehicleListing listing, IEnumerable<OrderedFileDto> photoDtos)
    {
        int availablePhotos = 10 - listing.Photos.Count;

        if (availablePhotos <= 0)
            throw new ValidationException("Maximum 10 photos are allowed");

        foreach (var photoDto in photoDtos.Take(availablePhotos))
        {
            string hash = hashService.ComputeHash(photoDto.File.Stream);

            if (listing.Photos.Any(x => x.Hash == hash))
                continue;

            var uploadStrategy = uploadStrategyFactory.CreateFileUploadStrategy(PhotoCategory.Listing);
            FileUploadResult uploadResult =
                await fileStorage.Upload(uploadStrategy, photoDto.File.Stream, photoDto.File.Name, userId);

            listing.Photos.Add(new VehiclePhoto
            {
                PhotoUrl = uploadResult.Url,
                PublicId = uploadResult.PublicId,
                VehicleListing = listing,
                Hash = hash,
                SortOrder = photoDto.SortOrder,
            });
        }
    }

    private async Task DeletePhotos(VehicleListing listing, IEnumerable<int> photosToRemove)
    {
        List<VehiclePhoto> toRemove = listing.Photos.Where(p => photosToRemove.Contains(p.Id)).ToList();
        foreach (var photo in toRemove)
        {
            if (!string.IsNullOrEmpty(photo.PublicId))
                await fileStorage.Delete(photo.PublicId);

            listing.Photos.Remove(photo);
        }
    }

    private async Task UpdateSortOrder(VehicleListing listing, IEnumerable<ListingPhotoSortOrder> updateSortOrder)
    {
        foreach (var photo in updateSortOrder)
        {
            var listingPhoto = listing.Photos.FirstOrDefault(x => x.Id == photo.PhotoId);

            if (listingPhoto != null)
                listingPhoto.SortOrder = photo.SortOrder;
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