using Microsoft.AspNetCore.Mvc;

namespace Web.MVC.Controllers
{
    public class InformationController : Controller
    {
        [Route("forbidden")]
        public IActionResult AccessForbidden()
        {
            return View();
        }

        [Route("not-found")]
        public IActionResult PageNotFound()
        {
            return View();
        }

        [Route("server-error")]
        public IActionResult ServerError()
        {
            return View();
        }

        [Route("bad-request")]
        public IActionResult BadRequest()
        {
            return View();
        }
    }
}
