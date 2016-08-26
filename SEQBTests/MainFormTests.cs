using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SEQB.Tests
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
    }
}