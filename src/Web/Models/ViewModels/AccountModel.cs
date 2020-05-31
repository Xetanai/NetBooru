using NetBooru.Data;

namespace NetBooru.Web.Models
{
    public class AccountModel
    {
        private User _user = null!;

        public string username => _user.UserName;

        public AccountModel(User user)
        {
            _user = user;
        }
    }
}
