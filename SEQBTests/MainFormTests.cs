using Microsoft.VisualStudio.TestTools.UnitTesting;
using SEQB;

namespace SEQBTests
{
    [TestClass()]
    public class MainFormTests
    {
        [TestMethod()]
        public void DeleteInvoiceByNumberTest()
        {
            var result = MainForm.DeleteInvoiceByNumber("3462");
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void DeleteInvoiceAndUpdatePackages()
        {
            MainForm.DeleteInvoiceAndUpdatePackages("3462");
        }
    }
}