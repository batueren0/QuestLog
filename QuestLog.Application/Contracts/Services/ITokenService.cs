using QuestLog.Application.Common;
using QuestLog.Domain.Entities;

namespace QuestLog.Application.Contracts.Services;

public interface ITokenService
{
    AccessToken CreateToken(AppUser user, IList<string> roles);
}
