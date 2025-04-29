﻿using Auth.Domain.Models;

namespace Auth.Domain.Repositories
{
    public interface IPermissionRepository
    {
        Task<Permission> GetByIdAsync(Guid id);
        Task<Permission> GetByNameAsync(string name);
        Task<IEnumerable<Permission>> GetAllAsync();
        Task AddAsync(Permission permission);
        Task UpdateAsync(Permission permission);
        Task DeleteAsync(Guid id);
    }
}
