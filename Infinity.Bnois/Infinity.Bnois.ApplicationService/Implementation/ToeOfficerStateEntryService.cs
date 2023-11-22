using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.Api;
using Infinity.Bnois.Configuration;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class ToeOfficerStateEntryService : IToeOfficerStateEntryService
    {
        private readonly IBnoisRepository<DashBoardBranchByAdminAuthority700> dashBoardBranchByAdminAuthority700Repository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public ToeOfficerStateEntryService(IBnoisRepository<DashBoardBranchByAdminAuthority700> dashBoardBranchByAdminAuthority700Repository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.dashBoardBranchByAdminAuthority700Repository = dashBoardBranchByAdminAuthority700Repository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }
        public List<DashBoardBranchByAdminAuthority700Model> GetToeOfficerStateEntryList(int pageSize, int pageNumber, string searchText, out int total)
        {
            IQueryable<DashBoardBranchByAdminAuthority700> dashBoardBranchByAdminAuthority700s = dashBoardBranchByAdminAuthority700Repository.FilterWithInclude(x => x.IsActive && ((x.Branch.ShortName.Contains(searchText) || String.IsNullOrEmpty(searchText)) || (x.Rank.ShortName.Contains(searchText) || String.IsNullOrEmpty(searchText))), "Rank", "Branch");
            total = dashBoardBranchByAdminAuthority700s.Count();
            dashBoardBranchByAdminAuthority700s = dashBoardBranchByAdminAuthority700s.OrderByDescending(x => x.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            List<DashBoardBranchByAdminAuthority700Model> models = ObjectConverter<DashBoardBranchByAdminAuthority700, DashBoardBranchByAdminAuthority700Model>.ConvertList(dashBoardBranchByAdminAuthority700s.ToList()).ToList();
            return models;
        }
        public async Task<DashBoardBranchByAdminAuthority700Model> GetToeOfficerStateEntry(int id)
        {
            if (id <= 0)
            {
                return new DashBoardBranchByAdminAuthority700Model();
            }
            DashBoardBranchByAdminAuthority700 dashBoardBranchByAdminAuthority700 = await dashBoardBranchByAdminAuthority700Repository.FindOneAsync(x => x.Id == id);
            if (dashBoardBranchByAdminAuthority700 == null)
            {
                return new DashBoardBranchByAdminAuthority700Model();
            }
            DashBoardBranchByAdminAuthority700Model model = ObjectConverter<DashBoardBranchByAdminAuthority700, DashBoardBranchByAdminAuthority700Model>.Convert(dashBoardBranchByAdminAuthority700);
            return model;
        }

        public async Task<DashBoardBranchByAdminAuthority700Model> SaveToeOfficerStateEntry(int id, DashBoardBranchByAdminAuthority700Model model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Toe Authorized data missing!");
            }
            

            DashBoardBranchByAdminAuthority700 dashBoardBranchByAdminAuthority700 = ObjectConverter<DashBoardBranchByAdminAuthority700Model, DashBoardBranchByAdminAuthority700>.Convert(model);
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            if (dashBoardBranchByAdminAuthority700.Id > 0)
            {
                dashBoardBranchByAdminAuthority700 = await dashBoardBranchByAdminAuthority700Repository.FindOneAsync(x => x.Id == id);
                if (dashBoardBranchByAdminAuthority700 == null)
                {
                    throw new InfinityNotFoundException("Previous Experience data not found!");
                }

                DashBoardBranchByAdminAuthority700 updateDataExist = await dashBoardBranchByAdminAuthority700Repository.FindOneAsync(x => x.Id != model.Id && x.BranchId == model.BranchId && x.RankId == model.RankId && x.TransferTypeId == model.TransferTypeId);
                if (updateDataExist != null)
                {
                    throw new InfinityArgumentMissingException("Record with Same Branch, Rank and Type is already inserted!");
                }
                dashBoardBranchByAdminAuthority700.ModifiedDate = DateTime.Now;
                dashBoardBranchByAdminAuthority700.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "DashBoardBranchByAdminAuthority700";
                bnLog.TableEntryForm = "TO&E Officer State Entry";
                bnLog.PreviousValue = "Id: " + model.Id;
                bnLog.UpdatedValue = "Id: " + model.Id;
                int bnoisUpdateCount = 0;
                
                if (dashBoardBranchByAdminAuthority700.BranchId != model.BranchId)
                {
                    if (dashBoardBranchByAdminAuthority700.BranchId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Branch", "BranchId", dashBoardBranchByAdminAuthority700.BranchId ?? 0);
                        bnLog.PreviousValue += ", BranchId: " + ((dynamic)prev).ShortName;
                    }
                    if (model.BranchId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Branch", "BranchId", model.BranchId ?? 0);
                        bnLog.UpdatedValue += ", Branch: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (dashBoardBranchByAdminAuthority700.RankId != model.RankId)
                {
                    if (dashBoardBranchByAdminAuthority700.RankId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Rank", "RankId", dashBoardBranchByAdminAuthority700.RankId ?? 0);
                        bnLog.PreviousValue += ", Rank: " + ((dynamic)prev).ShortName;
                    }
                    if (model.RankId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId ?? 0);
                        bnLog.UpdatedValue += ", Rank: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (dashBoardBranchByAdminAuthority700.TransferTypeId != model.TransferTypeId)
                {
                    bnLog.PreviousValue += ", Type: " + (dashBoardBranchByAdminAuthority700.TransferTypeId == 1 ? "In BN" : dashBoardBranchByAdminAuthority700.TransferTypeId == 2 ? "Inside BN" : "");
                    bnLog.UpdatedValue += ", Type: " + (dashBoardBranchByAdminAuthority700.TransferTypeId == 1 ? "In BN" : dashBoardBranchByAdminAuthority700.TransferTypeId == 2 ? "Inside BN" : "");
                    bnoisUpdateCount += 1;
                }
                if (dashBoardBranchByAdminAuthority700.No != model.No)
                {
                    bnLog.PreviousValue += ", No: " + dashBoardBranchByAdminAuthority700.No;
                    bnLog.UpdatedValue += ", No: " + model.No;
                    bnoisUpdateCount += 1;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                if (bnoisUpdateCount > 0)
                {
                    await bnoisLogRepository.SaveAsync(bnLog);

                }
                else
                {
                    throw new InfinityNotFoundException("Please Update Any Field!");
                }
                //data log section end
            }
            else
            {

                DashBoardBranchByAdminAuthority700 saveDataExist = await dashBoardBranchByAdminAuthority700Repository.FindOneAsync(x => x.BranchId == model.BranchId && x.RankId == model.RankId && x.TransferTypeId == model.TransferTypeId);
                if (saveDataExist != null)
                {
                    throw new InfinityArgumentMissingException("Record with Same Branch, Rank and Type is already inserted!");
                }
                dashBoardBranchByAdminAuthority700.CreatedBy = userId;
                dashBoardBranchByAdminAuthority700.CreatedDate = DateTime.Now;
                dashBoardBranchByAdminAuthority700.IsActive = true;
            }

            dashBoardBranchByAdminAuthority700.BranchId = model.BranchId;
            dashBoardBranchByAdminAuthority700.RankId = model.RankId;
            dashBoardBranchByAdminAuthority700.TransferTypeId = model.TransferTypeId;
            dashBoardBranchByAdminAuthority700.No = model.No;
            
            
            await dashBoardBranchByAdminAuthority700Repository.SaveAsync(dashBoardBranchByAdminAuthority700);
            model.Id = dashBoardBranchByAdminAuthority700.Id;

            return model;
        }
        public async Task<bool> DeleteToeOfficerStateEntry(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            DashBoardBranchByAdminAuthority700 dashBoardBranchByAdminAuthority700 = await dashBoardBranchByAdminAuthority700Repository.FindOneAsync(x => x.Id == id);
            if (dashBoardBranchByAdminAuthority700 == null)
            {
                throw new InfinityNotFoundException("Toe Authorized not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "DashBoardBranchByAdminAuthority700";
                bnLog.TableEntryForm = "TO&E Officer State Entry";
                bnLog.PreviousValue = "Id: " + dashBoardBranchByAdminAuthority700.Id;

                if (dashBoardBranchByAdminAuthority700.BranchId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Branch", "BranchId", dashBoardBranchByAdminAuthority700.BranchId ?? 0);
                    bnLog.PreviousValue += ", Branch: " + ((dynamic)prev).ShortName;
                }
                if (dashBoardBranchByAdminAuthority700.RankId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Rank", "RankId", dashBoardBranchByAdminAuthority700.RankId ?? 0);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)prev).ShortName;
                }
                bnLog.PreviousValue += ", Type: " + (dashBoardBranchByAdminAuthority700.TransferTypeId == 1 ? "In BN" : dashBoardBranchByAdminAuthority700.TransferTypeId == 2 ? "Inside BN" : ""); 

                bnLog.PreviousValue += ", No: " + dashBoardBranchByAdminAuthority700.No;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await dashBoardBranchByAdminAuthority700Repository.DeleteAsync(dashBoardBranchByAdminAuthority700);
            }
        }
        public List<SelectModel> GetOrgTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(OODOrgType)).Cast<OODOrgType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt32(v) })
                    .ToList();
            return selectModels;
        }
    }
}
