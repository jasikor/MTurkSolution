using MTurk.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTurk.DataAccess
{
    public interface IGameParametersService
    {
        List<GameParametersModel> GetAllParameters();
        Task SaveGameParameters(GameParametersModel gp);
        void DeleteGameParameters(int id);


    }
}