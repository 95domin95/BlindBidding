using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBidding.Models.ManageViewModels
{
    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "{0} musi mieć conajmniej {2} znaków i maksymalnie {1}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nowe hasło")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź nowe hasło")]
        [Compare("NewPassword", ErrorMessage = "Hasła muszą być identyczne.")]
        public string ConfirmPassword { get; set; }

        public string StatusMessage { get; set; }
    }
}
