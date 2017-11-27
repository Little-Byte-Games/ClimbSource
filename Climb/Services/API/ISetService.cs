using Climb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Climb.Services
{
    public interface ISetService
    {
        Task Put(Set set, IList<Match> matches);
    }
}