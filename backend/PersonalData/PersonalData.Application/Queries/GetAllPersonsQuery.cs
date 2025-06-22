using MediatR;
using PersonalData.Application.DTOs;
using PersonalData.Domain.Interfaces;

namespace PersonalData.Application.Queries
{
    public class GetAllPersonsQuery : IRequest<ApiResponse<PagedResult<PersonResponseDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
    }

    public class GetAllPersonsQueryHandler : IRequestHandler<GetAllPersonsQuery, ApiResponse<PagedResult<PersonResponseDto>>>
    {
        private readonly IPersonRepository _personRepository;

        public GetAllPersonsQueryHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<ApiResponse<PagedResult<PersonResponseDto>>> Handle(GetAllPersonsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var (persons, totalCount) = await _personRepository.GetPagedAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.SearchTerm);

                var personDtos = persons.Select(p => new PersonResponseDto
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    FullName = p.FullName,
                    Address = p.Address,
                    BirthDate = p.BirthDate,
                    Age = p.Age,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToList();

                var result = new PagedResult<PersonResponseDto>
                {
                    Data = personDtos,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                return ApiResponse<PagedResult<PersonResponseDto>>.SuccessResult(result, "ดึงข้อมูลบุคคลสำเร็จ");
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResult<PersonResponseDto>>.ErrorResult(
                    "เกิดข้อผิดพลาดในการดึงข้อมูล",
                    new List<string> { ex.Message });
            }
        }
    }
}
