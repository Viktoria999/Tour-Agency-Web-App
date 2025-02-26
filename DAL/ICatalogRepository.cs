using DAL.Filters;
using Domain.Models;

namespace DAL
{
    public interface ICatalogRepository
    {
        public IEnumerable<CatalogItem> Select(CatalogItemFilter filter);
        public void Insert(CatalogItem item);
        public void Edit(CatalogItem item);
        public void Delete(int itemId);
    }
}