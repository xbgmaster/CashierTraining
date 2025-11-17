using catalog_safeway.Models;

namespace catalog_safeway.Services
{
    public interface IProductServices
    {
        public Product getRandomProduct(List<Product> products, bool flagCleanCode = false);

        //public void cleanObject(Product model);
       
        }
}
