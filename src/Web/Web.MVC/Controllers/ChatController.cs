using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;
using GeneralLibrary.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Models.ApiResponses;
using Web.MVC.Models.ApiResponses.Chat;
using Web.MVC.Models.ApiResponses.Company;
using Web.MVC.Models.ApiResponses.Employer;
using Web.MVC.Models.View_models;

namespace Web.MVC.Controllers
{
    public class ChatController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;
        public ChatController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            url = $"{configuration["Url:Protocol"]}://{configuration["Url:Domain"]}";
        }

        [Authorize]
        [HttpGet]
        [Route("chats")]
        public async Task<IActionResult> GetChatsView()
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var accountType = User.FindFirst(ClaimTypeConstants.AccountTypeClaimName).Value;
            var model = new GetChatsViewModel();
            switch (accountType)
            {
                case AccountTypeConstants.Employee:
                    var employeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
                    employeeResponse.EnsureSuccessStatusCode();
                    var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployerResponse>();
                    model.EmployeeId = employee.Id;
                    break;
                case AccountTypeConstants.Employer:
                    var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
                    employerResponse.EnsureSuccessStatusCode();
                    var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();
                    model.EmployerId = employer.Id;
                    break;
            }

            return View(model);
        }

        [Authorize]
        [Route("employer/chats")]
        public async Task<IActionResult> GetJsonEmployerChats(Guid employerId, string? query, int pageNumber)
        {
            var encodedQuery = HttpUtility.UrlEncode(query);
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var currentEmployerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            currentEmployerResponse.EnsureSuccessStatusCode();
            var employer = await currentEmployerResponse.Content.ReadFromJsonAsync<EmployerResponse>();
            if (employer.Id != employerId)
                return StatusCode((int)HttpStatusCode.Forbidden);

            var chatsResponse = await httpClient.GetAsync(
                $"{url}/api/Chat/GetChatListByEmployerId/{employerId}?searchingQuery={encodedQuery}&pageNumber={pageNumber}");
            chatsResponse.EnsureSuccessStatusCode();
            var chats = await chatsResponse.Content.ReadFromJsonAsync<List<ChatResponse>>();
            return new JsonResult(chats);
        }

        [Authorize]
        [Route("employee/chats")]
        public async Task<IActionResult> GetJsonEmployeeChats(Guid employeeId, string? query, int pageNumber)
        {
            var encodedQuery = HttpUtility.UrlEncode(query);
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var currentEmployeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
            currentEmployeeResponse.EnsureSuccessStatusCode();
            var employee = await currentEmployeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();
            if (employee.Id != employeeId)
                return StatusCode((int)HttpStatusCode.Forbidden);

            var chatsResponse = await httpClient.GetAsync(
                $"{url}/api/Chat/GetChatListByEmployeeId/{employeeId}?searchingQuery={encodedQuery}&pageNumber={pageNumber}");
            chatsResponse.EnsureSuccessStatusCode();
            var chats = await chatsResponse.Content.ReadFromJsonAsync<List<ChatResponse>>();
            return new JsonResult(chats);
        }

        [Authorize]
        [HttpGet]
        [Route("chats/{chatId}")] //sensible route
        public async Task<IActionResult> GetChatById(Guid chatId)
        {
            GetChatByIdViewModel model = new GetChatByIdViewModel{ChatId = chatId};
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var chatResponse = await httpClient.GetAsync($"{url}/api/Chat/GetChatByChatId/{chatId}");
            chatResponse.EnsureSuccessStatusCode();
            var chat = await chatResponse.Content.ReadFromJsonAsync<ChatResponse>();

            Guid? employeeId = null;
            Guid? employerId = null;
            var accountType = User.FindFirst(ClaimTypeConstants.AccountTypeClaimName).Value;
            switch (accountType)
            {
                case AccountTypeConstants.Employee:
                    var employeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
                    employeeResponse.EnsureSuccessStatusCode();
                    var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();
                    employeeId = employee.Id;
                    model.CurrentId = employee.Id;
                    model.CurrentEmail = employee.Email;

                    var receiverEmployerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerById/{chat.EmployerId}");
                    receiverEmployerResponse.EnsureSuccessStatusCode();
                    var receiverEmployer = await receiverEmployerResponse.Content.ReadFromJsonAsync<EmployerResponse>();
                    model.ReceiverEmail = receiverEmployer.Email;
                    model.InterlocutorFullName = receiverEmployer.Name + " " + receiverEmployer.Surname;

                    var employerCompanyResponse = await httpClient.GetAsync($"{url}/api/Company/GetCompanyByCompanyId/{receiverEmployer.CompanyId}");
                    employerCompanyResponse.EnsureSuccessStatusCode();
                    var company = await employerCompanyResponse.Content.ReadFromJsonAsync<CompanyResponse>();
                    model.InterlocutorCompanyName = company.CompanyName;
                    break;
                case AccountTypeConstants.Employer:
                    var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
                    employerResponse.EnsureSuccessStatusCode();
                    var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();
                    employerId = employer.Id;
                    model.CurrentId = employer.Id;
                    model.CurrentEmail = employer.Email;

                    var receiverEmployeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeById/{chat.EmployeeId}");
                    receiverEmployeeResponse.EnsureSuccessStatusCode();
                    var receiverEmployee = await receiverEmployeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();
                    model.ReceiverEmail = receiverEmployee.Email;
                    model.InterlocutorFullName = receiverEmployee.Name + " " + receiverEmployee.Surname;
                    break;
            }

            if (chat.EmployeeId != employeeId && chat.EmployerId != employerId)
                return StatusCode((int)HttpStatusCode.Forbidden);

            return View(model);
        }

        [Authorize]
        [Route("chat/{chatId}/get-messages")]
        public async Task<IActionResult> GetJsonChatMessages(Guid chatId, int pageNumber)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var chatMessagesResponse = await httpClient.GetAsync($"{url}/api/Message/GetLastMessagesByChatId/{chatId}?pageNumber={pageNumber}");
            chatMessagesResponse.EnsureSuccessStatusCode();
            var chatMessages = await chatMessagesResponse.Content.ReadFromJsonAsync<Queue<MessageResponse>>();

            return new JsonResult(chatMessages);
        }

        [HttpPost]
        [Authorize]
        [Route("chat/create")]
        public async Task<IActionResult> CreateChat(Guid employeeId, Guid employerId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeById/{employeeId}");
            employeeResponse.EnsureSuccessStatusCode();
            var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerById/{employerId}");
            employerResponse.EnsureSuccessStatusCode();
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var accountType = User.FindFirst(ClaimTypeConstants.AccountTypeClaimName).Value;
            if (accountType == AccountTypeConstants.Employee)
            {
                if (User.Identity.Name != employee.Email)
                    return StatusCode((int)HttpStatusCode.Forbidden);
            }
            else
            {
                if (User.Identity.Name != employer.Email)
                    return StatusCode((int)HttpStatusCode.Forbidden);
            }

            var chatResponse = await httpClient.GetAsync($"{url}/api/Chat/GetChat?employeeId={employeeId}&employerId={employerId}");
            if (chatResponse.IsSuccessStatusCode)
            {
                var chat = await chatResponse.Content.ReadFromJsonAsync<ChatResponse>();
                return RedirectToAction("GetChatById", new { chatId = chat.Id });
            }

            if (chatResponse.StatusCode != HttpStatusCode.NotFound)
                return StatusCode((int)HttpStatusCode.InternalServerError);

            Guid chatId = Guid.NewGuid();
            using StringContent jsonContent = new(JsonSerializer.Serialize(new
            {
                Id = chatId,
                EmployeeId = employee.Id,
                EmployeeFullName = employee.Name + " " + employee.Surname,
                EmployerId = employer.Id,
                EmployerFullName = employer.Name + " " + employer.Surname
            }), Encoding.UTF8, "application/json");
            var createChatResponse = await httpClient.PostAsync($"{url}/api/Chat/CreateChat", jsonContent);
            createChatResponse.EnsureSuccessStatusCode();

            return RedirectToAction("GetChatById", new { chatId });
        }
    }
}
