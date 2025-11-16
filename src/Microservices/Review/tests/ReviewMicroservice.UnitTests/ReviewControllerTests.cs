using Microsoft.AspNetCore.Mvc;
using Moq;
using ReviewMicroservice.Api.Controllers;
using ReviewMicroservice.Api.DTOs;
using ReviewMicroservice.Api.Kafka.Producer;
using ReviewMicroservice.Api.Models;
using ReviewMicroservice.Api.Services;
using ReviewMicroservice.Api.Services.Pagination;

namespace ReviewMicroservice.UnitTests
{
    public class ReviewControllerTests
    {
        [Fact]
        public async Task GetReviewById_ReturnsReview()
        {
            var mock = new Mock<IReviewRepository>();
            Guid reviewId = Guid.NewGuid();
            mock.Setup(x => x.GetByIdAsync(reviewId)).ReturnsAsync(new Review { Id = reviewId });
            var controller = new ReviewController(mock.Object, new Mock<IPaginationService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetReviewById(reviewId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var review = Assert.IsType<Review>(methodResult.Value);
            Assert.Equal(reviewId, review.Id);
        }

        [Fact]
        public async Task AddReview_ReturnsOk()
        {
            Guid reviewId = Guid.NewGuid();
            int workingConditions = 4, colleagues = 4, management = 3;
            int growthOpportunities = 5, restConditions = 4, salaryLevel = 5;
            double generalEstimation = (workingConditions + colleagues + management + growthOpportunities + restConditions + salaryLevel) / 6d;
            generalEstimation = Math.Round(generalEstimation, 1);
            var addReviewModel = new AddReviewDto
            {
                Id = reviewId,  WorkingConditions = workingConditions, Colleagues = colleagues, Management = management, 
                GrowthOpportunities = growthOpportunities, RestConditions = restConditions, SalaryLevel = 5
            };
            var mock = new Mock<IReviewRepository>();
            var kafkaMock = new Mock<IKafkaProducer>();
            mock.Setup(x => x.GetAllReviewsByCompanyIdAsync(addReviewModel.CompanyId)).ReturnsAsync(new List<Review>{new Review
            {
                GeneralEstimation = generalEstimation
            }});
            var controller = new ReviewController(mock.Object, new Mock<IPaginationService>().Object, kafkaMock.Object);

            var result = await controller.AddReviewAsync(addReviewModel);

            Assert.IsType<OkResult>(result);
            mock.Verify(x => x.GetAllReviewsByCompanyIdAsync(addReviewModel.CompanyId));
            mock.VerifyAll();
            kafkaMock.VerifyAll();
        }

        [Fact]
        public async Task RemoveReview_ReturnsOk()
        {
            Guid reviewId = Guid.NewGuid();
            Guid companyId = Guid.NewGuid();
            var review = new Review { Id = reviewId, CompanyId = companyId };
            var reviewRepositoryMock = new Mock<IReviewRepository>();
            var kafkaMock = new Mock<IKafkaProducer>();
            reviewRepositoryMock.Setup(x => x.GetByIdAsync(reviewId)).ReturnsAsync(review);
            reviewRepositoryMock.Setup(x => x.DeleteReviewAsync(reviewId));
            reviewRepositoryMock.Setup(x => x.GetAllReviewsByCompanyIdAsync(review.CompanyId)).ReturnsAsync(new List<Review>());
            var controller = new ReviewController(reviewRepositoryMock.Object, new Mock<IPaginationService>().Object, kafkaMock.Object);

            var result = await controller.RemoveReviewAsync(reviewId);

            Assert.IsType<OkResult>(result);
            kafkaMock.VerifyAll();
            reviewRepositoryMock.Verify(x => x.GetByIdAsync(reviewId));
            reviewRepositoryMock.Verify(x => x.DeleteReviewAsync(reviewId));
            reviewRepositoryMock.Verify(x => x.GetAllReviewsByCompanyIdAsync(review.CompanyId));
            reviewRepositoryMock.VerifyAll();
        }
    }
}
