using System.ComponentModel.DataAnnotations;

namespace ContactApi.Data.Entities
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required()]
        [StringLength(50, MinimumLength = 2)]
        public string Salutation { get; set; }

        [Required()]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required()]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        public string DisplayName { get; set; }

        public DateTime? BirthDate { get; set; }

        public DateTime CreationTimestamp { get; set; }

        public DateTime? LastChangeTimestamp { get; set; }

        [Required()]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
