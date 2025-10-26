using Web.MVC.Models.ApiResponses.Review;

namespace Web.MVC.Models.View_models
{
    public class GetReviewsByCompanyId
    {
        public List<ReviewResponse> Reviews { get; set; }
        public Guid CompanyId { get; set; }
        public double GeneralCompanyEstimation { get; set; }
        public bool IsNextPageExisted { get; set; }
        public int CurrentPageNumber { get; set; }
    }
}
