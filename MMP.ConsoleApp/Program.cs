using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMP.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var ctx = new CoreClassLibrary.MMPEntities())
            {
                Console.WriteLine(ctx.Users.Count());
            }
        }
    }
}
