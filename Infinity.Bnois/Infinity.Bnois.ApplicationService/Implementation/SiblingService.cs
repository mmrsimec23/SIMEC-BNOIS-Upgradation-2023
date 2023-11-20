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
    public class SiblingService : ISiblingService
    {

        private readonly IBnoisRepository<Sibling> siblingRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public SiblingService(IBnoisRepository<Sibling> siblingRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.siblingRepository = siblingRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }


        public List<SiblingModel> GetSiblings(int employeeId)
        {
            List<Sibling> educations = siblingRepository.FilterWithInclude(x => x.EmployeeId == employeeId, "Occupation").ToList();
            List<SiblingModel> models = ObjectConverter<Sibling, SiblingModel>.ConvertList(educations.ToList()).ToList();
            models = models.Select(x =>
            {
                x.SiblingTypeName = Enum.GetName(typeof(SiblingType), x.SiblingType);
                return x;
            }).ToList();

            return models;
        }

        public async Task<SiblingModel> GetSibling(int siblingId)
        {
            if (siblingId <= 0)
            {
                return new SiblingModel();
            }
            Sibling sibling = await siblingRepository.FindOneAsync(x => x.SiblingId == siblingId, new List<string> { "Employee" });
            if (sibling == null)
            {
                throw new InfinityNotFoundException(" Sibling not found");
            }
            SiblingModel model = ObjectConverter<Sibling, SiblingModel>.Convert(sibling);
            return model;
        }

        public async Task<SiblingModel> SaveSibling(int siblingId, SiblingModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Sibling data missing");
            }

            bool isExistData = siblingRepository.Exists(x => x.OccupationId == model.OccupationId && x.EmployeeId != model.EmployeeId && x.Name == model.Name && x.SiblingType==model.SiblingType && x.SiblingId != siblingId);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Sibling sibling = ObjectConverter<SiblingModel, Sibling>.Convert(model);
            if (siblingId > 0)
            {
                sibling = await siblingRepository.FindOneAsync(x => x.SiblingId == siblingId);
                if (sibling == null)
                {
                    throw new InfinityNotFoundException("Sibling data not found !");
                }

                sibling.ModifiedDate = DateTime.Now;
                sibling.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Sibling";
                bnLog.TableEntryForm = "Employee Siblings Information";
                bnLog.PreviousValue = "Id: " + model.SiblingId;
                bnLog.UpdatedValue = "Id: " + model.SiblingId;
                int bnoisUpdateCount = 0;
                if (sibling.EmployeeId != model.EmployeeId)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", sibling.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    bnLog.UpdatedValue += ", Name: " + ((dynamic)emp).PNo + "_" + ((dynamic)emp).FullNameEng;
                    bnoisUpdateCount += 1;
                }
                if (sibling.SiblingType != model.SiblingType)
                {
                    bnLog.PreviousValue += ", Sibling Type: " + (sibling.SiblingType == 1 ? "Brother" : sibling.SiblingType == 2 ? "Sister" : "");
                    bnLog.UpdatedValue += ", Sibling Type: " + (model.SiblingType == 1 ? "Brother" : model.SiblingType == 2 ? "Sister" : "");
                    bnoisUpdateCount += 1;
                }
                if (sibling.OccupationId != model.OccupationId)
                {
                    if (sibling.OccupationId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", sibling.OccupationId ?? 0);
                        bnLog.PreviousValue += ", Occupation: " + ((dynamic)prev).Name;
                    }
                    if (model.OccupationId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", model.OccupationId ?? 0);
                        bnLog.UpdatedValue += ", Occupation: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (sibling.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + sibling.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                    bnoisUpdateCount += 1;
                }
                if (sibling.SpouseName != model.SpouseName)
                {
                    bnLog.PreviousValue += ", Name of the spouse: " + sibling.SpouseName;
                    bnLog.UpdatedValue += ", Name of the spouse: " + model.SpouseName;
                    bnoisUpdateCount += 1;
                }
                if (sibling.DateOfBirth != model.DateOfBirth)
                {
                    bnLog.PreviousValue += ", Date Of Birth: " + sibling.DateOfBirth?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Date Of Birth: " + model.DateOfBirth?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (sibling.Age != model.Age)
                {
                    bnLog.PreviousValue += ", Age: " + sibling.Age;
                    bnLog.UpdatedValue += ", Age: " + model.Age;
                    bnoisUpdateCount += 1;
                }
                if (sibling.FileName != model.FileName)
                {
                    bnLog.PreviousValue += ", File Name: " + sibling.FileName;
                    bnLog.UpdatedValue += ", File Name: " + model.FileName;
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
                sibling.IsActive = true;
                sibling.CreatedDate = DateTime.Now;
                sibling.CreatedBy = userId;
            }

            sibling.EmployeeId = model.EmployeeId;
            sibling.OccupationId = model.OccupationId;
            sibling.Name = model.Name;
            sibling.SpouseName = model.SpouseName;
            sibling.DateOfBirth=model.DateOfBirth;
           // sibling.Age=DateTime.Today.Year-model.DateOfBirth.Year;
            sibling.SiblingType = model.SiblingType;

            await siblingRepository.SaveAsync(sibling);
            model.SiblingId = sibling.SiblingId;

            return model;
        }


        public async Task<bool> DeleteSibling(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Sibling sibling = await siblingRepository.FindOneAsync(x => x.SiblingId == id);
            if (sibling == null)
            {
                throw new InfinityNotFoundException("Sibling not found");
            }
            else
            {
                bool IsDeleted = false;
                try
                {
                    // data log section start
                    BnoisLog bnLog = new BnoisLog();
                    bnLog.TableName = "Sibling";
                    bnLog.TableEntryForm = "Employee Siblings Information";
                    bnLog.PreviousValue = "Id: " + sibling.SiblingId;
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", sibling.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    bnLog.PreviousValue += ", Sibling Type: " + (sibling.SiblingType == 1 ? "Brother" : sibling.SiblingType == 2 ? "Sister" : "");
                    if (sibling.OccupationId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", sibling.OccupationId ?? 0);
                        bnLog.PreviousValue += ", Occupation: " + ((dynamic)prev).Name;
                    }
                    bnLog.PreviousValue += ", Name: " + sibling.Name + ", Name of the spouse: " + sibling.SpouseName + ", Date Of Birth: " + sibling.DateOfBirth?.ToString("dd/MM/yyyy");
                    bnLog.PreviousValue += ", Age: " + sibling.Age + ", File Name: " + sibling.FileName;


                    bnLog.UpdatedValue = "This Record has been Deleted!";
                    bnLog.LogStatus = 2; // 1 for update, 2 for delete
                    bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                    bnLog.LogCreatedDate = DateTime.Now;


                    await bnoisLogRepository.SaveAsync(bnLog);

                    //data log section end
                    IsDeleted =  siblingRepository.Delete(sibling);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return IsDeleted;
            }
        }


        public List<SelectModel> GetSiblingTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(SiblingType)).Cast<SiblingType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }

        public async Task<SiblingModel> UpdateSibling(SiblingModel model)
        {
                if (model == null)
                {
                    throw new InfinityArgumentMissingException("Sibling data missing");
                }

                string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                Sibling Sibling = ObjectConverter<SiblingModel, Sibling>.Convert(model);

                Sibling = await siblingRepository.FindOneAsync(x => x.SiblingId == model.SiblingId);
                if (Sibling == null)
                {
                    throw new InfinityNotFoundException("Sibling not found !");
                }

                if (model.FileName != null)
                {
                    Sibling.FileName = model.FileName;
                }

                Sibling.ModifiedDate = DateTime.Now;
                Sibling.ModifiedBy = userId;
                await siblingRepository.SaveAsync(Sibling);
                model.SiblingId = Sibling.SiblingId;
                return model;
           
        }
    }
}