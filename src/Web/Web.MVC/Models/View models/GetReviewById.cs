using Web.MVC.Models.ApiResponses;
using Web.MVC.Models.ApiResponses.Review;

namespace Web.MVC.Models.View_models
{
    public class GetReviewById
    {
        public EmployeeResponse? Employee { get; set; }
        public ReviewResponse Review { get; set; }
        public string ReturnUrl { get; set; }
    }
}
