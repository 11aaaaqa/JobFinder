using Web.MVC.Models.ApiResponses.Review;

namespace Web.MVC.Models.View_models
{
    public class GetEmployeeCompanyReviews
    {
        public List<ReviewResponse> Reviews { get; set; }
        public int CurrentPageNumber { get; set; }
        public bool IsNextPageExisted { get; set; }
    }
}
