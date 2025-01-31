using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Поле \"Адрес эл. почты\" обязательно")]
        [Display(Name = "Адрес эл. почты")]
        [DataType(DataType.EmailAddress)]
        [StringLength(80, ErrorMessage = "Максимальная длина поля \"Адрес эл. почты\" - 80 символов")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле \"Пароль\" обязательно")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [StringLength(50)]
        public string Password { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
