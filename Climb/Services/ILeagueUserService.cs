using System.Threading.Tasks;
using Climb.Models;

namespace Climb.Services
{
    public interface ILeagueUserService
    {
        Task<Character> GetFavoriteCharacter(int id);
    }
}