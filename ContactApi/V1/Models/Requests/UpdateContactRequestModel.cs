namespace ContactApi.V1.Models.Requests
{
    public class UpdateContactRequestModel
    {
        public string Salutation { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DisplayName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}
