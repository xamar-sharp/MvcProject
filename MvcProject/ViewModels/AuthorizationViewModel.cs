using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
namespace MvcProject.ViewModels
{
    public class AuthorizationViewModel
    {
        [Required(ErrorMessage ="Email_Required")]
        [EmailAddress(ErrorMessage ="Email_Structure")]
        [BindingBehavior(BindingBehavior.Required)]
        [Display(Name ="Email_Display")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Password_Required")]
        [BindRequired]
        [StrictStringLength(5,ErrorMessage ="password length must be 5")]
        [DataType(DataType.Password)]
        [Display(Name ="Password_Display")]
        public string Password { get; set; }
        
    }
}
