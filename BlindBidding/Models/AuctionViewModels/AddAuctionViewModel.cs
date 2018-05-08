using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBidding.Models.AuctionViewModels
{
    public class AddAuctionViewModel
    {
        [Required(ErrorMessage ="Pole wymagane")]
        [Display(Name = "Tytuł aukcji")]
        [StringLength(100, ErrorMessage = "{0} musi mieć conajmniej {2} znaków i maksymalnie {1}.", MinimumLength = 6)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Pole wymagane")]
        [Display(Name = "Krótki opis")]
        [StringLength(5000, ErrorMessage = "{0} musi mieć conajmniej {2} znaków i maksymalnie {1}.", MinimumLength = 6)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Pole wymagane")]
        [Display(Name = "Czas trwania(dni)")]
        [StringLength(1, ErrorMessage = "Maksymalny czas trwania 9 dni")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Niepoprawna ilość dni")]
        public string Duration { get; set; }

        [Display(Name = "Wybierz kategorię")]
        public string Category { get; set; }
        public IEnumerable<Category> CategoryList { get; set; }
    }
}
