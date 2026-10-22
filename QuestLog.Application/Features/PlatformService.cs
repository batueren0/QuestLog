using FluentValidation;
using QuestLog.Application.Common;
using QuestLog.Application.Contracts.Services;
using QuestLog.Application.DTOs.Platforms;
using QuestLog.Domain.Entities;

namespace QuestLog.Application.Features;

public class PlatformService(IUnitOfWork uow,
    IValidator<CreatePlatformRequest> createValidator,
    IValidator<UpdatePlatformRequest> updateValidator
    ) : IPlatformService
{
    public async Task<Result<PlatformResponse>> CreateAsync(CreatePlatformRequest request)
    {
        var validationResult = await createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result<PlatformResponse>.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        if (await uow.Platforms.NameExistsAsync(request.Name))
            return Result<PlatformResponse>.Failure(ErrorType.Conflict, "A platform with this name already exists.");


        var platform = new Platform
        {
            Name = request.Name,
            Slug = request.Name.ToKebabCase()
        };
        await uow.Platforms.AddAsync(platform);
        await uow.SaveChangesAsync();

        return Result<PlatformResponse>.Success(ToResponse(platform));
    }

    public async Task<Result> UpdateAsync(Guid id, UpdatePlatformRequest request)
    {
        // 1) İstediğin biçim bozuk mu? > 400
        var validationResult = await updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        // 2) Güncellenecek kayıt var mı? > 404
        var platform = await uow.Platforms.GetByIdAsync(id);
        if (platform is null)
            return Result.Failure(ErrorType.NotFound, "Platform not found.");

        // 3) yeni isim başka bir platformda kullanılıyor mu? > 409
        if (await uow.Platforms.NameExistsAsync(request.Name, id))
            return Result.Failure(ErrorType.Conflict, "A platform with this name already exists.");

        // 4) bütün kontrollar geçti: değiştir ve kaydet.
        platform.Name = request.Name;
        platform.Slug = request.Name.ToKebabCase();

        uow.Platforms.Update(platform);
        await uow.SaveChangesAsync();

        return Result.Success();

    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var platform = await uow.Platforms.GetByIdAsync(id);
        if (platform is null)
            return Result.Failure(ErrorType.NotFound, "Platform not found.");

        if (await uow.Platforms.HasGamesAsync(id))
            return Result.Failure(ErrorType.Conflict, "This platform has games and cannot be deleted.");

        uow.Platforms.Delete(platform);
        await uow.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<IReadOnlyList<PlatformResponse>> GetAllAsync()
    {
        var platforms = await uow.Platforms.GetAllAsync();

        return platforms.Select(ToResponse).ToList();
    }

    public async Task<Result<PlatformResponse>> GetByIdAsync(Guid id)
    {
        var platform = await uow.Platforms.GetByIdAsync(id);
        if (platform is null)
            return Result<PlatformResponse>.Failure(ErrorType.NotFound, "Platform not found.");

        return Result<PlatformResponse>.Success(ToResponse(platform));
    }


    private static PlatformResponse ToResponse(Platform platform) => new(platform.Id, platform.Name, platform.Slug);

}