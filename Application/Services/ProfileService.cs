using Application.DTOs.Profile;
using Application.Enums;
using Application.Exceptions;
using Application.Interfaces.Persistence;
using Application.Interfaces.Persistence.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class ProfileService(
    IUserRepository users,
    IFileStorage fileStorage,
    IUnitOfWork unitOfWork,
    IFileUploadStrategyFactory uploadStrategyFactory,
    IFileHashService hashService
) : IProfileService
{
    public async Task<User> GetUser(int userId)
    {
        User? user = await users.GetByIdAsync(userId);
        return user ?? throw new NotFoundException("User not found");
    }

    public async Task UpdateProfile(int userId, UpdateProfileDto dto)
    {
        var user = await users.GetByIdAsync(userId);

        if (user == null)
            throw new NotFoundException("User not found");

        user.FirstName = dto.FirstName ?? user.FirstName;
        user.LastName = dto.LastName ?? user.LastName;
        user.Country = dto.Country ?? user.Country;
        user.AboutYourself = dto.AboutYourself ?? user.AboutYourself;
        user.Address = dto.Address ?? user.Address;
        user.DateOfBirth = dto.DateOfBirth ?? user.DateOfBirth;
        user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;

        if (dto.Photo != null)
        {
            string avatarHash = hashService.ComputeHash(dto.Photo.Stream);

            if (user.Avatar is { IsExternal: false } && user.Avatar.Hash == avatarHash)
            {
                await unitOfWork.SaveChangesAsync();
                return;
            }

            IFileUploadStrategy uploadStrategy = uploadStrategyFactory.CreateFileUploadStrategy(PhotoCategory.Avatar);

            string avatarUrl = await fileStorage.Upload(uploadStrategy, dto.Photo.Stream, dto.Photo.Name, userId);

            user.Avatar ??= new UserAvatar();

            user.Avatar.Url = avatarUrl;
            user.Avatar.Hash = avatarHash;
            user.Avatar.IsExternal = false;
        }

        await unitOfWork.SaveChangesAsync();
    }
}