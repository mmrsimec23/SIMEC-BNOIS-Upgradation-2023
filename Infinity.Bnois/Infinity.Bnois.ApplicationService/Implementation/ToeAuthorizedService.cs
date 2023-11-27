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
    public class ToeAuthorizedService : IToeAuthorizedService
    {
        private readonly IBnoisRepository<ToeAuthorized> toeAuthorizedRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public ToeAuthorizedService(IBnoisRepository<ToeAuthorized> toeAuthorizedRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.toeAuthorizedRepository = toeAuthorizedRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }
        public List<ToeAuthorizedModel> GetToeAuthorizeds(int pageSize, int pageNumber, string searchText, out int total)
        {
            IQueryable<ToeAuthorized> toeAuthorizeds = toeAuthorizedRepository.FilterWithInclude(x => x.IsActive && ((x.Branch.ShortName.Contains(searchText) || String.IsNullOrEmpty(searchText)) || (x.Office.ShortName.Contains(searchText) || String.IsNullOrEmpty(searchText)) || (x.Rank.ShortName.Contains(searchText) || String.IsNullOrEmpty(searchText))), "Rank","Branch","Office");
            total = toeAuthorizeds.Count();
            toeAuthorizeds = toeAuthorizeds.OrderByDescending(x => x.OfficeId).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            List<ToeAuthorizedModel> models = ObjectConverter<ToeAuthorized, ToeAuthorizedModel>.ConvertList(toeAuthorizeds.ToList()).ToList();
            return models;
        }
        public async Task<ToeAuthorizedModel> GetToeAuthorized(int toeAuthorizedId)
        {
            if (toeAuthorizedId <= 0)
            {
                return new ToeAuthorizedModel();
            }
            ToeAuthorized toeAuthorized = await toeAuthorizedRepository.FindOneAsync(x => x.ToeAuthorizedid == toeAuthorizedId);
            if (toeAuthorized == null)
            {
                return new ToeAuthorizedModel();
            }
            ToeAuthorizedModel model = ObjectConverter<ToeAuthorized, ToeAuthorizedModel>.Convert(toeAuthorized);
            return model;
        }

        public async Task<ToeAuthorizedModel> SaveToeAuthorized(int toeAuthorizedId, ToeAuthorizedModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Toe Authorized data missing!");
            }

            ToeAuthorized toeAuthorized = ObjectConverter<ToeAuthorizedModel, ToeAuthorized>.Convert(model);
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            if (toeAuthorized.ToeAuthorizedid > 0)
            {
                toeAuthorized = await toeAuthorizedRepository.FindOneAsync(x => x.ToeAuthorizedid == toeAuthorizedId);
                if (toeAuthorized == null)
                {
                    throw new InfinityNotFoundException("Previous Experience data not found!");
                }
                toeAuthorized.ModifiedDate = DateTime.Now;
                toeAuthorized.ModifiedBy = userId;

                ToeAuthorized updateDataExist = await toeAuthorizedRepository.FindOneAsync(x => x.ToeAuthorizedid != model.ToeAuthorizedid && x.BranchId == model.BranchId && x.RankId == model.RankId && x.OfficeId == model.OfficeId);
                if (updateDataExist != null)
                {
                    throw new InfinityArgumentMissingException("Record with Same Branch, Rank and Type is already inserted!");
                }

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ToeAuthorized";
                bnLog.TableEntryForm = "Toe Authorized";
                bnLog.PreviousValue = "Id: " + model.ToeAuthorizedid;
                bnLog.UpdatedValue = "Id: " + model.ToeAuthorizedid;
                int bnoisUpdateCount = 0;
                
                if (toeAuthorized.BranchId != model.BranchId)
                {
                    if (toeAuthorized.BranchId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Branch", "BranchId", toeAuthorized.BranchId ?? 0);
                        bnLog.PreviousValue += ", Branch: " + ((dynamic)prev).Name;
                    }
                    if (model.BranchId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Branch", "BranchId", model.BranchId ?? 0);
                        bnLog.UpdatedValue += ", Branch: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (toeAuthorized.RankId != model.RankId)
                {
                    if (toeAuthorized.RankId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Rank", "RankId", toeAuthorized.RankId ?? 0);
                        bnLog.PreviousValue += ", Rank: " + ((dynamic)prev).ShortName;
                    }
                    if (model.RankId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId ?? 0);
                        bnLog.UpdatedValue += ", Rank: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (toeAuthorized.OfficeId != model.OfficeId)
                {
                    if (toeAuthorized.OfficeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", toeAuthorized.OfficeId ?? 0);
                        bnLog.PreviousValue += ", Office: " + ((dynamic)prev).ShortName;
                    }
                    if (model.OfficeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Office", "OfficeId", model.OfficeId ?? 0);
                        bnLog.UpdatedValue += ", Office: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (toeAuthorized.No != model.No)
                {
                    bnLog.PreviousValue += ", No: " + toeAuthorized.No;
                    bnLog.UpdatedValue += ", No: " + model.No;
                    bnoisUpdateCount += 1;
                }
                if (toeAuthorized.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + toeAuthorized.Remarks;
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
                ToeAuthorized updateDataExist = await toeAuthorizedRepository.FindOneAsync(x => x.BranchId == model.BranchId && x.RankId == model.RankId && x.OfficeId == model.OfficeId);
                if (updateDataExist != null)
                {
                    throw new InfinityArgumentMissingException("Record with Same Branch, Rank and Type is already inserted!");
                }
                toeAuthorized.CreatedBy = userId;
                toeAuthorized.CreatedDate = DateTime.Now;
                toeAuthorized.IsActive = true;
            }

            toeAuthorized.BranchId = model.BranchId;
            toeAuthorized.RankId = model.RankId;
            toeAuthorized.OfficeId = model.OfficeId;
            toeAuthorized.No = model.No;
            toeAuthorized.Remarks = model.Remarks;
            
            
            await toeAuthorizedRepository.SaveAsync(toeAuthorized);
            model.ToeAuthorizedid = toeAuthorized.ToeAuthorizedid;

            return model;
        }
        public async Task<bool> DeleteToeAuthorized(int toeAuthorizedId)
        {
            if (toeAuthorizedId < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            ToeAuthorized toeAuthorized = await toeAuthorizedRepository.FindOneAsync(x => x.ToeAuthorizedid == toeAuthorizedId);
            if (toeAuthorized == null)
            {
                throw new InfinityNotFoundException("Toe Authorized not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ToeAuthorized";
                bnLog.TableEntryForm = "Toe Authorized";
                bnLog.PreviousValue = "Id: " + toeAuthorized.ToeAuthorizedid;
                if(toeAuthorized.BranchId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Branch", "BranchId", toeAuthorized.BranchId ?? 0);
                    bnLog.PreviousValue += ", Branch: " + ((dynamic)prev).Name;
                }
                if (toeAuthorized.RankId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Rank", "RankId", toeAuthorized.RankId ?? 0);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)prev).ShortName;
                }
                if (toeAuthorized.OfficeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", toeAuthorized.OfficeId ?? 0);
                    bnLog.PreviousValue += ", Office: " + ((dynamic)prev).ShortName;
                }
                bnLog.PreviousValue += ", No: " + toeAuthorized.No + ", Remarks: " + toeAuthorized.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await toeAuthorizedRepository.DeleteAsync(toeAuthorized);
            }
        }
    }
}
