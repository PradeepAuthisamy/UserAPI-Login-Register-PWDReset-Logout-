using System.Threading.Tasks;

namespace UserAPI.Providers.Interface
{
    public interface IJWTProvider
    {
        Task<string> GetTokenAsync(string userName);
    }
}