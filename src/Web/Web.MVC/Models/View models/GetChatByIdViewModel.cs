namespace Web.MVC.Models.View_models
{
    public class GetChatByIdViewModel
    {
        public Guid CurrentId { get; set; }
        public string CurrentEmail { get; set; }
        public Guid ChatId { get; set; }
        public string ReceiverEmail { get; set; }
        public string InterlocutorFullName { get; set; }
        public string? InterlocutorCompanyName { get; set; }
    }
}
