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
    public class BatchService : IBatchService
    {
        private readonly IBnoisRepository<Batch> batchRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public BatchService(IBnoisRepository<Batch> batchRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.batchRepository = batchRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }
        

        public List<BatchModel> GetBatches(int ps, int pn, string qs, out int total)
        {
            IQueryable<Batch> batches = batchRepository.FilterWithInclude(x => x.IsActive && (x.Name.Contains(qs) || String.IsNullOrEmpty(qs)|| (x.Session.Contains(qs) || String.IsNullOrEmpty(qs))));
            total = batches.Count();
            batches = batches.OrderByDescending(x => x.BatchId).Skip((pn - 1) * ps).Take(ps);
            List<BatchModel> models = ObjectConverter<Batch, BatchModel>.ConvertList(batches.ToList()).ToList();
            return models;
        }

        public async Task<BatchModel> GetBatch(int id)
        {
            if (id <= 0)
            {
                return new BatchModel();
            }
            Batch batch= await batchRepository.FindOneAsync(x => x.BatchId == id);
            if (batch == null)
            {
                throw new InfinityNotFoundException("Batch not found");
            }
            BatchModel model = ObjectConverter<Batch, BatchModel>.Convert(batch);
            return model;
        }

        public async Task<BatchModel> SaveBatch(int id, BatchModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Batch data missing");
            }
            bool isExist = batchRepository.Exists(x => x.Name == model.Name && x.Session == model.Session && x.BatchId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Batch batch = ObjectConverter<BatchModel, Batch>.Convert(model);
            if (id > 0)
            {
                batch = await batchRepository.FindOneAsync(x => x.BatchId == id);
                if (batch == null)
                {
                    throw new InfinityNotFoundException("Batch not found !");
                }

                batch.ModifiedDate = DateTime.Now;
                batch.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Batch";
                bnLog.TableEntryForm = "Batch";
                bnLog.PreviousValue = "Id: " + model.BatchId;
                bnLog.UpdatedValue = "Id: " + model.BatchId;
                if (batch.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + batch.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (batch.Session != model.Session)
                {
                    bnLog.PreviousValue += ", Session: " + batch.Session;
                    bnLog.UpdatedValue += ", Session: " + model.Session;
                }
                if (batch.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + batch.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (batch.Name != model.Name || batch.Session != model.Session || batch.Remarks != model.Remarks)
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
                batch.IsActive = true;
                batch.CreatedDate = DateTime.Now;
                batch.CreatedBy = userId;
            }
            batch.Name = model.Name;
            batch.Session = model.Session;
         
            batch.Remarks = model.Remarks;
            await batchRepository.SaveAsync(batch);
            model.BatchId = batch.BatchId;
            return model;
        }

        public async Task<bool> DeleteBatch(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Batch batch = await batchRepository.FindOneAsync(x => x.BatchId == id);
            if (batch == null)
            {
                throw new InfinityNotFoundException("Batch not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Batch";
                bnLog.TableEntryForm = "Batch";
                bnLog.PreviousValue = "Id: " + batch.BatchId + ", Name: " + batch.Name + ", Session: " + batch.Session + ", Remarks: " + batch.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await batchRepository.DeleteAsync(batch);
            }
        }

        public async Task<List<SelectModel>> GetBatchSelectModels()
        {
            ICollection<Batch> models = await batchRepository.FilterAsync(x => x.IsActive);
            return models.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.BatchId
            }).ToList();

        }
    }
}
