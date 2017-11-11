using PeakUp.Integrations.Parasut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakUp.Integrations.Parasut.Terminal
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = new ParasutProvider();
            var accounts = provider.Accounts(provider.Client.CompanyId);
            foreach (var account in accounts)
            {
                Console.WriteLine($"{account.Attributes.Name}");
            }
            Console.ReadKey(true);
        }
    }
}
