using Climb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Climb.Requests.Sets;

namespace Climb.Services
{
    public interface ISetService
    {
        Task Put(Set set, List<MatchPutRequest> matches);
        string GetSetUrl(Set set, IUrlHelper urlHelper);
    }
}