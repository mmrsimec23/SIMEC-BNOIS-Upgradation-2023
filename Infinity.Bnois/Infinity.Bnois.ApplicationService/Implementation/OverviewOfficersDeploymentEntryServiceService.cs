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
    public class OverviewOfficersDeploymentEntryService : IOverviewOfficersDeploymentEntryService
    {
        private readonly IBnoisRepository<DashBoardBranchByAdminAuthority600Entry> dashBoardBranchByAdminAuthority600EntryRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public OverviewOfficersDeploymentEntryService(IBnoisRepository<DashBoardBranchByAdminAuthority600Entry> dashBoardBranchByAdminAuthority600EntryRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.dashBoardBranchByAdminAuthority600EntryRepository = dashBoardBranchByAdminAuthority600EntryRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }
        public List<DashBoardBranchByAdminAuthority600EntryModel> GetOverviewOfficersDeploymentEntryList(int pageSize, int pageNumber, string searchText, out int total)
        {
            IQueryable<DashBoardBranchByAdminAuthority600Entry> dashBoardBranchByAdminAuthority600Entrys = dashBoardBranchByAdminAuthority600EntryRepository.FilterWithInclude(x => x.IsActive && ((x.Rank.ShortName.Contains(searchText) || String.IsNullOrEmpty(searchText))), "Rank");
            total = dashBoardBranchByAdminAuthority600Entrys.Count();
            dashBoardBranchByAdminAuthority600Entrys = dashBoardBranchByAdminAuthority600Entrys.OrderByDescending(x => x.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            List<DashBoardBranchByAdminAuthority600EntryModel> models = ObjectConverter<DashBoardBranchByAdminAuthority600Entry, DashBoardBranchByAdminAuthority600EntryModel>.ConvertList(dashBoardBranchByAdminAuthority600Entrys.ToList()).ToList();
            return models;
        }
        public async Task<DashBoardBranchByAdminAuthority600EntryModel> GetOverviewOfficersDeploymentEntry(int id)
        {
            if (id <= 0)
            {
                return new DashBoardBranchByAdminAuthority600EntryModel();
            }
            DashBoardBranchByAdminAuthority600Entry dashBoardBranchByAdminAuthority600Entry = await dashBoardBranchByAdminAuthority600EntryRepository.FindOneAsync(x => x.Id == id);
            if (dashBoardBranchByAdminAuthority600Entry == null)
            {
                return new DashBoardBranchByAdminAuthority600EntryModel();
            }
            DashBoardBranchByAdminAuthority600EntryModel model = ObjectConverter<DashBoardBranchByAdminAuthority600Entry, DashBoardBranchByAdminAuthority600EntryModel>.Convert(dashBoardBranchByAdminAuthority600Entry);
            return model;
        }

        public async Task<DashBoardBranchByAdminAuthority600EntryModel> SaveOverviewOfficersDeploymentEntry(int id, DashBoardBranchByAdminAuthority600EntryModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Toe Authorized data missing!");
            }

            DashBoardBranchByAdminAuthority600Entry dashBoardBranchByAdminAuthority600Entry = ObjectConverter<DashBoardBranchByAdminAuthority600EntryModel, DashBoardBranchByAdminAuthority600Entry>.Convert(model);
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            if (dashBoardBranchByAdminAuthority600Entry.Id > 0)
            {
                dashBoardBranchByAdminAuthority600Entry = await dashBoardBranchByAdminAuthority600EntryRepository.FindOneAsync(x => x.Id == id);
                if (dashBoardBranchByAdminAuthority600Entry == null)
                {
                    throw new InfinityNotFoundException("Previous Experience data not found!");
                }
                dashBoardBranchByAdminAuthority600Entry.ModifiedDate = DateTime.Now;
                dashBoardBranchByAdminAuthority600Entry.ModifiedBy = userId;

                DashBoardBranchByAdminAuthority600Entry updateDataExist = await dashBoardBranchByAdminAuthority600EntryRepository.FindOneAsync(x => x.Id != model.Id && x.RankId == model.RankId && x.OrgType == model.OrgType);
                if (updateDataExist != null)
                {
                    throw new InfinityArgumentMissingException("Record with Same Branch, Rank and Type is already inserted!");
                }

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "DashBoardBranchByAdminAuthority600Entry";
                bnLog.TableEntryForm = "Overview of Officers Deployment Entry";
                bnLog.PreviousValue = "Id: " + model.Id;
                bnLog.UpdatedValue = "Id: " + model.Id;
                int bnoisUpdateCount = 0;
                
                if (dashBoardBranchByAdminAuthority600Entry.RankId != model.RankId)
                {
                    if (dashBoardBranchByAdminAuthority600Entry.RankId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Rank", "RankId", dashBoardBranchByAdminAuthority600Entry.RankId ?? 0);
                        bnLog.PreviousValue += ", Rank: " + ((dynamic)prev).ShortName;
                    }
                    if (model.RankId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId ?? 0);
                        bnLog.UpdatedValue += ", Rank: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (dashBoardBranchByAdminAuthority600Entry.OrgType != model.OrgType)
                {
                    bnLog.PreviousValue += ", Type: " + (dashBoardBranchByAdminAuthority600Entry.OrgType == 1 ? "Within_Navy" : dashBoardBranchByAdminAuthority600Entry.OrgType == 2 ? "Second_Ment" : dashBoardBranchByAdminAuthority600Entry.OrgType == 3 ? "CG" : dashBoardBranchByAdminAuthority600Entry.OrgType == 4 ? "ISO_And_Others" : "");
                    bnLog.UpdatedValue += ", Type: " + (model.OrgType == 1 ? "Within_Navy" : model.OrgType == 2 ? "Second_Ment" : model.OrgType == 3 ? "CG" : model.OrgType == 4 ? "ISO_And_Others" : "");
                    bnoisUpdateCount += 1;
                }
                if (dashBoardBranchByAdminAuthority600Entry.No != model.No)
                {
                    bnLog.PreviousValue += ", No: " + dashBoardBranchByAdminAuthority600Entry.No;
                    bnLog.UpdatedValue += ", No: " + model.No;
                    bnoisUpdateCount += 1;
                }
                if (dashBoardBranchByAdminAuthority600Entry.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + dashBoardBranchByAdminAuthority600Entry.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
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
                DashBoardBranchByAdminAuthority600Entry updateDataExist = await dashBoardBranchByAdminAuthority600EntryRepository.FindOneAsync(x => x.RankId == model.RankId && x.OrgType == model.OrgType);
                if (updateDataExist != null)
                {
                    throw new InfinityArgumentMissingException("Record with Same Branch, Rank and Type is already inserted!");
                }

                dashBoardBranchByAdminAuthority600Entry.CreatedBy = userId;
                dashBoardBranchByAdminAuthority600Entry.CreatedDate = DateTime.Now;
                dashBoardBranchByAdminAuthority600Entry.IsActive = true;
            }

            dashBoardBranchByAdminAuthority600Entry.RankId = model.RankId;
            dashBoardBranchByAdminAuthority600Entry.OrgType = model.OrgType;
            dashBoardBranchByAdminAuthority600Entry.No = model.No;
            dashBoardBranchByAdminAuthority600Entry.Remarks = model.Remarks;
            
            
            await dashBoardBranchByAdminAuthority600EntryRepository.SaveAsync(dashBoardBranchByAdminAuthority600Entry);
            model.Id = dashBoardBranchByAdminAuthority600Entry.Id;

            return model;
        }
        public async Task<bool> DeleteOverviewOfficersDeploymentEntry(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            DashBoardBranchByAdminAuthority600Entry dashBoardBranchByAdminAuthority600Entry = await dashBoardBranchByAdminAuthority600EntryRepository.FindOneAsync(x => x.Id == id);
            if (dashBoardBranchByAdminAuthority600Entry == null)
            {
                throw new InfinityNotFoundException("Toe Authorized not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "DashBoardBranchByAdminAuthority600Entry";
                bnLog.TableEntryForm = "Toe Authorized";
                bnLog.PreviousValue = "Id: " + dashBoardBranchByAdminAuthority600Entry.Id;
                
                if (dashBoardBranchByAdminAuthority600Entry.RankId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Rank", "RankId", dashBoardBranchByAdminAuthority600Entry.RankId ?? 0);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)prev).ShortName;
                }
                bnLog.PreviousValue += ", Type: " + (dashBoardBranchByAdminAuthority600Entry.OrgType == 1 ? "Within_Navy" : dashBoardBranchByAdminAuthority600Entry.OrgType == 2 ? "Second_Ment" : dashBoardBranchByAdminAuthority600Entry.OrgType == 3 ? "CG" : dashBoardBranchByAdminAuthority600Entry.OrgType == 4 ? "ISO_And_Others" : ""); ;
                
                bnLog.PreviousValue += ", No: " + dashBoardBranchByAdminAuthority600Entry.No + ", Remarks: " + dashBoardBranchByAdminAuthority600Entry.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await dashBoardBranchByAdminAuthority600EntryRepository.DeleteAsync(dashBoardBranchByAdminAuthority600Entry);
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
