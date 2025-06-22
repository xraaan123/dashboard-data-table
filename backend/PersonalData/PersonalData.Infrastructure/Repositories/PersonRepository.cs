using Microsoft.EntityFrameworkCore;
using PersonalData.Domain.Entities;
using PersonalData.Domain.Interfaces;
using PersonalData.Infrastructure.Data;

namespace PersonalData.Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PersonEntity>> GetAllAsync()
        {
            return await _context.Persons
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        public async Task<(List<PersonEntity> Data, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null)
        {
            var query = _context.Persons.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.ToLower();
                query = query.Where(p =>
                    p.FirstName.ToLower().Contains(search) ||
                    p.LastName.ToLower().Contains(search) ||
                    p.Address.ToLower().Contains(search)
                );
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        public async Task<PersonEntity> AddAsync(PersonEntity entity)
        {
            _context.Persons.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(int id, PersonEntity entity)
        {
            var existingPerson = await _context.Persons.FindAsync(id);
            if (existingPerson == null)
                return false;

            existingPerson.FirstName = entity.FirstName;
            existingPerson.LastName = entity.LastName;
            existingPerson.Address = entity.Address;
            existingPerson.BirthDate = entity.BirthDate;
            existingPerson.Age = entity.Age;
            existingPerson.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
                return false;

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Persons.AnyAsync(p => p.Id == id);
        }
    }
}
