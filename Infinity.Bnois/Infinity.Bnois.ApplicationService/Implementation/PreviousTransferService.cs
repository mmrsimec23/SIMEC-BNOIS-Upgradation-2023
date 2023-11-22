using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class PreviousTransferService : IPreviousTransferService
    {
        private readonly IBnoisRepository<PreviousTransfer> previousTransferRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public PreviousTransferService(IBnoisRepository<PreviousTransfer> previousTransferRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.previousTransferRepository = previousTransferRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public async Task<PreviousTransferModel> GetPreviousTransfer(int previousTransferId)
        {
            if (previousTransferId <= 0)
            {
                return new PreviousTransferModel();
            }
            PreviousTransfer previousTransfer = await previousTransferRepository.FindOneAsync(x => x.PreviousTransferId == previousTransferId);
            if (previousTransfer == null)
            {
                return new PreviousTransferModel();
            }

            PreviousTransferModel model = ObjectConverter<PreviousTransfer, PreviousTransferModel>.Convert(previousTransfer);
            return model;
        }

        public List<PreviousTransferModel> GetPreviousTransfers(int employeeId)
        {
            List<PreviousTransfer> previousTransfers = previousTransferRepository.FilterWithInclude(x => x.EmployeeId == employeeId,"Rank").ToList();
            List<PreviousTransferModel> models = ObjectConverter<PreviousTransfer, PreviousTransferModel>.ConvertList(previousTransfers.ToList()).ToList();
            return models;
        }

        public async Task<PreviousTransferModel> SavePreviousTransfer(int previousTransferId, PreviousTransferModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Officer Transfer data missing!");
            }

            PreviousTransfer previousTransfer = ObjectConverter<PreviousTransferModel, PreviousTransfer>.Convert(model);
            string userId= ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            if (previousTransferId > 0)
            {
                previousTransfer = await previousTransferRepository.FindOneAsync(x => x.PreviousTransferId == previousTransferId);
                if (previousTransfer == null)
                {
                    throw new InfinityNotFoundException("Transfer Not found !");
                }

                previousTransfer.ModifiedDate = DateTime.Now;
                previousTransfer.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PreviousTransfer";
                bnLog.TableEntryForm = "Employee Previous Transfer";
                bnLog.PreviousValue = "Id: " + model.PreviousTransferId;
                bnLog.UpdatedValue = "Id: " + model.PreviousTransferId;
                int bnoisUpdateCount = 0;
                if (previousTransfer.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", previousTransfer.EmployeeId ?? 0);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId ?? 0);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                    //bnoisUpdateCount += 1;
                }
                if (previousTransfer.Billet != model.Billet)
                {
                    bnLog.PreviousValue += ", Billet: " + previousTransfer.Billet;
                    bnLog.UpdatedValue += ", Billet: " + model.Billet;
                    bnoisUpdateCount += 1;
                }
                if (previousTransfer.RankId != model.RankId)
                {
                    if (previousTransfer.RankId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("PreCommissionRank", "PreCommissionRankId", previousTransfer.RankId ?? 0);
                        bnLog.PreviousValue += ", Leave Type: " + ((dynamic)prev).Name;
                    }
                    if (model.RankId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("PreCommissionRank", "PreCommissionRankId", model.RankId ?? 0);
                        bnLog.UpdatedValue += ", Leave Type: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (previousTransfer.FromDate != model.FromDate)
                {
                    bnLog.PreviousValue += ", From Date: " + previousTransfer.FromDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", From Date: " + model.FromDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (previousTransfer.ToDate != model.ToDate)
                {
                    bnLog.PreviousValue += ", To Date: " + previousTransfer.ToDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", To Date: " + model.ToDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (previousTransfer.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + previousTransfer.Remarks;
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
                previousTransfer.EmployeeId = model.EmployeeId;
                previousTransfer.CreatedBy = userId;
                previousTransfer.CreatedDate = DateTime.Now;
                previousTransfer.IsActive = true;
            }

            previousTransfer.RankId = model.RankId;
            previousTransfer.Billet = model.Billet;
            previousTransfer.FromDate = model.FromDate;
            previousTransfer.ToDate = model.ToDate;
            previousTransfer.Remarks = model.Remarks;
            await previousTransferRepository.SaveAsync(previousTransfer);
            model.PreviousTransferId = previousTransfer.PreviousTransferId;
            return model;
        }


        public async Task<bool> DeletePreviousTransfer(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            PreviousTransfer previousTransfer = await previousTransferRepository.FindOneAsync(x => x.PreviousTransferId == id);
            if (previousTransfer == null)
            {
                throw new InfinityNotFoundException("Transfer not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PreviousTransfer";
                bnLog.TableEntryForm = "Employee Previous Transfer";
                bnLog.PreviousValue = "Id: " + previousTransfer.PreviousTransferId;

                var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", previousTransfer.EmployeeId ?? 0);
                bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                bnLog.PreviousValue += ", Billet: " + previousTransfer.Billet;
                if (previousTransfer.RankId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("PreCommissionRank", "PreCommissionRankId", previousTransfer.RankId ?? 0);
                    bnLog.PreviousValue += ", Leave Type: " + ((dynamic)prev).Name;
                }
                bnLog.PreviousValue += ", From Date: " + previousTransfer.FromDate?.ToString("dd/MM/yyyy") + ", To Date: " + previousTransfer.ToDate?.ToString("dd/MM/yyyy") + ", Remarks: " + previousTransfer.Remarks;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await previousTransferRepository.DeleteAsync(previousTransfer);
            }
        }
    }
}
