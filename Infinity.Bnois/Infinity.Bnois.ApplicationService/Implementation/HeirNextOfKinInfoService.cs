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
    public class HeirNextOfKinInfoService : IHeirNextOfKinInfoService
    {

        private readonly IBnoisRepository<HeirNextOfKinInfo> heirNextOfKinInfoRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public HeirNextOfKinInfoService(IBnoisRepository<HeirNextOfKinInfo> heirNextOfKinInfoRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.heirNextOfKinInfoRepository = heirNextOfKinInfoRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }


        public List<HeirNextOfKinInfoModel> GetHeirNextOfKinInfoList(int employeeId)
        {
            List<HeirNextOfKinInfo> educations = heirNextOfKinInfoRepository.FilterWithInclude(x => x.EmployeeId == employeeId, "Occupation","Gender","Relation","HeirType").ToList();
            List<HeirNextOfKinInfoModel> models = ObjectConverter<HeirNextOfKinInfo, HeirNextOfKinInfoModel>.ConvertList(educations.ToList()).ToList();
            models = models.Select(x =>
            {
                x.HeirKinTypeName = Enum.GetName(typeof(HeirKinType), x.HeirKinType);
                return x;
            }).ToList();
            return models;
        }

        public async Task<HeirNextOfKinInfoModel> GetHeirNextOfKinInfo(int heirNextOfKinInfoId)
        {
            if (heirNextOfKinInfoId <= 0)
            {
                return new HeirNextOfKinInfoModel();
            }
            HeirNextOfKinInfo heirNextOfKinInfo = await heirNextOfKinInfoRepository.FindOneAsync(x => x.HeirNextOfKinInfoId == heirNextOfKinInfoId, new List<string> { "Employee" });
            if (heirNextOfKinInfo == null)
            {
                throw new InfinityNotFoundException(" Heir/Next Of Kin Info not found");
            }
            HeirNextOfKinInfoModel model = ObjectConverter<HeirNextOfKinInfo, HeirNextOfKinInfoModel>.Convert(heirNextOfKinInfo);
            return model;
        }

        public async Task<HeirNextOfKinInfoModel> SaveHeirNextOfKinInfo(int heirNextOfKinInfoId, HeirNextOfKinInfoModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Heir/Next Of Kin Info data missing");
            }

            bool isExistData = heirNextOfKinInfoRepository.Exists(x => x.OccupationId == model.OccupationId && x.GenderId==model.GenderId && x.RelationId==model.RelationId && x.NameEng == model.NameEng && x.HeirTypeId==model.HeirTypeId  && x.HeirNextOfKinInfoId != model.HeirNextOfKinInfoId);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            if (model.HeirKinType == 1)
            {
                bool isExistNextOfKin = heirNextOfKinInfoRepository.Exists(x => x.HeirKinType == model.HeirKinType && x.EmployeeId==model.EmployeeId && x.HeirNextOfKinInfoId !=model.HeirNextOfKinInfoId);

                if (isExistNextOfKin)
                {
                    throw new InfinityInvalidDataException("Only 1 Next of kin can be added. !");
                }
            }
           

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            HeirNextOfKinInfo heirNextOfKinInfo = ObjectConverter<HeirNextOfKinInfoModel, HeirNextOfKinInfo>.Convert(model);
            if (heirNextOfKinInfoId > 0)
            {
                heirNextOfKinInfo = await heirNextOfKinInfoRepository.FindOneAsync(x => x.HeirNextOfKinInfoId == heirNextOfKinInfoId);
                if (heirNextOfKinInfo == null)
                {
                    throw new InfinityNotFoundException("Heir/Next Of Kin Info data not found !");
                }

                heirNextOfKinInfo.ModifiedDate = DateTime.Now;
                heirNextOfKinInfo.ModifiedBy = userId;


                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "HeirNextOfKinInfo";
                bnLog.TableEntryForm = "Employee Heir & Next Of Kin Information";
                bnLog.PreviousValue = "Id: " + model.HeirNextOfKinInfoId;
                bnLog.UpdatedValue = "Id: " + model.HeirNextOfKinInfoId;
                int bnoisUpdateCount = 0;
                if (heirNextOfKinInfo.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", heirNextOfKinInfo.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                    //bnoisUpdateCount += 1;
                }
                if (heirNextOfKinInfo.HeirKinType != model.HeirKinType)
                {
                    bnLog.PreviousValue += ", Type: " + (heirNextOfKinInfo.HeirKinType == 1 ? "Next_Of_Kin" : heirNextOfKinInfo.HeirKinType == 2 ? "Heir" : "");
                    bnLog.UpdatedValue += ", Type: " + (model.HeirKinType == 1 ? "Next_Of_Kin" : model.HeirKinType == 2 ? "Heir" : ""); ;
                    bnoisUpdateCount += 1;
                }
                if (heirNextOfKinInfo.HeirTypeId != model.HeirTypeId)
                {
                    if (heirNextOfKinInfo.HeirTypeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("HeirType", "HeirTypeId", heirNextOfKinInfo.HeirTypeId ?? 0);
                        bnLog.PreviousValue += ", Heir Type: " + ((dynamic)prev).Name;
                    }
                    if (model.HeirTypeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("HeirType", "HeirTypeId", model.HeirTypeId ?? 0);
                        bnLog.UpdatedValue += ", Heir Type: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (heirNextOfKinInfo.NameEng != model.NameEng)
                {
                    bnLog.PreviousValue += ", Name: " + heirNextOfKinInfo.NameEng;
                    bnLog.UpdatedValue += ", Name: " + model.NameEng;
                    bnoisUpdateCount += 1;
                }
                if (heirNextOfKinInfo.NameBan != model.NameBan)
                {
                    bnLog.PreviousValue += ", Name (বাংলা): " + heirNextOfKinInfo.NameBan;
                    bnLog.UpdatedValue += ", Name (বাংলা): " + model.NameBan;
                    bnoisUpdateCount += 1;
                }
                if (heirNextOfKinInfo.GenderId != model.GenderId)
                {
                    if (heirNextOfKinInfo.GenderId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Gender", "GenderId", heirNextOfKinInfo.GenderId);
                        bnLog.PreviousValue += ", Gender: " + ((dynamic)prev).Name;
                    }
                    if (model.GenderId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Gender", "GenderId", model.GenderId);
                        bnLog.UpdatedValue += ", Gender: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (heirNextOfKinInfo.RelationId != model.RelationId)
                {
                    if (heirNextOfKinInfo.RelationId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Relation", "RelationId", heirNextOfKinInfo.RelationId);
                        bnLog.PreviousValue += ", Relation: " + ((dynamic)prev).Name;
                    }
                    if (model.RelationId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Relation", "RelationId", model.RelationId);
                        bnLog.UpdatedValue += ", Relation: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (heirNextOfKinInfo.Email != model.Email)
                {
                    bnLog.PreviousValue += ", Email: " + heirNextOfKinInfo.Email;
                    bnLog.UpdatedValue += ", Email: " + model.Email;
                    bnoisUpdateCount += 1;
                }
                if (heirNextOfKinInfo.ContactNumber != model.ContactNumber)
                {
                    bnLog.PreviousValue += ", Contact Number: " + heirNextOfKinInfo.ContactNumber;
                    bnLog.UpdatedValue += ", Contact Number: " + model.ContactNumber;
                    bnoisUpdateCount += 1;
                }
                if (heirNextOfKinInfo.PresentAddress != model.PresentAddress)
                {
                    bnLog.PreviousValue += ", Present Address: " + heirNextOfKinInfo.PresentAddress;
                    bnLog.UpdatedValue += ", Present Address: " + model.PresentAddress;
                    bnoisUpdateCount += 1;
                }
                if (heirNextOfKinInfo.PermanentAddress != model.PermanentAddress)
                {
                    bnLog.PreviousValue += ", Permanent Address: " + heirNextOfKinInfo.PermanentAddress;
                    bnLog.UpdatedValue += ", Permanent Address: " + model.PermanentAddress;
                    bnoisUpdateCount += 1;
                }
                if (heirNextOfKinInfo.PassportNumber != model.PassportNumber)
                {
                    bnLog.PreviousValue += ", Passport Number: " + heirNextOfKinInfo.PassportNumber;
                    bnLog.UpdatedValue += ", Passport Number: " + model.PassportNumber;
                    bnoisUpdateCount += 1;
                }
                if (heirNextOfKinInfo.OccupationId != model.OccupationId)
                {
                    if (heirNextOfKinInfo.OccupationId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", heirNextOfKinInfo.OccupationId ?? 0);
                        bnLog.PreviousValue += ", Occupation: " + ((dynamic)prev).Name;
                    }
                    if (model.OccupationId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", model.OccupationId ?? 0);
                        bnLog.UpdatedValue += ", Occupation: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (heirNextOfKinInfo.Pradhikar != model.Pradhikar)
                {
                    bnLog.PreviousValue += ", Pradhikar (% of Wealth): " + heirNextOfKinInfo.Pradhikar;
                    bnLog.UpdatedValue += ", Pradhikar (% of Wealth): " + model.Pradhikar;
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
                heirNextOfKinInfo.IsActive = true;
                heirNextOfKinInfo.CreatedDate = DateTime.Now;
                heirNextOfKinInfo.CreatedBy = userId;
            }

            heirNextOfKinInfo.NameEng = model.NameEng;
            heirNextOfKinInfo.NameBan = model.NameBan;
            heirNextOfKinInfo.Email = model.Email;
            heirNextOfKinInfo.ContactNumber = model.ContactNumber;
            heirNextOfKinInfo.PresentAddress = model.PresentAddress;
            heirNextOfKinInfo.PermanentAddress = model.PermanentAddress;
            heirNextOfKinInfo.PassportNumber = model.PassportNumber;
            heirNextOfKinInfo.HeirKinType = model.HeirKinType;
            heirNextOfKinInfo.Pradhikar = model.Pradhikar;
            heirNextOfKinInfo.HeirTypeId = model.HeirTypeId;
            heirNextOfKinInfo.RelationId = model.RelationId;
            heirNextOfKinInfo.GenderId = model.GenderId;
            heirNextOfKinInfo.EmployeeId = model.EmployeeId;
            heirNextOfKinInfo.OccupationId = model.OccupationId;


            await heirNextOfKinInfoRepository.SaveAsync(heirNextOfKinInfo);
            model.HeirNextOfKinInfoId = heirNextOfKinInfo.HeirNextOfKinInfoId;

            return model;
        }



        public async Task<bool> DeleteHeirNextOfKinInfo(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            HeirNextOfKinInfo nextOfKinInfo = await heirNextOfKinInfoRepository.FindOneAsync(x => x.HeirNextOfKinInfoId == id);
            if (nextOfKinInfo == null)
            {
                throw new InfinityNotFoundException("Heir Or Next Of Kin not found");
            }
            else
            {
                bool IsDeleted = false;
                try
                {
                    // data log section start
                    BnoisLog bnLog = new BnoisLog();
                    bnLog.TableName = "HeirNextOfKinInfo";
                    bnLog.TableEntryForm = "Employee Heir & Next Of Kin Information";
                    bnLog.PreviousValue = "Id: " + nextOfKinInfo.HeirNextOfKinInfoId;
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", nextOfKinInfo.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    bnLog.PreviousValue += ", Type: " + (nextOfKinInfo.HeirKinType == 1 ? "Next_Of_Kin" : nextOfKinInfo.HeirKinType == 2 ? "Heir" : "");
                    if (nextOfKinInfo.HeirTypeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("HeirType", "HeirTypeId", nextOfKinInfo.HeirTypeId ?? 0);
                        bnLog.PreviousValue += ", Heir Type: " + ((dynamic)prev).Name;
                    }
                    bnLog.PreviousValue += ", Name: " + nextOfKinInfo.NameEng + ", Name (বাংলা): " + nextOfKinInfo.NameBan;
                    if (nextOfKinInfo.GenderId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Gender", "GenderId", nextOfKinInfo.GenderId);
                        bnLog.PreviousValue += ", Gender: " + ((dynamic)prev).Name;
                    }
                    if (nextOfKinInfo.RelationId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Relation", "RelationId", nextOfKinInfo.RelationId);
                        bnLog.PreviousValue += ", Relation: " + ((dynamic)prev).Name;
                    }
                    bnLog.PreviousValue += ", Email: " + nextOfKinInfo.Email + ", Contact Number: " + nextOfKinInfo.ContactNumber + ", Present Address: " + nextOfKinInfo.PresentAddress;
                    bnLog.PreviousValue += ", Permanent Address: " + nextOfKinInfo.PermanentAddress + ", Passport Number: " + nextOfKinInfo.PassportNumber;
                    if (nextOfKinInfo.OccupationId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Occupation", "OccupationId", nextOfKinInfo.OccupationId ?? 0);
                        bnLog.PreviousValue += ", Occupation: " + ((dynamic)prev).Name;
                    }
                    bnLog.PreviousValue += ", Pradhikar (% of Wealth): " + nextOfKinInfo.Pradhikar;
                   
                    bnLog.UpdatedValue = "This Record has been Deleted!";
                    
                    bnLog.LogStatus = 2; // 1 for update, 2 for delete
                    bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                    bnLog.LogCreatedDate = DateTime.Now;


                    await bnoisLogRepository.SaveAsync(bnLog);

                    //data log section end
                    IsDeleted = await heirNextOfKinInfoRepository.DeleteAsync(nextOfKinInfo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return IsDeleted;
            }
        }


        public List<SelectModel> GetHeirKinTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(HeirKinType)).Cast<HeirKinType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }

        public async Task<HeirNextOfKinInfoModel> UpdateHeirNextOfKinInfo(HeirNextOfKinInfoModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("HeirNextOfKin data missing");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            HeirNextOfKinInfo heirNextOfKinInfo = ObjectConverter<HeirNextOfKinInfoModel, HeirNextOfKinInfo>.Convert(model);

            heirNextOfKinInfo = await heirNextOfKinInfoRepository.FindOneAsync(x => x.HeirNextOfKinInfoId == model.HeirNextOfKinInfoId);
            if (heirNextOfKinInfo == null)
            {
                throw new InfinityNotFoundException("HeirNextOfKin not found !");
            }

            if (model.FileName != null)
            {
                heirNextOfKinInfo.FileName = model.FileName;
            }

            heirNextOfKinInfo.ModifiedDate = DateTime.Now;
            heirNextOfKinInfo.ModifiedBy = userId;
            await heirNextOfKinInfoRepository.SaveAsync(heirNextOfKinInfo);
            model.HeirNextOfKinInfoId = heirNextOfKinInfo.HeirNextOfKinInfoId;
            return model;
        }
    }
}