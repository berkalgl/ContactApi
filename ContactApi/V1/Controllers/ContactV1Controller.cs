using ContactApi.Data;
using ContactApi.Data.Entities;
using ContactApi.Exceptions;
using ContactApi.V1.Models.Requests;
using ContactApi.V1.Models.Responses;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactApi.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/contacts")]
    [ApiExplorerSettings(GroupName = "Contacts")]
    public class ContactV1Controller : ControllerBase
    {
        private readonly ContactDbContext _dbContext;
        private readonly IValidator<CreateContactRequestModel> _createContactRequestModelValidator;
        private readonly IValidator<UpdateContactRequestModel> _updateContactRequestModelValidator;

        public ContactV1Controller(
            ContactDbContext dbContext,
            IValidator<CreateContactRequestModel> createContactRequestModelValidator,
            IValidator<UpdateContactRequestModel> updateContactRequestModelValidator)
        {
            _dbContext = dbContext;
            _createContactRequestModelValidator = createContactRequestModelValidator;
            _updateContactRequestModelValidator = updateContactRequestModelValidator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CreateContactRequestModel request)
        {
            ValidationResult validationResult = await _createContactRequestModelValidator.ValidateAsync(request);

            ThrowExceptionIfRequestIsInvalid(validationResult);

            Contact existContact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Email.Equals(request.Email));

            ThrowExceptionIfContactExistWithEmail(existContact);

            if (string.IsNullOrEmpty(request.DisplayName))
            {
                request.DisplayName = string.Join("", new string[] { request.Salutation, " ", request.FirstName, " ", request.LastName });
            }

            Contact contact = new Contact()
            {
                Salutation = request.Salutation,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.DisplayName,
                BirthDate = request.BirthDate,
                CreationTimestamp = DateTime.UtcNow,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };

            await _dbContext.Contacts.AddAsync(contact);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = contact.Id }, contact);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContactResponseModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            Contact contact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id.Equals(id));

            ThrowExceptionIfContactNotFound(contact);

            ContactResponseModel response = GetContactResponseModel(contact);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            Contact contact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id.Equals(id));

            if (contact is null)
            {
                return NoContent();
            }

            _dbContext.Remove(contact);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] UpdateContactRequestModel request)
        {
            ValidationResult validationResult = await _updateContactRequestModelValidator.ValidateAsync(request);

            ThrowExceptionIfRequestIsInvalid(validationResult);

            Contact contact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id.Equals(id));

            ThrowExceptionIfContactNotFound(contact);

            Contact existContact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Email.Equals(request.Email) && c.Id != contact.Id);

            ThrowExceptionIfContactExistWithEmail(existContact);

            if (string.IsNullOrEmpty(request.DisplayName))
            {
                request.DisplayName = string.Join("", new string[] { request.Salutation, " ", request.FirstName, " ", request.LastName });
            }

            contact.Salutation = request.Salutation;
            contact.FirstName = request.FirstName;
            contact.LastName = request.LastName;
            contact.DisplayName = request.DisplayName;
            contact.BirthDate = request.BirthDate;
            contact.Email = request.Email;
            contact.PhoneNumber = request.PhoneNumber;
            contact.LastChangeTimestamp = DateTime.UtcNow;

            _dbContext.Update(contact);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContactResponseModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<ContactResponseModel> response = await _dbContext.Contacts
                .AsNoTracking()
                .Select(c => GetContactResponseModel(c))
                .ToListAsync();

            return Ok(response);
        }

        #region Private Methods

        private static void ThrowExceptionIfContactNotFound(Contact contact)
        {
            if (contact is null)
            {
                throw new ProblemDetailsException(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "Contact could not be found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = "Contact could not be found",
                });
            }
        }

        private static void ThrowExceptionIfContactExistWithEmail(Contact existContact)
        {
            if (existContact is not null)
            {
                throw new ProblemDetailsException(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                    Title = $"Contact already exists with the email",
                    Status = StatusCodes.Status409Conflict,
                    Detail = $"Contact already exists with the email",
                });
            }
        }

        private static void ThrowExceptionIfRequestIsInvalid(ValidationResult validationResult)
        {
            if (!validationResult.IsValid)
            {
                throw new ProblemDetailsException(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "One or more validation errors occured",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = validationResult.Errors?.FirstOrDefault()?.ErrorMessage
                });
            }
        }

        private static ContactResponseModel GetContactResponseModel(Contact contact)
        {
            return new ContactResponseModel()
            {
                Id = contact.Id,
                Salutation = contact.Salutation,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                DisplayName = contact.DisplayName,
                BirthDate = contact.BirthDate,
                CreationTimestamp = contact.CreationTimestamp,
                LastChangeTimestamp = contact.LastChangeTimestamp,
                NotifyHasBirthdaySoon = GetNotifyHasBirthdaySoon(contact.BirthDate),
                Email = contact.Email,
                PhoneNumber = contact.PhoneNumber
            };
        }

        private static bool GetNotifyHasBirthdaySoon(DateTime? birthDate)
        {
            if(birthDate is null)
            {
                return false;
            }

            return birthDate.Value.Date >= DateTime.UtcNow.AddDays(1).Date
                && birthDate.Value.Date < DateTime.UtcNow.AddDays(14).Date;
        }

        #endregion
    }
}