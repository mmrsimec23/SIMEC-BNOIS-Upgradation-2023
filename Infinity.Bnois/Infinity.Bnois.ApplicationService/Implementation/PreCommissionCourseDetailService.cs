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

  public  class PreCommissionCourseDetailService: IPreCommissionCourseDetailService
    {
        private readonly IBnoisRepository<PreCommissionCourseDetail> preCommissionCourseDetailRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public PreCommissionCourseDetailService(IBnoisRepository<PreCommissionCourseDetail> preCommissionCourseDetailRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.preCommissionCourseDetailRepository = preCommissionCourseDetailRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public async Task<PreCommissionCourseDetailModel> GetPreCommissionCourseDetail(int preCommissionCourseDetailId)
        {
            if (preCommissionCourseDetailId <= 0)
            {
                return new PreCommissionCourseDetailModel();
            }
            PreCommissionCourseDetail preCommissionCourseDetail = await preCommissionCourseDetailRepository.FindOneAsync(x => x.PreCommissionCourseDetailId == preCommissionCourseDetailId);
            if (preCommissionCourseDetail == null)
            {
                throw new InfinityNotFoundException("Pre Commission Course Detail not found !");
            }
            PreCommissionCourseDetailModel model = ObjectConverter<PreCommissionCourseDetail, PreCommissionCourseDetailModel>.Convert(preCommissionCourseDetail);
            return model;
        }

        public List<PreCommissionCourseDetailModel> GetPreCommissionCourseDetails(int preCommissionCourseId)
        {
            ICollection<PreCommissionCourseDetail> preCommissionCourseDetails = preCommissionCourseDetailRepository.FilterWithInclude(x => x.IsActive && x.PreCommissionCourseId == preCommissionCourseId).ToList();
            List<PreCommissionCourseDetailModel> models = ObjectConverter<PreCommissionCourseDetail, PreCommissionCourseDetailModel>.ConvertList(preCommissionCourseDetails).ToList();
            return models;
        }

        public async Task<PreCommissionCourseDetailModel> SavePreCommissionCourseDetail(int preCommissionCourseDetailId, PreCommissionCourseDetailModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Pre Commission Course Detail data missing !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            PreCommissionCourseDetail preCommissionCourseDetail = ObjectConverter<PreCommissionCourseDetailModel, PreCommissionCourseDetail>.Convert(model);
            if (preCommissionCourseDetailId > 0)
            {
                preCommissionCourseDetail = await preCommissionCourseDetailRepository.FindOneAsync(x => x.PreCommissionCourseDetailId == preCommissionCourseDetailId);
                if (preCommissionCourseDetail == null)
                {
                    throw new InfinityNotFoundException("Pre Commission Course Detail not found !");
                }

                preCommissionCourseDetail.ModifiedDate = DateTime.Now;
                preCommissionCourseDetail.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PreCommissionCourseDetail";
                bnLog.TableEntryForm = "Employee Pre Commission Course Details";
                bnLog.PreviousValue = "Id: " + model.PreCommissionCourseDetailId;
                bnLog.UpdatedValue = "Id: " + model.PreCommissionCourseDetailId;
                int bnoisUpdateCount = 0;
                //if (preCommissionCourseDetail.EmployeeId != model.EmployeeId)
                //{
                //    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", preCommissionCourse.EmployeeId);
                //    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                //    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                //    bnLog.UpdatedValue += ", Name: " + ((dynamic)emp).PNo + "_" + ((dynamic)emp).FullNameEng;
                //    bnoisUpdateCount += 1;
                //}
                if (preCommissionCourseDetail.PreCommissionCourseId != model.PreCommissionCourseId)
                {
                    bnLog.PreviousValue += ", PreCommissionCourseId: " + preCommissionCourseDetail.PreCommissionCourseId;
                    bnLog.UpdatedValue += ", PreCommissionCourseId: " + model.PreCommissionCourseId;
                    bnoisUpdateCount += 1;
                }
                if (preCommissionCourseDetail.ModuleName != model.ModuleName)
                {
                    bnLog.PreviousValue += ", Module Name: " + (preCommissionCourseDetail.ModuleName);
                    bnLog.UpdatedValue += ", Module Name: " + (model.ModuleName);
                    bnoisUpdateCount += 1;
                }
                if (preCommissionCourseDetail.OutOfMark != model.OutOfMark)
                {
                    bnLog.PreviousValue += ", Out Of : " + preCommissionCourseDetail.OutOfMark;
                    bnLog.UpdatedValue += ", Out Of : " + model.OutOfMark;
                    bnoisUpdateCount += 1;
                }
                if (preCommissionCourseDetail.AchievedMark != model.AchievedMark)
                {
                    bnLog.PreviousValue += ", Achieved Mark: " + preCommissionCourseDetail.AchievedMark;
                    bnLog.UpdatedValue += ", Achieved Mark: " + model.AchievedMark;
                    bnoisUpdateCount += 1;
                }
                if (preCommissionCourseDetail.Position != model.Position)
                {
                    bnLog.PreviousValue += ", Position: " + preCommissionCourseDetail.Position;
                    bnLog.UpdatedValue += ", Position: " + model.Position;
                    bnoisUpdateCount += 1;
                }
                if (preCommissionCourseDetail.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + preCommissionCourseDetail.Remarks;
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
                preCommissionCourseDetail.IsActive = true;
                preCommissionCourseDetail.CreatedDate = DateTime.Now;
                preCommissionCourseDetail.CreatedBy = userId;
            }
            preCommissionCourseDetail.PreCommissionCourseId = model.PreCommissionCourseId;
            preCommissionCourseDetail.ModuleName = model.ModuleName;
            preCommissionCourseDetail.OutOfMark = model.OutOfMark;
            preCommissionCourseDetail.AchievedMark = model.AchievedMark;
            preCommissionCourseDetail.Position = model.Position;
            preCommissionCourseDetail.Remarks = model.Remarks;

            await preCommissionCourseDetailRepository.SaveAsync(preCommissionCourseDetail);
            model.PreCommissionCourseDetailId = preCommissionCourseDetail.PreCommissionCourseDetailId;
            return model;
        }
    }
}
