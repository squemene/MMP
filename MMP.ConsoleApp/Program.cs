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
            using (var ctx = new MMPModel.MMPEntities())
            {
                ToolsLibrary.Logger.Debug("" + ctx.People.Count());
            }
        }
    }
}
