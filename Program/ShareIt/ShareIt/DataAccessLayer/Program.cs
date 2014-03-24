using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new RentIt08Entities())
            {
                db.UserAcc.Add( new UserAcc
                {
                    Username = "Jacob",
                    Password = "Thomassucks"
                });
                db.SaveChanges();
            }
        
        }
    }
}
