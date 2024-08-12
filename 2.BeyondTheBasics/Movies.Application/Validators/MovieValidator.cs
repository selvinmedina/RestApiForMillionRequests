using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Validators
{
    public class MovieValidator: AbstractValidator<Movie>
    {
        private readonly IMovieRepository _movieRepository;


        public MovieValidator(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Genres)
                .NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.YearOfRelease)
                .NotEmpty()
                .InclusiveBetween(1900, DateTime.UtcNow.Year);

            RuleFor(x => x.Slug)
                .MustAsync(ValidateSlug)
                .WithMessage("This moie already exists in the system.");
        }

        private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken cancellationToken)
        {
            var existingMovie = await _movieRepository.GetBySlugAsync(movie.Slug);

            return existingMovie == null || existingMovie.Id == movie.Id;
        }
    }
}
