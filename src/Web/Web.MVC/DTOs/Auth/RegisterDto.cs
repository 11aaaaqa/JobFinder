using System.ComponentModel.DataAnnotations;
using Uri = Web.MVC.Models.Uri;

namespace Web.MVC.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Поле \"Имя\" обязательно")]
        [Display(Name = "Имя")]
        [StringLength(30, ErrorMessage = "Максимальная длина поля \"Имя\" - 30 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле \"Фамилия\" обязательно")]
        [Display(Name = "Фамилия")]
        [StringLength(30, ErrorMessage = "Максимальная длина поля \"Фамилия\" - 30 символов")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Поле \"Адрес эл. почты\" обязательно")]
        [Display(Name = "Адрес эл. почты")]
        [DataType(DataType.EmailAddress)]
        [StringLength(80, ErrorMessage = "Максимальная длина поля \"Адрес эл. почты\" - 80 символов")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле \"Пароль\" обязательно")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Минимальная длина пароля - 8 символов")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле \"Подтвердите пароль\" обязательно")]
        [Display(Name = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Минимальная длина пароля - 8 символов")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string AccountType { get; set; }

        public Uri? ConfirmEmailMethodUri { get; set; }
    }
}
