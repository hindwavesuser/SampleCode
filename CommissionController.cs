using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trip2Go.Buisness.Interface;
using Trip2Go.DomainModel;
using Trip2Go.Utility;
using Trip2Go.ViewModel;

namespace Trip2Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Common.AllRoles)]
    public class CommissionController : BaseController
    {
        #region Private Properties
        private readonly IMapper mapper;
        private readonly ICommissionBusiness commissionBusiness;
        #endregion

        #region Constructor
        public CommissionController(IMapper _mapper, ICommissionBusiness _commissionBusiness)
        {
            mapper = _mapper;
            commissionBusiness = _commissionBusiness;
        }
        #endregion

        #region Client

        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<ResultVM<List<CommissionVM>>>> GetAllCommissions(int pageSize, int pageNumber, string sort, string filter)
        {
            ResultVM<List<CommissionVM>> resultVM = new ResultVM<List<CommissionVM>>();
            try
            {
                var result = await commissionBusiness.GetAllCommissions(pageSize, pageNumber, sort, filter);
                mapper.Map(result, resultVM);
            }
            catch (Exception ex)
            {
                Common.LogMessage(ex.Message);
                resultVM.Message = ex.Message;
                resultVM.StatusCode = Convert.ToInt32(Enums.StatusCode.BadRequest);
                return StatusCode(StatusCodes.Status400BadRequest, new { Result = resultVM, Codes = new string[] { "ServerError" } });
            }
            return resultVM;
        }

        [HttpGet]
        [Route("Get")]
        public async Task<ActionResult<ResultVM<CommissionVM>>> GetCommission(int id)
        {
            ResultVM<CommissionVM> resultVM = new ResultVM<CommissionVM>();
            try
            {
                var result = await commissionBusiness.GetCommission(id);
                mapper.Map(result, resultVM);
            }
            catch (Exception ex)
            {
                Common.LogMessage(ex.Message);
                resultVM.Message = ex.Message;
                resultVM.StatusCode = Convert.ToInt32(Enums.StatusCode.BadRequest);
                return StatusCode(StatusCodes.Status400BadRequest, new { Result = resultVM, Codes = new string[] { "ServerError" } });
            }
            return resultVM;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult<ResultVM<CommissionVM>>> Create(CommissionVM commissionVM)
        {
            ResultVM<CommissionVM> resultVM = new ResultVM<CommissionVM>();
            try
            {
                var commissionModel = mapper.Map<CommissionModel>(commissionVM);
                var result = await commissionBusiness.CreateCommission(commissionModel, GetUserId());
                mapper.Map(result, resultVM);
            }
            catch (Exception ex)
            {
                Common.LogMessage(ex.Message);
                resultVM.Message = ex.Message;
                resultVM.StatusCode = Convert.ToInt32(Enums.StatusCode.BadRequest);
                return StatusCode(StatusCodes.Status400BadRequest, new { Result = resultVM, Codes = new string[] { "ServerError" } });
            }
            return resultVM;
        }

        [HttpPost]
        [Route("Replace")]
        public async Task<ActionResult<ResultVM<CommissionVM>>> Replace(CommissionVM commissionVM)
        {
            ResultVM<CommissionVM> resultVM = new ResultVM<CommissionVM>();
            try
            {
                var commissionModel = mapper.Map<CommissionModel>(commissionVM);
                var result = await commissionBusiness.ReplaceCommission(commissionModel, GetUserId());
                mapper.Map(result, resultVM);
            }
            catch (Exception ex)
            {
                Common.LogMessage(ex.Message);
                resultVM.Message = ex.Message;
                resultVM.StatusCode = Convert.ToInt32(Enums.StatusCode.BadRequest);
                return StatusCode(StatusCodes.Status400BadRequest, new { Result = resultVM, Codes = new string[] { "ServerError" } });
            }
            return resultVM;
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<ActionResult<ResultVM<bool>>> Delete(int Id)
        {
            ResultVM<bool> resultVM = new ResultVM<bool>();
            try
            {
                var isAssociated = commissionBusiness.AssociatedbCommission(Id);
                if (isAssociated)
                {
                    resultVM.Data = false;
                    resultVM.StatusCode = Convert.ToInt32(Enums.StatusCode.Exists);
                    return StatusCode(StatusCodes.Status502BadGateway, new { Result = resultVM, Codes = new string[] { "AlreadyAssociated" } });
                }
                var result = await commissionBusiness.DeleteCommission(Id);
                mapper.Map(result, resultVM);
            }
            catch (Exception ex)
            {
                Common.LogMessage(ex.Message);
                resultVM.Message = ex.Message;
                resultVM.StatusCode = Convert.ToInt32(Enums.StatusCode.BadRequest);
                return StatusCode(StatusCodes.Status400BadRequest, new { Result = resultVM, Codes = new string[] { "ServerError" } });
            }
            return resultVM;

        }

        #endregion

    }
}