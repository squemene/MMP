using MMPModel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolsLibrary;

namespace MMP.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var svcProvider = new ServiceFactory();
            var userSvc = svcProvider.Get<UserService>();
            foreach (var user in userSvc.GetUsers())
            {
                Logger.Debug(user.ToString());
            }
        }
    
    }
}
