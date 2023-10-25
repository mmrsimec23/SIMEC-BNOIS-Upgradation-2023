﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IPromotionPolicyService
    {
        List<PromotionPolicyModel> GetPromotionPolicies();
        Task<PromotionPolicyModel> GetPromotionPolicy(int id);
        Task<PromotionPolicyModel> SavePromotionPolicy(int v, PromotionPolicyModel model);
        Task<bool> DeletePromotionPolicy(int id);
    }
}
