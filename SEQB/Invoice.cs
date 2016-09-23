namespace SEQB
{
    internal class Invoice
    {
        public string InvoiceNumber { get; set; }
        public int Qty { get; set; }
        public double Amount { get; set; }
        public double Tax { get; set; }
        public string ShipDate { get; set; }
        public string TrxnDate { get; set; }
    }
}