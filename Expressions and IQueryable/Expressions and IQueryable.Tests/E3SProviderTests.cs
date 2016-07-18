using System;
using System.Configuration;
using System.Linq;
using ExpressionsAndIQueryable.E3S;
using ExpressionsAndIQueryable.E3S.E3SClient;
using ExpressionsAndIQueryable.E3S.E3SClient.Entities;
using NUnit.Framework;

namespace ExpressionsAndIQueryable.Tests
{
    [TestFixture]
    public class E3SProviderTests
    {
        [Test]
        public void WithoutProvider()
        {
            var client = new E3SQueryClient(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);
            var res = client.SearchFTS<EmployeeEntity>("workstation:(EPRUIZHW0249)", 0, 1);

            foreach (var emp in res)
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }
        }

        [Test]
        public void WithoutProviderNonGeneric()
        {
            var client = new E3SQueryClient(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);
            var res = client.SearchFTS(typeof(EmployeeEntity), "workstation:(EPRUIZHW0249)", 0, 10);

            foreach (var emp in res.OfType<EmployeeEntity>())
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }
        }


        [Test]
        public void WithProvider()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            var resultEmployees = employees.Where(e => e.workstation == "EPRUIZHW0249");

            Assert.IsTrue(resultEmployees.AsEnumerable().Any());

            foreach (var emp in resultEmployees)
            {
                Assert.IsTrue(emp.workstation == "EPRUIZHW0249");
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }
        }

        [Test]
        public void WithProviderReverse()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            var resultEmployees = employees.Where(e => "EPRUIZHW0249" == e.workstation);

            Assert.IsTrue(resultEmployees.AsEnumerable().Any());

            foreach (var emp in resultEmployees)
            {
                Assert.IsTrue(emp.workstation == "EPRUIZHW0249");
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }
        }

        [Test]
        public void WithProviderStartsWithConstant()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            var resultEmployees = employees.Where(e => e.workstation.StartsWith("EPRUIZHW024"));

            Assert.IsTrue(resultEmployees.AsEnumerable().Any());

            foreach (var emp in resultEmployees)
            {
                Assert.IsTrue(emp.workstation.StartsWith("EPRUIZHW024"));
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }
        }

        [Test]
        public void WithProviderEndsWithConstant()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            var resultEmployees = employees.Where(e => e.workstation.EndsWith("IZHW0249"));

            Assert.IsTrue(resultEmployees.AsEnumerable().Any());

            foreach (var emp in resultEmployees)
            {
                Assert.IsTrue(emp.workstation.EndsWith("IZHW0249"));
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }
        }

        [Test]
        public void WithProviderContainsConstant()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            var resultEmployees = employees.Where(e => e.workstation.Contains("IZHW024"));

            Assert.IsTrue(resultEmployees.AsEnumerable().Any());

            foreach (var emp in resultEmployees)
            {
                Assert.IsTrue(emp.workstation.Contains("IZHW024"));
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }
        }

        [Test]
        public void WithProviderAND()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            var resultEmployees = employees.Where(e => e.workstation.StartsWith("EPRUIZHW024")).Where(e => e.startworkdate == "2010-08-30");

            Assert.IsTrue(resultEmployees.AsEnumerable().Any());

            foreach (var emp in resultEmployees)
            {
                Assert.IsTrue(emp.workstation.StartsWith("EPRUIZHW024") && emp.startworkdate == "2010-08-30");
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }
        }
    } 
}
