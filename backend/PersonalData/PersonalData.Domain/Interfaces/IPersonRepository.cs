using PersonalData.Domain.Entities;

namespace PersonalData.Domain.Interfaces
{
    public interface IPersonRepository
    {
        Task<List<PersonEntity>> GetAllAsync();
        Task<(List<PersonEntity> Data, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<PersonEntity> AddAsync(PersonEntity entity);
        Task<bool> UpdateAsync(int id, PersonEntity entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
