using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.WebUI.Infrastructure.Abstract
{
    public interface IAuthProvider
    {
        bool Autherticated(string username, string password);
    }
}
