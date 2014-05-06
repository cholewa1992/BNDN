using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    /// <summary>
    /// This class is the only public concrete class in the BusinessLogicLayer.
    /// It gives access to the different concrete factories in the abstract factory pattern (BusinessLogicFactory and
    /// TestFactory).
    /// </summary>
    public static class BusinessLogicEntryFactory
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
