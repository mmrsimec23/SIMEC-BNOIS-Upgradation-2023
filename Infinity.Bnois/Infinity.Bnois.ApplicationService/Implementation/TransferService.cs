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
    public class TransferService : ITransferService
    {
        private readonly IBnoisRepository<Transfer> transferRepository;
        private readonly IBnoisRepository<vwTransfer> vwTransferRepository;
        private readonly IBnoisRepository<Employee> employeeRepository;
        private readonly IBnoisRepository<EmployeeGeneral> employeeGeneralRepository;
        private readonly IBnoisRepository<OfficeAppRank> officeAppRankRepository;
        private readonly IBnoisRepository<OfficeAppBranch> officeAppBranchRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public TransferService(IBnoisRepository<Transfer> transferRepository, IBnoisRepository<vwTransfer> vwTransferRepository, IBnoisRepository<Employee> employeeRepository,
            IBnoisRepository<EmployeeGeneral> employeeGeneralRepository, IBnoisRepository<OfficeAppRank> officeAppRankRepository, IBnoisRepository<OfficeAppBranch> officeAppBranchRepository,
            IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.transferRepository = transferRepository;
            this.vwTransferRepository = vwTransferRepository;
            this.employeeRepository = employeeRepository;
            this.employeeGeneralRepository = employeeGeneralRepository;
            this.officeAppRankRepository = officeAppRankRepository;
            this.officeAppBranchRepository = officeAppBranchRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;

        }

        public List<vwTransfer> GetTransfers(int employeeId, int type, int mode)
        {
            var transfers = vwTransferRepository.FilterWithInclude(x => x.EmployeeId == employeeId && x.TransferFor == type && x.TransferMode == mode).OrderBy(x => x.FromDate).ToList();

            return transfers;
        }


        public async Task<TransferModel> GetTransfer(int id)
        {

            if (id <= 0)
            {
                return new TransferModel();
            }
            Transfer transfer = await transferRepository.FindOneAsync(x => x.TransferId == id, new List<string> { "Employee" });
            if (transfer == null)
            {
                throw new InfinityNotFoundException("Transfer not found");
            }
            TransferModel model = ObjectConverter<Transfer, TransferModel>.Convert(transfer);
            return model;
        }

        public async Task<TransferModel> SaveTransfer(int id, TransferModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Transfer  data missing");
            }

            if (model.FromDate > model.ToDate)
            {
                throw new InfinityInvalidDataException("FromDate is greater than ToDate");
            }

            if (model.TransferMode == (int)TransferMode.Permanent && model.CurrentBornOfficeId == null)
            {
                throw new InfinityInvalidDataException("Please enter born office.");
            }


            if (model.TransferFor == (int)TransferFor.Office && model.TransferMode == (int)TransferMode.Permanent && model.AttachOfficeId == null)
            {
                throw new InfinityInvalidDataException("Please enter attach office.");
            }


            if (model.TransferFor != (int)TransferFor.Office)
            {
                bool isExistData = transferRepository.Exists(x => x.TransferFor == model.TransferFor && x.NominationId == model.NominationId && x.EmployeeId == model.EmployeeId && x.TransferId != id);
                if (isExistData)
                {
                    throw new InfinityInvalidDataException("Data already exists !");
                }
            }

            if (model.TransferFor == (int)TransferFor.Office && model.TransferMode == (int)TransferMode.Permanent)
            {
                bool isExistData = transferRepository.Exists(x => x.TransferFor == model.TransferFor && x.TransferMode==(int)TransferMode.Permanent && x.FromDate == model.FromDate && x.EmployeeId == model.EmployeeId && x.TransferId != id);
                if (isExistData)
                {
                    throw new InfinityInvalidDataException("Data already exists !");
                }
            }



            if (model.TransferFor == (int)TransferFor.Office && model.AppointmentType == 1 && model.AppointmentId != null && model.TransferMode == (int)TransferMode.Permanent && id == 0)
            {
                EmployeeGeneral employeeGeneral =
                    await employeeGeneralRepository.FindOneAsync(x => x.EmployeeId == model.EmployeeId);

                bool officeAppRank =
                    officeAppRankRepository.Exists(x => x.OffAppId == model.AppointmentId && x.RankId == model.Employee.RankId);


                bool officeAppBranch =
                    officeAppBranchRepository.Exists(x => x.OffAppId == model.AppointmentId && x.BranchId == employeeGeneral.BranchId);

                if (!officeAppBranch && !officeAppRank)
                {
                    throw new InfinityNotFoundException("Officer Not Allowed");
                }
                else if (officeAppBranch && !officeAppRank)
                {
                    throw new InfinityNotFoundException("Officer Not Allowed");
                }
                else if (officeAppRank && !officeAppBranch)
                {
                    throw new InfinityNotFoundException("Officer Not Allowed");
                }
            }


            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Transfer transfer = ObjectConverter<TransferModel, Transfer>.Convert(model);
            if (id > 0)
            {
                transfer = await transferRepository.FindOneAsync(x => x.TransferId == id);
                if (transfer == null)
                {
                    throw new InfinityNotFoundException("Transfer not found !");
                }

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Transfer";
                bnLog.TableEntryForm = "Transfer";
                bnLog.PreviousValue = "Id: " + model.TransferId;
                bnLog.UpdatedValue = "Id: " + model.TransferId;
                int bnoisUpdateCount = 0;

                if (transfer.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", transfer.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                    //bnoisUpdateCount += 1;
                }
                if (transfer.DistrictId != model.DistrictId)
                {
                    if (transfer.DistrictId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("District", "DistrictId", transfer.DistrictId ?? 0);
                        bnLog.PreviousValue += ", District: " + ((dynamic)prev).Name;
                    }
                    if (model.DistrictId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("District", "DistrictId", model.DistrictId ?? 0);
                        bnLog.UpdatedValue += ", District: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (transfer.RankId != model.RankId)
                {
                    if (transfer.RankId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Rank", "RankId", transfer.RankId ?? 0);
                        bnLog.PreviousValue += ", Rank: " + ((dynamic)prev).ShortName;
                    }
                    if (model.DistrictId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId ?? 0);
                        bnLog.UpdatedValue += ", Rank: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (transfer.TransferFor != model.TransferFor)
                {
                    bnLog.PreviousValue += ", Transfer For: " + (transfer.TransferFor == 1 ? "Office": transfer.TransferFor == 2 ? "Course" : transfer.TransferFor == 3 ? "Mission" : "");
                    bnLog.UpdatedValue += ", Transfer For: " + (model.TransferFor == 1 ? "Office" : model.TransferFor == 2 ? "Course" : model.TransferFor == 3 ? "Mission" : "");
                    bnoisUpdateCount += 1;
                }
                if (transfer.TransferMode != model.TransferMode)
                {
                    bnLog.PreviousValue += ", Transfer Mode: " + (transfer.TransferMode == 1 ? "Permanent" : transfer.TransferMode == 2 ? "Temporary" : "");
                    bnLog.UpdatedValue += ", Transfer Mode: " + (model.TransferMode == 1 ? "Permanent" : model.TransferMode == 2 ? "Temporary" : "");
                    bnoisUpdateCount += 1;
                }
                if (transfer.TranferType != model.TranferType)
                {
                    bnLog.PreviousValue += ", Tranfer Type: " + (transfer.TranferType == 1 ? "Inside" : transfer.TranferType == 2 ? "Outside" : transfer.TranferType == 3 ? "CostGuard" : "");
                    bnLog.UpdatedValue += ", Tranfer Type: " + (model.TranferType == 1 ? "Inside" : model.TranferType == 2 ? "Outside" : model.TranferType == 3 ? "CostGuard" : "");
                    bnoisUpdateCount += 1;
                }
                if (transfer.TempTransferType != model.TempTransferType)
                {
                    bnLog.PreviousValue += ", Temporary  Tranfer Type: " + (transfer.TempTransferType == 1 ? "TY_Duty" : transfer.TempTransferType == 2 ? "TY_Attachment" : "");
                    bnLog.UpdatedValue += ", Temporary  Tranfer Type: " + (model.TempTransferType == 1 ? "TY_Duty" : model.TempTransferType == 2 ? "TY_Attachment" : "");
                    bnoisUpdateCount += 1;
                }
                if (transfer.FromDate != model.FromDate)
                {
                    bnLog.PreviousValue += ", From Date: " + transfer.FromDate.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", From Date: " + model.FromDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (transfer.ToDate != model.ToDate)
                {
                    bnLog.PreviousValue += ", From Date: " + transfer.ToDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", From Date: " + model.ToDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (transfer.CurrentBornOfficeId != model.CurrentBornOfficeId)
                {
                    if (transfer.CurrentBornOfficeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", transfer.CurrentBornOfficeId ?? 0);
                        bnLog.PreviousValue += ", Current Born Office: " + ((dynamic)prev).ShortName;
                    }
                    if (model.CurrentBornOfficeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Office", "OfficeId", model.CurrentBornOfficeId ?? 0);
                        bnLog.UpdatedValue += ", Current Born Office: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (transfer.AttachOfficeId != model.AttachOfficeId)
                {
                    if (transfer.AttachOfficeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", transfer.AttachOfficeId ?? 0);
                        bnLog.PreviousValue += ", Attach Office: " + ((dynamic)prev).ShortName;
                    }
                    if (model.AttachOfficeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Office", "OfficeId", model.AttachOfficeId ?? 0);
                        bnLog.UpdatedValue += ", Attach Office: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (transfer.AppointmentId != model.AppointmentId)
                {
                    if (transfer.AppointmentId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("OfficeAppointment", "OffAppId", transfer.AppointmentId ?? 0);
                        bnLog.PreviousValue += ", Appointment: " + ((dynamic)prev).ShortName;
                    }
                    if (model.AppointmentId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("OfficeAppointment", "OffAppId", model.AppointmentId ?? 0);
                        bnLog.UpdatedValue += ", Appointment: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (transfer.AppointmentType != model.AppointmentType)
                {
                    bnLog.PreviousValue += ", Appointment Type: " + (transfer.AppointmentType == 1 ? "Without Appointment" : transfer.AppointmentType == 2 ? "Additional" : transfer.AppointmentType == 3 ? "Additional for Retirement" : "");
                    bnLog.UpdatedValue += ", Appointment Type: " + (model.AppointmentType == 1 ? "Without Appointment" : model.AppointmentType == 2 ? "Additional" : model.AppointmentType == 3 ? "Additional for Retirement" : "");
                    bnoisUpdateCount += 1;
                }
                if (transfer.NominationId != model.NominationId)
                {
                    if (transfer.NominationId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("vwNomination", "NominationId", transfer.NominationId ?? 0);
                        bnLog.PreviousValue += ", Nomination: " + ((dynamic)prev).Nomination + " [" + ((dynamic)prev).MissionAppointment + "]";
                    }
                    if (model.NominationId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("vwNomination", "NominationId", model.NominationId ?? 0);
                        bnLog.UpdatedValue += ", Nomination: " + ((dynamic)newv).Nomination + " [" + ((dynamic)newv).MissionAppointment + "]";
                    }
                    bnoisUpdateCount += 1;
                }
                
                if (transfer.IsBackLog != model.IsBackLog)
                {
                    bnLog.PreviousValue += ", Back Log: " + transfer.IsBackLog;
                    bnLog.UpdatedValue += ", Back Log: " + model.IsBackLog;
                    bnoisUpdateCount += 1;
                }
                if (transfer.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + transfer.Remarks;
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

                transfer.RankId = model.RankId;
                transfer.ModifiedDate = DateTime.Now;
                transfer.ModifiedBy = userId;
            }
            else
            {
                if (model.IsBackLog)
                {
                    transfer.RankId = model.RankId;
                }
                else
                {
                    transfer.RankId = model.Employee.RankId;
                }
                
                transfer.IsActive = false;
                transfer.CreatedDate = DateTime.Now;
                transfer.CreatedBy = userId;

            }

            if (model.CurrentBornOfficeId > 0)
            {
                var newv = employeeService.GetDynamicTableInfoById("Office", "OfficeId", model.CurrentBornOfficeId ?? 0);
                transfer.Transcd = ((dynamic)newv).ZoneId;
            }

            transfer.Employee = null;




            if (model.IsBackLog && (model.TransferMode == (int)TransferMode.Permanent))
            {
                //transfer.RankId = model.RankId;
                //var previousTransfer = transferRepository.Where(x => x.EmployeeId == model.EmployeeId && x.TransferMode == (int)TransferMode.Permanent && x.FromDate < model.FromDate).OrderByDescending(x => x.FromDate).FirstOrDefault();
                var nextTransfer = transferRepository.Where(x => x.EmployeeId == model.EmployeeId && x.TransferMode == (int)TransferMode.Permanent && x.FromDate > model.FromDate).OrderBy(x => x.FromDate).FirstOrDefault();

                if (nextTransfer == null)
                {
                    throw new InfinityNotFoundException("Back Log Entry Not Allow.");
                }

                //if (previousTransfer != null)
                //{
                //    previousTransfer.ToDate = Convert.ToDateTime(model.FromDate).AddDays(-1);
                //    await transferRepository.SaveAsync(previousTransfer);

                //}
                transfer.ToDate = nextTransfer.FromDate.AddDays(-1);

            }
            else
            {
                //transfer.RankId = model.Employee.RankId;

                if (model.TransferMode == (int)TransferMode.Permanent && id == 0)
                {
                    var transferInfo = transferRepository.Where(x => x.EmployeeId == model.EmployeeId && x.TransferMode == (int)TransferMode.Permanent).OrderByDescending(x => x.FromDate).FirstOrDefault();
                    if (transferInfo != null)
                    {

                        if (model.FromDate < transferInfo.FromDate)
                        {
                            throw new InfinityNotFoundException("From Date is less than Last From Date.");
                            //transferInfo.ToDate = Convert.ToDateTime(model.FromDate).AddDays(-1);
                            //await transferRepository.SaveAsync(transferInfo);
                        }                      

                    }

                }
                else
                {
                    transfer.ToDate = model.ToDate;
                }
            }

            if (model.TransferMode == (int)TransferMode.Temporary && id == 0)
            {
                var tyTransferInfo = transferRepository.Where(x => x.EmployeeId == model.EmployeeId && x.TransferMode == (int)TransferMode.Temporary && x.TempTransferType==(int)TemporaryTransferType.TY_Attachment).OrderByDescending(x => x.FromDate).FirstOrDefault();
                if (tyTransferInfo != null)
                {

                    if (tyTransferInfo.ToDate == null)
                    {
                        if (tyTransferInfo.TransferFor == (int)TransferFor.Office)
                        {
                            if (model.TempTransferType==(int)TemporaryTransferType.TY_Attachment)
                            {
                                throw new InfinityNotFoundException("Please Close Last Office TY.");
                            }
                        }
                        else if (tyTransferInfo.TransferFor == (int)TransferFor.Course)
                        {
                            throw new InfinityNotFoundException("Please Close Last Course TY.");
                        }
                        else
                        {
                            throw new InfinityNotFoundException("Please Close Last Mission TY.");

                        }

                    }



                }
            }


            transfer.EmployeeId = model.EmployeeId;
            transfer.DistrictId = model.DistrictId;
            transfer.CurrentBornOfficeId = model.CurrentBornOfficeId;
            transfer.AttachOfficeId = model.AttachOfficeId;
            transfer.NominationId = model.NominationId;
            transfer.TransferMode = model.TransferMode;
            transfer.TranferType = model.TranferType;
            transfer.TempTransferType = model.TempTransferType;
            transfer.AppointmentId = model.AppointmentId;
            transfer.FromDate = model.FromDate ?? transfer.FromDate;
            transfer.TransferFor = model.TransferFor;
            transfer.FileName = model.FileName;
            transfer.AppointmentType = model.AppointmentType;
            transfer.Remarks = model.Remarks;

            await transferRepository.SaveAsync(transfer);
            model.TransferId = transfer.TransferId;


  


            return model;
        }

        public async Task<bool> DeleteTransfer(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Transfer transfer = await transferRepository.FindOneAsync(x => x.TransferId == id);
            if (transfer == null)
            {
                throw new InfinityNotFoundException("Transfer not found");
            }
            else
            {

                var deletedTransferInfo = await transferRepository.DeleteAsync(transfer);

                if (deletedTransferInfo)
                {
                    // data log section start
                    BnoisLog bnLog = new BnoisLog();
                    bnLog.TableName = "Transfer";
                    bnLog.TableEntryForm = "Transfer";
                    bnLog.PreviousValue = "Id: " + transfer.TransferId;

                    if (transfer.EmployeeId > 0)
                    {
                        var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", transfer.EmployeeId);
                        bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    }
                    if (transfer.DistrictId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("District", "DistrictId", transfer.DistrictId ?? 0);
                        bnLog.PreviousValue += ", District: " + ((dynamic)prev).Name;
                    }
                    if (transfer.RankId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Rank", "RankId", transfer.RankId ?? 0);
                        bnLog.PreviousValue += ", Rank: " + ((dynamic)prev).ShortName;
                    }
                    bnLog.PreviousValue += ", Transfer For: " + (transfer.TransferFor == 1 ? "Office" : transfer.TransferFor == 2 ? "Course" : transfer.TransferFor == 3 ? "Mission" : "");

                    bnLog.PreviousValue += ", Transfer Mode: " + (transfer.TransferMode == 1 ? "Permanent" : transfer.TransferMode == 2 ? "Temporary" : "");

                    bnLog.PreviousValue += ", Tranfer Type: " + (transfer.TranferType == 1 ? "Inside" : transfer.TranferType == 2 ? "Outside" : transfer.TranferType == 3 ? "CostGuard" : "");

                    bnLog.PreviousValue += ", Temporary  Tranfer Type: " + (transfer.TempTransferType == 1 ? "TY_Duty" : transfer.TempTransferType == 2 ? "TY_Attachment" : "");

                    bnLog.PreviousValue += ", From Date: " + transfer.FromDate.ToString("dd/MM/yyyy") + ", From Date: " + transfer.ToDate?.ToString("dd/MM/yyyy");

                    if (transfer.CurrentBornOfficeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", transfer.CurrentBornOfficeId ?? 0);
                        bnLog.PreviousValue += ", Current Born Office: " + ((dynamic)prev).ShortName;
                    }
                    if (transfer.AttachOfficeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", transfer.AttachOfficeId ?? 0);
                        bnLog.PreviousValue += ", Attach Office: " + ((dynamic)prev).ShortName;
                    }
                    if (transfer.AppointmentId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("OfficeAppointment", "OffAppId", transfer.AppointmentId ?? 0);
                        bnLog.PreviousValue += ", Appointment: " + ((dynamic)prev).ShortName;
                    }
                    bnLog.PreviousValue += ", Appointment Type: " + (transfer.AppointmentType == 1 ? "Without Appointment" : transfer.AppointmentType == 2 ? "Additional" : transfer.AppointmentType == 3 ? "Additional for Retirement" : "");

                    if (transfer.NominationId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("vwNomination", "NominationId", transfer.NominationId ?? 0);
                        bnLog.PreviousValue += ", Nomination: " + ((dynamic)prev).Nomination + " [" + ((dynamic)prev).MissionAppointment + "]";
                    }
                    bnLog.PreviousValue += ", Back Log: " + transfer.IsBackLog + ", Remarks: " + transfer.Remarks;
                    bnLog.UpdatedValue = "This Record has been Deleted!";


                    bnLog.LogStatus = 2; // 1 for update, 2 for delete
                    bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                    bnLog.LogCreatedDate = DateTime.Now;

                    await bnoisLogRepository.SaveAsync(bnLog);

                    //data log section end

                    var permanentTransferInfo = transferRepository.Where(x => x.EmployeeId == transfer.EmployeeId && x.TransferMode == (int)TransferMode.Permanent && x.IsActive).OrderByDescending(x => x.FromDate).FirstOrDefault();

                    var employee = await employeeRepository.FindOneAsync(x => x.EmployeeId == transfer.EmployeeId);
                    if (employee != null)
                    {
                        employee.ModifiedDate = DateTime.Now;
                        employee.ModifiedBy = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();

                        if (permanentTransferInfo != null)
                        {
                            employee.TransferId = permanentTransferInfo.TransferId;

                        }



                        await employeeRepository.SaveAsync(employee);
                    }

                }
                
                return deletedTransferInfo;
            }
        }

        public List<SelectModel> GetTransferModeSelectModels()
        {
            List<SelectModel> selectModels =
           Enum.GetValues(typeof(TransferMode)).Cast<TransferMode>()
               .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt32(v) })
               .ToList();
            return selectModels;
        }

        public List<SelectModel> GetTransferTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(TransferType)).Cast<TransferType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt32(v) })
                    .ToList();
            return selectModels;
        }

        public List<SelectModel> GetTemporaryTransferTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(TemporaryTransferType)).Cast<TemporaryTransferType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt32(v) })
                    .ToList();
            return selectModels;
        }
        public List<SelectModel> GetTransferForSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(TransferFor)).Cast<TransferFor>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt32(v) })
                    .ToList();
            return selectModels;
        }
        //public List<SelectModel> GetCourseMissionAbroadSelectModels()
        //{
        //    List<SelectModel> selectModels =
        //        Enum.GetValues(typeof(TransferFor)).Cast<TransferFor>()
        //            .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt32(v) })
        //            .ToList();
        //    selectModels.RemoveAt(0);
        //    return selectModels;
        //}

        public vwTransfer GetLastTransfer(int employeeId)
        {
            var transfer = vwTransferRepository.Where(x => x.EmployeeId == employeeId && x.TransferMode == (int)TransferMode.Permanent && x.isActive).OrderByDescending(x => x.FromDate).FirstOrDefault();

            return transfer;


        }


        public async Task<List<SelectModel>> GetTransferHistory(int employeeId)
        {
            IQueryable<vwTransfer> query = vwTransferRepository.FilterWithInclude(x => x.EmployeeId == employeeId && x.TransferMode == (int)TransferMode.Permanent && x.isActive);
            List<vwTransfer> transfers = await query.OrderByDescending(x => x.FromDate).ToListAsync();
            List<SelectModel> transferSelectModels = transfers.OrderByDescending(x => x.FromDate).Select(x => new SelectModel()
            {
                Text = x.BornOffice + '/' + x.CurrentAttach + '/' + x.Appointment,
                Value = x.TransferId
            }).ToList();
            return transferSelectModels;


        }

        public bool ExecuteTransfer()
        {
            transferRepository.ExecWithSqlQuery(String.Format("exec [spUpdateEmployeeTransfer]"));

            return true;
        }

    }
}
