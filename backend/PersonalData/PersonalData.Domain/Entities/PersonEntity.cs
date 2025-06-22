using PersonalData.Domain.Common;
using System.ComponentModel;

namespace PersonalData.Domain.Entities
{
    public class PersonEntity : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();

    }
}
