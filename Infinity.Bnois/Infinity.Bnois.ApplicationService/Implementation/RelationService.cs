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
    public class RelationService : IRelationService
    {
        private readonly IBnoisRepository<Relation> relationRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public RelationService(IBnoisRepository<Relation> relationRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.relationRepository = relationRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

    
        public List<RelationModel> GetRelations(int ps, int pn, string qs, out int total)
        {
            IQueryable<Relation> relations = relationRepository.FilterWithInclude(x => x.IsActive && (x.Name.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = relations.Count();
            relations = relations.OrderByDescending(x => x.RelationId).Skip((pn - 1) * ps).Take(ps);
            List<RelationModel> models = ObjectConverter<Relation, RelationModel>.ConvertList(relations.ToList()).ToList();
            return models;
        }
        public async Task<RelationModel> GetRelation(int id)
        {
            if (id <= 0)
            {
                return new RelationModel();
            }
            Relation relation = await relationRepository.FindOneAsync(x => x.RelationId == id);
            if (relation == null)
            {
                throw new InfinityNotFoundException("Relation not found");
            }
            RelationModel model = ObjectConverter<Relation, RelationModel>.Convert(relation);
            return model;
        }

        public async Task<RelationModel> SaveRelation(int id, RelationModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Relation data missing");
            }
            bool isExist = relationRepository.Exists(x => x.Name == model.Name && x.RelationId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Relation relation = ObjectConverter<RelationModel, Relation>.Convert(model);
            if (id > 0)
            {
                relation = await relationRepository.FindOneAsync(x => x.RelationId == id);
                if (relation == null)
                {
                    throw new InfinityNotFoundException("Relation not found !");
                }

                relation.ModifiedDate = DateTime.Now;
                relation.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Relation";
                bnLog.TableEntryForm = "Relation";
                bnLog.PreviousValue = "Id: " + model.RelationId;
                bnLog.UpdatedValue = "Id: " + model.RelationId;
                if (relation.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + relation.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (relation.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + relation.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (relation.Name != model.Name || relation.Remarks != model.Remarks)
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
                relation.IsActive = true;
                relation.CreatedDate = DateTime.Now;
                relation.CreatedBy = userId;
            }
            relation.Name = model.Name;
            relation.Remarks = model.Remarks;
        
            await relationRepository.SaveAsync(relation);
            model.RelationId = relation.RelationId;
            return model;
        }

        public async Task<bool> DeleteRelation(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Relation Relation = await relationRepository.FindOneAsync(x => x.RelationId == id);
            if (Relation == null)
            {
                throw new InfinityNotFoundException("Relation not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Relation";
                bnLog.TableEntryForm = "Relation";
                bnLog.PreviousValue = "Id: " + Relation.RelationId + ", Name: " + Relation.Name + ", Remarks: " + Relation.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await relationRepository.DeleteAsync(Relation);
            }
        }

        public async Task<List<SelectModel>> GetRelationSelectModels()
        {
            ICollection<Relation> relations = await relationRepository.FilterAsync(x => x.IsActive);
            List<Relation> query = relations.OrderBy(x => x.Name).ToList();
            return query.Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.RelationId
            }).ToList();
        }
    }
}
