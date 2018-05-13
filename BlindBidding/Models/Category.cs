using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBidding.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int? SubcategoryOfId { get; set; }
        [ForeignKey("SubcategoryOfId")]
        public virtual Category SubcategoryOf { get; set; }
        public virtual IEnumerable<Category> Subcategories { get; set; }
    }
}
