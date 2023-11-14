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
    public class SpouseService : ISpouseService
    {

        private readonly IBnoisRepository<Spouse> spouseRepository;
        private readonly IBnoisRepository<EmployeeGeneral> employeeGeneralRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public SpouseService(IBnoisRepository<Spouse> spouseRepository, IBnoisRepository<EmployeeGeneral> employeeGeneralRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.spouseRepository = spouseRepository;
            this.employeeGeneralRepository = employeeGeneralRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }


        public List<SpouseModel> GetSpouses(int employeeId)
        {
            List<Spouse> educations = spouseRepository.FilterWithInclude(x => x.EmployeeId == employeeId, "Occupation", "Occupation1", "Occupation2","RankCategory").ToList();
            List<SpouseModel> models = ObjectConverter<Spouse, SpouseModel>.ConvertList(educations.ToList()).ToList();
            models = models.Select(x =>
            {
                x.RelationTypeName = Enum.GetName(typeof(RelationType), x.RelationType);
                x.CurrentStatusName = Enum.GetName(typeof(CurrentStatus), x.CurrentStatus);
                return x;
            }).ToList();
            return models;
        }

        public async Task<SpouseModel> GetSpouse(int spouseId)
        {
            if (spouseId <= 0)
            {
                return new SpouseModel();
            }
            Spouse spouse = await spouseRepository.FindOneAsync(x => x.SpouseId == spouseId, new List<string> { "Employee" });
            if (spouse == null)
            {
                throw new InfinityNotFoundException(" Spouse not found");
            }
            SpouseModel model = ObjectConverter<Spouse, SpouseModel>.Convert(spouse);
            return model;
        }


        public async Task<SpouseModel> SaveSpouse(int spouseId, SpouseModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Spouse data missing");
            }

            bool isExistData = spouseRepository.Exists(x => x.OccupationId == model.OccupationId && x.BNameEng == model.BNameEng && x.ANameEng == model.ANameEng && x.SpouseId != spouseId);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Spouse spouse = ObjectConverter<SpouseModel, Spouse>.Convert(model);
            if (spouseId > 0)
            {
                spouse = await spouseRepository.FindOneAsync(x => x.SpouseId == spouseId);
                if (spouse == null)
                {
                    throw new InfinityNotFoundException("Spouse data not found !");
                }

                spouse.ModifiedDate = DateTime.Now;
                spouse.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Spouse";
                bnLog.TableEntryForm = "Employee Spouse Information";
                bnLog.PreviousValue = "Id: " + model.SpouseId;
                bnLog.UpdatedValue = "Id: " + model.SpouseId;
                int bnoisUpdateCount = 0;
                if (spouse.EmployeeId != model.EmployeeId)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", spouse.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    bnLog.UpdatedValue += ", Name: " + ((dynamic)emp).PNo + "_" + ((dynamic)emp).FullNameEng;
                    bnoisUpdateCount += 1;
                }
                if (spouse.RelationType != model.RelationType)
                {
                    bnLog.PreviousValue += ", Marriage Type: " + (spouse.RelationType == 1 ? "First" : spouse.RelationType == 2 ? "Second" : spouse.RelationType == 3 ? "Third" : spouse.RelationType == 4 ? "Fourth" : "");
                    bnLog.UpdatedValue += ", Marriage Type: " + (model.RelationType == 1 ? "First" : model.RelationType == 2 ? "Second" : model.RelationType == 3 ? "Third" : model.RelationType == 4 ? "Fourth" : "");
                    bnoisUpdateCount += 1;
                }
                if (spouse.BNameEng != model.BNameEng)
                {
                    bnLog.PreviousValue += ", Name (Before Marriage): " + spouse.BNameEng;
                    bnLog.UpdatedValue += ", Name (Before Marriage): " + model.BNameEng;
                    bnoisUpdateCount += 1;
                }
                if (spouse.BNameBan != model.BNameBan)
                {
                    bnLog.PreviousValue += ", Full Name (বাংলা): " + spouse.BNameBan;
                    bnLog.UpdatedValue += ", Name (বাংলা) Before Marriage: " + model.BNameBan;
                    bnoisUpdateCount += 1;
                }
                if (spouse.ANameEng != model.ANameEng)
                {
                    bnLog.PreviousValue += ", Name (After Marriage): " + spouse.ANameEng;
                    bnLog.UpdatedValue += ", Name (After Marriage): " + model.ANameEng;
                    bnoisUpdateCount += 1;
                }
                if (spouse.ANameBan != model.ANameBan)
                {
                    bnLog.PreviousValue += ", Contact No: " + spouse.ANameBan;
                    bnLog.UpdatedValue += ", Contact No: " + model.ANameBan;
                    bnoisUpdateCount += 1;
                }
                if (spouse.NickName != model.NickName)
                {
                    bnLog.PreviousValue += ", Nick Name: " + spouse.NickName;
                    bnLog.UpdatedValue += ", Nick Name: " + model.NickName;
                    bnoisUpdateCount += 1;
                }
                if (spouse.NID != model.NID)
                {
                    bnLog.PreviousValue += ", NID: " + spouse.NID;
                    bnLog.UpdatedValue += ", NID: " + model.NID;
                    bnoisUpdateCount += 1;
                }
                if (spouse.MarriageDate != model.MarriageDate)
                {
                    bnLog.PreviousValue += ", Marriage Date: " + spouse.MarriageDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Marriage Date: " + model.MarriageDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (spouse.DateofBirth != model.DateofBirth)
                {
                    bnLog.PreviousValue += ", Date of Birth: " + spouse.DateofBirth?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Date of Birth: " + model.DateofBirth?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (spouse.OccupationId != model.OccupationId)
                {
                    if (spouse.OccupationId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", spouse.OccupationId ?? 0);
                        bnLog.PreviousValue += ", Occupation: " + ((dynamic)prev).Name;
                    }
                    if (model.OccupationId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", model.OccupationId ?? 0);
                        bnLog.UpdatedValue += ", Occupation: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (spouse.BirthPlace != model.BirthPlace)
                {
                    bnLog.PreviousValue += ", Birth Place: " + spouse.BirthPlace;
                    bnLog.UpdatedValue += ", Birth Place: " + model.BirthPlace;
                    bnoisUpdateCount += 1;
                }
                if (spouse.Age != model.Age)
                {
                    bnLog.PreviousValue += ", Age: " + spouse.Age;
                    bnLog.UpdatedValue += ", Age: " + model.Age;
                    bnoisUpdateCount += 1;
                }
                if (spouse.CurrentStatus != model.CurrentStatus)
                {
                    bnLog.PreviousValue += ", Current Status: " + (spouse.CurrentStatus == 1 ? "Alive" : spouse.CurrentStatus == 2 ? "Divorce" : spouse.CurrentStatus == 3 ? "Dead" : "");
                    bnLog.UpdatedValue += ", Current Status: " + (model.CurrentStatus == 1 ? "Alive" : model.CurrentStatus == 2 ? "Divorce" : model.CurrentStatus == 3 ? "Dead" : "");
                    bnoisUpdateCount += 1;
                }
                if (spouse.FileName != model.FileName)
                {
                    bnLog.PreviousValue += ", File Name: " + spouse.FileName;
                    bnLog.UpdatedValue += ", File Name: " + model.FileName;
                    bnoisUpdateCount += 1;
                }
                if (spouse.GenFormFileName != model.GenFormFileName)
                {
                    bnLog.PreviousValue += ", Spouse Gen form File: " + spouse.GenFormFileName;
                    bnLog.UpdatedValue += ", Spouse Gen form File: " + model.GenFormFileName;
                    bnoisUpdateCount += 1;
                }
                if (spouse.DeadDate != model.DeadDate)
                {
                    bnLog.PreviousValue += ", Dead Date: " + spouse.DeadDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Dead Date: " + model.DeadDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (spouse.DeadReason != model.DeadReason)
                {
                    bnLog.PreviousValue += ", Dead Reason: " + spouse.DeadReason;
                    bnLog.UpdatedValue += ", Dead Reason: " + model.DeadReason;
                    bnoisUpdateCount += 1;
                }
                if (spouse.IdMark != model.IdMark)
                {
                    bnLog.PreviousValue += ", Identification Mark: " + spouse.IdMark;
                    bnLog.UpdatedValue += ", Identification Mark: " + model.IdMark;
                    bnoisUpdateCount += 1;
                }
                if (spouse.EduQualification != model.EduQualification)
                {
                    bnLog.PreviousValue += ", Educational Qualification: " + spouse.EduQualification;
                    bnLog.UpdatedValue += ", Educational Qualification: " + model.EduQualification;
                    bnoisUpdateCount += 1;
                }
                if (spouse.ServiceAddress != model.ServiceAddress)
                {
                    bnLog.PreviousValue += ", Service Address: " + spouse.ServiceAddress;
                    bnLog.UpdatedValue += ", Service Address: " + model.ServiceAddress;
                    bnoisUpdateCount += 1;
                }
                if (spouse.Degination != model.Degination)
                {
                    bnLog.PreviousValue += ", Degination: " + spouse.Degination;
                    bnLog.UpdatedValue += ", Degination: " + model.Degination;
                    bnoisUpdateCount += 1;
                }
                if (spouse.IsArmedForceExp != model.IsArmedForceExp)
                {
                    bnLog.PreviousValue += ", Currently Served in Armed Forces Organization: " + spouse.IsArmedForceExp;
                    bnLog.UpdatedValue += ", Currently Served in Armed Forces Organization: " + model.IsArmedForceExp;
                    bnoisUpdateCount += 1;
                }
                if (spouse.RankCategoryId != model.RankCategoryId)
                {
                    if (spouse.RankCategoryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("RankCategory", "RankCategoryId", spouse.RankCategoryId ?? 0);
                        bnLog.PreviousValue += ", Rank Category: " + ((dynamic)prev).Name;
                    }
                    if (model.RankCategoryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("RankCategory", "RankCategoryId", model.RankCategoryId ?? 0);
                        bnLog.UpdatedValue += ", Rank Category: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (spouse.PNo != model.PNo)
                {
                    bnLog.PreviousValue += ", PNo: " + spouse.PNo;
                    bnLog.UpdatedValue += ", PNo: " + model.PNo;
                    bnoisUpdateCount += 1;
                }
                if (spouse.RankId != model.RankId)
                {
                    if (spouse.RankId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Rank", "RankId", spouse.RankId ?? 0);
                        bnLog.PreviousValue += ", Rank: " + ((dynamic)prev).ShortName;
                    }
                    if (model.RankId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId ?? 0);
                        bnLog.UpdatedValue += ", Rank: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (spouse.SocialActivity != model.SocialActivity)
                {
                    bnLog.PreviousValue += ",  Other Information(Social Activities): " + spouse.SocialActivity;
                    bnLog.UpdatedValue += ",  Other Information(Social Activities): " + model.SocialActivity;
                    bnoisUpdateCount += 1;
                }
                if (spouse.FatherName != model.FatherName)
                {
                    bnLog.PreviousValue += ",  Father's Name: " + spouse.FatherName;
                    bnLog.UpdatedValue += ",  Father's Name: " + model.FatherName;
                    bnoisUpdateCount += 1;
                }
                if (spouse.FatherNameBan != model.FatherNameBan)
                {
                    bnLog.PreviousValue += ", Father's Name (বাংলা): " + spouse.FatherNameBan;
                    bnLog.UpdatedValue += ", Father's Name (বাংলা): " + model.FatherNameBan;
                    bnoisUpdateCount += 1;
                }
                if (spouse.FatherPreAddress != model.FatherPreAddress)
                {
                    bnLog.PreviousValue += ", Father's Present Address: " + spouse.FatherPreAddress;
                    bnLog.UpdatedValue += ", Father's Present Address: " + model.FatherPreAddress;
                    bnoisUpdateCount += 1;
                }
                if (spouse.FatherPerAddress != model.FatherPerAddress)
                {
                    bnLog.PreviousValue += ", Father's Permanent  Address: " + spouse.FatherPerAddress;
                    bnLog.UpdatedValue += ", Father's Permanent  Address: " + model.FatherPerAddress;
                    bnoisUpdateCount += 1;
                }
                if (spouse.FatherOccupationId != model.FatherOccupationId)
                {
                    if (spouse.FatherOccupationId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", spouse.FatherOccupationId ?? 0);
                        bnLog.PreviousValue += ", Occupation: " + ((dynamic)prev).Name;
                    }
                    if (model.FatherOccupationId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", model.FatherOccupationId ?? 0);
                        bnLog.UpdatedValue += ", Occupation: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (spouse.FatherOtherInfo != model.FatherOtherInfo)
                {
                    bnLog.PreviousValue += ", Father's Other Information: " + spouse.FatherOtherInfo;
                    bnLog.UpdatedValue += ", Father's Other Information: " + model.FatherOtherInfo;
                    bnoisUpdateCount += 1;
                }
                if (spouse.IsFatherDead != model.IsFatherDead)
                {
                    bnLog.PreviousValue += ", Father's Status: " + (spouse.IsFatherDead == true ? "Dead" : "Alive");
                    bnLog.UpdatedValue += ", Father's Status: " + (model.IsFatherDead == true ? "Dead" : "Alive");
                    bnoisUpdateCount += 1;
                }
                if (spouse.MotherName != model.MotherName)
                {
                    bnLog.PreviousValue += ",  Mother's Name: " + spouse.MotherName;
                    bnLog.UpdatedValue += ",  Mother's Name: " + model.MotherName;
                    bnoisUpdateCount += 1;
                }
                if (spouse.MotherNameBan != model.MotherNameBan)
                {
                    bnLog.PreviousValue += ", Mother's Name (বাংলা): " + spouse.MotherNameBan;
                    bnLog.UpdatedValue += ", Mother's Name (বাংলা): " + model.MotherNameBan;
                    bnoisUpdateCount += 1;
                }
                if (spouse.MotherPreAddress != model.MotherPreAddress)
                {
                    bnLog.PreviousValue += ", Mother's Present Address: " + spouse.MotherPreAddress;
                    bnLog.UpdatedValue += ", Mother's Present Address: " + model.MotherPreAddress;
                    bnoisUpdateCount += 1;
                }
                if (spouse.MotherPerAddress != model.MotherPerAddress)
                {
                    bnLog.PreviousValue += ", Mother's Permanent Address: " + spouse.MotherPerAddress;
                    bnLog.UpdatedValue += ", Mother's Permanent Address: " + model.MotherPerAddress;
                    bnoisUpdateCount += 1;
                }
                if (spouse.MotherOccupationId != model.MotherOccupationId)
                {
                    if (spouse.MotherOccupationId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", spouse.MotherOccupationId ?? 0);
                        bnLog.PreviousValue += ", Occupation: " + ((dynamic)prev).Name;
                    }
                    if (model.FatherOccupationId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", model.MotherOccupationId ?? 0);
                        bnLog.UpdatedValue += ", Occupation: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (spouse.MotherOtherInfo != model.MotherOtherInfo)
                {
                    bnLog.PreviousValue += ", Mother's  Other Information: " + spouse.MotherOtherInfo;
                    bnLog.UpdatedValue += ", Mother's  Other Information: " + model.MotherOtherInfo;
                    bnoisUpdateCount += 1;
                }
                if (spouse.IsMotherDead != model.IsMotherDead)
                {
                    bnLog.PreviousValue += ", Mother's  Status: " + (spouse.IsMotherDead == true ? "Dead" : "Alive");
                    bnLog.UpdatedValue += ", Mother's  Status: " + (model.IsMotherDead == true ? "Dead" : "Alive");
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
                spouse.IsActive = true;
                spouse.CreatedDate = DateTime.Now;
                spouse.CreatedBy = userId;
            }

            spouse.EmployeeId = model.EmployeeId;
            spouse.OccupationId = model.OccupationId;
            spouse.BNameEng = model.BNameEng;
            spouse.BNameBan = model.BNameBan;
            spouse.ANameEng = model.ANameEng;
            spouse.ANameBan = model.ANameBan;
            spouse.NickName = model.NickName;
            spouse.NID = model.NID;
            spouse.MarriageDate = model.MarriageDate;
            spouse.BirthPlace = model.BirthPlace;
            spouse.RelationType = model.RelationType;
            spouse.CurrentStatus = model.CurrentStatus;
            if (model.CurrentStatus == 1)
            {
                spouse.DeadDate = null;
                spouse.DeadReason = null;
            }
            else 
            { 
                spouse.DeadDate = model.DeadDate;
                spouse.DeadReason = model.DeadReason;
            }
            spouse.IdMark = model.IdMark;
            spouse.EduQualification = model.EduQualification;
            spouse.ServiceAddress = model.ServiceAddress;
            spouse.SocialActivity = model.SocialActivity;
            spouse.Degination = model.Degination;
            spouse.DateofBirth=model.DateofBirth;
            //spouse.Age=DateTime.Today.Year-model.DateofBirth.Value.Year;

            spouse.IsFatherDead = model.IsFatherDead;
            spouse.FatherName = model.FatherName;
            spouse.FatherNameBan = model.FatherNameBan;
            spouse.FatherPreAddress = model.FatherPreAddress;
            spouse.FatherPerAddress = model.FatherPerAddress;
            spouse.FatherOccupationId = model.FatherOccupationId;
            spouse.FatherOtherInfo = model.FatherOtherInfo;

            spouse.IsMotherDead = model.IsMotherDead;
            spouse.MotherName = model.MotherName;
            spouse.MotherNameBan = model.MotherNameBan;
            spouse.MotherPreAddress = model.MotherPreAddress;
            spouse.MotherPerAddress = model.MotherPerAddress;
            spouse.MotherOccupationId = model.MotherOccupationId;
            spouse.MotherOtherInfo = model.MotherOtherInfo;

            spouse.IsArmedForceExp = model.IsArmedForceExp;
            spouse.RankCategoryId = model.RankCategoryId;
            spouse.PNo = model.PNo;
            await spouseRepository.SaveAsync(spouse);
            if(spouse.RelationType == 1){
                EmployeeGeneral employeeGeneral = await employeeGeneralRepository.FindOneAsync(x => x.EmployeeId == spouse.EmployeeId);
                employeeGeneral.MarriageDate = spouse.MarriageDate;
                await employeeGeneralRepository.SaveAsync(employeeGeneral);
            }



            return model;
        }


        public async Task<bool> DeleteSpouse(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Spouse spouse = await spouseRepository.FindOneAsync(x => x.SpouseId == id);
            if (spouse == null)
            {
                throw new InfinityNotFoundException("Spouse not found");
            }
            else
            {
                bool IsDeleted = false;
                try
                {
                    IsDeleted = await spouseRepository.DeleteAsync(spouse);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                if (IsDeleted)
                {
                    // data log section start
                    BnoisLog bnLog = new BnoisLog();
                    bnLog.TableName = "Spouse";
                    bnLog.TableEntryForm = "Employee Spouse Information";
                    bnLog.PreviousValue = "Id: " + spouse.SpouseId;
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", spouse.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    bnLog.PreviousValue += ", Marriage Type: " + (spouse.RelationType == 1 ? "First" : spouse.RelationType == 2 ? "Second" : spouse.RelationType == 3 ? "Third" : spouse.RelationType == 4 ? "Fourth" : "");
                    bnLog.PreviousValue += ", Name (Before Marriage): " + spouse.BNameEng + ", Full Name (বাংলা): " + spouse.BNameBan + ", Name (After Marriage): " + spouse.ANameEng;
                    bnLog.PreviousValue += ", Contact No: " + spouse.ANameBan + ", Nick Name: " + spouse.NickName + ", NID: " + spouse.NID + ", Marriage Date: " + spouse.MarriageDate?.ToString("dd/MM/yyyy");
                    bnLog.PreviousValue += ", Date of Birth: " + spouse.DateofBirth?.ToString("dd/MM/yyyy");
                    if (spouse.OccupationId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", spouse.OccupationId ?? 0);
                        bnLog.PreviousValue += ", Occupation: " + ((dynamic)prev).Name;
                    }
                    bnLog.PreviousValue += ", Birth Place: " + spouse.BirthPlace + ", Age: " + spouse.Age;
                    bnLog.PreviousValue += ", Current Status: " + (spouse.CurrentStatus == 1 ? "Alive" : spouse.CurrentStatus == 2 ? "Divorce" : spouse.CurrentStatus == 3 ? "Dead" : "");
                    bnLog.PreviousValue += ", File Name: " + spouse.FileName + ", Spouse Gen form File: " + spouse.GenFormFileName  + ", Dead Date: " + spouse.DeadDate?.ToString("dd/MM/yyyy");
                    bnLog.PreviousValue += ", Dead Reason: " + spouse.DeadReason + ", Identification Mark: " + spouse.IdMark + ", Educational Qualification: " + spouse.EduQualification;
                    bnLog.PreviousValue += ", Service Address: " + spouse.ServiceAddress + ", Degination: " + spouse.Degination + ", Currently Served in Armed Forces Organization: " + spouse.IsArmedForceExp;
                    if (spouse.RankCategoryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("RankCategory", "RankCategoryId", spouse.RankCategoryId ?? 0);
                        bnLog.PreviousValue += ", Rank Category: " + ((dynamic)prev).Name;
                    }
                    bnLog.PreviousValue += ", PNo: " + spouse.PNo;
                    if (spouse.RankId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Rank", "RankId", spouse.RankId ?? 0);
                        bnLog.PreviousValue += ", Rank: " + ((dynamic)prev).ShortName;
                    }
                    bnLog.PreviousValue += ",  Other Information(Social Activities): " + spouse.SocialActivity + ",  Father's Name: " + spouse.FatherName;
                    bnLog.PreviousValue += ", Father's Name (বাংলা): " + spouse.FatherNameBan + ", Father's Present Address: " + spouse.FatherPreAddress;
                    bnLog.PreviousValue += ", Father's Permanent  Address: " + spouse.FatherPerAddress;
                    if (spouse.FatherOccupationId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", spouse.FatherOccupationId ?? 0);
                        bnLog.PreviousValue += ", Occupation: " + ((dynamic)prev).Name;
                    }
                    bnLog.PreviousValue += ", Father's Other Information: " + spouse.FatherOtherInfo + ", Father's Status: " + (spouse.IsFatherDead == true ? "Dead" : "Alive");
                    bnLog.PreviousValue += ",  Mother's Name: " + spouse.MotherName + ", Mother's Name (বাংলা): " + spouse.MotherNameBan + ", Mother's Present Address: " + spouse.MotherPreAddress;
                    bnLog.PreviousValue += ", Mother's Permanent Address: " + spouse.MotherPerAddress;
                    if (spouse.MotherOccupationId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", spouse.MotherOccupationId ?? 0);
                        bnLog.PreviousValue += ", Occupation: " + ((dynamic)prev).Name;
                    }
                    bnLog.PreviousValue += ", Mother's  Other Information: " + spouse.MotherOtherInfo;
                    bnLog.PreviousValue += ", Mother's  Status: " + (spouse.IsMotherDead == true ? "Dead" : "Alive");
                    bnLog.UpdatedValue = "This Record has been Deleted!";

                    bnLog.LogStatus = 2; // 1 for update, 2 for delete
                    bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                    bnLog.LogCreatedDate = DateTime.Now;

                    await bnoisLogRepository.SaveAsync(bnLog);

                    //data log section end
                }
                return IsDeleted;
            }
        }


        public List<SelectModel> GetCurrentStatusSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(CurrentStatus)).Cast<CurrentStatus>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }

        public List<SelectModel> GetRelationTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(RelationType)).Cast<RelationType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }

       
        public async Task<List<SelectModel>> GetSpouseSelectModels(int employeeId)
        {
            ICollection<Spouse> shipCategories = await spouseRepository.FilterAsync(x => x.IsActive && x.EmployeeId==employeeId);
            return shipCategories.Select(x => new SelectModel()
            {
                Text = String.Format("{0}'s Child",x.BNameEng),
                Value = x.SpouseId
            }).ToList();
        }

        public async Task<SpouseModel> UpdateSpouse(SpouseModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Spouse data missing");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Spouse spouse = ObjectConverter<SpouseModel, Spouse>.Convert(model);

            spouse = await spouseRepository.FindOneAsync(x => x.SpouseId == model.SpouseId);
            if (spouse == null)
            {
                throw new InfinityNotFoundException("Spouse not found !");
            }

            if (model.FileName != null)
            {
                spouse.FileName = model.FileName;
            }

            if (model.GenFormFileName != null)
            {
                spouse.GenFormFileName = model.GenFormFileName;
            }
            spouse.ModifiedDate = DateTime.Now;
            spouse.ModifiedBy = userId;
            await spouseRepository.SaveAsync(spouse);
            model.SpouseId = spouse.SpouseId;
            return model;
        }
    }
}