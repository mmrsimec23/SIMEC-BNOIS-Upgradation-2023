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
    public class OfficeAppointmentService : IOfficeAppointmentService
    {
        private readonly IBnoisRepository<OfficeAppointment> officeAppointmentRepository;
        private readonly IBnoisRepository<OfficeAppRank> officeAppRankRepository;
        private readonly IBnoisRepository<OfficeAppBranch> officeAppBranchRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public OfficeAppointmentService(IBnoisRepository<OfficeAppRank> officeAppRankRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService, IBnoisRepository<OfficeAppBranch> officeAppBranchRepository, IBnoisRepository<OfficeAppointment> officeAppointmentRepository)
        {
            this.officeAppointmentRepository = officeAppointmentRepository;
            this.officeAppRankRepository = officeAppRankRepository;
            this.officeAppBranchRepository = officeAppBranchRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<OfficeAppointmentModel> GetOfficeAppointments(int ps, int pn, string qs, int officeId,int type, out int total)
        {
            IQueryable<OfficeAppointment> officeAppointments = officeAppointmentRepository.FilterWithInclude(x => x.IsActive && x.OfficeId==officeId && x.AppointmentType==type
                && ((x.Office.ShortName.Contains(qs) || String.IsNullOrEmpty(qs)) || (x.AptNat.ANatShnm.Contains(qs) || String.IsNullOrEmpty(qs)) ||
                (x.AptCat.ACatShNm.Contains(qs) || String.IsNullOrEmpty(qs))), "Office", "AptNat", "AptCat");
            total = officeAppointments.Count();
            officeAppointments = officeAppointments.OrderBy(x => x.AptNat.ANatNm).Skip((pn - 1) * ps).Take(ps);
            List<OfficeAppointmentModel> models = ObjectConverter<OfficeAppointment, OfficeAppointmentModel>.ConvertList(officeAppointments.ToList()).ToList();
            return models;
        }


        public async Task<List<SelectModel>> GetOfficeAppointmentsByOfficeId(int officeId)
        {
            ICollection<OfficeAppointment> officeAppointments = await officeAppointmentRepository.FilterAsync(x => x.IsActive && x.OfficeId == officeId && x.AppointmentType==1);
            List<SelectModel> selectModels = officeAppointments.OrderBy(x => x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.OffAppId
            }).ToList();
            return selectModels;
        }

        public async Task<List<SelectModel>> GetOfficeAppointmentSelectModels()
        {
            ICollection<OfficeAppointment> officeAppointments = await officeAppointmentRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = officeAppointments.OrderBy(x=>x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.OffAppId
            }).ToList();
            return selectModels;
        }


        public async Task<List<SelectModel>> GetOfficeAdditionalAppointmentsByOfficeId(int officeId)
        {
            ICollection<OfficeAppointment> officeAppointments = await officeAppointmentRepository.FilterAsync(x => x.IsActive && x.OfficeId == officeId && x.AppointmentType == 2);
            List<SelectModel> selectModels = officeAppointments.OrderBy(x => x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.OffAppId
            }).ToList();
            return selectModels;
        }
        public async Task<List<SelectModel>> GetAppointmentByShipType(int shipType)
        {
            ICollection<OfficeAppointment> officeAppointments = await officeAppointmentRepository.FilterAsync(x => x.IsActive && x.Office.ShipType == shipType);
            List<SelectModel> selectModels = officeAppointments.OrderBy(x => x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.OffAppId
            }).ToList();
            return selectModels;
        }

        public async Task<List<SelectModel>> GetAppointmentByOrganizationPattern(int officeId)
        {
            ICollection<OfficeAppointment> officeAppointments = await officeAppointmentRepository.FilterAsync(x => x.IsActive && x.Office.ParentId == officeId);
            List<SelectModel> selectModels = officeAppointments.OrderBy(x => x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.OffAppId
            }).ToList();
            return selectModels;
        }


        public async Task<OfficeAppointmentModel> GetOfficeAppointment(int id)
        {
            if (id <= 0)
            {
                return new OfficeAppointmentModel();
            }
            OfficeAppointment officeAppointment = await officeAppointmentRepository.FindOneAsync(x => x.OffAppId == id);

            if (officeAppointment == null)
            {
                throw new InfinityNotFoundException("Office Appointment not found");
            }
            int[] rankIds=  officeAppRankRepository.Where(x => x.OffAppId == id).Select(x => x.RankId).ToArray();
            int[] branchIds = officeAppBranchRepository.Where(x => x.OffAppId == id).Select(x => x.BranchId).ToArray();
            OfficeAppointmentModel model = ObjectConverter<OfficeAppointment, OfficeAppointmentModel>.Convert(officeAppointment);
            model.RankIds = rankIds;
            model.BranchIds = branchIds;
            return model;
        }

        public async Task<OfficeAppointmentModel> SaveOfficeAppointment(int id, OfficeAppointmentModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Office Appointment  data missing");
            }


            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            OfficeAppointment officeAppointment = ObjectConverter<OfficeAppointmentModel, OfficeAppointment>.Convert(model);
            if (id > 0)
            {
                officeAppointment = await officeAppointmentRepository.FindOneAsync(x => x.OffAppId == id);
                if (officeAppointment == null)
                {
                    throw new InfinityNotFoundException("Office Appointment not found !");
                }
                ICollection<OfficeAppRank> trainingRanks = await officeAppRankRepository.FilterAsync(x => x.OffAppId == id);
                officeAppRankRepository.RemoveRange(trainingRanks);

                ICollection<OfficeAppBranch> trainingBranches = await officeAppBranchRepository.FilterAsync(x => x.OffAppId == id);
                officeAppBranchRepository.RemoveRange(trainingBranches);

                officeAppointment.ModifiedDate = DateTime.Now;
                officeAppointment.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "OfficeAppointment";
                bnLog.TableEntryForm = "Office Appointment";
                bnLog.PreviousValue = ", Id: " + model.OffAppId;
                bnLog.UpdatedValue = ", Id: " + model.OffAppId;
                int bnoisUpdateCount = 0;


                if (officeAppointment.OfficeId != model.OfficeId)
                {
                    if (officeAppointment.OfficeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", officeAppointment.OfficeId);
                        bnLog.PreviousValue += ", Office: " + ((dynamic)prev).ShortName;
                    }
                    if (model.OfficeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Office", "OfficeId", model.OfficeId);
                        bnLog.UpdatedValue += ", Office: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.AptNatId != model.AptNatId)
                {
                    if (officeAppointment.AptNatId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("AptNat", "AptNatId", officeAppointment.AptNatId);
                        bnLog.PreviousValue += ", Appointment Nature: " + ((dynamic)prev).ANatNm;
                    }
                    if (model.AptNatId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("AptNat", "AptNatId", model.AptNatId);
                        bnLog.UpdatedValue += ", Appointment Nature: " + ((dynamic)newv).ANatNm;
                    }
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.AptCatId != model.AptCatId)
                {
                    if (officeAppointment.AptCatId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("AptCat", "AptCatId", officeAppointment.AptCatId);
                        bnLog.PreviousValue += ", Appointment Category: " + ((dynamic)prev).AcatNm;
                    }
                    if (model.AptCatId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("AptCat", "AptCatId", model.AptCatId);
                        bnLog.UpdatedValue += ", Appointment Category: " + ((dynamic)newv).AcatNm;
                    }
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.AppointmentType != model.AppointmentType)
                {
                    bnLog.PreviousValue += ", Appointment Type: " + (officeAppointment.AppointmentType==1?"Permanent": officeAppointment.AppointmentType ==2?"Additional":"");
                    bnLog.UpdatedValue += ", Appointment Type: " + (model.AppointmentType == 1 ? "Permanent" : model.AppointmentType == 2 ? "Additional" : "");
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.Name != model.Name)
                {
                    bnLog.PreviousValue += ",  Name: " + officeAppointment.Name;
                    bnLog.UpdatedValue += ",  Name: " + model.Name;
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ",  Short Name: " + officeAppointment.ShortName;
                    bnLog.UpdatedValue += ",  Short Name: " + model.ShortName;
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.NameBangla != model.NameBangla)
                {
                    bnLog.PreviousValue += ", Name (বাংলা): " + officeAppointment.NameBangla;
                    bnLog.UpdatedValue += ", Name (বাংলা): " + model.NameBangla;
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.ShortNameBangla != model.ShortNameBangla)
                {
                    bnLog.PreviousValue += ", Short Name (বাংলা): " + officeAppointment.ShortNameBangla;
                    bnLog.UpdatedValue += ", Short Name (বাংলা): " + model.ShortNameBangla;
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.GovtApproved != model.GovtApproved)
                {
                    bnLog.PreviousValue += ",  Govt. Approved: " + officeAppointment.GovtApproved;
                    bnLog.UpdatedValue += ",  Govt. Approved: " + model.GovtApproved;
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.HeadofDpt != model.HeadofDpt)
                {
                    bnLog.PreviousValue += ", HOD: " + officeAppointment.HeadofDpt;
                    bnLog.UpdatedValue += ", HOD: " + model.HeadofDpt;
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.OfficeHead != model.OfficeHead)
                {
                    bnLog.PreviousValue += ", Office Head: " + officeAppointment.OfficeHead;
                    bnLog.UpdatedValue += ", Office Head: " + model.OfficeHead;
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.IsInstrServiceCount != model.IsInstrServiceCount)
                {
                    bnLog.PreviousValue += ", Instructor Service: " + officeAppointment.IsInstrServiceCount;
                    bnLog.UpdatedValue += ", Instructor Service: " + model.IsInstrServiceCount;
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.OrgCd != model.OrgCd)
                {
                    bnLog.PreviousValue += ", Additional for Training: " + officeAppointment.OrgCd;
                    bnLog.UpdatedValue += ", Additional for Training: " + model.OrgCd;
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
                officeAppointment.IsActive = true;
                officeAppointment.CreatedDate = DateTime.Now;
                officeAppointment.CreatedBy = userId;

            }
            if (model.RankIds.Any())
            {
                officeAppointment.OfficeAppRank = model.RankIds.Select(x => new OfficeAppRank() { RankId = x }).ToList();
            }
            if (model.BranchIds.Any())
            {
                officeAppointment.OfficeAppBranch = model.BranchIds.Select(x => new OfficeAppBranch() { BranchId = x }).ToList();
            }
            officeAppointment.OfficeId = model.OfficeId;
            officeAppointment.AptCatId = model.AptCatId;
            officeAppointment.AptNatId = model.AptNatId;
            officeAppointment.Name = model.Name;
            officeAppointment.ShortName = model.ShortName;
            officeAppointment.NameBangla = model.NameBangla;
            officeAppointment.ShortNameBangla = model.ShortNameBangla;           
            officeAppointment.GovtApproved = model.GovtApproved;
            officeAppointment.HeadofDpt = model.HeadofDpt;
            officeAppointment.OfficeHead = model.OfficeHead;
            officeAppointment.IsInstrServiceCount = model.IsInstrServiceCount;
            officeAppointment.ParentOffAppId = model.ParentOffAppId;
            officeAppointment.AppointmentType = 1;
            officeAppointment.OrgCd = model.OrgCd;
            await officeAppointmentRepository.SaveAsync(officeAppointment);
            model.OffAppId = officeAppointment.OffAppId;
            return model;
        }


        public async Task<OfficeAppointmentModel> SaveOfficeAdditionalAppointment(int id, OfficeAppointmentModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Office Additional Appointment  data missing");
            }


            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            OfficeAppointment officeAppointment = ObjectConverter<OfficeAppointmentModel, OfficeAppointment>.Convert(model);
            if (id > 0)
            {
                officeAppointment = await officeAppointmentRepository.FindOneAsync(x => x.OffAppId == id);
                if (officeAppointment == null)
                {
                    throw new InfinityNotFoundException("Office Additional Appointment not found !");
                }

                officeAppointment.ModifiedDate = DateTime.Now;
                officeAppointment.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "OfficeAppointment";
                bnLog.TableEntryForm = "Office Appointment";
                bnLog.PreviousValue = ", Id: " + model.OffAppId;
                bnLog.UpdatedValue = ", Id: " + model.OffAppId;
                int bnoisUpdateCount = 0;


                if (officeAppointment.OfficeId != model.OfficeId)
                {
                    if (officeAppointment.OfficeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", officeAppointment.OfficeId);
                        bnLog.PreviousValue += ", Office: " + ((dynamic)prev).ShortName;
                    }
                    if (model.OfficeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Office", "OfficeId", model.OfficeId);
                        bnLog.UpdatedValue += ", Office: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.AptNatId != model.AptNatId)
                {
                    if (officeAppointment.AptNatId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("AptNat", "AptNatId", officeAppointment.AptNatId);
                        bnLog.PreviousValue += ", Appointment Nature: " + ((dynamic)prev).ANatNm;
                    }
                    if (model.AptNatId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("AptNat", "AptNatId", model.AptNatId);
                        bnLog.UpdatedValue += ", Appointment Nature: " + ((dynamic)newv).ANatNm;
                    }
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.AptCatId != model.AptCatId)
                {
                    if (officeAppointment.AptCatId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("AptCat", "AptCatId", officeAppointment.AptCatId);
                        bnLog.PreviousValue += ", Appointment Category: " + ((dynamic)prev).AcatNm;
                    }
                    if (model.AptCatId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("AptCat", "AptCatId", model.AptCatId);
                        bnLog.UpdatedValue += ", Appointment Category: " + ((dynamic)newv).AcatNm;
                    }
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.AppointmentType != model.AppointmentType)
                {
                    bnLog.PreviousValue += ", Appointment Type: " + (officeAppointment.AppointmentType == 1 ? "Permanent" : officeAppointment.AppointmentType == 2 ? "Additional" : "");
                    bnLog.UpdatedValue += ", Appointment Type: " + (model.AppointmentType == 1 ? "Permanent" : model.AppointmentType == 2 ? "Additional" : "");
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.Name != model.Name)
                {
                    bnLog.PreviousValue += ",  Name: " + officeAppointment.Name;
                    bnLog.UpdatedValue += ",  Name: " + model.Name;
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ",  Short Name: " + officeAppointment.ShortName;
                    bnLog.UpdatedValue += ",  Short Name: " + model.ShortName;
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.NameBangla != model.NameBangla)
                {
                    bnLog.PreviousValue += ", Name (বাংলা): " + officeAppointment.NameBangla;
                    bnLog.UpdatedValue += ", Name (বাংলা): " + model.NameBangla;
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.ShortNameBangla != model.ShortNameBangla)
                {
                    bnLog.PreviousValue += ", Short Name (বাংলা): " + officeAppointment.ShortNameBangla;
                    bnLog.UpdatedValue += ", Short Name (বাংলা): " + model.ShortNameBangla;
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.GovtApproved != model.GovtApproved)
                {
                    bnLog.PreviousValue += ",  Govt. Approved: " + officeAppointment.GovtApproved;
                    bnLog.UpdatedValue += ",  Govt. Approved: " + model.GovtApproved;
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.HeadofDpt != model.HeadofDpt)
                {
                    bnLog.PreviousValue += ", HOD: " + officeAppointment.HeadofDpt;
                    bnLog.UpdatedValue += ", HOD: " + model.HeadofDpt;
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.OfficeHead != model.OfficeHead)
                {
                    bnLog.PreviousValue += ", Office Head: " + officeAppointment.OfficeHead;
                    bnLog.UpdatedValue += ", Office Head: " + model.OfficeHead;
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.IsInstrServiceCount != model.IsInstrServiceCount)
                {
                    bnLog.PreviousValue += ", Instructor Service: " + officeAppointment.IsInstrServiceCount;
                    bnLog.UpdatedValue += ", Instructor Service: " + model.IsInstrServiceCount;
                    bnoisUpdateCount += 1;
                }
                if (officeAppointment.OrgCd != model.OrgCd)
                {
                    bnLog.PreviousValue += ", Additional for Training: " + officeAppointment.OrgCd;
                    bnLog.UpdatedValue += ", Additional for Training: " + model.OrgCd;
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
                officeAppointment.IsActive = true;
                officeAppointment.CreatedDate = DateTime.Now;
                officeAppointment.CreatedBy = userId;

            }
           
            officeAppointment.OfficeId = model.OfficeId;
            officeAppointment.AptCatId = model.AptCatId;
            officeAppointment.AptNatId = model.AptNatId;
            officeAppointment.Name = model.Name;
            officeAppointment.ShortName = model.ShortName;
			officeAppointment.IsAdditional= model.IsAdditional;
			officeAppointment.AppointmentType = 2;
            await officeAppointmentRepository.SaveAsync(officeAppointment);
            model.OffAppId = officeAppointment.OffAppId;
            return model;
        }


        public async Task<bool> DeleteOfficeAppointment(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            OfficeAppointment officeAppointment = await officeAppointmentRepository.FindOneAsync(x => x.OffAppId == id);
            if (officeAppointment == null)
            {
                throw new InfinityNotFoundException("Office Appointment not found");
            }
            else
            {
                officeAppRankRepository.Delete(x => x.OffAppId == officeAppointment.OffAppId);
                officeAppBranchRepository.Delete(x => x.OffAppId == officeAppointment.OffAppId);

                return await officeAppointmentRepository.DeleteAsync(officeAppointment);
            } 
        }


        public async Task<List<SelectModel>> GetParentAppointmentSelectModels(int officeId,int OffAppId)
        {
            ICollection<OfficeAppointment> officeAppointments = await officeAppointmentRepository.FilterAsync(x => x.IsActive && x.OfficeId == officeId && x.AppointmentType==1 && x.OffAppId != OffAppId);
            List<SelectModel> selectModels = officeAppointments.OrderBy(x=>x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.OffAppId
            }).ToList();
            return selectModels;
        }
       
    }
}
