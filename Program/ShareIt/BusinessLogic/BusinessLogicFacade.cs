using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public static class BusinessLogicFacade
    {
        public static IBusinessLogicFactory GetTestFactory()
        {
            return new TestFactory();
        }

        public static IBusinessLogicFactory GetBusinessFactory()
        {
            return new BusinessFactory();
        }
    }
}
