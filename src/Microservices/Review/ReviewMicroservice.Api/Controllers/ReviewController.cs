using System.Text.Json;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using ReviewMicroservice.Api.DTOs;
using ReviewMicroservice.Api.Kafka.Producer;
using ReviewMicroservice.Api.Models;
using ReviewMicroservice.Api.Services;
using ReviewMicroservice.Api.Services.Pagination;

namespace ReviewMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController(IReviewRepository reviewRepository, IPaginationService paginationService, IKafkaProducer kafkaProducer) : ControllerBase
    {
        [HttpGet]
        [Route("GetAllReviewsByEmployeeId/{employeeId}")]
        public async Task<IActionResult> GetAllReviewsByEmployeeIdAsync(Guid employeeId)
            => Ok(await reviewRepository.GetAllReviewsByEmployeeIdAsync(employeeId));

        [HttpGet]
        [Route("GetAllReviewsByCompanyId/{companyId}")]
        public async Task<IActionResult> GetAllReviewsByCompanyIdAsync(Guid companyId)
            => Ok(await reviewRepository.GetAllReviewsByCompanyIdAsync(companyId));

        [HttpGet]
        [Route("GetReviewsByEmployeeIdPagination/{employeeId}")]
        public async Task<IActionResult> GetReviewsByEmployeeIdPaginationAsync(Guid employeeId, int pageNumber)
            => Ok(await reviewRepository.GetReviewsByEmployeeIdPaginationAsync(employeeId, pageNumber));

        [HttpGet]
        [Route("IsNextReviewsByEmployeeIdPageExisted/{employeeId}")]
        public async Task<IActionResult> IsNextReviewsByEmployeeIdPageExistedAsync(Guid companyId, int currentPageNumber)
            => Ok(await paginationService.IsNextReviewsByEmployeeIdPageExistedAsync(companyId, currentPageNumber));

        [HttpGet]
        [Route("GetReviewsByCompanyIdPagination/{companyId}")]
        public async Task<IActionResult> GetReviewsByCompanyIdPaginationAsync(Guid companyId, int pageNumber)
            => Ok(await reviewRepository.GetReviewsByCompanyIdPaginationAsync(companyId, pageNumber));

        [HttpGet]
        [Route("IsNextReviewsByCompanyIdPageExisted/{companyId}")]
        public async Task<IActionResult> IsNextReviewsByCompanyIdPageExistedAsync(Guid companyId, int currentPageNumber)
            => Ok(await paginationService.IsNextReviewsByCompanyIdPageExistedAsync(companyId, currentPageNumber));

        [HttpGet]
        [Route("GetReviewById/{reviewId}")]
        public async Task<IActionResult> GetReviewById(Guid reviewId)
            => Ok(await reviewRepository.GetByIdAsync(reviewId));

        [HttpPost]
        [Route("AddReview")]
        public async Task<IActionResult> AddReviewAsync([FromBody] AddReviewDto model)
        {
            double generalEstimation = (model.WorkingConditions + model.Colleagues + model.Management 
                                        + model.GrowthOpportunities + model.RestConditions + model.SalaryLevel) / 6d;
            generalEstimation = Math.Round(generalEstimation, 1);
            await reviewRepository.AddReviewAsync(new Review
            {
                WorkingConditions = model.WorkingConditions, Colleagues = model.Colleagues,
                Advantages = model.Advantages,
                CanBeImproved = model.CanBeImproved, City = model.City, CompanyId = model.CompanyId,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                EmployeeId = model.EmployeeId, GeneralEstimation = generalEstimation,
                GrowthOpportunities = model.GrowthOpportunities, Id = model.Id,
                Management = model.Management, RestConditions = model.RestConditions, SalaryLevel = model.SalaryLevel,
                Position = model.Position,
                WorkingState = model.WorkingState, WorkingTime = model.WorkingTime
            });

            var companyReviews = await reviewRepository.GetAllReviewsByCompanyIdAsync(model.CompanyId);
            double companyEstimation = companyReviews.Sum(companyReview => companyReview.GeneralEstimation) / companyReviews.Count;

            await kafkaProducer.ProduceAsync("company-rating-updated-topic", new Message<Null, string> { Value = JsonSerializer.Serialize(new
            {
                NewCompanyRating = companyEstimation,
                model.CompanyId
            }) });
            return Ok();
        }

        [HttpDelete]
        [Route("RemoveReview/{reviewId}")]
        public async Task<IActionResult> RemoveReviewAsync(Guid reviewId)
        {
            var review = await reviewRepository.GetByIdAsync(reviewId);
            await reviewRepository.DeleteReviewAsync(reviewId);

            var companyReviews = await reviewRepository.GetAllReviewsByCompanyIdAsync(review.CompanyId);
            double companyEstimation = companyReviews.Sum(companyReview => companyReview.GeneralEstimation) / companyReviews.Count;

            await kafkaProducer.ProduceAsync("company-rating-updated-topic", new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(new
                {
                    NewCompanyRating = companyEstimation,
                    review.CompanyId
                })
            });

            return Ok();
        }
    }
}