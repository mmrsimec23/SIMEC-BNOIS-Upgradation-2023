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
    public class NominationScheduleService : INominationScheduleService
    {
        private readonly IBnoisRepository<NominationSchedule> nominationScheduleRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;

        public NominationScheduleService(IBnoisRepository<NominationSchedule> nominationScheduleRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.nominationScheduleRepository = nominationScheduleRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;

        }

        public List<NominationScheduleModel> GetNominationSchedules(int ps, int pn,string qs, int type, out int total)
        {
            IQueryable<NominationSchedule> nominationSchedules = nominationScheduleRepository.FilterWithInclude(x => x.IsActive && x.NominationScheduleType==type
                && (x.TitleName.Contains(qs) || x.Purpose.Contains(qs) || x.VisitCategory.Name.Contains(qs) || x.VisitSubCategory.Name.Contains(qs) ||
                (x.Country.FullName.Contains(qs) || String.IsNullOrEmpty(qs)) ), "Country","VisitCategory","VisitSubCategory");
            total = nominationSchedules.Count();
	        nominationSchedules = nominationSchedules.OrderByDescending(x => x.NominationScheduleId).Skip((pn - 1) * ps).Take(ps);
            List<NominationScheduleModel> models = ObjectConverter<NominationSchedule, NominationScheduleModel>.ConvertList(nominationSchedules.ToList()).ToList();
            return models;
        }

        public async Task<NominationScheduleModel> GetNominationSchedule(int id)
        {
            if (id <= 0)
            {
                return new NominationScheduleModel();
            }
            NominationSchedule nominationSchedule = await nominationScheduleRepository.FindOneAsync(x => x.NominationScheduleId == id);
            if (nominationSchedule == null)
            {
                throw new InfinityNotFoundException("Nomination Schedule not found");
            }
            NominationScheduleModel model = ObjectConverter<NominationSchedule, NominationScheduleModel>.Convert(nominationSchedule);
            return model;
        }

        public async Task<NominationScheduleModel> SaveNominationSchedule(int id, NominationScheduleModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Nomination Schedule  data missing");
            }
            bool isExistData;


            switch (model.NominationScheduleType)
            {
                //case 1:
                //    isExistData = nominationScheduleRepository.Exists(x => x.CountryId == model.CountryId && x.TitleName == model.TitleName && x.FromDate == model.FromDate && x.ToDate == model.ToDate && x.NominationScheduleId != id);
                //    if (isExistData)
                //    {
                //        throw new InfinityInvalidDataException("Data already exists !");
                //    }
                //    break;
                case 2:
                    isExistData = nominationScheduleRepository.Exists(x => x.CountryId == model.CountryId  && x.FromDate == model.FromDate && x.ToDate == model.ToDate && x.VisitSubCategoryId == model.VisitSubCategoryId && x.NominationScheduleId != id);
                    if (isExistData)
                    {
                        throw new InfinityInvalidDataException("Data already exists !");
                    }
                    break;
                case 3:
                    isExistData = nominationScheduleRepository.Exists(x => x.FromDate == model.FromDate && x.ToDate == model.ToDate && x.Purpose.Trim() == model.Purpose.Trim() && x.NominationScheduleId != id);
                    if (isExistData)
                    {
                        throw new InfinityInvalidDataException("Data already exists !");
                    }
                    break;
            }
            //if (model.NominationScheduleType != 3)
            //{
               
            //    bool isExistData = nominationScheduleRepository.Exists(x =>x.CountryId == model.CountryId  && x.TitleName == model.TitleName && x.FromDate == model.FromDate  && x.NominationScheduleId != id);
            //    if (isExistData)
            //    {
            //        throw new InfinityInvalidDataException("Data already exists !");
            //    }
            //}
           

            if (model.FromDate>model.ToDate)
            {
                throw new InfinityInvalidDataException("From Date is greater than To Date.!");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            NominationSchedule NominationSchedule = ObjectConverter<NominationScheduleModel, NominationSchedule>.Convert(model);
            if (id > 0)
            {
                NominationSchedule = await nominationScheduleRepository.FindOneAsync(x => x.NominationScheduleId == id);
                if (NominationSchedule == null)
                {
                    throw new InfinityNotFoundException("Nomination Schedule not found !");
                }
  

                NominationSchedule.ModifiedDate = DateTime.Now;
                NominationSchedule.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "NominationSchedule";
                bnLog.TableEntryForm = "Nomination Schedule";
                bnLog.PreviousValue = "Id: " + model.NominationScheduleId;
                bnLog.UpdatedValue = "Id: " + model.NominationScheduleId;
                int bnoisUpdateCount = 0;


                bnLog.PreviousValue += ", Nomination Schedule Type: " + (NominationSchedule.NominationScheduleType == 1 ? "UN Mission" : NominationSchedule.NominationScheduleType == 2 ? "Foreign Visit" : NominationSchedule.NominationScheduleType == 3 ? "Others" : "");
                bnLog.UpdatedValue += ", Nomination Schedule Type: " + (model.NominationScheduleType == 1 ? "UN Mission" : model.NominationScheduleType == 2 ? "Foreign Visit" : model.NominationScheduleType == 3 ? "Others" : "");
                    
                
                if (NominationSchedule.VisitCategoryId != model.VisitCategoryId)
                {
                    if (NominationSchedule.VisitCategoryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("VisitCategory", "VisitCategoryId", NominationSchedule.VisitCategoryId ?? 0);
                        bnLog.PreviousValue += ", Visit Category: " + ((dynamic)prev).Name;
                    }
                    if (model.VisitCategoryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("VisitCategory", "VisitCategoryId", model.VisitCategoryId ?? 0);
                        bnLog.UpdatedValue += ", Visit Category: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (NominationSchedule.VisitSubCategoryId != model.VisitSubCategoryId)
                {
                    if (NominationSchedule.VisitSubCategoryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("VisitSubCategory", "VisitSubCategoryId", NominationSchedule.VisitSubCategoryId ?? 0);
                        bnLog.PreviousValue += ", Visit Sub Category: " + ((dynamic)prev).Name;
                    }
                    if (model.VisitSubCategoryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("VisitSubCategory", "VisitSubCategoryId", model.VisitSubCategoryId ?? 0);
                        bnLog.UpdatedValue += ", Visit Sub Category: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (NominationSchedule.CountryId != model.CountryId)
                {
                    if (NominationSchedule.CountryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", NominationSchedule.CountryId ?? 0);
                        bnLog.PreviousValue += ", Country: " + ((dynamic)prev).FullName;
                    }
                    if (model.CountryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Country", "CountryId", model.CountryId ?? 0);
                        bnLog.UpdatedValue += ", Country: " + ((dynamic)newv).FullName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (NominationSchedule.TitleName != model.TitleName)
                {
                    bnLog.PreviousValue += ", Title Name: " + NominationSchedule.TitleName;
                    bnLog.UpdatedValue += ", Title Name: " + model.TitleName;
                    bnoisUpdateCount += 1;
                }
                if (NominationSchedule.Purpose != model.Purpose)
                {
                    bnLog.PreviousValue += ",  Purpose: " + NominationSchedule.Purpose;
                    bnLog.UpdatedValue += ",  Purpose: " + model.Purpose;
                    bnoisUpdateCount += 1;
                }
                if (NominationSchedule.Location != model.Location)
                {
                    bnLog.PreviousValue += ",  Location: " + NominationSchedule.Location;
                    bnLog.UpdatedValue += ",  Location: " + model.Location;
                    bnoisUpdateCount += 1;
                }
                if (NominationSchedule.FromDate != model.FromDate)
                {
                    bnLog.PreviousValue += ", From Date: " + NominationSchedule.FromDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", From Date: " + model.FromDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (NominationSchedule.ToDate != model.ToDate)
                {
                    bnLog.PreviousValue += ", To Date: " + NominationSchedule.ToDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", To Date: " + model.ToDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (NominationSchedule.NumberOfPost != model.NumberOfPost)
                {
                    bnLog.PreviousValue += ", Number Of Post: " + NominationSchedule.NumberOfPost;
                    bnLog.UpdatedValue += ", Number Of Post: " + model.NumberOfPost;
                    bnoisUpdateCount += 1;
                }
                if (NominationSchedule.AssignPost != model.AssignPost)
                {
                    bnLog.PreviousValue += ",  Assign Post: " + NominationSchedule.AssignPost;
                    bnLog.UpdatedValue += ",  Assign Post: " + model.AssignPost;
                    bnoisUpdateCount += 1;
                }
                if (NominationSchedule.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ",  Remarks: " + NominationSchedule.Remarks;
                    bnLog.UpdatedValue += ",  Remarks: " + model.Remarks;
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
                NominationSchedule.IsActive = true;
                NominationSchedule.CreatedDate = DateTime.Now;
                NominationSchedule.CreatedBy = userId;
            
            }
            if (model.NominationScheduleType == 2)
            {
                NominationSchedule.VisitCategoryId = model.VisitCategoryId;
                NominationSchedule.VisitSubCategoryId = model.VisitSubCategoryId;
            }
            
            NominationSchedule.CountryId = model.CountryId;
            NominationSchedule.NominationScheduleType = model.NominationScheduleType;
            NominationSchedule.TitleName = model.TitleName;
            NominationSchedule.FromDate = model.FromDate;
            NominationSchedule.ToDate = model.ToDate;
            NominationSchedule.Purpose = model.Purpose;
            NominationSchedule.Location = model.Location;
            NominationSchedule.Remarks = model.Remarks;
            NominationSchedule.NumberOfPost = model.NumberOfPost;
            NominationSchedule.AssignPost = model.AssignPost;
            try
            {
                await nominationScheduleRepository.SaveAsync(NominationSchedule);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            model.NominationScheduleId = NominationSchedule.NominationScheduleId;
            return model;
        }


        public async Task<bool> DeleteNominationSchedule(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            NominationSchedule nominationSchedule = await nominationScheduleRepository.FindOneAsync(x => x.NominationScheduleId == id);
            if (nominationSchedule == null)
            {
                throw new InfinityNotFoundException("Nomination Schedule not found");
            }
            else
            {

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "NominationSchedule";
                bnLog.TableEntryForm = "Nomination Schedule";
                bnLog.PreviousValue = "Id: " + nominationSchedule.NominationScheduleId;
                
                bnLog.PreviousValue += ", Nomination Schedule Type: " + (nominationSchedule.NominationScheduleType == 1 ? "UN Mission" : nominationSchedule.NominationScheduleType == 2 ? "Foreign Visit" : nominationSchedule.NominationScheduleType == 3 ? "Others" : "");
                
                if (nominationSchedule.VisitCategoryId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("VisitCategory", "VisitCategoryId", nominationSchedule.VisitCategoryId ?? 0);
                    bnLog.PreviousValue += ", Visit Category: " + ((dynamic)prev).Name;
                }
                if (nominationSchedule.VisitSubCategoryId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("VisitSubCategory", "VisitSubCategoryId", nominationSchedule.VisitSubCategoryId ?? 0);
                    bnLog.PreviousValue += ", Visit Sub Category: " + ((dynamic)prev).Name;
                }
                if (nominationSchedule.CountryId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", nominationSchedule.CountryId ?? 0);
                    bnLog.PreviousValue += ", Country: " + ((dynamic)prev).FullName;
                }
                bnLog.PreviousValue += ", Title Name: " + nominationSchedule.TitleName + ",  Purpose: " + nominationSchedule.Purpose + ",  Location: " + nominationSchedule.Location + ", From Date: " + nominationSchedule.FromDate?.ToString("dd/MM/yyyy") + ", To Date: " + nominationSchedule.ToDate?.ToString("dd/MM/yyyy") + ", Number Of Post: " + nominationSchedule.NumberOfPost + ",  Assign Post: " + nominationSchedule.AssignPost + ",  Remarks: " + nominationSchedule.Remarks;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await nominationScheduleRepository.DeleteAsync(nominationSchedule);
            }
        }

        public List<SelectModel> GetNominationScheduleTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(NominationScheduleType)).Cast<NominationScheduleType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }

        public async Task<List<SelectModel>> GetMissionNominationScheduleSelectModels()
        {
            ICollection<NominationSchedule> models = await nominationScheduleRepository.FilterWithInclude(x => x.IsActive && x.NominationScheduleType == 1 , "Country").ToListAsync();
            return models.OrderByDescending(x => x.FromDate).Select(x => new SelectModel() 
            {
                Text = String.Format("{0},{1},[{2} To{3}]", x.TitleName, x.Country.FullName, String.Format("{0:dd-MM-yyyy}", x.FromDate), String.Format("{0:dd-MM-yyyy}", x.ToDate)),
                Value = x.NominationScheduleId
            }).ToList();
        }


        public async Task<List<SelectModel>> GetForeignVisitNominationScheduleSelectModels()
        {
            ICollection<NominationSchedule> models = await nominationScheduleRepository.FilterWithInclude(x => x.IsActive && x.NominationScheduleType==2 , "Country","VisitCategory","VisitSubCategory").ToListAsync();
            return models.OrderByDescending(x => x.FromDate).Select(x => new SelectModel()
            {
                Text = String.Format("{0}-{1}, {2} [{3} To {4}]", x.VisitCategory.Name,x.VisitSubCategory.Name, x.Country.FullName, String.Format("{0:dd-MM-yyyy}", x.FromDate), String.Format("{0:dd-MM-yyyy}", x.ToDate)),
                Value = x.NominationScheduleId
            }).ToList();
        }

        public async Task<List<SelectModel>> GetOtherNominationScheduleSelectModels()
        {
            ICollection<NominationSchedule> models = await nominationScheduleRepository.FilterWithInclude(x => x.IsActive && x.NominationScheduleType == 3).ToListAsync();
            return models.OrderByDescending(x => x.FromDate).Select(x => new SelectModel()
            {
                Text = String.Format("{0} [{1} To {2}] ",  x.Purpose, String.Format("{0:dd-MM-yyyy}", x.FromDate), String.Format("{0:dd-MM-yyyy}", x.ToDate)),
                Value = x.NominationScheduleId
            }).ToList();
        }
    }
}
