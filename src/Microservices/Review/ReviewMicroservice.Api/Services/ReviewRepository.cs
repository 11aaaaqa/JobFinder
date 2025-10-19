using Microsoft.EntityFrameworkCore;
using ReviewMicroservice.Api.Constants;
using ReviewMicroservice.Api.Database;
using ReviewMicroservice.Api.Models;

namespace ReviewMicroservice.Api.Services
{
    public class ReviewRepository(ApplicationDbContext context) : IReviewRepository
    {
        public async Task<List<Review>> GetAllReviewsByCompanyIdAsync(Guid companyId)
        {
            return await context.Reviews.Where(x => x.CompanyId == companyId).ToListAsync();
        }

        public async Task<List<Review>> GetAllReviewsByEmployeeIdAsync(Guid employeeId)
        {
            return await context.Reviews.Where(x => x.EmployeeId == employeeId).ToListAsync();
        }

        public async Task<List<Review>> GetReviewsByEmployeeIdPaginationAsync(Guid employeeId, int pageNumber)
        {
            var reviews = await context.Reviews
                .Where(x => x.EmployeeId == employeeId)
                .Skip((pageNumber - 1) * PaginationConstants.ReviewsPageSize)
                .Take(PaginationConstants.ReviewsPageSize)
                .ToListAsync();
            return reviews;
        }

        public async Task<List<Review>> GetReviewsByCompanyIdPaginationAsync(Guid companyId, int pageNumber)
        {
            var reviews = await context.Reviews
                .Where(x => x.CompanyId == companyId)
                .Skip((pageNumber - 1) * PaginationConstants.ReviewsPageSize)
                .Take(PaginationConstants.ReviewsPageSize)
                .ToListAsync();
            return reviews;
        }

        public async Task<Review> GetByIdAsync(Guid reviewId)
            => await context.Reviews.SingleAsync(x => x.Id == reviewId);

        public async Task AddReviewAsync(Review review)
        {
            await context.Reviews.AddAsync(review);
            await context.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(Guid reviewId)
        {
            var reviewToDelete = await context.Reviews.SingleOrDefaultAsync(x => x.Id == reviewId);
            if (reviewToDelete == null)
                return;
            context.Reviews.Remove(reviewToDelete);
            await context.SaveChangesAsync();
        }
    }
}
