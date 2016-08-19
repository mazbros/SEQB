using System;
using System.Configuration;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Globalization;

namespace SEQB
{
    public partial class MainForm : Form
    {
        //private readonly string _appToken = ConfigurationManager.AppSettings["appToken"];
        //private readonly string _accessTokenSecret = ConfigurationManager.AppSettings["appToken"];
        //private readonly string _consumerKey = ConfigurationManager.AppSettings["OAuthConsumerKey"];
        //private readonly string _consumerSecret = ConfigurationManager.AppSettings["OAuthConsumerSecret"];

        //private List<string> familyGroups = new List<string>() { "g1", "g2", "g3" };

        private int _totalQty;
        private double _totalTax;
        private double _totalAmount;

        public MainForm()
        {
            InitializeComponent();

            MaximumSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            FillcbFamilyGroup();

            try
            {
                #region Commented
                //var oauthValidator = new OAuthRequestValidator(_appToken,_accessTokenSecret,_consumerKey,_consumerSecret);
                //var context = new ServiceContext(_appToken, IntuitServicesType.QBD, oauthValidator);
                //var conn = new DataService(context);

                //var invoiceItemLine = new Line
                //{
                //    Id = "1",
                //    Description = "Description",
                //    Amount = 10m,
                //    AmountSpecified = true,
                //    DetailType = LineDetailTypeEnum.ItemReceiptLineDetail,
                //    DetailTypeSpecified = true,
                //    AnyIntuitObject = 456.78m,
                //    LineNum = "1",

                //};

                //var lineDetail = new SalesItemLineDetail
                //{
                //    ServiceDate = DateTime.Now,
                //    AnyIntuitObject = 10m,
                //    ItemElementName = ItemChoiceType.UnitPrice,
                //    Qty = 10,
                //    QtySpecified = true,
                //    TaxCodeRef = new ReferenceType()
                //    {
                //        Value = "TAX"
                //    },
                //    TaxInclusiveAmt = 1.25m,
                //    TaxInclusiveAmtSpecified = true
                //};

                //invoiceItemLine.AnyIntuitObject = lineDetail;

                //var invoice = new Invoice
                //{
                //    Id = "1",
                //    Deposit = 0,
                //    DueDate = DateTime.Now.AddMonths(1),
                //    domain = idDomainEnum.QBSDK.ToString(),
                //    sparse = false,
                //    SyncToken = "0",
                //    MetaData = new ModificationMetaData
                //    {
                //        CreateTime = DateTime.Now,
                //        LastUpdatedTime = DateTime.Now
                //    },
                //    BillAddr = new PhysicalAddress
                //    {
                //        Id = "1",
                //        Line1 = "Russ Sonnenschein",
                //        Line2 = "Sonnenschein Family Store",
                //        Line3 = "5647 Cypress Hill Ave.",
                //        Line4 = "Middlefield, CA  94303"
                //    },
                //    ShipAddr = new PhysicalAddress
                //    {
                //        Id = "1",
                //        Line1 = "5647 Cypress Hill Ave.",
                //        City = "Middlefield",
                //        CountrySubDivisionCode = "CA",
                //        PostalCode = "94303"
                //    },
                //    SalesTermRef = new ReferenceType
                //    {
                //        Value = "3"
                //    },
                //    TotalAmt = 362.07M,
                //    ApplyTaxAfterDiscount = false,
                //    PrintStatus = PrintStatusEnum.NotSet,
                //    EmailStatus = EmailStatusEnum.NotSet,
                //    BillEmail = new EmailAddress
                //    {
                //        Address = "amazure@wdsystems.com"
                //    },
                //    Balance = 362.07M,
                //    Line = new[] {invoiceItemLine}
                //};


                //conn.Add(invoice); 
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillcbFamilyGroup()
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                var cmd = new SqlCommand(@"
                    select FamilyGroup from InventoryItem
                    where FamilyGroup <> ''
                    group by FamilyGroup", conn);
                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                cbFamilyGroup.DataSource = dt;
                cbFamilyGroup.ValueMember = "FamilyGroup";
                cbFamilyGroup.DisplayMember = "FamilyGroup";
            }
                
        }

        private void FilllvInventories()
        {
            lvInventories.Items.Clear();
            lvInventories.View = View.Details;
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                var cmd = new SqlCommand("FamilyGroupItemsForQBInvoice", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@FG", cbFamilyGroup.Text));
                cmd.Parameters.Add(new SqlParameter("@date", dtDate.Value));
                cmd.Parameters.Add(new SqlParameter("@TQty", DbType.Int16));
                cmd.Parameters.Add(new SqlParameter("@TTax", SqlDbType.VarChar, 16));
                cmd.Parameters.Add(new SqlParameter("@TAmount", SqlDbType.VarChar, 16));
                cmd.Parameters["@TQty"].Direction = ParameterDirection.Output;
                cmd.Parameters["@TTax"].Direction = ParameterDirection.Output;
                cmd.Parameters["@TAmount"].Direction = ParameterDirection.Output;
                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    var item = new ListViewItem(row[0].ToString());
                    for (var i = 1; i < dt.Columns.Count; i++)
                    {
                        item.SubItems.Add(row[i].ToString());
                    }
                    lvInventories.Items.Add(item);
                }
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                conn.Close();

                _totalQty = int.Parse(cmd.Parameters["@TQty"].Value.ToString());
                _totalTax = double.Parse(cmd.Parameters["@TTax"].Value.ToString());
                _totalAmount = double.Parse(cmd.Parameters["@TAmount"].Value.ToString());

                lblQty.Text = _totalQty.ToString();
                lblTax.Text = _totalTax.ToString("C2", CultureInfo.CurrentCulture);
                lblAmount.Text = _totalAmount.ToString("C2", CultureInfo.CurrentCulture);
            }

            SizeLastColumn(lvInventories);

            btnCreateInvoice.Enabled = lvInventories.Items.Count > 0;
        }

        private void btnViewInventories_Click(object sender, EventArgs e)
        {
            lvInventories.Items.Clear();
            FilllvInventories();
        }

        private void cbFamilyGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvInventories.Items.Clear();
            FilllvInventories();
        }

        private void dtDate_ValueChanged(object sender, EventArgs e)
        {
            lvInventories.Items.Clear();
            FilllvInventories();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lvInventories.View = View.Details;
            lvInventories.Columns.AddRange(new[] { LineNum, Id, PartNumber, FamilyGroup, Description, Qty, UnitPrice, Tax, Amount, Dummy });
            SizeLastColumn(lvInventories);
        }
        private void SizeLastColumn(ListView lv)
        {
            lv.Columns[lv.Columns.Count - 1].Width = -2;
        }
        private void btnCreateInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            SizeLastColumn(lvInventories);
        }

        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            SizeLastColumn(lvInventories);
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            SizeLastColumn(lvInventories);
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
