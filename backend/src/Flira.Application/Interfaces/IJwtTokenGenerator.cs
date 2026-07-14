using System.Collections.Generic;

namespace Flira.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string email, string fullName, IList<string> roles);
}
