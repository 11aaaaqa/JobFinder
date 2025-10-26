using Web.MVC.Models.ApiResponses.Company;
using Web.MVC.Models.ApiResponses.Vacancy;

namespace Web.MVC.Models.View_models
{
    public class GetVacancyViewModel
    {
        public VacancyResponse Vacancy { get; set; }
        public CompanyResponse Company { get; set; }
    }
}
