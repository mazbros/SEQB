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
            var result = MainForm.DeleteInvoiceByNumber("3586");
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void DeleteInvoiceAndUpdatePackages()
        {
            var result = MainForm.DeleteInvoiceAndUpdatePackages("3586");
            Assert.IsTrue(result);
        }
    }
}