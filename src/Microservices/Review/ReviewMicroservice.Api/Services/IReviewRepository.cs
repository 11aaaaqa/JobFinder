using ReviewMicroservice.Api.Models;

namespace ReviewMicroservice.Api.Services
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetAllReviewsByCompanyIdAsync(Guid companyId);
        Task<List<Review>> GetAllReviewsByEmployeeIdAsync(Guid employeeId);
        Task<List<Review>> GetReviewsByEmployeeIdPaginationAsync(Guid employeeId, int pageNumber);
        Task<List<Review>> GetReviewsByCompanyIdPaginationAsync(Guid companyId, int pageNumber);
        Task<Review> GetByIdAsync(Guid reviewId);
        Task AddReviewAsync(Review review);
        Task DeleteReviewAsync(Guid reviewId);
    }
}
