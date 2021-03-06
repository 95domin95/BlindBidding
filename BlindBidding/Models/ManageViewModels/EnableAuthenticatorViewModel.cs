﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BlindBidding.Models.ManageViewModels
{
    public class EnableAuthenticatorViewModel
    {
            [Required]
            [StringLength(7, ErrorMessage = "{0} musi mieć conajmniej {2} znaków i maksymalnie {1}.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Kod potwierdzający")]
            public string Code { get; set; }

            [BindNever]
            public string SharedKey { get; set; }

            [BindNever]
            public string AuthenticatorUri { get; set; }
    }
}
