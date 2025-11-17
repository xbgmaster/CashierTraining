using catalog_safeway.Models;
namespace catalog_safeway.Services
{
    public class ProductService : IProductServices
    {
        public Product getRandomProduct(List<Product> products, bool flagCleanCode = false)
        {
            //Random value of list to print in first load
            var random = new Random();
            int index = random.Next(products.Count);
            var selectedProduct = products[index];
            if (flagCleanCode)
            {
                selectedProduct.Code = "";
            }
            return selectedProduct;
        }

        //public void cleanObject(Product model)
        //{
        //    ModelState.Remove(nameof(model.Id));
        //    ModelState.Remove(nameof(model.Description));
        //    ModelState.Remove(nameof(model.Code));
        //    ModelState.Remove(nameof(model.ImagePath));
        //}
    }
}
