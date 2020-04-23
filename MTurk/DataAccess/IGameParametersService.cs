using MTurk.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTurk.DataAccess
{
    public interface IGameParametersService
    {
        Task<List<GameParametersModel>> GetAllParametersAsync();
        Task SaveGameParameters(GameParametersModel gp);
        Task DeleteGameParameters(int id);


    }
}