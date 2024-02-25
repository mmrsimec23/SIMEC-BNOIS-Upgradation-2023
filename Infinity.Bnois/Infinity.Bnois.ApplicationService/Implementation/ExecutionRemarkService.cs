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
    public class ExecutionRemarkService : IExecutionRemarkService
    {
        private readonly IBnoisRepository<ExecutionRemark> executionRemarkRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public ExecutionRemarkService(IBnoisRepository<ExecutionRemark> executionRemarkRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.executionRemarkRepository = executionRemarkRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

        public List<ExecutionRemarkModel> GetExecutionRemarks(int ps, int pn, string qs, out int total,int type)
        {
            IQueryable<ExecutionRemark> executionRemarks = executionRemarkRepository.FilterWithInclude(x => x.IsActive && x.Type==type && (x.Name.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = executionRemarks.Count();
            executionRemarks = executionRemarks.OrderByDescending(x => x.ExecutionRemarkId).Skip((pn - 1) * ps).Take(ps);
            List<ExecutionRemarkModel> models = ObjectConverter<ExecutionRemark, ExecutionRemarkModel>.ConvertList(executionRemarks.ToList()).ToList();
            return models;
        }

        public async Task<ExecutionRemarkModel> GetExecutionRemark(int id)
        {
            if (id <= 0)
            {
                return new ExecutionRemarkModel();
            }
            ExecutionRemark executionRemark = await executionRemarkRepository.FindOneAsync(x => x.ExecutionRemarkId == id);
            if (executionRemark == null)
            {
                throw new InfinityNotFoundException("ExecutionRemark not found");
            }
            ExecutionRemarkModel model = ObjectConverter<ExecutionRemark, ExecutionRemarkModel>.Convert(executionRemark);
            return model;
        }

        public async Task<ExecutionRemarkModel> SaveExecutionRemark(int id, ExecutionRemarkModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("ExecutionRemark data missing");
            }
            bool isExist = executionRemarkRepository.Exists(x => x.Name== model.Name  && x.ExecutionRemarkId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            ExecutionRemark executionRemark = ObjectConverter<ExecutionRemarkModel, ExecutionRemark>.Convert(model);
            if (id > 0)
            {
                executionRemark = await executionRemarkRepository.FindOneAsync(x => x.ExecutionRemarkId == id);
                if (executionRemark == null)
                {
                    throw new InfinityNotFoundException("ExecutionRemark not found !");
                }

                executionRemark.ModifiedDate = DateTime.Now;
                executionRemark.ModifiedBy = userId;


                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ExecutionRemark";
                bnLog.TableEntryForm = "Execution Remark";
                bnLog.PreviousValue = "Id: " + model.ExecutionRemarkId;
                bnLog.UpdatedValue = "Id: " + model.ExecutionRemarkId;
                int bnoisUpdateCount = 0;

                
                if (executionRemark.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + executionRemark.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                    bnoisUpdateCount += 1;
                }
                if (executionRemark.Type != model.Type)
                {
                    bnLog.PreviousValue += ", Type: " + executionRemark.Type;
                    bnLog.UpdatedValue += ", Type: " + model.Type;
                    bnoisUpdateCount += 1;
                }
                if (executionRemark.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ", Short Name: " + executionRemark.ShortName;
                    bnLog.UpdatedValue += ", Short Name: " + model.ShortName;
                    bnoisUpdateCount += 1;
                }
                if (executionRemark.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + executionRemark.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                    bnoisUpdateCount += 1;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString(); ;
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
                executionRemark.IsActive = true;
                executionRemark.CreatedDate = DateTime.Now;
                executionRemark.CreatedBy = userId;
            }
            executionRemark.Type = model.Type;
            executionRemark.Name = model.Name;
            executionRemark.Remarks = model.Remarks;
            await executionRemarkRepository.SaveAsync(executionRemark);
            model.ExecutionRemarkId = executionRemark.ExecutionRemarkId;
            return model;
        }

        public async Task<bool> DeleteExecutionRemark(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            ExecutionRemark executionRemark = await executionRemarkRepository.FindOneAsync(x => x.ExecutionRemarkId == id);
            if (executionRemark == null)
            {
                throw new InfinityNotFoundException("ExecutionRemark not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ExecutionRemark";
                bnLog.TableEntryForm = "Execution Remark";
                bnLog.PreviousValue = "Id: " + executionRemark.ExecutionRemarkId;

                
                bnLog.PreviousValue += ", Name: " + executionRemark.Name + ", Type: " + executionRemark.Type + ", Short Name: " + executionRemark.ShortName + ", Ext Lpr Date: " + ", Remarks: " + executionRemark.Remarks;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await executionRemarkRepository.DeleteAsync(executionRemark);
            }
        }

        public async Task<List<SelectModel>> GetExecutionRemarkSelectModels(int type)
        {
            ICollection<ExecutionRemark> models = await executionRemarkRepository.FilterAsync(x => x.IsActive && x.Type==type);
            return models.Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.ExecutionRemarkId
            }).ToList();

        }
    }
}
