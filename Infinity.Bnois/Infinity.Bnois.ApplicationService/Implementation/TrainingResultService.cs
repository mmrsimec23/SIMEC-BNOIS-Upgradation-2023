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
    public class TrainingResultService : ITrainingResultService
    {
        private readonly IBnoisRepository<TrainingResult> trainingResultRepository;
        private readonly IBnoisRepository<TrainingPlan> trainingPlanRepository;
        private readonly IProcessRepository processRepository;
        private readonly IBnoisRepository<NominationDetail> nominationDetailRepository;
        private readonly IBnoisRepository<Nomination> nominationRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public TrainingResultService(IBnoisRepository<TrainingResult> trainingResultRepository, IBnoisRepository<TrainingPlan> trainingPlanRepository, IProcessRepository processRepository, IBnoisRepository<NominationDetail> nominationDetailRepository, IBnoisRepository<Nomination> nominationRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.trainingResultRepository = trainingResultRepository;
            this.trainingPlanRepository = trainingPlanRepository;
            this.processRepository = processRepository;
            this.nominationDetailRepository = nominationDetailRepository;
            this.nominationRepository = nominationRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<TrainingResultModel> GetTrainingResults(int ps, int pn, string qs, out int total)
        {
            IQueryable<TrainingResult> trainingResults = trainingResultRepository.FilterWithInclude(x => x.IsActive
                && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs)||x.ResultType.TypeName==qs || String.IsNullOrEmpty(qs)), "Employee", "TrainingPlan", "ResultType", "TrainingPlan.Course");
            total = trainingResults.Count();
            trainingResults = trainingResults.OrderByDescending(x => x.TrainingResultId).Skip((pn - 1) * ps).Take(ps);
            List<TrainingResultModel> models = ObjectConverter<TrainingResult, TrainingResultModel>.ConvertList(trainingResults.ToList()).ToList();
            return models;
        }

        public async Task<TrainingResultModel> GetTrainingResult(int id)
        {
            if (id <= 0)
            {
                return new TrainingResultModel();
            }
            TrainingResult trainingResult = await trainingResultRepository.FindOneAsync(x => x.TrainingResultId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch","TrainingPlan.Country" });
            if (trainingResult == null)
            {
                throw new InfinityNotFoundException("Training Result not found");
            }
            TrainingResultModel model = ObjectConverter<TrainingResult, TrainingResultModel>.Convert(trainingResult);
            return model;
        }

        public async Task<TrainingResultModel> GetTrainingResultByEmployee(int id)
        {
            if (id <= 0)
            {
                return new TrainingResultModel();
            }
            TrainingResult trainingResult = await trainingResultRepository.FindOneAsync(x => x.EmployeeId == id, new List<string> { "Employee", "TrainingPlan", "TrainingPlan.Course", "TrainingPlan.TrainingInstitute", "TrainingPlan.Country", "TrainingPlan.CourseCategory", "TrainingPlan.CourseSubCategory", "ResultType" });
            if (trainingResult == null)
            {
                throw new InfinityNotFoundException("Training Result not found");
            }
            TrainingResultModel model = ObjectConverter<TrainingResult, TrainingResultModel>.Convert(trainingResult);
            return model;
        }

        public async Task<TrainingResultModel> SaveTrainingResult(int id, TrainingResultModel model)
        {
            model.Employee = null;

            if (model == null)
            {
                throw new InfinityArgumentMissingException("Training Result  data missing");
            }     
            if (model.TrainingPlanId >0)
            {
                TrainingPlan trainingPlan = await trainingPlanRepository.FindOneAsync(x => x.TrainingPlanId == model.TrainingPlanId);

                if (trainingPlan.ToDate > DateTime.Today && (model.ResultTypeId>1))
                { 
                    throw new InfinityArgumentMissingException("Course not finished yet.");
                }
              

                Nomination nominationWithoutTransfer = await nominationRepository.FindOneAsync(x => x.EnitityType == 1 && x.EntityId == model.TrainingPlanId && x.WithoutTransfer == false);
                if (nominationWithoutTransfer != null)
                {
                    bool isExistData = nominationDetailRepository.Exists(x => x.NominationId == nominationWithoutTransfer.NominationId && x.EmployeeId == model.EmployeeId);
                    if (!isExistData)
                    {
                        throw new InfinityInvalidDataException("Officer is not nominated for this course. !!");
                    }
                }

                Nomination nominationWithTransfer = await nominationRepository.FindOneAsync(x => x.EnitityType==1&& x.EntityId == model.TrainingPlanId && x.WithoutTransfer);
                if (nominationWithTransfer != null)
                {
                    bool isExistData = nominationDetailRepository.Exists(x => x.NominationId == nominationWithTransfer.NominationId && x.EmployeeId == model.EmployeeId);
                    if (!isExistData)
                    {
                        throw new InfinityInvalidDataException("Officer is not nominated for this course. !!");
                    }
                }
                bool isExist = await trainingResultRepository.ExistsAsync(x =>x.TrainingPlanId == model.TrainingPlanId && x.EmployeeId == model.EmployeeId && x.TrainingResultId != model.TrainingResultId);

                if (isExist)
                {
                    throw new InfinityInvalidDataException("This result already exist !");
                }
                model.CourseCategoryId = trainingPlan.CourseCategoryId;
                model.CourseSubCategoryId = trainingPlan.CourseSubCategoryId;
               
            }


            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            TrainingResult trainingResult = ObjectConverter<TrainingResultModel, TrainingResult>.Convert(model);
            if (id > 0)
            {
                trainingResult = await trainingResultRepository.FindOneAsync(x => x.TrainingResultId == id);
                if (trainingResult == null)
                {
                    throw new InfinityNotFoundException("Training Result not found !");
                }

                trainingResult.ModifiedDate = DateTime.Now;
                trainingResult.ModifiedBy = userId;



                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "TrainingResult";
                bnLog.TableEntryForm = "Participant Result";
                bnLog.PreviousValue = "Id: " + model.TrainingResultId;
                bnLog.UpdatedValue = "Id: " + model.TrainingResultId;
                int bnoisUpdateCount = 0;


                if (trainingResult.EmployeeId > 0)
                {
                    if (trainingResult.EmployeeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", trainingResult.EmployeeId);
                        bnLog.PreviousValue += ", P No: " + ((dynamic)prev).PNo;
                    }
                    if (model.EmployeeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                        bnLog.UpdatedValue += ", P No: " + ((dynamic)newv).PNo;
                    }
                    //bnoisUpdateCount += 1;
                }
                if (trainingResult.CourseCategoryId != model.CourseCategoryId)
                {
                    if (trainingResult.CourseCategoryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("CourseCategory", "CourseCategoryId", trainingResult.CourseCategoryId??0);
                        bnLog.PreviousValue += ", Course Category: " + ((dynamic)prev).Name;
                    }
                    if (model.EmployeeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("CourseCategory", "CourseCategoryId", model.CourseCategoryId??0);
                        bnLog.UpdatedValue += ", Course Category: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (trainingResult.CourseSubCategoryId != model.CourseSubCategoryId)
                {
                    if (trainingResult.CourseSubCategoryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("CourseSubCategory", "CourseSubCategoryId", trainingResult.CourseSubCategoryId ?? 0);
                        bnLog.PreviousValue += ", Course Sub Category: " + ((dynamic)prev).Name;
                    }
                    if (model.CourseSubCategoryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("CourseSubCategory", "CourseSubCategoryId", model.CourseSubCategoryId ?? 0);
                        bnLog.UpdatedValue += ", Course Sub Category: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (trainingResult.TrainingPlanId != model.TrainingPlanId)
                {
                    if (trainingResult.TrainingPlanId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("TrainingPlan", "TrainingPlanId", trainingResult.TrainingPlanId);
                        bnLog.PreviousValue += ", TrainingPlanId: " + ((dynamic)prev).TrainingPlanId;
                    }
                    if (model.CourseSubCategoryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("TrainingPlanId", "TrainingPlanId", model.TrainingPlanId);
                        bnLog.UpdatedValue += ", TrainingPlanId: " + ((dynamic)newv).TrainingPlanId;
                    }
                    bnoisUpdateCount += 1;
                }
                if (trainingResult.ResultTypeId != model.ResultTypeId)
                {
                    if (trainingResult.ResultTypeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("ResultType", "ResultTypeId", trainingResult.ResultTypeId);
                        bnLog.PreviousValue += ", Result Type: " + ((dynamic)prev).TypeName;
                    }
                    if (model.ResultTypeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("ResultTypeId", "ResultTypeId", model.ResultTypeId);
                        bnLog.UpdatedValue += ", Result Type: " + ((dynamic)newv).TpyeName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (trainingResult.ResultDate != model.ResultDate)
                {
                    bnLog.PreviousValue += ", Result Date: " + trainingResult.ResultDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Result Date: " + model.ResultDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (trainingResult.Percentage != model.Percentage)
                {
                    bnLog.PreviousValue += ", Percentage: " + trainingResult.Percentage;
                    bnLog.UpdatedValue += ", Percentage: " + model.Percentage;
                    bnoisUpdateCount += 1;
                }
                if (trainingResult.Positon != model.Positon)
                {
                    bnLog.PreviousValue += ", Positon: " + trainingResult.Positon;
                    bnLog.UpdatedValue += ", Positon: " + model.Positon;
                    bnoisUpdateCount += 1;
                }
                if (trainingResult.ResultStatus != model.ResultStatus)
                {
                    bnLog.PreviousValue += ",  Result Status: " + (trainingResult.ResultStatus == 1 ? "Good" : trainingResult.ResultStatus == 2 ? "Poor" : "");
                    bnLog.UpdatedValue += ",  Result Status: " + (model.ResultStatus == 1 ? "Good" : model.ResultStatus == 2 ? "Poor" : "");
                    bnoisUpdateCount += 1;
                }
                if (trainingResult.ResultSection != model.ResultSection)
                {
                    bnLog.PreviousValue += ", Result Section: " + trainingResult.ResultSection;
                    bnLog.UpdatedValue += ", Result Section: " + model.ResultSection;
                    bnoisUpdateCount += 1;
                }
                if (trainingResult.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + trainingResult.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                    bnoisUpdateCount += 1;
                }
                if (trainingResult.Unit != model.Unit)
                {
                    bnLog.PreviousValue += ", Appointment Reccomendation (For Report): " + trainingResult.Unit;
                    bnLog.UpdatedValue += ", Appointment Reccomendation (For Report): " + model.Unit;
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
                trainingResult.IsActive = true;
                trainingResult.CreatedDate = DateTime.Now;
                trainingResult.CreatedBy = userId;
            }
            trainingResult.EmployeeId = model.EmployeeId;
            trainingResult.TrainingPlanId = model.TrainingPlanId;
            trainingResult.ResultTypeId = model.ResultTypeId;
            trainingResult.CourseCategoryId = model.CourseCategoryId;
            trainingResult.CourseSubCategoryId = model.CourseSubCategoryId;
            trainingResult.Positon = model.Positon;
            trainingResult.Percentage = model.Percentage;
            trainingResult.ResultDate = model.ResultDate ?? trainingResult.ResultDate;
            trainingResult.ResultStatus = model.ResultStatus;
            trainingResult.Remarks = model.Remarks;
            trainingResult.Unit = model.Unit;

            await trainingResultRepository.SaveAsync(trainingResult);
            model.TrainingResultId = trainingResult.TrainingResultId;

            await processRepository.UpdateNamingConvention(trainingResult.EmployeeId);

            return model;
        }

        public async Task<bool> DeleteTrainingResult(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            TrainingResult trainingResult = await trainingResultRepository.FindOneAsync(x => x.TrainingResultId == id);
            if (trainingResult == null)
            {
                throw new InfinityNotFoundException("Training Result not found");
            }
            else
            {


                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "TrainingResult";
                bnLog.TableEntryForm = "Participant Result";
                bnLog.PreviousValue = "Id: " + trainingResult.TrainingResultId;

                if (trainingResult.EmployeeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", trainingResult.EmployeeId);
                    bnLog.PreviousValue += ", P No: " + ((dynamic)prev).PNo;
                }
                if (trainingResult.CourseCategoryId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("CourseCategory", "CourseCategoryId", trainingResult.CourseCategoryId ?? 0);
                    bnLog.PreviousValue += ", Course Category: " + ((dynamic)prev).Name;
                }
                if (trainingResult.CourseSubCategoryId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("CourseSubCategory", "CourseSubCategoryId", trainingResult.CourseSubCategoryId ?? 0);
                    bnLog.PreviousValue += ", Course Sub Category: " + ((dynamic)prev).Name;
                }
                if (trainingResult.TrainingPlanId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("TrainingPlan", "TrainingPlanId", trainingResult.TrainingPlanId);
                    bnLog.PreviousValue += ", TrainingPlanId: " + ((dynamic)prev).TrainingPlanId;
                }
                if (trainingResult.ResultTypeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("ResultType", "ResultTypeId", trainingResult.ResultTypeId);
                    bnLog.PreviousValue += ", Result Type: " + ((dynamic)prev).TypeName;
                }
                bnLog.PreviousValue += ", Result Date: " + trainingResult.ResultDate?.ToString("dd/MM/yyyy") + ", Percentage: " + trainingResult.Percentage + ", Positon: " + trainingResult.Positon + ",  Result Status: " + (trainingResult.ResultStatus == 1 ? "Good" : trainingResult.ResultStatus == 2 ? "Poor" : "") + ", Result Section: " + trainingResult.ResultSection + ", Remarks: " + trainingResult.Remarks + ", Appointment Reccomendation (For Report): " + trainingResult.Unit;


                bnLog.UpdatedValue = "This Record has been Deleted!";
                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);
                //data log section end

                var result = await trainingResultRepository.DeleteAsync(trainingResult);
                await processRepository.UpdateNamingConvention(trainingResult.EmployeeId);
                return result;
            }
        }


        public List<SelectModel> GetResultStatusSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(ResultStatus)).Cast<ResultStatus>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }

        public async Task<TrainingResultModel> UpdateTrainingResult(TrainingResultModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Training Result  data missing");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            TrainingResult trainingResult = ObjectConverter<TrainingResultModel, TrainingResult>.Convert(model);

            trainingResult = await trainingResultRepository.FindOneAsync(x => x.TrainingResultId == model.TrainingResultId);
            if (trainingResult == null)
            {
                throw new InfinityNotFoundException("Training Result not found !");
            }
        
            if (model.ResultSection != null)
            {
                trainingResult.ResultSection = model.ResultSection;
                trainingResult.FileName = model.ResultSection;
            }
            trainingResult.ModifiedDate = DateTime.Now;
            trainingResult.ModifiedBy = userId;
            await trainingResultRepository.SaveAsync(trainingResult);
            model.TrainingResultId = trainingResult.TrainingResultId;
            return model;
        }
    }
}
