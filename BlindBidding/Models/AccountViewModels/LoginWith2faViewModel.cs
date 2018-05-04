using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBidding.Models.AccountViewModels
{
    public class LoginWith2faViewModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "{0} musi mieć conajmniej {2} znaków i maksymalnie {1}.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Kod potwierdzający")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "Zapamiętaj to urządzenie")]
        public bool RememberMachine { get; set; }

        public bool RememberMe { get; set; }
    }
}
