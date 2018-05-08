using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBidding.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public Category SubcategoryOf { get; set; }
    }
}
