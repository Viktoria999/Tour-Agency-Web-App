using Domain.Models;

namespace DAL
{
    public interface IAccountRepository
    {
        public void Insert(Account account);
        public Account? GetByUserName(string userName);
        public void Edit(Account account);
    }
}
