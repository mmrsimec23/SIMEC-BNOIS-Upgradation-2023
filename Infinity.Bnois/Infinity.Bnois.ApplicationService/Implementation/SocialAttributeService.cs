using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class SocialAttributeService : ISocialAttributeService
    {
        private readonly IBnoisRepository<SocialAttribute> socialAttributeRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public SocialAttributeService(IBnoisRepository<SocialAttribute> socialAttributeRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.socialAttributeRepository = socialAttributeRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public async Task<SocialAttributeModel> GetSocialAttribute(int employeeId)
        {
            if (employeeId <= 0)
            {
                return new SocialAttributeModel();
            }
            SocialAttribute socialAttribute = await socialAttributeRepository.FindOneAsync(x => x.EmployeeId == employeeId);
            if (socialAttribute == null)
            {
                return new SocialAttributeModel();
            }
            SocialAttributeModel model = ObjectConverter<SocialAttribute, SocialAttributeModel>.Convert(socialAttribute);
            return model;
        }

        public async Task<SocialAttributeModel> SaveSocialAttribute(int employeeId, SocialAttributeModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Social Attribute data missing!");
            }

            SocialAttribute socialAttribute = ObjectConverter<SocialAttributeModel, SocialAttribute>.Convert(model);

            if (socialAttribute.SocialAttributeId > 0)
            {
                socialAttribute = await socialAttributeRepository.FindOneAsync(x => x.EmployeeId == employeeId);
                if (socialAttribute == null)
                {
                    throw new InfinityNotFoundException("Social Attribute data not found!");
                }
                socialAttribute.ModifiedDate = DateTime.Now;
                socialAttribute.ModifiedBy = model.ModifiedBy;



                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "SocialAttribute";
                bnLog.TableEntryForm = "Employee Social Attributes & Hobby";
                bnLog.PreviousValue = "Id: " + model.SocialAttributeId;
                bnLog.UpdatedValue = "Id: " + model.SocialAttributeId;
                int bnoisUpdateCount = 0;
                if (socialAttribute.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", socialAttribute.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                    bnoisUpdateCount += 1;
                }

                if (socialAttribute.IsSocialAttribute != model.IsSocialAttribute)
                {
                    bnLog.PreviousValue += ", Highly Accepted Social Qualities and Happy Family Record: " + (socialAttribute.IsSocialAttribute == true ? "Yes" : "No");
                    bnLog.UpdatedValue += ", Highly Accepted Social Qualities and Happy Family Record: " + (model.IsSocialAttribute == true ? "Yes" : "No");
                    bnoisUpdateCount += 1;
                }
                if (socialAttribute.SARemarks != model.SARemarks)
                {
                    bnLog.PreviousValue += ", Comments: " + socialAttribute.SARemarks;
                    bnLog.UpdatedValue += ", Comments: " + model.SARemarks;
                    bnoisUpdateCount += 1;
                }
                if (socialAttribute.IsCirculationValue != model.IsCirculationValue)
                {
                    bnLog.PreviousValue += ", Good Circulation Value: " + (socialAttribute.IsCirculationValue == true ? "Yes" : "No");
                    bnLog.UpdatedValue += ", Good Circulation Value: " + (model.IsCirculationValue == true ? "Yes" : "No");
                    bnoisUpdateCount += 1;
                }
                if (socialAttribute.CVRemarks != model.CVRemarks)
                {
                    bnLog.PreviousValue += ", Comments: " + socialAttribute.CVRemarks;
                    bnLog.UpdatedValue += ", Comments: " + model.CVRemarks;
                    bnoisUpdateCount += 1;
                }
                if (socialAttribute.IsPersonalityPerChar != model.IsPersonalityPerChar)
                {
                    bnLog.PreviousValue += ", Personality and Personal Character: " + (socialAttribute.IsPersonalityPerChar == true ? "Yes" : "No");
                    bnLog.UpdatedValue += ", Personality and Personal Character: " + (model.IsPersonalityPerChar == true ? "Yes" : "No");
                    bnoisUpdateCount += 1;
                }
                if (socialAttribute.PPCRemarks != model.PPCRemarks)
                {
                    bnLog.PreviousValue += ", Comments: " + socialAttribute.PPCRemarks;
                    bnLog.UpdatedValue += ", Comments: " + model.PPCRemarks;
                    bnoisUpdateCount += 1;
                }
                if (socialAttribute.Hobby != model.Hobby)
                {
                    bnLog.PreviousValue += ", Hobby: " + socialAttribute.Hobby;
                    bnLog.UpdatedValue += ", Hobby: " + model.Hobby;
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
                socialAttribute.EmployeeId = employeeId;
                socialAttribute.CreatedBy = model.CreatedBy;
                socialAttribute.CreatedDate = DateTime.Now;
                socialAttribute.IsActive = true;
            }

            socialAttribute.IsSocialAttribute = model.IsSocialAttribute;
            socialAttribute.SARemarks = model.SARemarks;
            socialAttribute.IsCirculationValue = model.IsCirculationValue;
            socialAttribute.CVRemarks = model.CVRemarks;
            socialAttribute.IsPersonalityPerChar = model.IsPersonalityPerChar;
            socialAttribute.PPCRemarks = model.PPCRemarks;
            socialAttribute.Hobby = model.Hobby;
            await socialAttributeRepository.SaveAsync(socialAttribute);
            model.SocialAttributeId = socialAttribute.SocialAttributeId;
            return model;
        }
    }
}
