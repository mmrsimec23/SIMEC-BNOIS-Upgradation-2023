using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class ResultService : IResultService
    {
        private readonly IBnoisRepository<Result> resultRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public ResultService(IBnoisRepository<Result> resultRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.resultRepository = resultRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

        public List<ResultModel> GetResults(int pageSize, int pageNumber, string searchText, out int total)
        {
            IQueryable<Result> results = resultRepository.FilterWithInclude(x => x.IsActive
               && ((x.Name.Contains(searchText)
               || String.IsNullOrEmpty(searchText))));
            total = results.Count();
            results = results.OrderBy(x => x.ResultCode).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return ObjectConverter<Result, ResultModel>.ConvertList(results.ToList()).ToList();
        }
        public async Task<ResultModel> GetResult(int resultId)
        {
            if (resultId == 0)
            {
                return new ResultModel();
            }
            Result result = await resultRepository.FindOneAsync(x => x.ResultId == resultId);
            if (result == null)
            {
                throw new InfinityNotFoundException("Result not found !");
            }
            return ObjectConverter<Result, ResultModel>.Convert(result);
        }

        public async Task<ResultModel> SaveResult(int resultId, ResultModel model)
        {
            bool isExist = await resultRepository.ExistsAsync(x =>x.Name == model.Name && x.ResultId != model.ResultId);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Result data already exist !");
            }
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Result data missing");
            }
            Result result = ObjectConverter<ResultModel, Result>.Convert(model);
            if (resultId > 0)
            {
                result = await resultRepository.FindOneAsync(x => x.ResultId == resultId);
                if (result == null)
                {
                    throw new InfinityNotFoundException("Result not found!");
                }
                result.ModifiedBy = model.ModifiedBy;
                result.ModifiedDate = DateTime.Now;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Result";
                bnLog.TableEntryForm = "Result";
                bnLog.PreviousValue = "Id: " + model.ResultId;
                bnLog.UpdatedValue = "Id: " + model.ResultId;
                int bnoisUpdateCount = 0;
                if (result.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + result.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                    bnoisUpdateCount += 1;
                }
                if (result.ResultCode != model.ResultCode)
                {
                    bnLog.PreviousValue += ", ResultCode: " + result.ResultCode;
                    bnLog.UpdatedValue += ", ResultCode: " + model.ResultCode;
                    bnoisUpdateCount += 1;
                }
                if (result.Description != model.Description)
                {
                    bnLog.PreviousValue += ", Description: " + result.Description;
                    bnLog.UpdatedValue += ", Description: " + model.Description;
                    bnoisUpdateCount += 1;
                }
                if (result.MinGPA != model.MinGPA)
                {
                    bnLog.PreviousValue += ", MinGPA: " + result.MinGPA;
                    bnLog.UpdatedValue += ", MinGPA: " + model.MinGPA;
                    bnoisUpdateCount += 1;
                }
                if (result.MaxGPA != model.MaxGPA)
                {
                    bnLog.PreviousValue += ", MaxGPA: " + result.MaxGPA;
                    bnLog.UpdatedValue += ", MaxGPA: " + model.MaxGPA;
                    bnoisUpdateCount += 1;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = model.CreatedBy;
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
                result.CreatedBy = model.CreatedBy;
                result.CreatedDate = DateTime.Now;
                result.IsActive = true;
            }
            result.ResultCode = model.ResultCode;
            result.Name = model.Name;
            result.Description = model.Description;
            result.MinGPA = model.MinGPA;
            result.MaxGPA = model.MaxGPA;
         
            await resultRepository.SaveAsync(result);
            model.ResultId = result.ResultId;
            return model;
        }

        public async Task<bool> DeleteResult(int resultId)
        {
            if (resultId <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Result result = await resultRepository.FindOneAsync(x => x.ResultId == resultId);
            if (result == null)
            {
                throw new InfinityNotFoundException("Result not found!");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Result";
                bnLog.TableEntryForm = "Result";
                bnLog.PreviousValue = "Id: " + result.ResultId + ", Name: " + result.Name + ", ResultCode: " + result.ResultCode + ", Description: " + result.Description + ", MinGPA: " + result.MinGPA + ", MaxGPA: " + result.MaxGPA;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await resultRepository.DeleteAsync(result);
            }
        }

        public async Task<List<SelectModel>> ResultsSelectModel(int? examCategoryId)
        {
            IQueryable<Result> queryable =  resultRepository.Where(x => x.IsActive).OrderBy(x => x.ResultCode);
            List<Result> results = await queryable.ToListAsync();
            List<SelectModel> resultSelectModels = results.OrderBy(x=>x.ResultCode).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.ResultId
            }).ToList();
            return resultSelectModels;
        }


        public async Task<List<SelectModel>> ResultSelectModels()
        {
            IQueryable<Result> queryable = resultRepository.Where(x => x.IsActive).OrderBy(x => x.ResultCode);
            List<Result> results = await queryable.ToListAsync();
            List<SelectModel> resultSelectModels = results.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.ResultId
            }).ToList();
            return resultSelectModels;
        }
    }
}
