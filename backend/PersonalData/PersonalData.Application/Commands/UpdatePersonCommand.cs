using MediatR;
using PersonalData.Application.DTOs;
using PersonalData.Domain.Entities;
using PersonalData.Domain.Interfaces;

namespace PersonalData.Application.Commands
{
    public class UpdatePersonCommand : IRequest<ApiResponse<PersonResponseDto>>
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
    }

    public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, ApiResponse<PersonResponseDto>>
    {
        private readonly IPersonRepository _personRepository;

        public UpdatePersonCommandHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<ApiResponse<PersonResponseDto>> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Calculate age
                var age = DateTime.Today.Year - request.BirthDate.Year;
                if (request.BirthDate.Date > DateTime.Today.AddYears(-age))
                    age--;

                var entity = new PersonEntity
                {
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    Address = request.Address.Trim(),
                    BirthDate = request.BirthDate,
                    Age = age
                };

                var updated = await _personRepository.UpdateAsync(request.Id, entity);

                if (!updated)
                {
                    return ApiResponse<PersonResponseDto>.ErrorResult($"ไม่พบข้อมูลบุคคล ID: {request.Id}");
                }

                // Get updated person for response
                var persons = await _personRepository.GetAllAsync();
                var updatedPerson = persons.FirstOrDefault(p => p.Id == request.Id);

                if (updatedPerson == null)
                {
                    return ApiResponse<PersonResponseDto>.ErrorResult("เกิดข้อผิดพลาดในการดึงข้อมูลที่อัพเดท");
                }

                var response = new PersonResponseDto
                {
                    Id = updatedPerson.Id,
                    FirstName = updatedPerson.FirstName,
                    LastName = updatedPerson.LastName,
                    FullName = updatedPerson.FullName,
                    Address = updatedPerson.Address,
                    BirthDate = updatedPerson.BirthDate,
                    Age = updatedPerson.Age,
                    CreatedAt = updatedPerson.CreatedAt,
                    UpdatedAt = updatedPerson.UpdatedAt
                };

                return ApiResponse<PersonResponseDto>.SuccessResult(response, "แก้ไขข้อมูลบุคคลสำเร็จ");
            }
            catch (Exception ex)
            {
                return ApiResponse<PersonResponseDto>.ErrorResult(
                    "เกิดข้อผิดพลาดในการแก้ไขข้อมูล",
                    new List<string> { ex.Message });
            }
        }
    }
}
