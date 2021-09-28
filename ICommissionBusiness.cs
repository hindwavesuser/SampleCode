using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trip2Go.DomainModel;

namespace Trip2Go.Buisness.Interface
{
    public interface ICommissionBusiness
    {
        Task<ResultModel<List<CommissionModel>>> GetAllCommissions(int pageSize, int pageNumber, string sort, string filter);
        Task<ResultModel<CommissionModel>> GetCommission(int id);
        Task<ResultModel<CommissionModel>> CreateCommission(CommissionModel commissionModel, int currUserId);
        Task<ResultModel<CommissionModel>> ReplaceCommission(CommissionModel commissionModel, int currUserId);
        Task<ResultModel<bool>> DeleteCommission(int Id);
        bool AssociatedbCommission(int Id);

    }
}
