using System;
using System.Configuration;
using System.Windows.Forms;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Security;

namespace SEQB
{
    public partial class MainForm : Form
    {
        private readonly string _appToken = ConfigurationManager.AppSettings["appToken"];
        private readonly string _accessTokenSecret = ConfigurationManager.AppSettings["appToken"];
        private readonly string _consumerKey = ConfigurationManager.AppSettings["OAuthConsumerKey"];
        private readonly string _consumerSecret = ConfigurationManager.AppSettings["OAuthConsumerSecret"];

        public MainForm()
        {
            InitializeComponent();
            try
            {
                var oauthValidator = new OAuthRequestValidator(_appToken,_accessTokenSecret,_consumerKey,_consumerSecret);
                var context = new ServiceContext(_appToken, IntuitServicesType.QBD, oauthValidator);
                var conn = new DataService(context);

                var invoiceItemLine = new Line
                {
                    Id = "1",
                    Description = "Description",
                    Amount = 10m,
                    AmountSpecified = true,
                    DetailType = LineDetailTypeEnum.ItemReceiptLineDetail,
                    DetailTypeSpecified = true,
                    AnyIntuitObject = 456.78m,
                    LineNum = "1",

                };

                var lineDetail = new SalesItemLineDetail
                {
                    ServiceDate = DateTime.Now,
                    AnyIntuitObject = 10m,
                    ItemElementName = ItemChoiceType.UnitPrice,
                    Qty = 10,
                    QtySpecified = true,
                    TaxCodeRef = new ReferenceType()
                    {
                        Value = "TAX"
                    },
                    TaxInclusiveAmt = 1.25m,
                    TaxInclusiveAmtSpecified = true
                };
                
                invoiceItemLine.AnyIntuitObject = lineDetail;

                var invoice = new Invoice
                {
                    Id = "1",
                    Deposit = 0,
                    DueDate = DateTime.Now.AddMonths(1),
                    domain = idDomainEnum.QBSDK.ToString(),
                    sparse = false,
                    SyncToken = "0",
                    MetaData = new ModificationMetaData
                    {
                        CreateTime = DateTime.Now,
                        LastUpdatedTime = DateTime.Now
                    },
                    BillAddr = new PhysicalAddress
                    {
                        Id = "1",
                        Line1 = "Russ Sonnenschein",
                        Line2 = "Sonnenschein Family Store",
                        Line3 = "5647 Cypress Hill Ave.",
                        Line4 = "Middlefield, CA  94303"
                    },
                    ShipAddr = new PhysicalAddress
                    {
                        Id = "1",
                        Line1 = "5647 Cypress Hill Ave.",
                        City = "Middlefield",
                        CountrySubDivisionCode = "CA",
                        PostalCode = "94303"
                    },
                    SalesTermRef = new ReferenceType
                    {
                        Value = "3"
                    },
                    TotalAmt = 362.07M,
                    ApplyTaxAfterDiscount = false,
                    PrintStatus = PrintStatusEnum.NotSet,
                    EmailStatus = EmailStatusEnum.NotSet,
                    BillEmail = new EmailAddress
                    {
                        Address = "amazure@wdsystems.com"
                    },
                    Balance = 362.07M,
                    Line = new[] {invoiceItemLine}
                };


                conn.Add(invoice);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
/*
 {
  "Line": [
    {
      "Amount": 100.00,
      "DetailType": "SalesItemLineDetail",
      "SalesItemLineDetail": {
        "ItemRef": {
          "value": "1",
          "name": "Services"
        }
      }
    }
  ],
  "CustomerRef": {
    "value": "1"
  }

    {
    "BillAddr": {
        "Line1": "123 Main Street",
        "City": "Mountain View",
        "Country": "USA",
        "CountrySubDivisionCode": "CA",
        "PostalCode": "94042"
    },
    "Notes": "Here are other details.",
    "Title": "Mr",
    "GivenName": "James",
    "MiddleName": "B",
    "FamilyName": "King",
    "Suffix": "Jr",
    "FullyQualifiedName": "King Groceries",
    "CompanyName": "King Groceries",
    "DisplayName": "King's Groceries",
    "PrimaryPhone": {
        "FreeFormNumber": "(555) 555-5555"
    },
    "PrimaryEmailAddr": {
        "Address": "jdrew@myemail.com"
    }
}

} 
 */
