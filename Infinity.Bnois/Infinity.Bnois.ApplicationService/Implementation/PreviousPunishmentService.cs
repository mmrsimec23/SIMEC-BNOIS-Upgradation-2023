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
    public class PreviousPunishmentService : IPreviousPunishmentService
    {
        private readonly IBnoisRepository<PreviousPunishment> previousPunishmentRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public PreviousPunishmentService(IBnoisRepository<PreviousPunishment> previousPunishmentRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.previousPunishmentRepository = previousPunishmentRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public async Task<PreviousPunishmentModel> GetPreviousPunishment(int previousPunishmentId)
        {
            if (previousPunishmentId <= 0)
            {
                return new PreviousPunishmentModel(){Type = 1};
            }
            PreviousPunishment previousPunishment = await previousPunishmentRepository.FindOneAsync(x => x.PreviousPunishmentId == previousPunishmentId);
            if (previousPunishment == null)
            {
                return new PreviousPunishmentModel();
            }

            PreviousPunishmentModel model = ObjectConverter<PreviousPunishment, PreviousPunishmentModel>.Convert(previousPunishment);
            return model;
        }

        public List<PreviousPunishmentModel> GetPreviousPunishments(int employeeId)
        {
            List<PreviousPunishment> previousPunishments = previousPunishmentRepository.FilterWithInclude(x => x.EmployeeId == employeeId).ToList();
            List<PreviousPunishmentModel> models = ObjectConverter<PreviousPunishment, PreviousPunishmentModel>.ConvertList(previousPunishments.ToList()).ToList();
            return models;
        }

        public async Task<PreviousPunishmentModel> SavePreviousPunishment(int previousPunishmentId, PreviousPunishmentModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Officer Punishment data missing!");
            }

            PreviousPunishment previousPunishment = ObjectConverter<PreviousPunishmentModel, PreviousPunishment>.Convert(model);
            string userId= ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            if (previousPunishmentId > 0)
            {
                previousPunishment = await previousPunishmentRepository.FindOneAsync(x => x.PreviousPunishmentId == previousPunishmentId);
                if (previousPunishment == null)
                {
                    throw new InfinityNotFoundException("Punishment Not found !");
                }

                previousPunishment.ModifiedDate = DateTime.Now;
                previousPunishment.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PreviousPunishment";
                bnLog.TableEntryForm = "Employee Previous Punishment";
                bnLog.PreviousValue = "Id: " + model.PreviousPunishmentId;
                bnLog.UpdatedValue = "Id: " + model.PreviousPunishmentId;
                int bnoisUpdateCount = 0;
                if (previousPunishment.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", previousPunishment.EmployeeId ?? 0);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId ?? 0);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                    bnoisUpdateCount += 1;
                }
                if (previousPunishment.Type != model.Type)
                {
                    bnLog.PreviousValue += ", Type: " + (previousPunishment.Type == 1 ? "Punishment" : "Achievement");
                    bnLog.UpdatedValue += ", Type: " + (model.Type == 1 ? "Punishment" : "Achievement");
                    bnoisUpdateCount += 1;
                }
                if (previousPunishment.Description != model.Description)
                {
                    bnLog.PreviousValue += ", Description: " + previousPunishment.Description;
                    bnLog.UpdatedValue += ", Description: " + model.Description;
                    bnoisUpdateCount += 1;
                }
                if (previousPunishment.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + previousPunishment.Remarks;
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
                previousPunishment.EmployeeId = model.EmployeeId;
                previousPunishment.CreatedBy = userId;
                previousPunishment.CreatedDate = DateTime.Now;
                previousPunishment.IsActive = true;
            }

            previousPunishment.Type = model.Type;
            previousPunishment.Remarks = model.Remarks;
            previousPunishment.Description = model.Description;
            previousPunishment.Remarks = model.Remarks;
            await previousPunishmentRepository.SaveAsync(previousPunishment);
            model.PreviousPunishmentId = previousPunishment.PreviousPunishmentId;
            return model;
        }


        public async Task<bool> DeletePreviousPunishment(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            PreviousPunishment previousPunishment = await previousPunishmentRepository.FindOneAsync(x => x.PreviousPunishmentId == id);
            if (previousPunishment == null)
            {
                throw new InfinityNotFoundException("Punishment not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PreviousPunishment";
                bnLog.TableEntryForm = "Employee Previous Punishment";
                bnLog.PreviousValue = "Id: " + previousPunishment.PreviousPunishmentId;

                var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", previousPunishment.EmployeeId ?? 0);
                bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                bnLog.PreviousValue += ", Type: " + (previousPunishment.Type == 1 ? "Punishment" : "Achievement");

                bnLog.PreviousValue += ", Description: " + previousPunishment.Description + ", Remarks: " + previousPunishment.Remarks;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end


                return await previousPunishmentRepository.DeleteAsync(previousPunishment);
            }
        }
    }
}
