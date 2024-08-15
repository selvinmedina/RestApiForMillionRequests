using FluentValidation;
using Movies.Application.Models;
    
namespace Movies.Application.Validators
{
    public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
    {
        private static readonly string[] AcceptableSortableFields = { "title", "yearOfRelease" };
        public GetAllMoviesOptionsValidator()
        {
            RuleFor(x => x.Year)
                .InclusiveBetween(1900, DateTime.UtcNow.Year)
                .WithMessage("Year must be between 1900 and the current year.");

            RuleFor(x => x.SortField)
                .Must(x => x is null || AcceptableSortableFields.Contains(x.ToLower(), StringComparer.OrdinalIgnoreCase))
                .WithMessage("You can only sort by 'title' or 'yearOfRelease'.");


        }

    }
}
