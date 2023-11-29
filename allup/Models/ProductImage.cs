namespace ALLUP2.Models
{
    public class ProductImage

    {
        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public bool? Isposter {  get; set; }
        public int ProductId {  get; set; }
        public Product Product { get; set; }
    }
}
