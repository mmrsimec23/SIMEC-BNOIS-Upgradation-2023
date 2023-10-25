using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;

namespace Infinity.Bnois.ApplicationService.Implementation
{
   public class CertificateService : ICertificateService
    {

        private readonly IBnoisRepository<Certificate> certificateRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public CertificateService(IBnoisRepository<Certificate> certificateRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.certificateRepository = certificateRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

        public List<CertificateModel> GetCertificates(int ps, int pn, string qs, out int total)
        {
            IQueryable<Certificate> certificates = certificateRepository.FilterWithInclude(x => x.IsActive && (x.FullName.Contains(qs) || String.IsNullOrEmpty(qs) ));
            total = certificates.Count();
            certificates = certificates.OrderByDescending(x => x.CertificateId).Skip((pn - 1) * ps).Take(ps);
            List<CertificateModel> models = ObjectConverter<Certificate, CertificateModel>.ConvertList(certificates.ToList()).ToList();
            return models;
        }

        public async Task<CertificateModel> GetCertificate(int id)
        {
            if (id <= 0)
            {
                return new CertificateModel();
            }
            Certificate certificate = await certificateRepository.FindOneAsync(x => x.CertificateId == id);
            if (certificate == null)
            {
                throw new InfinityNotFoundException("Certificate not found");
            }
            CertificateModel model = ObjectConverter<Certificate, CertificateModel>.Convert(certificate);
            return model;
        }

        public async Task<CertificateModel> SaveCertificate(int id, CertificateModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Certificate data missing");
            }
            bool isExist = certificateRepository.Exists(x => x.FullName == model.FullName  && x.CertificateId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Certificate certificate = ObjectConverter<CertificateModel, Certificate>.Convert(model);
            if (id > 0)
            {
                certificate = await certificateRepository.FindOneAsync(x => x.CertificateId == id);
                if (certificate == null)
                {
                    throw new InfinityNotFoundException("Certificate not found !");
                }

                certificate.ModifiedDate = DateTime.Now;
                certificate.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Certificate";
                bnLog.TableEntryForm = "Certificate";
                bnLog.PreviousValue = "Id: " + model.CertificateId;
                bnLog.UpdatedValue = "Id: " + model.CertificateId;
                if (certificate.FullName != model.FullName)
                {
                    bnLog.PreviousValue += ", FullName: " + certificate.FullName;
                    bnLog.UpdatedValue += ", FullName: " + model.FullName;
                }
                if (certificate.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ", ShortName: " + certificate.ShortName;
                    bnLog.UpdatedValue += ", ShortName: " + model.ShortName;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (certificate.FullName != model.FullName || certificate.ShortName != model.ShortName)
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
                certificate.IsActive = true;
                certificate.CreatedDate = DateTime.Now;
                certificate.CreatedBy = userId;
            }
            certificate.FullName = model.FullName;
            certificate.ShortName = model.ShortName;

            await certificateRepository.SaveAsync(certificate);
            model.CertificateId = certificate.CertificateId;
            return model;
        }

        public async Task<bool> DeleteCertificate(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Certificate certificate = await certificateRepository.FindOneAsync(x => x.CertificateId == id);
            if (certificate == null)
            {
                throw new InfinityNotFoundException("Certificate not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Certificate";
                bnLog.TableEntryForm = "Certificate";
                bnLog.PreviousValue = "Id: " + certificate.CertificateId + ", FullName: " + certificate.FullName + ", ShortName: " + certificate.ShortName;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await certificateRepository.DeleteAsync(certificate);
            }
        }

        public async Task<List<SelectModel>> GetCertificateSelectModels()
        {
            ICollection<Certificate> models = await certificateRepository.FilterAsync(x => x.IsActive);
            return models.Select(x => new SelectModel()
            {
                Text = x.FullName,
                Value = x.CertificateId
            }).ToList();

        }




    }
}
