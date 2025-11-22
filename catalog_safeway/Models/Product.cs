namespace catalog_safeway.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string? Description { get; set; }
        public string? Code { get; set; }
        //public string? ImagePath { get; set; }
        public byte[]? ImagePath { get; set; }
    }
}
