using MediatR;
using PersonalData.Application.DTOs;
using PersonalData.Domain.Entities;
using PersonalData.Domain.Interfaces;

namespace PersonalData.Application.Commands
{
    public class CreatePersonCommand : IRequest<ApiResponse<PersonResponseDto>>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
    }

    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, ApiResponse<PersonResponseDto>>
    {
        private readonly IPersonRepository _personRepository;

        public CreatePersonCommandHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<ApiResponse<PersonResponseDto>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var age = DateTime.Today.Year - request.BirthDate.Year;
                if (request.BirthDate.Date > DateTime.Today.AddYears(-age))
                    age--;

                var person = new PersonEntity
                {
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    Address = request.Address.Trim(),
                    BirthDate = request.BirthDate,
                    Age = age
                };

                var createdPerson = await _personRepository.AddAsync(person);

                var response = new PersonResponseDto
                {
                    Id = createdPerson.Id,
                    FirstName = createdPerson.FirstName,
                    LastName = createdPerson.LastName,
                    FullName = createdPerson.FullName,
                    Address = createdPerson.Address,
                    BirthDate = createdPerson.BirthDate,
                    Age = createdPerson.Age,
                    CreatedAt = createdPerson.CreatedAt,
                    UpdatedAt = createdPerson.UpdatedAt
                };

                return ApiResponse<PersonResponseDto>.SuccessResult(response, "เพิ่มข้อมูลบุคคลสำเร็จ");
            }
            catch (Exception ex)
            {
                return ApiResponse<PersonResponseDto>.ErrorResult(
                    "เกิดข้อผิดพลาดในการเพิ่มข้อมูล",
                    new List<string> { ex.Message });
            }
        }
    }
}
