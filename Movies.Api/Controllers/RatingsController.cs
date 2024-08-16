using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;

namespace Movies.Api.Controllers
{
    [ApiController]
    [ApiVersion(1.0)]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingsController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [Authorize]
        [HttpPut(ApiEndpoints.Movies.Rate)]
        public async Task<IActionResult> Rate([FromRoute] Guid movieId, [FromBody] RateMovieRequest request, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();
            var success = await _ratingService.RateMovieAsync(movieId, userId!.Value, request.Rating, cancellationToken);

            return success ? Ok() : BadRequest();
        }

        [Authorize]
        [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
        public async Task<IActionResult> DeleteRating([FromRoute] Guid movieId, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();
            var success = await _ratingService.DeleteRatingAsync(movieId, userId!.Value, cancellationToken);

            return success ? Ok() : NotFound();
        }

        [Authorize]
        [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
        public async Task<IActionResult> GetUserRatings(CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();
            var ratings = await _ratingService.GetRatingsForUserAsync(userId!.Value, cancellationToken);

            return Ok(ratings.MapToResponse());
        }
    }
}
