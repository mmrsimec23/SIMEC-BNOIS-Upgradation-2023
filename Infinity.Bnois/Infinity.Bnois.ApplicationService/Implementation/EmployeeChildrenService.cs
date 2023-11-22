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
    public class EmployeeChildrenService : IEmployeeChildrenService
    {
        private readonly IBnoisRepository<EmployeeChildren> employeeChildrenRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public EmployeeChildrenService(IBnoisRepository<EmployeeChildren> employeeChildrenRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.employeeChildrenRepository = employeeChildrenRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<SelectModel> GetChildrenTypeSelectModels()
        {
            List<SelectModel> selectModels =
           Enum.GetValues(typeof(ChildrenType)).Cast<ChildrenType>()
                .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                .ToList();
            return selectModels;
        }




        public async Task<EmployeeChildrenModel> GetEmployeeChildren(int employeeChildrenId)
        {
            if (employeeChildrenId <= 0)
            {
                return new EmployeeChildrenModel();
            }
            EmployeeChildren employeeChildren = await employeeChildrenRepository.FindOneAsync(x => x.EmployeeChildrenId == employeeChildrenId);

            if (employeeChildren == null)
            {
                throw new InfinityNotFoundException("Children not found!");
            }
            EmployeeChildrenModel model = ObjectConverter<EmployeeChildren, EmployeeChildrenModel>.Convert(employeeChildren);
            return model;
        }

        public List<EmployeeChildrenModel> GetEmployeeChildrens(int employeeId)
        {
            List<EmployeeChildren> employeeChildrens = employeeChildrenRepository.FilterWithInclude(x => x.EmployeeId == employeeId, "Employee", "Occupation").ToList();
            List<EmployeeChildrenModel> models = ObjectConverter<EmployeeChildren, EmployeeChildrenModel>.ConvertList(employeeChildrens.ToList()).ToList();
            models = models.Select(x =>
            {
                x.ChildrenTypeName = Enum.GetName(typeof(ChildrenType), x.ChildrenType);
                return x;
            }).ToList();
            return models;
        }

        public async Task<EmployeeChildrenModel> SaveEmployeeChildren(int id, EmployeeChildrenModel model)
        {

            if (model == null)
            {
                throw new InfinityArgumentMissingException("Children data missing");
            }
            EmployeeChildren employeeChildren = ObjectConverter<EmployeeChildrenModel, EmployeeChildren>.Convert(model);

            if (id > 0)
            {
                employeeChildren = await employeeChildrenRepository.FindOneAsync(x => x.EmployeeChildrenId == id);
                if (employeeChildren == null)
                {
                    throw new InfinityNotFoundException("Children not found!");
                }
                employeeChildren.ModifiedBy = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                employeeChildren.ModifiedDate = DateTime.Now;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeChildren";
                bnLog.TableEntryForm = "Employee Children Information";
                bnLog.PreviousValue = "Id: " + model.EmployeeChildrenId;
                bnLog.UpdatedValue = "Id: " + model.EmployeeChildrenId;
                int bnoisUpdateCount = 0;
                if (employeeChildren.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeChildren.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                    //bnoisUpdateCount += 1;
                }
                if (employeeChildren.ChildrenType != model.ChildrenType)
                {
                    bnLog.PreviousValue += ", Children Type: " + (employeeChildren.ChildrenType == 1 ?"Son": employeeChildren.ChildrenType == 2 ? "Daughter" :"");
                    bnLog.UpdatedValue += ", Children Type: " + (model.ChildrenType == 1 ? "Son" : model.ChildrenType == 2 ? "Daughter" : ""); ;
                    bnoisUpdateCount += 1;
                }
                if (employeeChildren.Child != model.Child)
                {
                    if(employeeChildren.Child==0)
                    {
                        bnLog.PreviousValue += ", Own/Spouse Child: Own's Child";
                    }
                    if(model.Child==0)
                    {
                        bnLog.UpdatedValue += ", Own/Spouse Child: Own's Child";
                    }
                    if (employeeChildren.Child > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Spouse", "SpouseId", employeeChildren.Child ?? 0);
                        bnLog.PreviousValue += ", Own/Spouse Child: " + ((dynamic)prev).ANameEng + "'s Child";
                    }
                    if (model.Child > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Spouse", "SpouseId", model.Child);
                        bnLog.UpdatedValue += ", Own/Spouse Child: " + ((dynamic)newv).ANameEng + "'s Child";
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeChildren.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name (English): " + employeeChildren.Name;
                    bnLog.UpdatedValue += ", Name (English): " + model.Name;
                    bnoisUpdateCount += 1;
                }
                if (employeeChildren.NameBan != model.NameBan)
                {
                    bnLog.PreviousValue += ", Name (Bangla): " + employeeChildren.NameBan;
                    bnLog.UpdatedValue += ", Name (Bangla): " + model.NameBan;
                    bnoisUpdateCount += 1;
                }
                if (employeeChildren.DateOfBirth != model.DateOfBirth)
                {
                    bnLog.PreviousValue += ", Date Of Birth: " + employeeChildren.DateOfBirth?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Date Of Birth: " + model.DateOfBirth?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeChildren.FileName != model.FileName)
                {
                    bnLog.PreviousValue += ", File Name: " + employeeChildren.FileName;
                    bnLog.UpdatedValue += ", File Name: " + model.FileName;
                    bnoisUpdateCount += 1;
                }
                if (employeeChildren.GenFormName != model.GenFormName)
                {
                    bnLog.PreviousValue += ", Gen Form Name: " + employeeChildren.GenFormName;
                    bnLog.UpdatedValue += ", Gen Form Name: " + model.GenFormName;
                    bnoisUpdateCount += 1;
                }
                if (employeeChildren.LivingWith != model.LivingWith)
                {
                    bnLog.PreviousValue += ", Living With: " + employeeChildren.LivingWith;
                    bnLog.UpdatedValue += ", Living With: " + model.LivingWith;
                    bnoisUpdateCount += 1;
                }
                if (employeeChildren.OccupationId != model.OccupationId)
                {
                    if (employeeChildren.OccupationId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", employeeChildren.OccupationId ?? 0);
                        bnLog.PreviousValue += ", Occupation: " + ((dynamic)prev).Name;
                    }
                    if (model.OccupationId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", model.OccupationId ?? 0);
                        bnLog.UpdatedValue += ", Occupation: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeChildren.ContactAddress != model.ContactAddress)
                {
                    bnLog.PreviousValue += ", Contact Address: " + employeeChildren.ContactAddress;
                    bnLog.UpdatedValue += ", Contact Address: " + model.ContactAddress;
                    bnoisUpdateCount += 1;
                }
                if (employeeChildren.OfficeAddress != model.OfficeAddress)
                {
                    bnLog.PreviousValue += ", Office Address: " + employeeChildren.OfficeAddress;
                    bnLog.UpdatedValue += ", Office Address: " + model.OfficeAddress;
                    bnoisUpdateCount += 1;
                }
                if (employeeChildren.GoldenChild != model.GoldenChild)
                {
                    bnLog.PreviousValue += ", Golden Child: " + employeeChildren.GoldenChild;
                    bnLog.UpdatedValue += ", Golden Child: " + model.GoldenChild;
                    bnoisUpdateCount += 1;
                }
                if (employeeChildren.IsDead != model.IsDead)
                {
                    bnLog.PreviousValue += ", Status: " + (employeeChildren.IsDead==true?"Dead":"Alive");
                    bnLog.UpdatedValue += ", Status: " + (model.IsDead == true ? "Dead" : "Alive");
                    bnoisUpdateCount += 1;
                }
                if (employeeChildren.DeadDate != model.DeadDate)
                {
                    bnLog.PreviousValue += ", Dead Date: " + employeeChildren.DeadDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Dead Date: " + model.DeadDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeChildren.DeadReason != model.DeadReason)
                {
                    bnLog.PreviousValue += ", Dead Reason: " + employeeChildren.DeadReason;
                    bnLog.UpdatedValue += ", Dead Reason: " + model.DeadReason;
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
                employeeChildren.IsActive = true;
                employeeChildren.CreatedBy = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                employeeChildren.CreatedDate = DateTime.Now;
            }
            employeeChildren.EmployeeId = model.EmployeeId;
            employeeChildren.ChildrenType = model.ChildrenType;
            employeeChildren.Child = model.Child;
            employeeChildren.Name = model.Name;
            employeeChildren.NameBan = model.NameBan;
            employeeChildren.DateOfBirth = model.DateOfBirth;
            employeeChildren.LivingWith = model.LivingWith;
            employeeChildren.ContactAddress = model.ContactAddress;
            employeeChildren.OccupationId = model.OccupationId;
            employeeChildren.OfficeAddress = model.OfficeAddress;
            employeeChildren.GoldenChild = model.GoldenChild;
            employeeChildren.IsDead = model.IsDead;
            employeeChildren.DeadDate = model.DeadDate;
            employeeChildren.DeadReason = model.DeadReason;
            await employeeChildrenRepository.SaveAsync(employeeChildren);
            model.EmployeeChildrenId = employeeChildren.EmployeeChildrenId;
            return model;
        }



        public async Task<bool> DeleteEmployeeChildren(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            EmployeeChildren employeeChildren = await employeeChildrenRepository.FindOneAsync(x => x.EmployeeChildrenId == id);
            if (employeeChildren == null)
            {
                throw new InfinityNotFoundException("Children not found");
            }
            else
            {
                bool IsDeleted = false;
                try
                {
                    // data log section start
                    BnoisLog bnLog = new BnoisLog();
                    bnLog.TableName = "EmployeeChildren";
                    bnLog.TableEntryForm = "Employee Children Information";
                    bnLog.PreviousValue = "Id: " + employeeChildren.EmployeeChildrenId;
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeChildren.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    bnLog.PreviousValue += ", Children Type: " + (employeeChildren.ChildrenType == 1 ? "Son" : employeeChildren.ChildrenType == 2 ? "Daughter" : "");
                    if (employeeChildren.Child == 0 || employeeChildren.Child == null)
                    {
                        bnLog.PreviousValue += ", Own/Spouse Child: Own's Child";
                    }
                    else
                    {
                        if (employeeChildren.Child > 0)
                        {
                            var prev = employeeService.GetDynamicTableInfoById("Spouse", "SpouseId", employeeChildren.Child ?? 0);
                            bnLog.PreviousValue += ", Own/Spouse Child: " + ((dynamic)prev).ANameEng + "'s Child";
                        }
                    }
                    bnLog.PreviousValue += ", Name (English): " + employeeChildren.Name + ", Name (Bangla): " + employeeChildren.NameBan + ", Date Of Birth: " + employeeChildren.DateOfBirth?.ToString("dd/MM/yyyy");
                    bnLog.PreviousValue += ", File Name: " + employeeChildren.FileName + ", Gen Form Name: " + employeeChildren.GenFormName + ", Living With: " + employeeChildren.LivingWith;
                    if (employeeChildren.OccupationId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", employeeChildren.OccupationId ?? 0);
                        bnLog.PreviousValue += ", Occupation: " + ((dynamic)prev).Name;
                    }
                    bnLog.PreviousValue += ", Contact Address: " + employeeChildren.ContactAddress + ", Office Address: " + employeeChildren.OfficeAddress;
                    bnLog.PreviousValue += ", Golden Child: " + employeeChildren.GoldenChild + ", Status: " + (employeeChildren.IsDead == true ? "Dead" : "Alive");
                    bnLog.PreviousValue += ", Dead Date: " + employeeChildren.DeadDate?.ToString("dd/MM/yyyy") + ", Dead Reason: " + employeeChildren.DeadReason;
                    bnLog.UpdatedValue = "This Record has been Deleted!";
                    bnLog.LogStatus = 2; // 1 for update, 2 for delete
                    bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                    bnLog.LogCreatedDate = DateTime.Now;


                    await bnoisLogRepository.SaveAsync(bnLog);

                    //data log section end
                    IsDeleted = await employeeChildrenRepository.DeleteAsync(employeeChildren);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return IsDeleted;
            }
        }

        public async Task<EmployeeChildrenModel> UpdateEmployeeChildren(EmployeeChildrenModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Children data missing");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            EmployeeChildren employeeChildren = ObjectConverter<EmployeeChildrenModel, EmployeeChildren>.Convert(model);

            employeeChildren = await employeeChildrenRepository.FindOneAsync(x => x.EmployeeChildrenId == model.EmployeeChildrenId);
            if (employeeChildren == null)
            {
                throw new InfinityNotFoundException("Children not found !");
            }

            if (model.FileName != null)
            {
                employeeChildren.FileName = model.FileName;
            }

            if (model.GenFormName != null)
            {
                employeeChildren.GenFormName = model.GenFormName;
            }

            employeeChildren.ModifiedDate = DateTime.Now;
            employeeChildren.ModifiedBy = userId;
            await employeeChildrenRepository.SaveAsync(employeeChildren);
            model.EmployeeChildrenId = employeeChildren.EmployeeChildrenId;
            return model;

        }
    }
}
