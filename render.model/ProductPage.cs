using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace render.model
{
    public class ProductPage : RenderSpec
    {
        public ProductPage() {
            Template = "product.cshtml";
        }

        public string Url { get; set; }
        public string Title { get; set; }

    }
}
