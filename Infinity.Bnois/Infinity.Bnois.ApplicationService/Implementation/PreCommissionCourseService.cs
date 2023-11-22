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
   public class PreCommissionCourseService: IPreCommissionCourseService
    {
        private readonly IBnoisRepository<PreCommissionCourse> preCommissionCourseRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public PreCommissionCourseService(IBnoisRepository<PreCommissionCourse> preCommissionCourseRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.preCommissionCourseRepository = preCommissionCourseRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }
        
        public async Task<PreCommissionCourseModel> GetPreCommissionCourse(int preCommissionCourseId)
        {
            if (preCommissionCourseId <= 0)
            {
                return new PreCommissionCourseModel();
            }
            PreCommissionCourse preCommissionCourse = await preCommissionCourseRepository.FindOneAsync(x => x.PreCommissionCourseId == preCommissionCourseId);

            if (preCommissionCourse == null)
            {
                throw new InfinityNotFoundException("Pre Commission Course not found!");
            }
            PreCommissionCourseModel model = ObjectConverter<PreCommissionCourse, PreCommissionCourseModel>.Convert(preCommissionCourse);
            return model;
        }

        public List<PreCommissionCourseModel> GetPreCommissionCourses(int employeeId)
        {
            List<PreCommissionCourse> preCommissionCourses = preCommissionCourseRepository.FilterWithInclude(x => x.EmployeeId == employeeId,"Employee","Country","Medal").ToList();
            List<PreCommissionCourseModel> models = ObjectConverter<PreCommissionCourse, PreCommissionCourseModel>.ConvertList(preCommissionCourses.ToList()).ToList();
            return models;
        }

        public async Task<PreCommissionCourseModel> SavePreCommissionCourse(int preCommissionCourseId, PreCommissionCourseModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Pre Commission Course data missing");
            }
            

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            PreCommissionCourse preCommissionCourse = ObjectConverter<PreCommissionCourseModel, PreCommissionCourse>.Convert(model);
            if (preCommissionCourseId > 0)
            {
                preCommissionCourse = await preCommissionCourseRepository.FindOneAsync(x => x.PreCommissionCourseId == preCommissionCourseId);
                if (preCommissionCourse == null)
                {
                    throw new InfinityNotFoundException("Pre Commission Course not found !");
                }
                preCommissionCourse.Modified = DateTime.Now;
                preCommissionCourse.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PreCommissionCourse";
                bnLog.TableEntryForm = "Employee Pre Commission Test";
                bnLog.PreviousValue = "Id: " + model.PreCommissionCourseId;
                bnLog.UpdatedValue = "Id: " + model.PreCommissionCourseId;
                int bnoisUpdateCount = 0;
                if (preCommissionCourse.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", preCommissionCourse.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                    //bnoisUpdateCount += 1;
                }
                if (preCommissionCourse.BnaNo != model.BnaNo)
                {
                    bnLog.PreviousValue += ", BNA No: " + preCommissionCourse.BnaNo;
                    bnLog.UpdatedValue += ", BNA No: " + model.BnaNo;
                    bnoisUpdateCount += 1;
                }
                if (preCommissionCourse.IsAbroad != model.IsAbroad)
                {
                    bnLog.PreviousValue += ", Course Place: " + (preCommissionCourse.IsAbroad == true ? "Outside" : "Inside");
                    bnLog.UpdatedValue += ", Course Place: " + (model.IsAbroad == true ? "Outside" : "Inside");
                    bnoisUpdateCount += 1;
                }
                if (preCommissionCourse.CountryId != model.CountryId)
                {
                    if (preCommissionCourse.CountryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", preCommissionCourse.CountryId ?? 0);
                        bnLog.PreviousValue += ", Country: " + ((dynamic)prev).FullName;
                    }
                    if (model.CountryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Country", "CountryId", model.CountryId ?? 0);
                        bnLog.UpdatedValue += ", Country: " + ((dynamic)newv).FullName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (preCommissionCourse.MedalId != model.MedalId)
                {
                    if (preCommissionCourse.MedalId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Medal", "MedalId", preCommissionCourse.MedalId ?? 0);
                        bnLog.PreviousValue += ", Medal: " + ((dynamic)prev).NameEng;
                    }
                    if (model.MedalId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Medal", "MedalId", model.MedalId ?? 0);
                        bnLog.UpdatedValue += ", Medal: " + ((dynamic)newv).NameEng;
                    }
                    bnoisUpdateCount += 1;
                }
                if (preCommissionCourse.Punishment != model.Punishment)
                {
                    bnLog.PreviousValue += ", Punishment: " + preCommissionCourse.Punishment;
                    bnLog.UpdatedValue += ", Punishment: " + model.Punishment;
                    bnoisUpdateCount += 1;
                }
                if (preCommissionCourse.AppointmentHeld != model.AppointmentHeld)
                {
                    bnLog.PreviousValue += ", Appointment Held: " + preCommissionCourse.AppointmentHeld;
                    bnLog.UpdatedValue += ", Appointment Held: " + model.AppointmentHeld;
                    bnoisUpdateCount += 1;
                }
                if (preCommissionCourse.ModuleD != model.ModuleD)
                {
                    bnLog.PreviousValue += ", Module D: " + preCommissionCourse.ModuleD;
                    bnLog.UpdatedValue += ", Module D: " + model.ModuleD;
                    bnoisUpdateCount += 1;
                }
                if (preCommissionCourse.Total != model.Total)
                {
                    bnLog.PreviousValue += ", Total: " + preCommissionCourse.Total;
                    bnLog.UpdatedValue += ", Total: " + model.Total;
                    bnoisUpdateCount += 1;
                }
                if (preCommissionCourse.FinalPosition != model.FinalPosition)
                {
                    bnLog.PreviousValue += ", Final Position: " + preCommissionCourse.FinalPosition;
                    bnLog.UpdatedValue += ", Final Position: " + model.FinalPosition;
                    bnoisUpdateCount += 1;
                }
                if (preCommissionCourse.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + preCommissionCourse.Remarks;
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
                preCommissionCourse.IsActive = true;
                preCommissionCourse.CreatedDate = DateTime.Now;
                preCommissionCourse.CreatedBy = userId;
            }
            preCommissionCourse.EmployeeId = model.EmployeeId;
            preCommissionCourse.BnaNo = model.BnaNo;
            preCommissionCourse.IsAbroad = model.IsAbroad;
            preCommissionCourse.CountryId = model.CountryId;
            preCommissionCourse.MedalId = model.MedalId;
            preCommissionCourse.AppointmentHeld = model.AppointmentHeld;
            preCommissionCourse.ModuleD = model.ModuleD;
            preCommissionCourse.Total = model.Total;
            preCommissionCourse.FinalPosition = model.FinalPosition;
            preCommissionCourse.Remarks = model.Remarks;
            
            await preCommissionCourseRepository.SaveAsync(preCommissionCourse);
            return model;
        }

        public async Task<bool> DeletePreCommissionCourse(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }

            PreCommissionCourse preCommissionCourse = await preCommissionCourseRepository.FindOneAsync(x => x.PreCommissionCourseId == id);
            if (preCommissionCourse == null)
            {
                throw new InfinityNotFoundException("Pre Commission Course not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PreCommissionCourse";
                bnLog.TableEntryForm = "Employee Pre Commission Test";
                bnLog.PreviousValue = "Id: " + preCommissionCourse.PreCommissionCourseId;
                var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", preCommissionCourse.EmployeeId);
                bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                bnLog.PreviousValue += ", BNA No: " + preCommissionCourse.BnaNo + ", Course Place: " + (preCommissionCourse.IsAbroad == true ? "Outside" : "Inside");
                if (preCommissionCourse.CountryId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", preCommissionCourse.CountryId ?? 0);
                    bnLog.PreviousValue += ", Country: " + ((dynamic)prev).FullName;
                }
                if (preCommissionCourse.MedalId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Medal", "MedalId", preCommissionCourse.MedalId ?? 0);
                    bnLog.PreviousValue += ", Medal: " + ((dynamic)prev).NameEng;
                }
                bnLog.PreviousValue += ", Punishment: " + preCommissionCourse.Punishment + ", Appointment Held: " + preCommissionCourse.AppointmentHeld + ", Module D: " + preCommissionCourse.ModuleD + ", Total: " + preCommissionCourse.Total + ", Final Position: " + preCommissionCourse.FinalPosition + ", Remarks: " + preCommissionCourse.Remarks;
                

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                bool isDeleted = false;
                try
                {
                    isDeleted = await preCommissionCourseRepository.DeleteAsync(preCommissionCourse);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                
                return isDeleted;
            }
        }
    }
}
