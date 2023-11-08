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
    public class EmployeeGeneralService : IEmployeeGeneralService
    {
        private readonly IBnoisRepository<EmployeeGeneral> employeeGeneralRepository;
        private readonly IBnoisRepository<AgeServicePolicy> ageServicePolicyRepository;
        private readonly IBnoisRepository<Employee> employeeRepository;
        private readonly IProcessRepository processRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;


        public EmployeeGeneralService(IBnoisRepository<EmployeeGeneral> employeeGeneralRepository,
            IBnoisRepository<AgeServicePolicy> ageServicePolicyRepository, IProcessRepository processRepository,
            IBnoisRepository<Employee> employeeRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.employeeGeneralRepository = employeeGeneralRepository;
            this.ageServicePolicyRepository = ageServicePolicyRepository;
            this.employeeRepository = employeeRepository;
            this.processRepository = processRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;

        }

        public async Task<EmployeeGeneralModel> GetEmployeeGenerals(int employeeId)
        {
            if (employeeId <= 0)

            {
                return new EmployeeGeneralModel();
            }
            EmployeeGeneral employeeGeneral = await employeeGeneralRepository.FindOneAsync(x => x.EmployeeId == employeeId, new List<string> { "Employee.Rank", "Category", "SubCategory", "CommissionType", "Branch", "SubBranch", "Nationality", "MaritalType", "Religion", "ReligionCast", "Subject", "OfficerStream", "Employee.OfficerType" });
            if (employeeGeneral == null)
            {
                return new EmployeeGeneralModel();
            }
            EmployeeGeneralModel model = ObjectConverter<EmployeeGeneral, EmployeeGeneralModel>.Convert(employeeGeneral);
            return model;
        }


        private void GetAgeLimitServiceLimitLprDate(int category, int? subCategory, int rank, DateTime commissionDate, DateTime dateOfBirth, string OfficerType, out DateTime ageLimit, out DateTime serviceLimit, out DateTime lprDate, out bool result)
        {
            AgeServicePolicy ageServicePolicy = ageServicePolicyRepository.FindOne(x => x.CategoryId == category && x.SubCategoryId == subCategory && x.RankId == rank);

            ageLimit = DateTime.Now;
            serviceLimit = DateTime.Now;
            lprDate = DateTime.Now;
            result = false;
            if (OfficerType.Equals("BN") && ageServicePolicy == null)
            {
                throw new InfinityNotFoundException("Age Service Policy not Set!");
            }

            if (ageServicePolicy != null)
            {
                ageLimit = dateOfBirth.AddYears(ageServicePolicy.AgeLimit);
                serviceLimit = commissionDate.AddYears(ageServicePolicy.ServiceLimit);
                lprDate = ageLimit > serviceLimit ? ageLimit : serviceLimit;

                ageLimit = ageLimit.AddDays(-1);
                serviceLimit = serviceLimit.AddDays(-1);
                lprDate = lprDate.AddDays(-1);
                result = true;
            }

        }


        public async Task<EmployeeGeneralModel> SaveEmployeeGeneral(int employeeId, EmployeeGeneralModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("General Information data missing!");
            }

            Employee employeeInfo = await employeeRepository.FindOneAsync(x => x.EmployeeId == employeeId,new List<string>{"OfficerType"});
            EmployeeGeneral employeeGeneral = ObjectConverter<EmployeeGeneralModel, EmployeeGeneral>.Convert(model);


            if (employeeGeneral.EmployeeGeneralId > 0)
            {
                employeeGeneral = await employeeGeneralRepository.FindOneAsync(x => x.EmployeeId == employeeId);
                if (employeeGeneral == null)
                {
                    throw new InfinityNotFoundException("Officer's General Information not found!");
                }
                employeeGeneral.ModifiedDate = DateTime.Now;
                employeeGeneral.ModifiedBy = model.ModifiedBy;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeGeneral";
                bnLog.TableEntryForm = "Employee General Info";
                bnLog.PreviousValue = "Id: " + model.EmployeeGeneralId;
                bnLog.UpdatedValue = "Id: " + model.EmployeeGeneralId;
                int bnoisUpdateCount = 0;
                if (employeeGeneral.EmployeeId != model.EmployeeId)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeGeneral.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    bnLog.UpdatedValue += ", Name: " + ((dynamic)emp).PNo + "_" + ((dynamic)emp).FullNameEng;
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ", Short Name: " + employeeGeneral.ShortName;
                    bnLog.UpdatedValue += ", Short Name: " + model.ShortName;
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.ShortNameBan != model.ShortNameBan)
                {
                    bnLog.PreviousValue += ", Short Name(বাংলা): " + employeeGeneral.ShortNameBan;
                    bnLog.UpdatedValue += ", Short Name(বাংলা): " + model.ShortNameBan;
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.NickName != model.NickName)
                {
                    bnLog.PreviousValue += ", Nick Name: " + employeeGeneral.NickName;
                    bnLog.UpdatedValue += ", Nick Name: " + model.NickName;
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.NickNameBan != model.NickNameBan)
                {
                    bnLog.PreviousValue += ", Nick Name (বাংলা): " + employeeGeneral.NickNameBan;
                    bnLog.UpdatedValue += ", Nick Name (বাংলা): " + model.NickNameBan;
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.DoB != model.DoB)
                {
                    bnLog.PreviousValue += ", Date of Birth: " + employeeGeneral.DoB.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Date of Birth: " + model.DoB?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.BirthPlace != model.BirthPlace)
                {
                    bnLog.PreviousValue += ", Birth Place: " + employeeGeneral.BirthPlace;
                    bnLog.UpdatedValue += ", Birth Place: " + model.BirthPlace;
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.MigrationDate != model.MigrationDate)
                {
                    bnLog.PreviousValue += ", Migration Date: " + employeeGeneral.MigrationDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Migration Date: " + model.MigrationDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.MigrationReason != model.MigrationReason)
                {
                    bnLog.PreviousValue += ", Migration Reason: " + employeeGeneral.MigrationReason;
                    bnLog.UpdatedValue += ", Migration Reason: " + model.MigrationReason;
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.MaritalTypeId != model.MaritalTypeId)
                {
                    if (employeeGeneral.MaritalTypeId > 0)
                    {
                        var prevMaritalType = employeeService.GetDynamicTableInfoById("MaritalType", "MaritalTypeId", employeeGeneral.MaritalTypeId??0);
                        bnLog.PreviousValue += ", Marital Status: " + ((dynamic)prevMaritalType).Name;
                        
                    }
                    if (model.MaritalTypeId > 0)
                    {
                        var newMaritalType = employeeService.GetDynamicTableInfoById("MaritalType", "MaritalTypeId", model.MaritalTypeId ?? 0);
                        bnLog.UpdatedValue += ", Marital Status: " + ((dynamic)newMaritalType).Name;
                        
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.MarriageDate != model.MarriageDate)
                {
                    bnLog.PreviousValue += ", Marriage Date: " + employeeGeneral.MarriageDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Marriage Date: " + model.MarriageDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.ContactNo != model.ContactNo)
                {
                    bnLog.PreviousValue += ", Contact No: " + employeeGeneral.ContactNo;
                    bnLog.UpdatedValue += ", Contact No: " + model.ContactNo;
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.EmailAddress != model.EmailAddress)
                {
                    bnLog.PreviousValue += ", Email: " + employeeGeneral.EmailAddress;
                    bnLog.UpdatedValue += ", Email: " + model.EmailAddress;
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.NationalityId != model.NationalityId)
                {
                    if (employeeGeneral.NationalityId > 0)
                    {
                        var prevNationality = employeeService.GetDynamicTableInfoById("Nationality", "NationalityId", employeeGeneral.NationalityId??0);
                        bnLog.PreviousValue += ", Nationality: " + ((dynamic)prevNationality).Name;
                    }
                    if (model.NationalityId > 0)
                    {
                        var newNationality = employeeService.GetDynamicTableInfoById("Nationality", "NationalityId", model.NationalityId??0);
                        bnLog.UpdatedValue += ", Nationality: " + ((dynamic)newNationality).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.ReligionId != model.ReligionId)
                {
                    if (employeeGeneral.ReligionId > 0)
                    {
                        var prevReligion = employeeService.GetDynamicTableInfoById("Religion", "ReligionId", employeeGeneral.ReligionId ?? 0);
                        bnLog.PreviousValue += ", Religion: " + ((dynamic)prevReligion).Name;
                    }
                    if (model.ReligionId > 0)
                    {
                        var newReligion = employeeService.GetDynamicTableInfoById("Religion", "ReligionId", model.ReligionId ?? 0);
                        bnLog.UpdatedValue += ", Religion: " + ((dynamic)newReligion).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.ReligionCastId != model.ReligionCastId)
                {
                    if (employeeGeneral.ReligionCastId > 0)
                    {
                        var prevReligionCast = employeeService.GetDynamicTableInfoById("ReligionCast", "ReligionCastId", employeeGeneral.ReligionCastId ?? 0);
                        bnLog.PreviousValue += ", Religion Cast: " + ((dynamic)prevReligionCast).Name;
                    }
                    if (model.ReligionId > 0)
                    {
                        var newReligionCast = employeeService.GetDynamicTableInfoById("ReligionCast", "ReligionCastId", model.ReligionCastId ?? 0);
                        bnLog.UpdatedValue += ", Religion Cast: " + ((dynamic)newReligionCast).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.IsDead != model.IsDead)
                {
                    bnLog.PreviousValue += ", Status: " + (employeeGeneral.IsDead == true ? "Dead" : "Alive");
                    bnLog.UpdatedValue += ", Status: " + (model.IsDead == true ? "Dead" : "Alive");
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.DeadDate != model.DeadDate)
                {
                    bnLog.PreviousValue += ", Dead Date: " + employeeGeneral.DeadDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Dead Date: " + model.DeadDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.DeadReason != model.DeadReason)
                {
                    bnLog.PreviousValue += ", Dead Reason: " + employeeGeneral.DeadReason;
                    bnLog.UpdatedValue += ", Dead Reason: " + model.DeadReason;
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.CategoryId != model.CategoryId)
                {
                    if (employeeGeneral.CategoryId > 0)
                    {
                        var prevCategory = employeeService.GetDynamicTableInfoById("Category", "CategoryId", employeeGeneral.CategoryId);
                        bnLog.PreviousValue += ", Category: " + ((dynamic)prevCategory).Name;
                    }
                    if (model.CategoryId > 0)
                    {
                        var newCategory = employeeService.GetDynamicTableInfoById("Category", "CategoryId", model.CategoryId);
                        bnLog.UpdatedValue += ", Category: " + ((dynamic)newCategory).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.SubCategoryId != model.SubCategoryId)
                {
                    if (employeeGeneral.SubCategoryId > 0)
                    {
                        var prevSubCategory = employeeService.GetDynamicTableInfoById("SubCategory", "SubCategoryId", employeeGeneral.SubCategoryId??0);
                        bnLog.PreviousValue += ", Sub Category: " + ((dynamic)prevSubCategory).Name;
                    }
                    if (model.SubCategoryId > 0)
                    {
                        var newSubCategory = employeeService.GetDynamicTableInfoById("SubCategory", "SubCategoryId", model.SubCategoryId??0);
                        bnLog.UpdatedValue += ", Sub Category: " + ((dynamic)newSubCategory).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.CommissionTypeId != model.CommissionTypeId)
                {
                    if (employeeGeneral.CommissionTypeId > 0)
                    {
                        var prevCommissionType = employeeService.GetDynamicTableInfoById("CommissionType", "CommissionTypeId", employeeGeneral.CommissionTypeId);
                        bnLog.PreviousValue += ", Commission Type: " + ((dynamic)prevCommissionType).TypeName;
                    }
                    if (model.CommissionTypeId > 0)
                    {
                        var newSubCategory = employeeService.GetDynamicTableInfoById("CommissionType", "CommissionTypeId", model.CommissionTypeId);
                        bnLog.UpdatedValue += ", Commission Type: " + ((dynamic)newSubCategory).TypeName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.CommissionDate != model.CommissionDate)
                {
                    bnLog.PreviousValue += ", Commission Date: " + employeeGeneral.CommissionDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Commission Date: " + model.CommissionDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.BranchId != model.BranchId)
                {
                    if (employeeGeneral.BranchId > 0)
                    {
                        var prevBranch = employeeService.GetDynamicTableInfoById("Branch", "BranchId", employeeGeneral.BranchId);
                        bnLog.PreviousValue += ", Branch: " + ((dynamic)prevBranch).Name;
                    }
                    if (model.BranchId > 0)
                    {
                        var newBranch = employeeService.GetDynamicTableInfoById("Branch", "BranchId", model.BranchId);
                        bnLog.UpdatedValue += ", Branch: " + ((dynamic)newBranch).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.SubBranchId != model.SubBranchId)
                {
                    if (employeeGeneral.SubBranchId > 0)
                    {
                        var prevSubBranch = employeeService.GetDynamicTableInfoById("SubBranch", "SubBranchId", employeeGeneral.SubBranchId??0);
                        bnLog.PreviousValue += ", Sub Branch: " + ((dynamic)prevSubBranch).FullName;
                    }
                    if (model.SubBranchId > 0)
                    {
                        var newSubBranch = employeeService.GetDynamicTableInfoById("SubBranch", "SubBranchId", model.SubBranchId??0);
                        bnLog.UpdatedValue += ", Sub Branch: " + ((dynamic)newSubBranch).FullName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.SubjectId != model.SubjectId)
                {
                    if (employeeGeneral.SubjectId > 0)
                    {
                        var prevSubject = employeeService.GetDynamicTableInfoById("Subject", "SubjectId", employeeGeneral.SubjectId ?? 0);
                        bnLog.PreviousValue += ", Subject: " + ((dynamic)prevSubject).Name;
                    }
                    if (model.SubjectId > 0)
                    {
                        var newSubject = employeeService.GetDynamicTableInfoById("Subject", "SubjectId", model.SubjectId ?? 0);
                        bnLog.UpdatedValue += ", Subject: " + ((dynamic)newSubject).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.JoiningDate != model.JoiningDate)
                {
                    bnLog.PreviousValue += ", Joining Date: " + employeeGeneral.JoiningDate.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Joining Date: " + model.JoiningDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.SeniorityDate != model.SeniorityDate)
                {
                    bnLog.PreviousValue += ", Seniority Date: " + employeeGeneral.SeniorityDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Seniority Date: " + model.SeniorityDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.OfficerStreamId != model.OfficerStreamId)
                {
                    if (employeeGeneral.OfficerStreamId > 0)
                    {
                        var prevOfficerStream = employeeService.GetDynamicTableInfoById("OfficerStream", "OfficerStreamId", employeeGeneral.OfficerStreamId ?? 0);
                        bnLog.PreviousValue += ", Stream: " + ((dynamic)prevOfficerStream).Name;
                    }
                    if (model.SubjectId > 0)
                    {
                        var newOfficerStream = employeeService.GetDynamicTableInfoById("OfficerStream", "OfficerStreamId", model.OfficerStreamId ?? 0);
                        bnLog.UpdatedValue += ", Stream: " + ((dynamic)newOfficerStream).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.LieutenantDate != model.LieutenantDate)
                {
                    bnLog.PreviousValue += ", Lieutenant Date: " + employeeGeneral.LieutenantDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Lieutenant Date: " + model.LieutenantDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.LastRLAvailedDate != model.LastRLAvailedDate)
                {
                    bnLog.PreviousValue += ", Last RL Availed: " + employeeGeneral.LastRLAvailedDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Last RL Availed: " + model.LastRLAvailedDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.IsContract != model.IsContract)
                {
                    bnLog.PreviousValue += ", Is Contract: " + employeeGeneral.IsContract;
                    bnLog.UpdatedValue += ", IsContract: " + model.IsContract;
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.ContractEndDate != model.ContractEndDate)
                {
                    bnLog.PreviousValue += ", Contract End Date: " + employeeGeneral.ContractEndDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Contract End Date: " + model.ContractEndDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.SasbStatus != model.SasbStatus)
                {
                    bnLog.PreviousValue += ", SASB Status: " + (employeeGeneral.SasbStatus==1?"Recom": employeeGeneral.SasbStatus == 2 ? "Not Recom" : employeeGeneral.SasbStatus == 3 ? "Yet To Apear" : "");
                    bnLog.UpdatedValue += ", SASB Status: " + (model.SasbStatus == 1 ? "Recom" : model.SasbStatus == 2 ? "Not Recom" : model.SasbStatus == 3 ? "Yet To Apear" : "");
                    bnoisUpdateCount += 1;
                }
                if (employeeGeneral.SasbRemarks != model.SasbRemarks)
                {
                    bnLog.PreviousValue += ", SASB Remarks: " + employeeGeneral.SasbRemarks;
                    bnLog.UpdatedValue += ", SASB Remarks: " + employeeGeneral.SasbRemarks;
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
                employeeGeneral.EmployeeId = employeeId;
                employeeGeneral.CreatedBy = model.CreatedBy;
                employeeGeneral.CreatedDate = DateTime.Now;
                employeeGeneral.IsActive = true;
            }


            GetAgeLimitServiceLimitLprDate(model.CategoryId, model.SubCategoryId, employeeInfo.RankId, model.CommissionDate ?? new DateTime(),Convert.ToDateTime(model.DoB), employeeInfo.OfficerType.ShortName, out DateTime ageLimit, out DateTime serviceLimit, out DateTime LprDate, out bool result);
            if (result)
            {
                employeeGeneral.AgeLimit = ageLimit;
                employeeGeneral.ServiceLimit = serviceLimit;
                employeeGeneral.LprDate = LprDate;
            }
            else
            {
                employeeGeneral.AgeLimit = model.AgeLimit;
                employeeGeneral.ServiceLimit = model.ServiceLimit;
                employeeGeneral.LprDate = model.LprDate;
            }
            employeeGeneral.ShortName = model.ShortName;
            employeeGeneral.ShortNameBan = model.ShortNameBan;
            employeeGeneral.NickName = model.NickName;
            employeeGeneral.NickNameBan = model.NickNameBan;
            employeeGeneral.EmployeeId = employeeId;
            employeeGeneral.CategoryId = model.CategoryId;
            employeeGeneral.SubCategoryId = model.SubCategoryId;
            employeeGeneral.CommissionTypeId = model.CommissionTypeId;
            employeeGeneral.BranchId = model.BranchId;
            employeeGeneral.SubBranchId = model.SubBranchId;
            employeeGeneral.SubjectId = model.SubjectId;
            employeeGeneral.OfficerStreamId = model.OfficerStreamId;
            employeeGeneral.SeniorityDate = model.SeniorityDate;
            employeeGeneral.DoB = Convert.ToDateTime(model.DoB);
            employeeGeneral.BirthPlace = model.BirthPlace;
            employeeGeneral.IsBirthOutside = model.IsBirthOutside;
            employeeGeneral.MigrationDate = model.MigrationDate;
            employeeGeneral.LieutenantDate = model.LieutenantDate;
            employeeGeneral.JoiningDate = Convert.ToDateTime(model.JoiningDate);
            employeeGeneral.MigrationReason = model.MigrationReason;
            employeeGeneral.CommissionDate = model.CommissionDate;
            employeeGeneral.NationalityId = model.NationalityId;
            employeeGeneral.MaritalTypeId = model.MaritalTypeId;
            employeeGeneral.MarriageDate = model.MarriageDate;
            employeeGeneral.ReligionId = model.ReligionId;
            employeeGeneral.ReligionCastId = model.ReligionCastId;
            employeeGeneral.ContactNo = model.ContactNo;
            employeeGeneral.EmailAddress = model.EmailAddress;
            employeeGeneral.IsDead = model.IsDead;
            employeeGeneral.DeadDate = model.DeadDate;
            employeeGeneral.DeadReason = model.DeadReason;
            employeeGeneral.LastRLAvailedDate = model.LastRLAvailedDate;     
            employeeGeneral.SasbStatus = model.SasbStatus;
            employeeGeneral.SasbRemarks = model.SasbRemarks;

            employeeGeneral.IsContract = model.IsContract;
            employeeGeneral.ContractEndDate = model.ContractEndDate;
            await employeeGeneralRepository.SaveAsync(employeeGeneral);
            model.EmployeeGeneralId = employeeGeneral.EmployeeGeneralId;

            if (employeeGeneral.IsDead)
            {
                Employee employee = await employeeRepository.FindOneAsync(x => x.EmployeeId == employeeGeneral.EmployeeId);
                if (employee == null)
                {
                    throw new InfinityNotFoundException("Officer data not found !");
                }
                employee.EmployeeStatusId = (int)OfficerCurrentStatus.Dead;
                await employeeRepository.SaveAsync(employee);
            }


            await processRepository.UpdateNamingConvention(employeeGeneral.EmployeeId);
            return model;
        }
        public async Task<EmployeeGeneralModel> GetEmployeeGeneralByPNo(string pno)
        {
            Employee employee = await employeeRepository.FindOneAsync(x => x.PNo == pno);
            EmployeeGeneral employeeGeneral = await employeeGeneralRepository.FindOneAsync(x => x.EmployeeId == employee.EmployeeId);
            if (employeeGeneral == null)
            {
                throw new InfinityNotFoundException("Officer General Information data not found !");
            }
            EmployeeGeneralModel model = ObjectConverter<EmployeeGeneral, EmployeeGeneralModel>.Convert(employeeGeneral);
            return model;
        }
    }
}
