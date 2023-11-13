using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.Data;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.ExceptionHelper;
using Infinity.Bnois.Configuration;

namespace Infinity.Bnois.ApplicationService.Implementation
{
   public class ExtracurricularService: IExtracurricularService
    {
        private readonly IBnoisRepository<Extracurricular> extracurricularRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public ExtracurricularService(IBnoisRepository<Extracurricular> extracurricularRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.extracurricularRepository = extracurricularRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public async Task<ExtracurricularModel> GetExtracurricular(int extracurricularId)
        {
            if (extracurricularId <= 0)
            {
                return new ExtracurricularModel();
            }
            Extracurricular extracurricular = await extracurricularRepository.FindOneAsync(x => x.ExtracurricularId == extracurricularId);

            if (extracurricular == null)
            {
                throw new InfinityNotFoundException("Extracurricular not found!");
            }
            ExtracurricularModel model = ObjectConverter<Extracurricular, ExtracurricularModel>.Convert(extracurricular);
            return model;
        }

        public List<ExtracurricularModel> GetExtracurriculars(int employeeId)
        {
            List<Extracurricular> extracurriculars = extracurricularRepository.FilterWithInclude(x => x.EmployeeId == employeeId, "ExtracurricularType").ToList();
            List<ExtracurricularModel> models = ObjectConverter<Extracurricular, ExtracurricularModel>.ConvertList(extracurriculars.ToList()).ToList();
            return models;
        }

        public async Task<ExtracurricularModel> SaveExtracurricular(int extracurricularId, ExtracurricularModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Extracurricular data missing");
            }
            bool isExist = await extracurricularRepository.ExistsAsync(x => x.ExtracurricularTypeId == model.ExtracurricularTypeId && x.EmployeeId == model.EmployeeId && x.ExtracurricularId != model.ExtracurricularId);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Extracurricular data already exist !");
            }
            Extracurricular extracurricular = ObjectConverter<ExtracurricularModel, Extracurricular>.Convert(model);
            string userId= ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            if (extracurricularId > 0)
            {
                extracurricular = await extracurricularRepository.FindOneAsync(x => x.ExtracurricularId == extracurricularId);
                if (extracurricular == null)
                {
                    throw new InfinityNotFoundException("Extracurricular not found!");
                }
                extracurricular.ModifiedBy = userId;
                extracurricular.ModifiedDate = DateTime.Now;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Extracurricular";
                bnLog.TableEntryForm = "Employee Extra Curricular Activities";
                bnLog.PreviousValue = "Id: " + model.ExtracurricularId;
                bnLog.UpdatedValue = "Id: " + model.ExtracurricularId;
                int bnoisUpdateCount = 0;
                if (extracurricular.EmployeeId != model.EmployeeId)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", extracurricular.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    bnLog.UpdatedValue += ", Name: " + ((dynamic)emp).PNo + "_" + ((dynamic)emp).FullNameEng;
                    bnoisUpdateCount += 1;
                }
                if(extracurricular.ExtracurricularTypeId != model.ExtracurricularTypeId)
                {
                    if(extracurricular.ExtracurricularTypeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("extracurricularType", "ExtracurricularTypeId", extracurricular.ExtracurricularTypeId);
                        bnLog.PreviousValue += ", Extracurricular Type: " + ((dynamic)prev).Name;
                    }
                    if(model.ExtracurricularTypeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("extracurricularType", "ExtracurricularTypeId", model.ExtracurricularTypeId);
                        bnLog.UpdatedValue += ", Extracurricular Type: " + ((dynamic)newv).Name;
                    }
                }
                if (extracurricular.HoldAnyPost != model.HoldAnyPost)
                {
                    bnLog.PreviousValue += ", Hold any post (School/College/University): " + extracurricular.HoldAnyPost;
                    bnLog.UpdatedValue += ", Hold any post (School/College/University): " + model.HoldAnyPost;
                    bnoisUpdateCount += 1;
                }
                if (extracurricular.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + extracurricular.Remarks;
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
                extracurricular.CreatedDate = DateTime.Now;
                extracurricular.CreatedBy = userId;
            }
            extracurricular.EmployeeId = model.EmployeeId;
            extracurricular.ExtracurricularTypeId = model.ExtracurricularTypeId;
            extracurricular.HoldAnyPost = model.HoldAnyPost;
            extracurricular.Remarks = model.Remarks;
            await extracurricularRepository.SaveAsync(extracurricular);
            model.ExtracurricularId = extracurricular.ExtracurricularId;
            return model;
        }


        public async Task<bool> DeleteExtraCurricular(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Extracurricular extracurricular = await extracurricularRepository.FindOneAsync(x => x.ExtracurricularId == id);
            if (extracurricular == null)
            {
                throw new InfinityNotFoundException("Extracurricular not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Extracurricular";
                bnLog.TableEntryForm = "Employee Extra Curricular Activities";
                bnLog.PreviousValue = "Id: " + extracurricular.ExtracurricularId;
                
                var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", extracurricular.EmployeeId);
                bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                
                if (extracurricular.ExtracurricularTypeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("extracurricularType", "ExtracurricularTypeId", extracurricular.ExtracurricularTypeId);
                    bnLog.PreviousValue += ", Extracurricular Type: " + ((dynamic)prev).Name;
                }
                bnLog.PreviousValue += ", Hold any post (School/College/University): " + extracurricular.HoldAnyPost  + ", Remarks: " + extracurricular.Remarks;
                       
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await extracurricularRepository.DeleteAsync(extracurricular);
            }
        }
    }
}
