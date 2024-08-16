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

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 25)
                .WithMessage("Page size must be between 1 and 25.");

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be greater than or equal to 1.");

        }

    }
}
