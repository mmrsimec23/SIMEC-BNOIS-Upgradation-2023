using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.Configuration;

namespace Infinity.Bnois.ApplicationService.Implementation
{
  public  class PhysicalConditionService : IPhysicalConditionService
    {
        private readonly IBnoisRepository<PhysicalCondition> physicalConditionRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public PhysicalConditionService(IBnoisRepository<PhysicalCondition> physicalConditionRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.physicalConditionRepository = physicalConditionRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public async Task<PhysicalConditionModel> GetPhysicalConditions(int employeeId)
        {
            if (employeeId <= 0)

            {
                return new PhysicalConditionModel();
            }
            PhysicalCondition physicalCondition = await physicalConditionRepository.FindOneAsync(x => x.EmployeeId == employeeId, new List<string> { "Color", "Color1", "Color2", "EyeVision", "BloodGroup", "PhysicalStructure", "MedicalCategory" });

            if (physicalCondition == null)
            {
                return new PhysicalConditionModel();
            }
            PhysicalConditionModel model = ObjectConverter<PhysicalCondition, PhysicalConditionModel>.Convert(physicalCondition);
            return model;
        }

        public async Task<PhysicalConditionModel> SavePhysicalCondition(int employeeId, PhysicalConditionModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Physical Condition data missing!");
            }

            PhysicalCondition physicalCondition = ObjectConverter<PhysicalConditionModel, PhysicalCondition>.Convert(model);

            if (physicalCondition.PhysicalConditionId > 0)
            {
                physicalCondition = await physicalConditionRepository.FindOneAsync(x => x.EmployeeId == employeeId);
                if (physicalCondition == null)
                {
                    throw new InfinityNotFoundException("Physical Condition data not found!");
                }
                physicalCondition.ModifiedDate = DateTime.Now;
                physicalCondition.ModifiedBy = model.ModifiedBy;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PhysicalCondition";
                bnLog.TableEntryForm = "Employee Physical Structure";
                bnLog.PreviousValue = "Id: " + model.PhysicalConditionId;
                bnLog.UpdatedValue = "Id: " + model.PhysicalConditionId;
                int bnoisUpdateCount = 0;
                if (physicalCondition.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", physicalCondition.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                    //bnoisUpdateCount += 1;
                }

                if (physicalCondition.EyeColorId != model.EyeColorId)
                {
                    if (physicalCondition.EyeColorId > 0)
                    {
                        var prevColor = employeeService.GetDynamicTableInfoById("Color", "ColorId", physicalCondition.EyeColorId);
                        bnLog.PreviousValue += ", Eye Color: " + ((dynamic)prevColor).Name;
                    }
                    if (model.EyeColorId > 0)
                    {
                        var newColor = employeeService.GetDynamicTableInfoById("Color", "ColorId", model.EyeColorId);
                        bnLog.UpdatedValue += ", Eye Color: " + ((dynamic)newColor).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (physicalCondition.SkinColorId != model.SkinColorId)
                {
                    if (physicalCondition.SkinColorId > 0)
                    {
                        var prevColor = employeeService.GetDynamicTableInfoById("Color", "ColorId", physicalCondition.SkinColorId);
                        bnLog.PreviousValue += ", Skin Color: " + ((dynamic)prevColor).Name;
                    }
                    if (model.SkinColorId > 0)
                    {
                        var newColor = employeeService.GetDynamicTableInfoById("Color", "ColorId", model.SkinColorId);
                        bnLog.UpdatedValue += ", Skin Color: " + ((dynamic)newColor).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (physicalCondition.HairColorId != model.HairColorId)
                {
                    if (physicalCondition.HairColorId > 0)
                    {
                        var prevColor = employeeService.GetDynamicTableInfoById("Color", "ColorId", physicalCondition.HairColorId);
                        bnLog.PreviousValue += ", Hair Color: " + ((dynamic)prevColor).Name;
                    }
                    if (model.HairColorId > 0)
                    {
                        var newColor = employeeService.GetDynamicTableInfoById("Color", "ColorId", model.HairColorId);
                        bnLog.UpdatedValue += ", Hair Color: " + ((dynamic)newColor).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (physicalCondition.EyeVisionId != model.EyeVisionId)
                {
                    if (physicalCondition.EyeVisionId > 0)
                    {
                        var prevEyeVision = employeeService.GetDynamicTableInfoById("EyeVision", "EyeVisionId", physicalCondition.EyeVisionId);
                        bnLog.PreviousValue += ", Eye Vision: " + ((dynamic)prevEyeVision).Name;
                    }
                    if (model.EyeVisionId > 0)
                    {
                        var newEyeVision = employeeService.GetDynamicTableInfoById("EyeVision", "EyeVisionId", model.EyeVisionId);
                        bnLog.UpdatedValue += ", Eye Vision: " + ((dynamic)newEyeVision).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (physicalCondition.BloodGroupId != model.BloodGroupId)
                {
                    if (physicalCondition.BloodGroupId > 0)
                    {
                        var prevBloodGroup = employeeService.GetDynamicTableInfoById("BloodGroup", "BloodGroupId", physicalCondition.BloodGroupId);
                        bnLog.PreviousValue += ", Blood Group: " + ((dynamic)prevBloodGroup).Name;
                    }
                    if (model.BloodGroupId > 0)
                    {
                        var newBloodGroup = employeeService.GetDynamicTableInfoById("BloodGroup", "BloodGroupId", model.BloodGroupId);
                        bnLog.UpdatedValue += ", Blood Group: " + ((dynamic)newBloodGroup).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (physicalCondition.PhysicalStructureId != model.PhysicalStructureId)
                {
                    if (physicalCondition.PhysicalStructureId > 0)
                    {
                        var prevPhysicalStructure = employeeService.GetDynamicTableInfoById("PhysicalStructure", "PhysicalStructureId", physicalCondition.PhysicalStructureId);
                        bnLog.PreviousValue += ", Physical Structure: " + ((dynamic)prevPhysicalStructure).Name;
                    }
                    if (model.PhysicalStructureId > 0)
                    {
                        var newPhysicalStructure = employeeService.GetDynamicTableInfoById("PhysicalStructure", "PhysicalStructureId", model.PhysicalStructureId);
                        bnLog.UpdatedValue += ", Physical Structure: " + ((dynamic)newPhysicalStructure).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (physicalCondition.IsPerMedicalCategory != model.IsPerMedicalCategory)
                {
                    bnLog.PreviousValue += ", Permanent Medical Category: " + physicalCondition.IsPerMedicalCategory;
                    bnLog.UpdatedValue += ", Permanent Medical Category: " + model.IsPerMedicalCategory;
                    
                    bnoisUpdateCount += 1;
                }
                if (physicalCondition.MedicalCategoryId != model.MedicalCategoryId)
                {
                    if (physicalCondition.MedicalCategoryId > 0)
                    {
                        var prevMedicalCategory = employeeService.GetDynamicTableInfoById("MedicalCategory", "MedicalCategoryId", physicalCondition.MedicalCategoryId);
                        bnLog.PreviousValue += ", Medical Category: " + ((dynamic)prevMedicalCategory).Name;
                    }
                    if (model.MedicalCategoryId > 0)
                    {
                        var newMedicalCategory = employeeService.GetDynamicTableInfoById("MedicalCategory", "MedicalCategoryId", model.MedicalCategoryId);
                        bnLog.UpdatedValue += ", Medical Category: " + ((dynamic)newMedicalCategory).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (physicalCondition.HeightInFeet != model.HeightInFeet)
                {
                    bnLog.PreviousValue += ", Height In Feet: " + physicalCondition.HeightInFeet;
                    bnLog.UpdatedValue += ", Height In Feet: " + model.HeightInFeet;

                    bnoisUpdateCount += 1;
                }
                if (physicalCondition.HeightInInc != model.HeightInInc)
                {
                    bnLog.PreviousValue += ", Height In Inch: " + physicalCondition.HeightInInc;
                    bnLog.UpdatedValue += ", Height In Inch: " + model.HeightInInc;

                    bnoisUpdateCount += 1;
                }
                if (physicalCondition.HeightInCM != model.HeightInCM)
                {
                    bnLog.PreviousValue += ", Height In CM: " + physicalCondition.HeightInCM;
                    bnLog.UpdatedValue += ", Height In CM: " + model.HeightInCM;

                    bnoisUpdateCount += 1;
                }
                if (physicalCondition.Weight != model.Weight)
                {
                    bnLog.PreviousValue += ", Weight: " + physicalCondition.Weight;
                    bnLog.UpdatedValue += ", Weight: " + model.Weight;

                    bnoisUpdateCount += 1;
                }
                if (physicalCondition.IdentificationMark != model.IdentificationMark)
                {
                    bnLog.PreviousValue += ", Identification Mark: " + physicalCondition.IdentificationMark;
                    bnLog.UpdatedValue += ", Identification Mark: " + model.IdentificationMark;

                    bnoisUpdateCount += 1;
                }
                if (physicalCondition.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + physicalCondition.Remarks;
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
                physicalCondition.EmployeeId = employeeId;
                physicalCondition.CreatedBy = model.CreatedBy;
                physicalCondition.CreatedDate = DateTime.Now;
                physicalCondition.IsActive = true;
            }



            physicalCondition.EyeColorId = model.EyeColorId;
            physicalCondition.SkinColorId = model.SkinColorId;
            physicalCondition.HairColorId = model.HairColorId;
            physicalCondition.EyeVisionId = model.EyeVisionId;
            physicalCondition.EmployeeId = employeeId;
            physicalCondition.BloodGroupId = model.BloodGroupId;
            physicalCondition.MedicalCategoryId = model.MedicalCategoryId;
            physicalCondition.IsPerMedicalCategory = model.IsPerMedicalCategory;
            physicalCondition.HeightInFeet = model.HeightInFeet;
            physicalCondition.HeightInInc = model.HeightInInc;
            physicalCondition.Weight = model.Weight;
            physicalCondition.IdentificationMark = model.IdentificationMark;
            physicalCondition.Remarks = model.Remarks;
            physicalCondition.HeightInCM = ((model.HeightInFeet*12)+model.HeightInInc)* 2.54;
            await physicalConditionRepository.SaveAsync(physicalCondition);
            model.PhysicalConditionId = physicalCondition.PhysicalConditionId;
            return model;
        }



        public async Task<PhysicalConditionModel> UpdatePhysicalCondition(PhysicalConditionModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Physical Condition data missing");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            PhysicalCondition physicalCondition = ObjectConverter<PhysicalConditionModel, PhysicalCondition>.Convert(model);

            physicalCondition = await physicalConditionRepository.FindOneAsync(x => x.EmployeeId == model.EmployeeId);
            if (physicalCondition == null)
            {
                throw new InfinityNotFoundException("Physical Condition not found !");
            }

            if (model.FileName != null)
            {
                physicalCondition.FileName = model.FileName;
            }


            physicalCondition.ModifiedDate = DateTime.Now;
            physicalCondition.ModifiedBy = userId;
            await physicalConditionRepository.SaveAsync(physicalCondition);
            model.PhysicalConditionId = physicalCondition.PhysicalConditionId;
            return model;
        }
    }
}
