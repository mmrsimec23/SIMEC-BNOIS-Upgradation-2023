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
   public class EmployeeOtherService: IEmployeeOtherService
    {
        private readonly IBnoisRepository<EmployeeOther> employeeOtherRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public EmployeeOtherService(IBnoisRepository<EmployeeOther> employeeOtherRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.employeeOtherRepository = employeeOtherRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public async Task<EmployeeOtherModel> GetEmployeeOthers(int employeeId)
        {
            if (employeeId <= 0)
            {
                return new EmployeeOtherModel();
            }
            EmployeeOther employeeOther = await employeeOtherRepository.FindOneAsync(x => x.EmployeeId == employeeId, new List<string> { "Employee"});

            if (employeeOther == null)
            {
                return new EmployeeOtherModel();
            }
            EmployeeOtherModel model = ObjectConverter<EmployeeOther, EmployeeOtherModel>.Convert(employeeOther);
            return model; ;
        }

        public async Task<EmployeeOtherModel> SaveEmployeeOther(int employeeId, EmployeeOtherModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Officer's Other information data missing!");
            }

            EmployeeOther employeeOther = ObjectConverter<EmployeeOtherModel, EmployeeOther>.Convert(model);

            if (employeeOther.EmployeeOtherId > 0)
            {
                employeeOther = await employeeOtherRepository.FindOneAsync(x => x.EmployeeId == employeeId);
                if (employeeOther == null)
                {
                    throw new InfinityNotFoundException("Officer's Other information not found!");
                }
                employeeOther.ModifiedDate = DateTime.Now;
                employeeOther.ModifiedBy = model.ModifiedBy;


                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeOther";
                bnLog.TableEntryForm = "Employee Other Info";
                bnLog.PreviousValue = "Id: " + model.EmployeeOtherId;
                bnLog.UpdatedValue = "Id: " + model.EmployeeOtherId;
                int bnoisUpdateCount = 0;
                if (employeeOther.EmployeeId != model.EmployeeId)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeOther.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    bnLog.UpdatedValue += ", Name: " + ((dynamic)emp).PNo + "_" + ((dynamic)emp).FullNameEng;
                    bnoisUpdateCount += 1;
                }

                if (employeeOther.NationalId != model.NationalId)
                {
                    bnLog.PreviousValue += ", National Id: " + employeeOther.NationalId;
                    bnLog.UpdatedValue += ", National Id: " + model.NationalId;
                    bnoisUpdateCount += 1;
                }

                if (employeeOther.IdIssueDate != model.IdIssueDate)
                {
                    bnLog.PreviousValue += ", National Id Issue Date: " + employeeOther.IdIssueDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", National Id Issue Date: " + model.IdIssueDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeOther.ServiceId != model.ServiceId)
                {
                    bnLog.PreviousValue += ", Service Id: " + employeeOther.ServiceId;
                    bnLog.UpdatedValue += ", Service Id: " + model.ServiceId;
                    bnoisUpdateCount += 1;
                }
                if (employeeOther.SerIssueDate != model.SerIssueDate)
                {
                    bnLog.PreviousValue += ", Service Id Issue date: " + employeeOther.SerIssueDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Service Id Issue date: " + model.SerIssueDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeOther.PassportNo != model.PassportNo)
                {
                    bnLog.PreviousValue += ", Passport No: " + employeeOther.PassportNo;
                    bnLog.UpdatedValue += ", Passport No: " + model.PassportNo;
                    bnoisUpdateCount += 1;
                }
                if (employeeOther.PassIssuePlace != model.PassIssuePlace)
                {
                    bnLog.PreviousValue += ", Passport Issue Place: " + employeeOther.PassIssuePlace;
                    bnLog.UpdatedValue += ", Passport Issue Place: " + model.PassIssuePlace;
                    bnoisUpdateCount += 1;
                }
                if (employeeOther.PassIssueDate != model.PassIssueDate)
                {
                    bnLog.PreviousValue += ", Passport Issue Date: " + employeeOther.PassIssueDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Passport Issue Date: " + model.PassIssueDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeOther.ExpiryDate != model.ExpiryDate)
                {
                    bnLog.PreviousValue += ", Passport Expiry Date: " + employeeOther.ExpiryDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Passport Expiry Date: " + model.ExpiryDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeOther.OldPassportNo != model.OldPassportNo)
                {
                    bnLog.PreviousValue += ", Old Passport No: " + employeeOther.OldPassportNo;
                    bnLog.UpdatedValue += ", Old Passport No: " + model.OldPassportNo;
                    bnoisUpdateCount += 1;
                }
                if (employeeOther.BirthCertificateNo != model.BirthCertificateNo)
                {
                    bnLog.PreviousValue += ", Birth Certificate No: " + employeeOther.BirthCertificateNo;
                    bnLog.UpdatedValue += ", Birth Certificate No: " + model.BirthCertificateNo;
                    bnoisUpdateCount += 1;
                }
                if (employeeOther.HasDrivingLicense != model.HasDrivingLicense)
                {
                    bnLog.PreviousValue += ", Driving License: " + employeeOther.HasDrivingLicense;
                    bnLog.UpdatedValue += ", Driving License: " + model.HasDrivingLicense;
                    bnoisUpdateCount += 1;
                }
                if (employeeOther.DrivingLicenseNo != model.DrivingLicenseNo)
                {
                    bnLog.PreviousValue += ", Driving License No: " + employeeOther.DrivingLicenseNo;
                    bnLog.UpdatedValue += ", Driving License No: " + model.DrivingLicenseNo;
                    bnoisUpdateCount += 1;
                }
                if (employeeOther.DLIssueDate != model.DLIssueDate)
                {
                    bnLog.PreviousValue += ", Driving License Issue Date: " + employeeOther.DLIssueDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Driving License Issue Date: " + model.DLIssueDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeOther.DLExpiryDate != model.DLExpiryDate)
                {
                    bnLog.PreviousValue += ", Driving License Expiry Date: " + employeeOther.DLExpiryDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Driving License Expiry Date: " + model.DLExpiryDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeOther.IsFreedomFighter != model.IsFreedomFighter)
                {
                    bnLog.PreviousValue += ", Freedom Fighter: " + employeeOther.IsFreedomFighter;
                    bnLog.UpdatedValue += ", Freedom Fighter: " + model.IsFreedomFighter;
                    bnoisUpdateCount += 1;
                }
                if (employeeOther.CertificateNo != model.CertificateNo)
                {
                    bnLog.PreviousValue += ", Freedom Fighter Certificate No.: " + employeeOther.CertificateNo;
                    bnLog.UpdatedValue += ", Freedom Fighter Certificate No.: " + model.CertificateNo;
                    bnoisUpdateCount += 1;
                }
                if (employeeOther.SectorNo != model.SectorNo)
                {
                    bnLog.PreviousValue += ", Sector No.: " + employeeOther.SectorNo;
                    bnLog.UpdatedValue += ", Sector No.: " + model.SectorNo;
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
                employeeOther.EmployeeId = employeeId;
                employeeOther.CreatedBy = model.CreatedBy;
                employeeOther.CreatedDate = DateTime.Now;
                employeeOther.IsActive = true;
            }
            employeeOther.NationalId = model.NationalId;
            employeeOther.IdIssueDate = model.IdIssueDate;
            employeeOther.ServiceId = model.ServiceId;
            employeeOther.SerIssueDate = model.SerIssueDate;
            employeeOther.EmployeeId = employeeId;
            employeeOther.PassportNo = model.PassportNo;
            employeeOther.PassIssueDate = model.PassIssueDate;
            employeeOther.ExpiryDate = model.ExpiryDate;
            employeeOther.OldPassportNo = model.OldPassportNo;
            employeeOther.PassIssuePlace = model.PassIssuePlace;
            employeeOther.HasDrivingLicense= model.HasDrivingLicense;
            employeeOther.DrivingLicenseNo = model.DrivingLicenseNo;
            employeeOther.DLIssueDate = model.DLIssueDate;
            employeeOther.DLExpiryDate = model.DLExpiryDate;
            employeeOther.BirthCertificateNo = model.BirthCertificateNo;
            
            employeeOther.IsFreedomFighter = model.IsFreedomFighter;
            employeeOther.CertificateNo = model.CertificateNo;
            employeeOther.SectorNo = model.SectorNo; 
            await employeeOtherRepository.SaveAsync(employeeOther);
            model.EmployeeOtherId = employeeOther.EmployeeOtherId;
            return model;
        }

        public async Task<EmployeeOtherModel> UpdateEmployeeOther(EmployeeOtherModel model)
        {
            string userId = Configuration.ConfigurationResolver.Get().LoggedInUser.UserId;
            EmployeeOther employeeOther = ObjectConverter<EmployeeOtherModel, EmployeeOther>.Convert(model);

            employeeOther = await employeeOtherRepository.FindOneAsync(x => x.EmployeeId == model.EmployeeId);

            if (employeeOther == null)
            {
                throw new InfinityNotFoundException("Employee Other not found !");
            }
            if (model.NIdFileName != null)
            {
                employeeOther.NIdFileName = model.NIdFileName;
            }
            if (model.DLFileName != null)
            {
                employeeOther.DLFileName = model.DLFileName;
            }

            if (model.PassportFIleName != null)
            {
                employeeOther.PassportFIleName = model.PassportFIleName;
            }

            employeeOther.ModifiedDate = DateTime.Now;
            employeeOther.ModifiedBy = userId;
            await employeeOtherRepository.SaveAsync(employeeOther);
            model = ObjectConverter<EmployeeOther, EmployeeOtherModel>.Convert(employeeOther);
            return model;
        }
    }
}
