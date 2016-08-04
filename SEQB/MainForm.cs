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
                var invoice = new Invoice
                {
                    Deposit = 0,
                    DueDate = DateTime.Now.AddMonths(1),
                    Balance = (decimal)100.50D,
                    BillAddr = new PhysicalAddress
                    {
                        Line1 = "123 Some Str",
                        City = "City",
                        CountrySubDivisionCode = "NJ",
                        PostalCode = "00000",
                        Country = "USA"
                    },
                    Line = new Line[] { }
                };

                conn.Add(invoice);

            }
            catch (Exception ex)
            {
                //
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
