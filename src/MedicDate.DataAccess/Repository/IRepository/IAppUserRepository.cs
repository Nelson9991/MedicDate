﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MedicDate.API.DTOs.AppUser;
using MedicDate.DataAccess.Entities;
using MedicDate.DataAccess.Helpers;

namespace MedicDate.DataAccess.Repository.IRepository
{
    public interface IAppUserRepository : IRepository<ApplicationUser>
    {
        public Task<OperationResult> UpdateUserAsync(string userId, AppUserRequestDto appUserRequestDto);
        public Task<List<AppRole>> GetRolesAsync();
    }
}