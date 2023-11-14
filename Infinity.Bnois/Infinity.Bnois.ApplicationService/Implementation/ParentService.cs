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
    public class ParentService : IParentService
    {
        private readonly IBnoisRepository<Parent> parentRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public ParentService(IBnoisRepository<Parent> parentRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.parentRepository = parentRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public async Task<ParentModel> Parents(int employeeId, int relationType)
        {
            if (employeeId <= 0)
            {
                return new ParentModel();
            }
            Parent parent = await parentRepository.FindOneAsync(x => x.EmployeeId == employeeId && x.RelationType == relationType, new List<string> { "Country", "Nationality", "Religion", "ReligionCast", "Occupation", "RankCategory" });
            if (parent == null)
            {
                return new ParentModel();
            }
            ParentModel model = ObjectConverter<Parent, ParentModel>.Convert(parent);
            return model;
        }

        public async Task<ParentModel> SaveParents(int employeeId, ParentModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Parents data missing!");
            }

            Parent parent = ObjectConverter<ParentModel, Parent>.Convert(model);

            if (parent.ParentId > 0)
            {
                parent = await parentRepository.FindOneAsync(x => x.EmployeeId == employeeId && x.RelationType == model.RelationType);
                if (parent == null)
                {
                    throw new InfinityNotFoundException("Parents data not found!");
                }
                parent.ModifiedDate = DateTime.Now;
                parent.ModifiedBy = model.ModifiedBy;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Parent";
                bnLog.TableEntryForm = "Employee Parents Information";
                bnLog.PreviousValue = parent.RelationType == 1 ? "(FATHER INFORMATION)> " : parent.RelationType == 2 ? "(MOTHER INFORMATION)> " : parent.RelationType == 3 ? "(STEP FATHER INFORMATION)> " : parent.RelationType == 4 ? "(STEP MOTHER INFORMATION)> " : "";
                bnLog.UpdatedValue = model.RelationType == 1 ? "(FATHER INFORMATION)> " : model.RelationType == 2 ? "(MOTHER INFORMATION)> " : model.RelationType == 3 ? "(STEP FATHER INFORMATION)> " : model.RelationType == 4 ? "(STEP MOTHER INFORMATION)> " : "";
                bnLog.PreviousValue = ", Id: " + model.ParentId;
                bnLog.UpdatedValue = ", Id: " + model.ParentId;
                int bnoisUpdateCount = 0;
                if (parent.EmployeeId != model.EmployeeId)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", parent.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    bnLog.UpdatedValue += ", Name: " + ((dynamic)emp).PNo + "_" + ((dynamic)emp).FullNameEng;
                    bnoisUpdateCount += 1;
                }
                if (parent.FullName != model.FullName)
                {
                    bnLog.PreviousValue += ", Full Name: " + parent.FullName;
                    bnLog.UpdatedValue += ", Full Name: " + model.FullName;
                    bnoisUpdateCount += 1;
                }
                if (parent.FullNameBan != model.FullNameBan)
                {
                    bnLog.PreviousValue += ", Full Name (বাংলা): " + parent.FullNameBan;
                    bnLog.UpdatedValue += ", Full Name (বাংলা): " + model.FullNameBan;
                    bnoisUpdateCount += 1;
                }
                if (parent.NickName != model.NickName)
                {
                    bnLog.PreviousValue += ", Nick Name: " + parent.NickName;
                    bnLog.UpdatedValue += ", Nick Name: " + model.NickName;
                    bnoisUpdateCount += 1;
                }
                if (parent.OtherName != model.OtherName)
                {
                    bnLog.PreviousValue += ", Contact No: " + parent.OtherName;
                    bnLog.UpdatedValue += ", Contact No: " + model.OtherName;
                    bnoisUpdateCount += 1;
                }
                if (parent.FamilyTitle != model.FamilyTitle)
                {
                    bnLog.PreviousValue += ", Family Title: " + parent.FamilyTitle;
                    bnLog.UpdatedValue += ", FamilyTitle: " + model.FamilyTitle;
                    bnoisUpdateCount += 1;
                }
                if (parent.EducationalQualification != model.EducationalQualification)
                {
                    bnLog.PreviousValue += ", Educational Qualification: " + parent.EducationalQualification;
                    bnLog.UpdatedValue += ", Educational Qualification: " + model.EducationalQualification;
                    bnoisUpdateCount += 1;
                }
                if (parent.DoB != model.DoB)
                {
                    bnLog.PreviousValue += ", Date of Birth: " + parent.DoB?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Date of Birth: " + model.DoB?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (parent.PlaceOfBirth != model.PlaceOfBirth)
                {
                    bnLog.PreviousValue += ", Place Of Birth: " + parent.PlaceOfBirth;
                    bnLog.UpdatedValue += ", Place Of Birth: " + model.PlaceOfBirth;
                    bnoisUpdateCount += 1;
                }
                if (parent.IsBirthOutSide != model.IsBirthOutSide)
                {
                    bnLog.PreviousValue += ", Birth Outside: " + parent.IsBirthOutSide;
                    bnLog.UpdatedValue += ",  Birth Outside: " + model.IsBirthOutSide;
                    bnoisUpdateCount += 1;
                }
                if (parent.CountryId != model.CountryId)
                {
                    if (parent.CountryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", parent.CountryId ?? 0);
                        bnLog.PreviousValue += ", Country: " + ((dynamic)prev).FullName;
                    }
                    if (model.CountryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Country", "CountryId", model.CountryId ?? 0);
                        bnLog.UpdatedValue += ", Country: " + ((dynamic)newv).FullName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (parent.ReasonOfMigration != model.ReasonOfMigration)
                {
                    bnLog.PreviousValue += ", Reason Of Migration: " + parent.ReasonOfMigration;
                    bnLog.UpdatedValue += ", Reason Of Migration: " + model.ReasonOfMigration;
                    bnoisUpdateCount += 1;
                }
                if (parent.MigrationDate != model.MigrationDate)
                {
                    bnLog.PreviousValue += ", Migration Date: " + parent.MigrationDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Migration Date: " + model.MigrationDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (parent.NationalityId != model.NationalityId)
                {
                    if (parent.NationalityId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Nationality", "NationalityId", parent.NationalityId ?? 0);
                        bnLog.PreviousValue += ", Nationality: " + ((dynamic)prev).Name;
                    }
                    if (model.NationalityId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Nationality", "NationalityId", model.NationalityId ?? 0);
                        bnLog.UpdatedValue += ", Nationality: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (parent.NID != model.NID)
                {
                    bnLog.PreviousValue += ", NID: " + parent.NID;
                    bnLog.UpdatedValue += ", NID: " + model.NID;
                    bnoisUpdateCount += 1;
                }
                if (parent.IsNationalityChange != model.IsNationalityChange)
                {
                    bnLog.PreviousValue += ",  Nationality Change: " + parent.IsNationalityChange;
                    bnLog.UpdatedValue += ",  Nationality Change: " + model.IsNationalityChange;
                    bnoisUpdateCount += 1;
                }
                if (parent.PreviousNationalityId != model.PreviousNationalityId)
                {
                    if (parent.PreviousNationalityId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Nationality", "NationalityId", parent.PreviousNationalityId ?? 0);
                        bnLog.PreviousValue += ", Nationality: " + ((dynamic)prev).Name;
                    }
                    if (model.PreviousNationalityId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Nationality", "NationalityId", model.PreviousNationalityId ?? 0);
                        bnLog.UpdatedValue += ", Nationality: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (parent.PreviousNationalityDate != model.PreviousNationalityDate)
                {
                    bnLog.PreviousValue += ", Previous Nationality Date: " + parent.PreviousNationalityDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Previous Nationality Date: " + model.PreviousNationalityDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (parent.ReligionId != model.ReligionId)
                {
                    if (parent.ReligionId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Religion", "ReligionId", parent.ReligionId ?? 0);
                        bnLog.PreviousValue += ", Religion: " + ((dynamic)prev).Name;
                    }
                    if (model.ReligionId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Religion", "ReligionId", model.ReligionId ?? 0);
                        bnLog.UpdatedValue += ", Religion: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (parent.ReligionCastId != model.ReligionCastId)
                {
                    if (parent.ReligionCastId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("ReligionCast", "ReligionCastId", parent.ReligionCastId ?? 0);
                        bnLog.PreviousValue += ", Religion Cast: " + ((dynamic)prev).Name;
                    }
                    if (model.ReligionCastId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("ReligionCast", "ReligionCastId", model.ReligionCastId ?? 0);
                        bnLog.UpdatedValue += ", Religion Cast: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (parent.IsDead != model.IsDead)
                {
                    bnLog.PreviousValue += ", Status: " + (parent.IsDead==true ? "Dead":"Alive");
                    bnLog.UpdatedValue += ", Status: " + (model.IsDead == true ? "Dead" : "Alive");
                    bnoisUpdateCount += 1;
                }
                if (parent.DeadDate != model.DeadDate)
                {
                    bnLog.PreviousValue += ", Dead Date: " + parent.DeadDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Dead Date: " + model.DeadDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (parent.IsDoingService != model.IsDoingService)
                {
                    bnLog.PreviousValue += ", Service/Professional History: " + parent.IsDoingService;
                    bnLog.UpdatedValue += ", Service/Professional History: " + model.IsDoingService;
                    bnoisUpdateCount += 1;
                }
                if (parent.OccupationId != model.OccupationId)
                {
                    if (parent.OccupationId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", parent.OccupationId ?? 0);
                        bnLog.PreviousValue += ", Religion Cast: " + ((dynamic)prev).Name;
                    }
                    if (model.OccupationId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", model.OccupationId ?? 0);
                        bnLog.UpdatedValue += ", Religion Cast: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (parent.Department != model.Department)
                {
                    bnLog.PreviousValue += ", Department: " + parent.Department;
                    bnLog.UpdatedValue += ", Department: " + model.Department;
                    bnoisUpdateCount += 1;
                }
                if (parent.Designation != model.Designation)
                {
                    bnLog.PreviousValue += ", Designation: " + parent.Designation;
                    bnLog.UpdatedValue += ", Designation: " + model.Designation;
                    bnoisUpdateCount += 1;
                }
                if (parent.IsRetired != model.IsRetired)
                {
                    bnLog.PreviousValue += ", Retired: " + parent.IsRetired;
                    bnLog.UpdatedValue += ", Retired: " + model.IsRetired;
                    bnoisUpdateCount += 1;
                }
                if (parent.ServiceAddress != model.ServiceAddress)
                {
                    bnLog.PreviousValue += ", Service Address: " + parent.ServiceAddress;
                    bnLog.UpdatedValue += ", Service Address: " + model.ServiceAddress;
                    bnoisUpdateCount += 1;
                }
                if (parent.PreServiceAddress != model.PreServiceAddress)
                {
                    bnLog.PreviousValue += ", Previous Service Address (If Any): " + parent.PreServiceAddress;
                    bnLog.UpdatedValue += ", Previous Service Address (If Any): " + model.PreServiceAddress;
                    bnoisUpdateCount += 1;
                }
                if (parent.FileName != model.FileName)
                {
                    bnLog.PreviousValue += ", File Name: " + parent.FileName;
                    bnLog.UpdatedValue += ", File Name: " + model.FileName;
                    bnoisUpdateCount += 1;
                }
                if (parent.YearlyIncome != model.YearlyIncome)
                {
                    bnLog.PreviousValue += ", Yearly Income: " + parent.YearlyIncome;
                    bnLog.UpdatedValue += ", Yearly Income: " + model.YearlyIncome;
                    bnoisUpdateCount += 1;
                }
                if (parent.PresentAddress != model.PresentAddress)
                {
                    bnLog.PreviousValue += ", Present Address: " + parent.PresentAddress;
                    bnLog.UpdatedValue += ", Present Address: " + model.PresentAddress;
                    bnoisUpdateCount += 1;
                }
                if (parent.PermanentAddress != model.PermanentAddress)
                {
                    bnLog.PreviousValue += ", Permanent Address: " + parent.PermanentAddress;
                    bnLog.UpdatedValue += ", Permanent Address: " + model.PermanentAddress;
                    bnoisUpdateCount += 1;
                }
                if (parent.PresentAddressBan != model.PresentAddressBan)
                {
                    bnLog.PreviousValue += ", Present Address (বাংলা): " + parent.PresentAddressBan;
                    bnLog.UpdatedValue += ", Present Address (বাংলা): " + model.PresentAddressBan;
                    bnoisUpdateCount += 1;
                }
                if (parent.PermanentAddressBan != model.PermanentAddressBan)
                {
                    bnLog.PreviousValue += ", Permanent Address (বাংলা): " + parent.PermanentAddressBan;
                    bnLog.UpdatedValue += ", Permanent Address (বাংলা): " + model.PermanentAddressBan;
                    bnoisUpdateCount += 1;
                }
                if (parent.OtherAddress != model.OtherAddress)
                {
                    bnLog.PreviousValue += ", Other Address: " + parent.OtherAddress;
                    bnLog.UpdatedValue += ", Other Address: " + model.OtherAddress;
                    bnoisUpdateCount += 1;
                }
                if (parent.OtherAddressBan != model.OtherAddressBan)
                {
                    bnLog.PreviousValue += ", Other Address (বাংলা): " + parent.OtherAddressBan;
                    bnLog.UpdatedValue += ", Other Address (বাংলা): " + model.OtherAddressBan;
                    bnoisUpdateCount += 1;
                }
                if (parent.IsArmedForceExp != model.IsArmedForceExp)
                {
                    bnLog.PreviousValue += ", Armed Force Experience: " + parent.IsArmedForceExp;
                    bnLog.UpdatedValue += ", Armed Force Experience: " + model.IsArmedForceExp;
                    bnoisUpdateCount += 1;
                }
                if (parent.RankCategoryId != model.RankCategoryId)
                {
                    if (parent.RankCategoryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("RankCategory", "RankCategoryId", parent.RankCategoryId ?? 0);
                        bnLog.PreviousValue += ", Rank Category: " + ((dynamic)prev).Name;
                    }
                    if (model.RankCategoryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("RankCategory", "RankCategoryId", model.RankCategoryId ?? 0);
                        bnLog.UpdatedValue += ", Rank Category: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (parent.PNo != model.PNo)
                {
                    bnLog.PreviousValue += ", PNo: " + parent.PNo;
                    bnLog.UpdatedValue += ", PNo: " + model.PNo;
                    bnoisUpdateCount += 1;
                }
                if (parent.RankName != model.RankName)
                {
                    bnLog.PreviousValue += ", Rank Name: " + parent.RankName;
                    bnLog.UpdatedValue += ", Rank Name: " + model.RankName;
                    bnoisUpdateCount += 1;
                }
                if (parent.PoliticalIdeology != model.PoliticalIdeology)
                {
                    bnLog.PreviousValue += ", Political Ideology: " + parent.PoliticalIdeology;
                    bnLog.UpdatedValue += ", Political Ideology: " + model.PoliticalIdeology;
                    bnoisUpdateCount += 1;
                }
                if (parent.Dependency != model.Dependency)
                {
                    bnLog.PreviousValue += ", Dependency on the Subj Indl: " + parent.Dependency;
                    bnLog.UpdatedValue += ", Dependency on the Subj Indl: " + model.Dependency;
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
                parent.EmployeeId = employeeId;
                parent.CreatedBy = model.CreatedBy;
                parent.CreatedDate = DateTime.Now;
                parent.IsActive = true;
            }

            bool exists;
            exists = Enum.IsDefined(typeof(ParentTypes), model.RelationType);
            if (!exists)
            {
                throw new InfinityNotFoundException("Something went wrong in relationship !");
            }
            parent.RelationType = model.RelationType;
            parent.FullName = model.FullName;

            parent.FullNameBan = model.FullNameBan;
            parent.NickName = model.NickName;
            parent.OtherName = model.OtherName;
            parent.FamilyTitle = model.FamilyTitle;
            parent.PresentAddressBan = model.PresentAddressBan;
            parent.PermanentAddressBan = model.PermanentAddressBan;
            parent.OtherAddress = model.OtherAddress;
            parent.OtherAddressBan = model.OtherAddressBan;
            parent.RankName = model.RankName;
            parent.PoliticalIdeology = model.PoliticalIdeology;
            parent.Dependency = model.Dependency;

            parent.DoB = model.DoB;
            parent.EducationalQualification = model.EducationalQualification;
            parent.PreServiceAddress = model.PreServiceAddress;
            parent.PlaceOfBirth = model.PlaceOfBirth;
            parent.IsBirthOutSide = model.IsBirthOutSide;
            parent.CountryId = model.CountryId;
            parent.ReasonOfMigration = model.ReasonOfMigration;
            parent.MigrationDate = model.MigrationDate;

            parent.NationalityId = model.NationalityId;
            parent.NID = model.NID;

            parent.IsNationalityChange = model.IsNationalityChange;
            parent.PreviousNationalityId = model.PreviousNationalityId;
            parent.PreviousNationalityDate = model.PreviousNationalityDate;
            parent.ReligionId = model.ReligionId;
            parent.ReligionCastId = model.ReligionCastId;
            parent.IsDead = model.IsDead;
            parent.OccupationId = model.OccupationId;
            parent.IsDoingService = model.IsDoingService;
            parent.Department = model.Department;
            parent.Designation = model.Designation;
            parent.IsRetired = model.IsRetired;
            parent.ServiceAddress = model.ServiceAddress;
            parent.YearlyIncome = model.YearlyIncome;
            parent.PresentAddress = model.PresentAddress;
            parent.PermanentAddress = model.PermanentAddress;
            parent.IsArmedForceExp = model.IsArmedForceExp;
            parent.RankCategoryId = model.RankCategoryId;
            parent.PNo = model.PNo;
            parent.DeadDate = model.DeadDate;
            await parentRepository.SaveAsync(parent);
            model.ParentId = parent.ParentId;
            return model;
        }

        public async Task<ParentModel> UpdateParent(ParentModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Parent data missing");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Parent parent = ObjectConverter<ParentModel, Parent>.Convert(model);

            parent = await parentRepository.FindOneAsync(x => x.EmployeeId == model.EmployeeId && x.RelationType == model.RelationType);
            if (parent == null)
            {
                throw new InfinityNotFoundException("Parent not found !");
            }

            if (model.FileName != null)
            {
                parent.FileName = model.FileName;
            }

            parent.ModifiedDate = DateTime.Now;
            parent.ModifiedBy = userId;
            await parentRepository.SaveAsync(parent);
            model.ParentId = parent.ParentId;
            return model;
        }
    }
}

