using Infinity.Bnois.ApplicationService.Implementation;
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
using Office = Infinity.Bnois.Data.Office;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class OfficeService : IOfficeService
    {
        private readonly IBnoisRepository<Office> officeRepository;
        private readonly IOfficeShipmentService officeShipmentService;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;


        public OfficeService(IBnoisRepository<Office> officeRepository, IOfficeShipmentService officeShipmentService, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.officeRepository = officeRepository;
            this.officeShipmentService = officeShipmentService;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }


        public async Task<OfficeModel> GetOffice(int id)
        {
            if (id <= 0)
            {
                return new OfficeModel();
            }
            Office office = await officeRepository.FindOneAsync(x => x.OfficeId == id);
            if (office == null)
            {
                throw new InfinityNotFoundException("Office not found");
            }
            OfficeModel model = ObjectConverter<Office, OfficeModel>.Convert(office);
            return model;
        }


        public async Task<OfficeModel> SaveOffice(int id, OfficeModel model)
        {
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Office data missing");
            }
            Office office = ObjectConverter<OfficeModel, Office>.Convert(model);
            if (id > 0)
            {
                office = await officeRepository.FindOneAsync(x => x.OfficeId == id);
                if (office == null)
                {
                    throw new InfinityNotFoundException("Office not found !");
                }

                office.ModifiedDate = DateTime.Now;
                office.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Office";
                bnLog.TableEntryForm = "Office";
                bnLog.PreviousValue = ", Id: " + model.OfficeId;
                bnLog.UpdatedValue = ", Id: " + model.OfficeId;
                int bnoisUpdateCount = 0;
                
                if (office.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Office Name: " + office.Name;
                    bnLog.UpdatedValue += ", Office Name: " + model.Name;
                    bnoisUpdateCount += 1;
                }
                if (office.NameBangla != model.NameBangla)
                {
                    bnLog.PreviousValue += ", Office Name (বাংলা): " + office.NameBangla;
                    bnLog.UpdatedValue += ", Office Name (বাংলা): " + model.NameBangla;
                    bnoisUpdateCount += 1;
                }
                if (office.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ", Short Name: " + office.ShortName;
                    bnLog.UpdatedValue += ", Short Name: " + model.ShortName;
                    bnoisUpdateCount += 1;
                }
                if (office.ShortNameBangla != model.ShortNameBangla)
                {
                    bnLog.PreviousValue += ", Short Name (বাংলা): " + office.ShortNameBangla;
                    bnLog.UpdatedValue += ", Short Name (বাংলা): " + model.ShortNameBangla;
                    bnoisUpdateCount += 1;
                }
                if (office.SecurityName != model.SecurityName)
                {
                    bnLog.PreviousValue += ", Security Name: " + office.SecurityName;
                    bnLog.UpdatedValue += ", Security Name: " + model.SecurityName;
                    bnoisUpdateCount += 1;
                }
                if (office.PatternId != model.PatternId)
                {
                    if (office.PatternId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Pattern", "PatternId", office.PatternId ?? 0);
                        bnLog.PreviousValue += ", Pattern: " + ((dynamic)prev).Name;
                    }
                    if (model.PatternId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Pattern", "PatternId", model.PatternId ?? 0);
                        bnLog.UpdatedValue += ", Pattern: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (office.ShipCategoryId != model.ShipCategoryId)
                {
                    if (office.ShipCategoryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("ShipCategory", "ShipCategoryId", office.ShipCategoryId ?? 0);
                        bnLog.PreviousValue += ", Ship Category: " + ((dynamic)prev).Name;
                    }
                    if (model.ShipCategoryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("ShipCategory", "ShipCategoryId", model.ShipCategoryId ?? 0);
                        bnLog.UpdatedValue += ", Ship Category: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (office.ShipType != model.ShipType)
                {
                    bnLog.PreviousValue += ", Ship Type: " + (office.ShipType == 1 ? "Small" : office.ShipType == 2 ? "Medium" : office.ShipType == 3 ? "Large" : "");
                    bnLog.UpdatedValue += ", Ship Type: " + (model.ShipType == 1 ? "Small" : model.ShipType == 2 ? "Medium" : model.ShipType == 3 ? "Large" : "");
                    bnoisUpdateCount += 1;
                }
                if (office.CountryId != model.CountryId)
                {
                    if (office.CountryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", office.CountryId ?? 0);
                        bnLog.PreviousValue += ", Country: " + ((dynamic)prev).FullName;
                    }
                    if (model.CountryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Country", "CountryId", model.CountryId ?? 0);
                        bnLog.UpdatedValue += ", Country: " + ((dynamic)newv).FullName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (office.ShipOriginId != model.ShipOriginId)
                {
                    if (office.ShipOriginId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", office.ShipOriginId ?? 0);
                        bnLog.PreviousValue += ", Ship Origin: " + ((dynamic)prev).FullName;
                    }
                    if (model.CountryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Country", "CountryId", model.ShipOriginId ?? 0);
                        bnLog.UpdatedValue += ", Ship Origin: " + ((dynamic)newv).FullName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (office.AddressInfo != model.AddressInfo)
                {
                    bnLog.PreviousValue += ", Office Address: " + office.AddressInfo;
                    bnLog.UpdatedValue += ", Office Address: " + model.AddressInfo;
                    bnoisUpdateCount += 1;
                }

                if (office.BornOfficeId != model.BornOfficeId)
                {
                    if (office.BornOfficeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", office.BornOfficeId ?? 0);
                        bnLog.PreviousValue += ", Ship Origin: " + ((dynamic)prev).ShortName;
                    }
                    if (model.BornOfficeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Office", "OfficeId", model.BornOfficeId ?? 0);
                        bnLog.UpdatedValue += ", Ship Origin: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (office.BornOffice != model.BornOffice)
                {
                    bnLog.PreviousValue += ", Born Office: " + office.BornOffice;
                    bnLog.UpdatedValue += ", Born Office: " + model.BornOffice;
                    bnoisUpdateCount += 1;
                }
                if (office.Establishment != model.Establishment)
                {
                    bnLog.PreviousValue += ", Establishment: " + office.Establishment;
                    bnLog.UpdatedValue += ",  Establishment: " + model.Establishment;
                    bnoisUpdateCount += 1;
                }
                if (office.GovtApproved != model.GovtApproved)
                {
                    bnLog.PreviousValue += ", Govt Approved: " + office.GovtApproved;
                    bnLog.UpdatedValue += ",  Govt Approved: " + model.GovtApproved;
                    bnoisUpdateCount += 1;
                }
                if (office.CoastGuard != model.CoastGuard)
                {
                    bnLog.PreviousValue += ", Coast Guard: " + office.CoastGuard;
                    bnLog.UpdatedValue += ",  Coast Guard: " + model.CoastGuard;
                    bnoisUpdateCount += 1;
                }
                if (office.IsOutsideOrg != model.IsOutsideOrg)
                {
                    bnLog.PreviousValue += ", Outside Org: " + office.IsOutsideOrg;
                    bnLog.UpdatedValue += ",  Outside Org: " + model.IsOutsideOrg;
                    bnoisUpdateCount += 1;
                }
                if (office.IndCmd != model.IndCmd)
                {
                    bnLog.PreviousValue += ", Independent Command: " + office.IndCmd;
                    bnLog.UpdatedValue += ",  Independent Command: " + model.IndCmd;
                    bnoisUpdateCount += 1;
                }
                if (office.SeaServiceCnt != model.SeaServiceCnt)
                {
                    bnLog.PreviousValue += ", Sea Service: " + office.SeaServiceCnt;
                    bnLog.UpdatedValue += ",  Sea Service: " + model.SeaServiceCnt;
                    bnoisUpdateCount += 1;
                }
                if (office.IsDocyardCount != model.IsDocyardCount)
                {
                    bnLog.PreviousValue += ", Dockyard: " + office.IsDocyardCount;
                    bnLog.UpdatedValue += ",  Dockyard: " + model.IsDocyardCount;
                    bnoisUpdateCount += 1;
                }
                if (office.IsIsoCount != model.IsIsoCount)
                {
                    bnLog.PreviousValue += ", ISO: " + office.IsIsoCount;
                    bnLog.UpdatedValue += ",  ISO: " + model.IsIsoCount;
                    bnoisUpdateCount += 1;
                }
                if (office.IsDeputationCount != model.IsDeputationCount)
                {
                    bnLog.PreviousValue += ", Deputation: " + office.IsDeputationCount;
                    bnLog.UpdatedValue += ",  Deputation: " + model.IsDeputationCount;
                    bnoisUpdateCount += 1;
                }
                if (office.IsIntelligenceServiceCount != model.IsIntelligenceServiceCount)
                {
                    bnLog.PreviousValue += ", Intelligent Service: " + office.IsIntelligenceServiceCount;
                    bnLog.UpdatedValue += ",  Intelligent Service: " + model.IsIntelligenceServiceCount;
                    bnoisUpdateCount += 1;
                }
                if (office.CmdService != model.CmdService)
                {
                    bnLog.PreviousValue += ", Command Service: " + office.CmdService;
                    bnLog.UpdatedValue += ",  Command Service: " + model.CmdService;
                    bnoisUpdateCount += 1;
                }
                if (office.Commissioned != model.Commissioned)
                {
                    bnLog.PreviousValue += ", Commissioned: " + office.Commissioned;
                    bnLog.UpdatedValue += ",  Commissioned: " + model.Commissioned;
                    bnoisUpdateCount += 1;
                }
                if (office.IsSubmarineCount != model.IsSubmarineCount)
                {
                    bnLog.PreviousValue += ", Submarine: " + office.IsSubmarineCount;
                    bnLog.UpdatedValue += ",  Submarine: " + model.IsSubmarineCount;
                    bnoisUpdateCount += 1;
                }
                if (office.ActiveStatus != model.ActiveStatus)
                {
                    bnLog.PreviousValue += ", Active Status: " + office.ActiveStatus;
                    bnLog.UpdatedValue += ",  Active Status: " + model.ActiveStatus;
                    bnoisUpdateCount += 1;
                }
                if (office.ObjType != model.ObjType)
                {
                    bnLog.PreviousValue += ", Objective: " + (office.ObjType == 3 ? "Training" : office.ObjType == 2 ? "Operational" : office.ObjType == 3 ? "Both" : office.ObjType == 4 ? "None" : "");
                    bnLog.UpdatedValue += ", Objective: " + (model.ObjType == 1 ? "Training" : model.ObjType == 2 ? "Operational" : model.ObjType == 3 ? "Both" : model.ObjType == 4 ? "None" : "");
                    bnoisUpdateCount += 1;
                }
                if (office.CommDate != model.CommDate)
                {
                    bnLog.PreviousValue += ", Commission Date: " + office.CommDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Commission Date: " + model.CommDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (office.CommissionBy != model.CommissionBy)
                {
                    bnLog.PreviousValue += ", Commission By: " + office.CommissionBy;
                    bnLog.UpdatedValue += ", Commission By: " + model.CommissionBy;
                    bnoisUpdateCount += 1;
                }
                if (office.DeCommissioned != model.DeCommissioned)
                {
                    bnLog.PreviousValue += ", De-Commissioned: " + office.DeCommissioned;
                    bnLog.UpdatedValue += ", De-Commissioned: " + model.DeCommissioned;
                    bnoisUpdateCount += 1;
                }
                if (office.DeComDate != model.DeComDate)
                {
                    bnLog.PreviousValue += ", De-Commission Date: " + office.DeComDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", De-Commission Date: " + model.DeComDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (office.DeComPurpose != model.DeComPurpose)
                {
                    bnLog.PreviousValue += ", De-Commission Purpose: " + office.DeComPurpose;
                    bnLog.UpdatedValue += ", De-Commission Purpose: " + model.DeComPurpose;
                    bnoisUpdateCount += 1;
                }
                if (office.ParentId != model.ParentId)
                {
                    if (office.ParentId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", office.ParentId);
                        bnLog.PreviousValue += ", Parent Office: " + ((dynamic)prev).ShortName;
                    }
                    if (model.ParentId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Office", "OfficeId", model.ParentId);
                        bnLog.UpdatedValue += ", Parent Office: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (office.ZoneId != model.ZoneId)
                {
                    if (office.ZoneId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Zone", "ZoneId", office.ZoneId??0);
                        bnLog.PreviousValue += ", Zone: " + ((dynamic)prev).Name;
                    }
                    if (model.ZoneId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Zone", "ZoneId", model.ZoneId??0);
                        bnLog.UpdatedValue += ", Zone: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (office.AdminAuthorityId != model.AdminAuthorityId)
                {
                    if (office.AdminAuthorityId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", office.AdminAuthorityId ?? 0);
                        bnLog.PreviousValue += ", Admin Authority: " + ((dynamic)prev).ShortName;
                    }
                    if (model.AdminAuthorityId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Office", "OfficeId", model.AdminAuthorityId??0);
                        bnLog.UpdatedValue += ", Admin Authority: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (office.SeServStartDate != model.SeServStartDate)
                {
                    bnLog.PreviousValue += ", Sea Service Start Date: " + office.SeServStartDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Sea Service Start Date: " + model.SeServStartDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (office.SeServEndDate != model.SeServEndDate)
                {
                    bnLog.PreviousValue += ", Sea Service End Date: " + office.SeServEndDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Sea Service End Date: " + model.SeServEndDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (office.CommandServStartDate != model.CommandServStartDate)
                {
                    bnLog.PreviousValue += ", Command Service Start Date: " + office.CommandServStartDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Command Service Start Date: " + model.CommandServStartDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (office.CommandServEndDate != model.CommandServEndDate)
                {
                    bnLog.PreviousValue += ", Command Service End Date: " + office.CommandServEndDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Command Service End Date: " + model.CommandServEndDate?.ToString("dd/MM/yyyy");
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
                office.CreatedDate = DateTime.Now;
                office.CreatedBy = userId;
                office.IsActive = true;
            }
            office.Name = model.Name;
            office.NameBangla = model.NameBangla;
            office.ShortName = model.ShortName;
            office.ShortNameBangla = model.ShortNameBangla;
            office.PatternId = model.PatternId;
            office.ShipCategoryId = model.ShipCategoryId;
            office.ShipType = model.ShipType;
            office.CountryId = model.CountryId;
            office.ShipOriginId = model.ShipOriginId;
            office.AddressInfo = model.AddressInfo;
            office.BornOffice = model.BornOffice;
            office.BornOfficeId = model.BornOfficeId;
            office.Establishment = model.Establishment;
            office.GovtApproved = model.GovtApproved;
            office.CoastGuard = model.CoastGuard;
            office.IsOutsideOrg = model.IsOutsideOrg;
            office.ObjType = model.ObjType;
            office.IndCmd = model.IndCmd;
            office.SeaServiceCnt = model.SeaServiceCnt;
            office.IsDocyardCount = model.IsDocyardCount;
            office.IsSubmarineCount = model.IsSubmarineCount;
            office.IsIntelligenceServiceCount = model.IsIntelligenceServiceCount;
            office.IsDeputationCount = model.IsDeputationCount;
            office.IsIsoCount = model.IsIsoCount;
            office.CmdService = model.CmdService;
            office.Commissioned = model.Commissioned;
            office.CommDate = model.CommDate;
            office.CommissionBy = model.CommissionBy;
            office.DeCommissioned = model.DeCommissioned;
            office.DeComDate = model.DeComDate;
            office.DeComPurpose = model.DeComPurpose;
            office.ParentId = model.ParentId;
            office.AdminAuthorityId = model.AdminAuthorityId;
            office.ZoneId = model.ZoneId;
            office.SeServStartDate = model.SeServStartDate;
            office.SeServEndDate = model.SeServEndDate;
            office.CommandServEndDate = model.CommandServEndDate;
            office.CommandServStartDate = model.CommandServStartDate;
            office.IsMoved = false;
            office.SecurityName = model.SecurityName;
            office.ActiveStatus = model.ActiveStatus;

            await officeRepository.SaveAsync(office);
            model.OfficeId = office.OfficeId;
            return model;
        }

        public async Task<List<SelectModel>> GetAdminAuthoritySelectModel()
        {

            ICollection<Office> offices = await officeRepository.FilterAsync(x => x.IsActive && x.Pattern.Name == "Admin Area" && x.ActiveStatus == true);
            List<Office> query = offices.OrderBy(x => x.ShortName).ToList();

            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.ShortName,
                Value = x.OfficeId
            }).ToList();
            return selectModels;
        }
        public async Task<List<SelectModel>> GetOfficeSelectModelByShip(int ship)
        {

            ICollection<Office> offices = await officeRepository.FilterAsync(x => x.IsActive && x.ShipType==ship && x.ActiveStatus == true);
            List<Office> query = offices.OrderBy(x => x.ShortName).ToList();

            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.ShortName,
                Value = x.OfficeId
            }).ToList();
            return selectModels;
        }


     




        public async Task<List<SelectModel>> GetBornOfficeSelectModel()
        {

            ICollection<Office> offices = await officeRepository.FilterAsync(x => x.IsActive && x.BornOffice && x.ActiveStatus==true);
            List<Office> query = offices.OrderBy(x => x.ShortName).ToList();
            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.ShortName,
                Value = x.OfficeId
            }).ToList();
            return selectModels;
        }

        public async Task<List<SelectModel>> GetBornOfficeFullNameSelectModel()
        {

            ICollection<Office> offices = await officeRepository.FilterAsync(x => x.IsActive && x.BornOffice && x.ActiveStatus == true);
            List<Office> query = offices.OrderBy(x => x.Name).ToList();
            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.OfficeId
            }).ToList();
            return selectModels;
        }


        public async Task<List<SelectModel>> GetParentOfficeSelectModel()
        {

            ICollection<Office> offices = await officeRepository.FilterAsync(x => x.IsActive && x.ActiveStatus == true);
            List<Office> query = offices.OrderBy(x => x.ShortName).ToList();
            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.ShortName,
                Value = x.OfficeId
            }).ToList();
            return selectModels;
        }


      

        public async Task<List<SelectModel>> GetMinistryOfficeSelectModel()
        {

            ICollection<Office> offices = await officeRepository.FilterAsync(x => x.IsActive && x.PatternId == 23 && x.ActiveStatus == true);
            List<Office> query = offices.OrderBy(x => x.Name).ToList();
            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.OfficeId
            }).ToList();
            return selectModels;
        }

        public async Task<List<SelectModel>> GetChildOfficeSelectModel(int parentId)
        {
            ICollection<Office> offices;
            if (parentId == -404)
            {
                offices = await officeRepository.FilterAsync(x => x.IsActive && x.CoastGuard == true && x.ActiveStatus == true);
            }
            else
            {
               offices = await officeRepository.FilterAsync(x => x.IsActive && x.ParentId == parentId && x.ActiveStatus == true);
            }
            
            List<Office> query = offices.OrderBy(x => x.ShortName).ToList();
            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.ShortName,
                Value = x.OfficeId
            }).ToList();
            return selectModels;
        }



        public async Task<List<SelectModel>> GetOfficeSelectModel(int type)
        {
            ICollection<Office> offices;

            if (type == (int)TransferType.CostGuard)
            {
                offices = await officeRepository.FilterAsync(x => x.IsActive && x.CoastGuard && x.ActiveStatus == true);
            }
            else if (type == (int)TransferType.Outside)
            {
                offices = await officeRepository.FilterAsync(x => x.IsActive && x.IsOutsideOrg && x.ActiveStatus == true);
            }
            else
            {
                offices = await officeRepository.FilterAsync(x => x.IsActive && x.CoastGuard == false && x.IsOutsideOrg == false && x.ActiveStatus == true);
            }

            List<Office> query = offices.OrderBy(x => x.ShortName).ToList();
            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.ShortName,
                Value = x.OfficeId
            }).ToList();
            return selectModels;
        }

        public async Task<List<SelectModel>> GetOrganizationSelectModel()
        {

            ICollection<Office> offices = await officeRepository.FilterAsync(x => x.IsActive && x.IsOutsideOrg && x.ActiveStatus == true);
            List<Office> query = offices.OrderBy(x => x.ShortName).ToList();
            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.ShortName,
                Value = x.OfficeId
            }).ToList();
            return selectModels;
        }

        public async Task<List<SelectModel>> GetOfficeByShipTypeSelectModel(int type)
        {

            ICollection<Office> offices = await officeRepository.FilterAsync(x => x.IsActive && x.ShipType == type);
            List<Office> query = offices.OrderBy(x => x.ShortName).ToList();
            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.ShortName,
                Value = x.OfficeId
            }).ToList();
            return selectModels;
        }

        public List<SelectModel> GetObjectiveSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(Objective)).Cast<Objective>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }


        public List<SelectModel> GetShipTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(ShipType)).Cast<ShipType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }


        public async Task<bool> DeleteOffice(int id)
        {
            if (id <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Office office = await officeRepository.FindOneAsync(x => x.OfficeId == id);
            if (office == null)
            {
                throw new InfinityNotFoundException("Office not found");
            }

            ICollection<Office> officeList = await officeRepository.FilterAsync(x => x.ParentId == id);

            if (officeList.Count > 0)
            {
                throw new InfinityNotFoundException("Remove Child Office first");
            }
            // data log section start
            BnoisLog bnLog = new BnoisLog();
            bnLog.TableName = "Office";
            bnLog.TableEntryForm = "Office";
            bnLog.PreviousValue = ", Id: " + office.OfficeId;
            
            bnLog.PreviousValue += ", Office Name: " + office.Name + ", Office Name (বাংলা): " + office.NameBangla + ", Short Name: " + office.ShortName + ", Short Name (বাংলা): " + office.ShortNameBangla + ", Security Name: " + office.SecurityName;
            if (office.PatternId > 0)
            {
                var prev = employeeService.GetDynamicTableInfoById("Pattern", "PatternId", office.PatternId ?? 0);
                bnLog.PreviousValue += ", Pattern: " + ((dynamic)prev).Name;
            }
            if (office.ShipCategoryId > 0)
            {
                var prev = employeeService.GetDynamicTableInfoById("ShipCategory", "ShipCategoryId", office.ShipCategoryId ?? 0);
                bnLog.PreviousValue += ", Ship Category: " + ((dynamic)prev).Name;
            }
            bnLog.PreviousValue += ", Ship Type: " + (office.ShipType == 1 ? "Small" : office.ShipType == 2 ? "Medium" : office.ShipType == 3 ? "Large" : "");
            if (office.CountryId > 0)
            {
                var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", office.CountryId ?? 0);
                bnLog.PreviousValue += ", Country: " + ((dynamic)prev).FullName;
            }
            if (office.ShipOriginId > 0)
            {
                var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", office.ShipOriginId ?? 0);
                bnLog.PreviousValue += ", Ship Origin: " + ((dynamic)prev).FullName;
            }
            bnLog.PreviousValue += ", Office Address: " + office.AddressInfo;
            if (office.BornOfficeId > 0)
            {
                var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", office.BornOfficeId ?? 0);
                bnLog.PreviousValue += ", Ship Origin: " + ((dynamic)prev).ShortName;
            }
            bnLog.PreviousValue += ", Born Office: " + office.BornOffice + ", Establishment: " + office.Establishment + ", Govt Approved: " + office.GovtApproved + ", Coast Guard: " + office.CoastGuard;
            bnLog.PreviousValue += ", Outside Org: " + office.IsOutsideOrg + ", Independent Command: " + office.IndCmd + ", Sea Service: " + office.SeaServiceCnt + ", Dockyard: " + office.IsDocyardCount;
            bnLog.PreviousValue += ", ISO: " + office.IsIsoCount + ", Deputation: " + office.IsDeputationCount + ", Intelligent Service: " + office.IsIntelligenceServiceCount + ", Command Service: " + office.CmdService;
            bnLog.PreviousValue += ", Commissioned: " + office.Commissioned + ", Submarine: " + office.IsSubmarineCount + ", Active Status: " + office.ActiveStatus;
            bnLog.PreviousValue += ", Objective: " + (office.ObjType == 3 ? "Training" : office.ObjType == 2 ? "Operational" : office.ObjType == 3 ? "Both" : office.ObjType == 4 ? "None" : "");
            bnLog.PreviousValue += ", Commission Date: " + office.CommDate?.ToString("dd/MM/yyyy") + ", Commission By: " + office.CommissionBy + ", De-Commissioned: " + office.DeCommissioned;
            bnLog.PreviousValue += ", De-Commission Date: " + office.DeComDate?.ToString("dd/MM/yyyy") + ", De-Commission Purpose: " + office.DeComPurpose;
            if (office.ParentId > 0)
            {
                var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", office.ParentId);
                bnLog.PreviousValue += ", Parent Office: " + ((dynamic)prev).ShortName;
            }
            if (office.ZoneId > 0)
            {
                var prev = employeeService.GetDynamicTableInfoById("Zone", "ZoneId", office.ZoneId ?? 0);
                bnLog.PreviousValue += ", Zone: " + ((dynamic)prev).Name;
            }
            if (office.AdminAuthorityId > 0)
            {
                var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", office.AdminAuthorityId ?? 0);
                bnLog.PreviousValue += ", Admin Authority: " + ((dynamic)prev).ShortName;
            }
            bnLog.PreviousValue += ", Sea Service Start Date: " + office.SeServStartDate?.ToString("dd/MM/yyyy") + ", Sea Service End Date: " + office.SeServEndDate?.ToString("dd/MM/yyyy");
            bnLog.PreviousValue += ", Command Service Start Date: " + office.CommandServStartDate?.ToString("dd/MM/yyyy") + ", Command Service End Date: " + office.CommandServEndDate?.ToString("dd/MM/yyyy");

            bnLog.UpdatedValue = "This Record has been Deleted!";


            bnLog.LogStatus = 2; // 1 for update, 2 for delete
            bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            bnLog.LogCreatedDate = DateTime.Now;

            await bnoisLogRepository.SaveAsync(bnLog);

            //data log section end

            return await officeRepository.DeleteAsync(office);

        }


        public async Task<List<SelectModel>> GetOfficeWithoutShipSelectModels()
        {
            ICollection<Office> offices = await officeRepository.FilterAsync(x => x.IsActive  && x.IsOutsideOrg==false && x.ActiveStatus == true);
            List<Office> query = offices.OrderBy(x => x.ShortName).ToList();
            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.ShortName,
                Value = x.OfficeId
            }).ToList();
            return selectModels;
        }

        public async Task<List<SelectModel>> GetShipSelectModels()
        {
            ICollection<Office> offices = await officeRepository.FilterAsync(x => x.IsActive && x.Pattern.IsMoveAble == true && x.ActiveStatus == true);
            List<Office> query = offices.OrderBy(x => x.ShortName).ToList();
            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.ShortName,
                Value = x.OfficeId
            }).ToList();
            return selectModels;
        }

        public async Task<List<SelectModel>> GetShipOfficeSelectModels()
        {
            ICollection<Office> offices = await officeRepository.FilterAsync(x => x.IsActive && x.PatternId==7 && x.ActiveStatus == true);
            List<Office> query = offices.OrderBy(x => x.ShortName).ToList();
            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.ShortName,
                Value = x.OfficeId
            }).ToList();
            return selectModels;
        }



        public List<OfficeModel> GetOfficeStructures()
        {

            List<Office> list;

            list = officeRepository.Where(a => a.IsActive).OrderBy(a => a.ShortName).ToList();


            List<Office> treeList = new List<Office>();
            if (list.Count > 0)
            {
                treeList = BuildStructure(list);
            }

            List<OfficeModel> models = ObjectConverter<Office, OfficeModel>.ConvertList(treeList.ToList()).ToList();
            return models;
        }


        public List<Office> BuildStructure(List<Office> list)
        {
            List<Office> returnList = new List<Office>();
            var topLevels = list.Where(a => a.ParentId == list.OrderBy(b => b.ParentId).FirstOrDefault().ParentId);
            returnList.AddRange(topLevels);
            foreach (var i in topLevels)
            {
                GetStructureView(list, i, ref returnList);
            }
            return returnList;
        }

        public void GetStructureView(List<Office> list, Office current, ref List<Office> returnList)
        {
            var childs = list.Where(a => a.ParentId == current.OfficeId).ToList();
            current.Count ="Ship/Establishment: "+ childs.Count;
            current.Children = new List<Office>();
            current.Children.AddRange(childs);
            foreach (var i in childs)
            {
                GetStructureView(list, i, ref returnList);
            }
        }

        public List<OfficeModel> GetOffices()
        {
            List<Office> list;

            list = officeRepository.Where(a => a.IsActive).OrderBy(a => a.ShortName).ToList();


            List<Office> treeList = new List<Office>();
            if (list.Count > 0)
            {
                treeList = BuildTree(list);
            }

            List<OfficeModel> models = ObjectConverter<Office, OfficeModel>.ConvertList(treeList.ToList()).ToList();
            return models;
        }

        public List<Office> BuildTree(List<Office> list)
        {
            List<Office> returnList = new List<Office>();
            var topLevels = list.Where(a => a.ParentId == list.OrderBy(b => b.ParentId).FirstOrDefault().ParentId);
            returnList.AddRange(topLevels);
            foreach (var i in topLevels)
            {
                GetTreeView(list, i, ref returnList);
            }
            return returnList;
        }

        public void GetTreeView(List<Office> list, Office current, ref List<Office> returnList)
        {
            var childs = list.Where(a => a.ParentId == current.OfficeId).ToList();
            current.Items = new List<Office>();
            current.Items.AddRange(childs);
            foreach (var i in childs)
            {
                GetTreeView(list, i, ref returnList);
            }
        }

     


        public async Task<OfficeModel> ShipMovement(int officeId, OfficeModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Office data missing");
            }

            if (model.ShipDate==null)
            {
                throw new InfinityArgumentMissingException("Date is required.");
            }

            Office office = ObjectConverter<OfficeModel, Office>.Convert(model);

            office = await officeRepository.FindOneAsync(x => x.OfficeId == officeId);
            if (office == null)
            {
                throw new InfinityNotFoundException("Office not found !");
            }


            OfficeShipmentModel officeShipment = new OfficeShipmentModel();
            officeShipment.OfficeId = officeId;
            officeShipment.PreviousParentId = office.ParentId;
            officeShipment.ShipDate = model.ShipDate;
            officeShipment.PreZoneId = office.ZoneId;
            officeShipment.PreAdminAuthorityId = office.AdminAuthorityId;
            officeShipment.PreAddress = office.AddressInfo;

           await officeShipmentService.SaveOfficeShipment(officeShipment);

            var officeDetails = await officeRepository.FindOneAsync(x => x.OfficeId == model.ParentId);

            bool isAdminArea =  officeRepository.Exists(x => x.OfficeId == model.ParentId && x.PatternId==1);

            if (isAdminArea)
            {
                office.AdminAuthorityId = model.ParentId;
            }
            else
            {
                office.AdminAuthorityId = officeDetails.AdminAuthorityId;
            }
            office.ParentId = model.ParentId;
            office.ZoneId = officeDetails.ZoneId;

            office.IsMoved = true;




            await officeRepository.SaveAsync(office);

            

            model.OfficeId = office.OfficeId;
            return model;
        }


        public List<object> GetAppointedOfficer(int officeId)
        {
            System.Data.DataTable dataTable = officeRepository.ExecWithSqlQuery(String.Format("exec [spGetAppointedOfficer] {0} ", officeId));

            return dataTable.ToJson().ToList();
        }


        public List<object> GetVacantAppointment(int officeId)
        {
            System.Data.DataTable dataTable = officeRepository.ExecWithSqlQuery(String.Format("exec [spGetVacantAppointment] {0} ", officeId));

            return dataTable.ToJson().ToList();
        }
        public List<object> GetOfficerListByBatch(int batchId)
        {
            System.Data.DataTable dataTable = officeRepository.ExecWithSqlQuery(String.Format("exec [SpGetOfficerListByBatch] {0} ", batchId));

            return dataTable.ToJson().ToList();
        }


    }
}
