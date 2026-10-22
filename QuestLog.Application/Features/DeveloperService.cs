using FluentValidation;
using QuestLog.Application.Common;
using QuestLog.Application.Contracts.Services;
using QuestLog.Application.DTOs.Developers;
using QuestLog.Domain.Entities;

namespace QuestLog.Application.Features;

public class DeveloperService(
    IUnitOfWork uow,
    IValidator<CreateDeveloperRequest> createValidator,
    IValidator<UpdateDeveloperRequest> updateValidator
    ) : IDeveloperService
{
    public async Task<Result<DeveloperResponse>> CreateAsync(CreateDeveloperRequest request)
    {
        var validationResult = await createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result<DeveloperResponse>.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        if (await uow.Developers.NameExistsAsync(request.Name))
            return Result<DeveloperResponse>.Failure(ErrorType.Conflict, "A developer with this name already exists.");


        var developer = new Developer
        {
            Name = request.Name,
            Slug = request.Name.ToKebabCase()
        };
        await uow.Developers.AddAsync(developer);
        await uow.SaveChangesAsync();

        return Result<DeveloperResponse>.Success(ToResponse(developer));
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateDeveloperRequest request)
    {
        // 1) İstediğin biçim bozuk mu? > 400
        var validationResult = await updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        // 2) Güncellenecek kayıt var mı? > 404
        var developer = await uow.Developers.GetByIdAsync(id);
        if (developer is null)
            return Result.Failure(ErrorType.NotFound, "Developer not found.");

        // 3) yeni isim başka bir developerda kullanılıyor mu? > 409
        if (await uow.Developers.NameExistsAsync(request.Name, id))
            return Result.Failure(ErrorType.Conflict, "A developer with this name already exists.");

        // 4) bütün kontrollar geçti: değiştir ve kaydet.
        developer.Name = request.Name;
        developer.Slug = request.Name.ToKebabCase();

        uow.Developers.Update(developer);
        await uow.SaveChangesAsync();

        return Result.Success();

    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var developer = await uow.Developers.GetByIdAsync(id);
        if (developer is null)
            return Result.Failure(ErrorType.NotFound, "Developer not found.");

        if (await uow.Developers.HasGamesAsync(id))
            return Result.Failure(ErrorType.Conflict, "This developer has games and cannot be deleted.");

        uow.Developers.Delete(developer);
        await uow.SaveChangesAsync();
        return Result.Success();

    }

    public async Task<IReadOnlyList<DeveloperResponse>> GetAllAsync()
    {
        var developers = await uow.Developers.GetAllAsync();

        return developers.Select(ToResponse).ToList();
    }

    public async Task<Result<DeveloperResponse>> GetByIdAsync(Guid id)
    {
        var developer = await uow.Developers.GetByIdAsync(id);
        if (developer is null)
            return Result<DeveloperResponse>.Failure(ErrorType.NotFound, "Developer not found.");

        return Result<DeveloperResponse>.Success(ToResponse(developer));
    }


    private static DeveloperResponse ToResponse(Developer developer) => new(developer.Id, developer.Name, developer.Slug);
}
