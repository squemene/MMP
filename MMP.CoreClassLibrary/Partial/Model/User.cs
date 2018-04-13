using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMPModel
{

    public partial class User
    {
        public override string ToString()
        {
            return String.Format("Id: {0} | Name: {1} | Email: {2} | CreditScore: {3} | WelcomeEmailSent: {4} | CreatedOn (UTC): {5}", Id, Name, Email, CreditScore, WelcomeEmailSent, CreatedOn.ToString("dd MMM yyyy - HH:mm:ss"));
        }
    }
}
