using Microsoft.AspNetCore.Identity;

namespace ThuVien_API.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}