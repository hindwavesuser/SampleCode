using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trip2Go.Buisness.Interface;
using Trip2Go.DomainModel;
using Trip2Go.Utility;
using Serilog;
using Trip2Go.EntityModel;
using AutoMapper;
using Trip2Go.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Newtonsoft.Json;
using Trip2Go.DataAccess.Entity;
using System.Data.SqlClient;

namespace Trip2Go.Buisness
{
    public class CommissionBusiness : ICommissionBusiness
    {

        #region Private Properties
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly Trip2GoContext context;
        #endregion

        #region Constructor
        public CommissionBusiness(IMapper _mapper, IUnitOfWork _unitOfWork, Trip2GoContext _context)
        {
            mapper = _mapper;
            unitOfWork = _unitOfWork;
            context = _context;
        }
        #endregion
        public async Task<ResultModel<List<CommissionModel>>> GetAllCommissions(int pageSize, int pageNumber, string sort, string filter)
        {
            ResultModel<List<CommissionModel>> result = new ResultModel<List<CommissionModel>>();
            try
            {
                var commissions = unitOfWork.CommissionRepository.GetAllAsQueryable(x => !x.IsDeleted).Include(x => x.SubCommissions).ToList();
                result.Data = new List<CommissionModel>();
                mapper.Map(commissions, result.Data);
                result.Success = true;
                result.StatusCode = (int)Enums.StatusCode.OK;
            }
            catch (Exception ex)
            {
                Common.LogMessage(ex.Message);
                throw ex;
            }
            return result;
        }

        public async Task<ResultModel<CommissionModel>> GetCommission(int id)
        {
            ResultModel<CommissionModel> result = new ResultModel<CommissionModel>();
            try
            {
                var commission = await unitOfWork.CommissionRepository.SingleOrDefaultAsync(x => x.Id == id);
                result.Data = new CommissionModel();
                result.Data = mapper.Map<CommissionModel>(commission);
                var subCommissions = unitOfWork.SubCommissionRepository.GetAllAsQueryable(x => x.CommissionId == commission.Id).ToList();
                result.Data.SubCommissions = new List<SubCommissionModel>();
                result.Data.SubCommissions = mapper.Map<List<SubCommissionModel>>(subCommissions);
                result.Success = true;
                result.StatusCode = (int)Enums.StatusCode.OK;
            }
            catch (Exception ex)
            {
                Common.LogMessage(ex.Message);
                throw ex;
            }
            return result;
        }
        public async Task<ResultModel<CommissionModel>> CreateCommission(CommissionModel commissionModel, int currUserId)
        {
            ResultModel<CommissionModel> result = new ResultModel<CommissionModel>();
            try
            {
                Commission commission = new Commission();
                commission.CreatedAt = DateTime.UtcNow;
                commission.CreatedBy = currUserId;
                commission.UpdatedAt = DateTime.UtcNow;
                commission.UpdatedBy = currUserId;
                unitOfWork.CommissionRepository.Insert(commission);
                foreach (var subCommissionModel in commissionModel.SubCommissions)
                {
                    var subCommission = mapper.Map<SubCommission>(subCommissionModel);
                    subCommission.CommissionId = commission.Id;
                    unitOfWork.SubCommissionRepository.Insert(subCommission);
                }
                result.Data = new CommissionModel();
                mapper.Map(commission, result.Data);
                result.Success = true;
                result.StatusCode = (int)Enums.StatusCode.OK;
            }
            catch (Exception ex)
            {
                Common.LogMessage(ex.Message);
                throw ex;
            }
            return result;
        }

        public async Task<ResultModel<CommissionModel>> ReplaceCommission(CommissionModel commissionModel, int currUserId)
        {
            ResultModel<CommissionModel> result = new ResultModel<CommissionModel>();
            try
            {
                var commission = await unitOfWork.CommissionRepository.SingleOrDefaultAsync(x => x.Id == commissionModel.Id);
                var subCommissions = unitOfWork.SubCommissionRepository.GetAll(x => x.CommissionId == commissionModel.Id).ToList();
                var deletedSubCommissions = subCommissions.Where(x => !commissionModel.SubCommissions.Any(y => y.Id == x.Id)).ToList();
                unitOfWork.SubCommissionRepository.DeleteAll(deletedSubCommissions);

                foreach (var subCommissionModel in commissionModel.SubCommissions)
                {
                    var subCommission = await unitOfWork.SubCommissionRepository.SingleOrDefaultAsync(x => x.Id == subCommissionModel.Id);
                    if (subCommission.IsNotNull())
                    {
                        subCommission.MinimumSale = subCommissionModel.MinimumSale;
                        subCommission.MaximumSale = subCommissionModel.MaximumSale;
                        subCommission.CommissionPercentage = subCommissionModel.CommissionPercentage;
                        unitOfWork.SubCommissionRepository.Update(subCommission);
                    }
                    else
                    {
                        subCommission = new SubCommission();
                        subCommission = mapper.Map<SubCommission>(subCommissionModel);
                        subCommission.CommissionId = commission.Id;
                        unitOfWork.SubCommissionRepository.Insert(subCommission);
                    }
                }
                result.Data = new CommissionModel();
                mapper.Map(commission, result.Data);
                result.Success = true;
                result.StatusCode = (int)Enums.StatusCode.OK;
            }
            catch (Exception ex)
            {
                Common.LogMessage(ex.Message);
                throw ex;
            }
            return result;
        }
        public async Task<ResultModel<bool>> DeleteCommission(int Id)
        {
            ResultModel<bool> result = new ResultModel<bool>();
            try
            {
                var commission = await unitOfWork.CommissionRepository.SingleOrDefaultAsync(x => x.Id == Id);
                commission.IsDeleted = true;
                commission.UpdatedAt = DateTime.UtcNow;
                unitOfWork.CommissionRepository.Update(commission);

                result.Data = true;
                result.Success = true;
                result.StatusCode = (int)Enums.StatusCode.OK;
            }
            catch (Exception ex)
            {
                Common.LogMessage(ex.Message);
                throw ex;
            }
            return result;
        }
        public bool AssociatedbCommission(int Id)
        {
            bool result = false;
            try
            {
                result = unitOfWork.UserRepository.Any(x => x.CommissionId == Id && !x.IsDeleted);
            }
            catch (Exception ex)
            {
                Common.LogMessage(ex.Message);
                throw ex;
            }
            return result;
        }

    }
}
