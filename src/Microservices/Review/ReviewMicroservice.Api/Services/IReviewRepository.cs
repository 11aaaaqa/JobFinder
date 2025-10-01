using ReviewMicroservice.Api.Models;

namespace ReviewMicroservice.Api.Services
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetReviewsByEmployeeIdAsync(Guid employeeId, int pageNumber);
        Task<List<Review>> GetReviewsByCompanyIdAsync(Guid companyId, int pageNumber);
        Task<Review> GetByIdAsync(Guid reviewId);
        Task AddReviewAsync(Review review);
        Task DeleteReviewAsync(Guid reviewId);
    }
}
