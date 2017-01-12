using QBFC13Lib;
using System;
using System.Globalization;

namespace SEQB
{
    public class QBHelper
    {
        public static event EventHandler<QBMsgEventArgs> MsgEvent;

        private static readonly string NewLine = Environment.NewLine;
        private static readonly string Tab = "\t";

        private static void RaiseEvent(string msg)
        {
            MsgEvent?.Invoke(typeof(QBHelper), new QBMsgEventArgs(msg));
        }

        public static void EnableErrorRecovery(QBSessionManager sessionManager)
        {
            // ERROR RECOVERY: 
            // All steps are described in QBFC Developers Guide, on pg 41
            // under section titled "Automated Error Recovery"

            // (1) Set the error recovery ID using ErrorRecoveryID function
            //		Value must be in GUID format
            //	You could use c:\Program Files\Microsoft Visual Studio\Common\Tools\GuidGen.exe 
            //	to create a GUID for your unique ID
            var errecid = "{E74068B5-0D6D-454d-B0FD-BDDF93CE67C3}";
            sessionManager.ErrorRecoveryID.SetValue(errecid);

            // (2) Set EnableErrorRecovery to true to enable error recovery
            sessionManager.EnableErrorRecovery = true;

            // (3) Set SaveAllMsgSetRequestInfo to true so the entire contents of the MsgSetRequest
            //		will be saved to disk. If SaveAllMsgSetRequestInfo is false (default), only the 
            //		newMessageSetID will be saved. 
            sessionManager.SaveAllMsgSetRequestInfo = true;

            // (4) Use IsErrorRecoveryInfo to check whether an unprocessed response exists. 
            //		If IsErrorRecoveryInfo is true:
            if (sessionManager.IsErrorRecoveryInfo())
            {
                ProcessQBFCErrors(sessionManager);
            }

        }

        private static void ProcessQBFCErrors(QBSessionManager sessionManager)
        {
            //string reqXML;
            //string resXML;

            // a. Get the response status, using GetErrorRecoveryStatus
            var resMsgSet = sessionManager.GetErrorRecoveryStatus();
            // resXML = resMsgSet.ToXMLString();
            // MessageBox.Show(resXML);

            if (resMsgSet.Attributes.MessageSetStatusCode.Equals("600"))
            {
                // This case may occur when a transaction has failed after QB processed 
                // the request but client app didn't get the response and started with 
                // another company file.
                RaiseEvent("The oldMessageSetID does not match any stored IDs, and no newMessageSetID is provided.");
            }
            else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9001"))
            {
                RaiseEvent("Invalid checksum. The newMessageSetID specified, matches the currently stored ID, but checksum fails.");
            }
            else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9002"))
            {
                // Response was not successfully stored or stored properly
                RaiseEvent("No stored response was found.");
            }
            // 9003 = Not used
            else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9004"))
            {
                // MessageSetID is set with a string of size > 24 char
                RaiseEvent("Invalid MessageSetID, greater than 24 character was given.");
            }
            else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9005"))
            {
                RaiseEvent("Unable to store response.");
            }
            else
            {
                IResponse res = resMsgSet.ResponseList.GetAt(0);
                var sCode = res.StatusCode;
                //string sMessage = res.StatusMessage;
                //string sSeverity = res.StatusSeverity;
                //MessageBox.Show("StatusCode = " + sCode + NewLine + "StatusMessage = " + sMessage + NewLine + "StatusSeverity = " + sSeverity);

                if (sCode == 0)
                {
                    //RaiseEvent("Last request was processed and Invoice was added successfully!");
                }
                else if (sCode > 0)
                {
                    //RaiseEvent("There was a warning but last request was processed successfully!");
                }
                else
                {
                    RaiseEvent("It seems that there was an error in processing last request");
                    // b. Get the saved request, using GetSavedMsgSetRequest
                    var reqMsgSet = sessionManager.GetSavedMsgSetRequest();
                    //reqXML = reqMsgSet.ToXMLString();
                    //MessageBox.Show(reqXML);

                    // c. Process the response, possibly using the saved request
                    resMsgSet = sessionManager.DoRequests(reqMsgSet);
                    var resp = resMsgSet.ResponseList.GetAt(0);
                    var statCode = resp.StatusCode;
                    if (statCode == 0)
                    {
                        //var invRet = resp.Detail as IInvoiceRet;
                        //var resStr = "The following invoice has been successfully submitted to QuickBooks:\n\n\n";
                        //if (invRet?.TxnNumber != null)
                        //{
                        //    resStr += "Txn Number = " + Convert.ToString(invRet.TxnNumber.GetValue()) + NewLine;
                        //}
                    } // if (statusCode == 0)
                } // else (sCode)
            } // else (MessageSetStatusCode)

            // d. Clear the response status, using ClearErrorRecovery
            sessionManager.ClearErrorRecovery();
            //RaiseEvent("Proceeding with current transaction.");
        }

        public static IMsgSetRequest GetLatestMsgSetRequest(QBSessionManager sessionManager)
        {
            // Find and adapt to supported version of QuickBooks
            var supportedVersion = QBFCLatestVersion(sessionManager);

            short qbXMLMajorVer;
            short qbXMLMinorVer;

            if (supportedVersion >= 6.0)
            {
                qbXMLMajorVer = 6;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 5.0)
            {
                qbXMLMajorVer = 5;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 4.0)
            {
                qbXMLMajorVer = 4;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 3.0)
            {
                qbXMLMajorVer = 3;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 2.0)
            {
                qbXMLMajorVer = 2;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 1.1)
            {
                qbXMLMajorVer = 1;
                qbXMLMinorVer = 1;
            }
            else
            {
                qbXMLMajorVer = 1;
                qbXMLMinorVer = 0;
                RaiseEvent("It seems that you are running QuickBooks 2002 Release 1. We strongly recommend that you use QuickBooks' online update feature to obtain the latest fixes and enhancements");
            }

            // Create the message set request object
            IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", qbXMLMajorVer, qbXMLMinorVer);
            return requestMsgSet;
        }

        // Code for handling different versions of QuickBooks
        private static double QBFCLatestVersion(QBSessionManager SessionManager)
        {
            // Use oldest version to ensure that this application work with any QuickBooks (US)
            IMsgSetRequest msgset = SessionManager.CreateMsgSetRequest("US", 1, 0);
            msgset.AppendHostQueryRq();
            IMsgSetResponse QueryResponse = SessionManager.DoRequests(msgset);
            //MessageBox.Show("Host query = " + msgset.ToXMLString());
            //SaveXML(msgset.ToXMLString());

            // The response list contains only one response,
            // which corresponds to our single HostQuery request
            IResponse response = QueryResponse.ResponseList.GetAt(0);

            // Please refer to QBFC Developers Guide for details on why 
            // "as" clause was used to link this derrived class to its base class
            var hostResponse = response.Detail as IHostRet;
            double lastVers = 0;

            if (hostResponse != null)
            {
                var supportedVersions = hostResponse.SupportedQBXMLVersionList;

                int i;

                for (i = 0; i <= supportedVersions.Count - 1; i++)
                {
                    var svers = supportedVersions.GetAt(i);
                    var vers = Convert.ToDouble(svers, CultureInfo.GetCultureInfo("en-US"));
                    if (vers > lastVers)
                    {
                        lastVers = vers;
                    }
                }
            }
            return lastVers;
        }

        public static bool ShowRequestResult(QBSessionManager sessionManager, IMsgSetRequest requestMsgSet)
        {
            var responseMsgSet = sessionManager.DoRequests(requestMsgSet);

            //Uncomment the following to view and save the request and response XML
            /*
            var requestXML = requestMsgSet.ToXMLString();
            RaiseEvent(requestXML);
            //SaveXML(requestXML);
            var responseXML = responseMsgSet.ToXMLString();
            MessageBox.Show(responseXML);
            // SaveXML(responseXML);
            */

            var response = responseMsgSet.ResponseList.GetAt(0);
            var statusCode = response.StatusCode;
            //var statusMessage = response.StatusMessage;
            //var statusSeverity = response.StatusSeverity;
            //RaiseEvent("Status:\nCode = " + statusCode + "\nMessage = " + statusMessage + "\nSeverity = " + statusSeverity);
            

            if (statusCode == 0)
            {
                var resString = string.Empty;
                var invoiceRet = response.Detail as IInvoiceRet;
                resString += "The following invoice has been successfully submitted to QuickBooks:\n\n";
                if (invoiceRet?.RefNumber != null)
                    resString += "Invoice Number: " + Tab + invoiceRet.RefNumber.GetValue() + NewLine;
                if (invoiceRet?.TimeCreated != null)
                    resString += "Created Date: " + Tab + Convert.ToString(invoiceRet.TimeCreated.GetValue().ToString("MM/dd/yyyy"), CultureInfo.GetCultureInfo("en-US")) + NewLine;
                /*
                if (invoiceRet?.TxnNumber != null)
                    resString += "Txn Number = " + Convert.ToString(invoiceRet.TxnNumber.GetValue()) + NewLine;
                */
                if (invoiceRet?.TxnDate != null)
                    resString += "Transaction Date: " + Tab + Convert.ToString(invoiceRet.TxnDate.GetValue().ToString("MM/dd/yyyy"), CultureInfo.GetCultureInfo("en-US")) + NewLine;
                /*
                if (invoiceRet?.CustomerRef.FullName != null)
                    resString += "Customer FullName = " + invoiceRet.CustomerRef.FullName.GetValue() + NewLine;
                
                resString += "\nBilling Address:" + NewLine;
                if (invoiceRet?.BillAddress.Addr1 != null)
                    resString += "Addr1 = " + invoiceRet.BillAddress.Addr1.GetValue() + NewLine;
                if (invoiceRet?.BillAddress.Addr2 != null)
                    resString += "Addr2 = " + invoiceRet.BillAddress.Addr2.GetValue() + NewLine;
                if (invoiceRet?.BillAddress.Addr3 != null)
                    resString += "Addr3 = " + invoiceRet.BillAddress.Addr3.GetValue() + NewLine;
                if (invoiceRet?.BillAddress.Addr4 != null)
                    resString += "Addr4 = " + invoiceRet.BillAddress.Addr4.GetValue() + NewLine;
                if (invoiceRet?.BillAddress.City != null)
                    resString += "City = " + invoiceRet.BillAddress.City.GetValue() + NewLine;
                if (invoiceRet?.BillAddress.State != null)
                    resString += "State = " + invoiceRet.BillAddress.State.GetValue() + NewLine;
                if (invoiceRet.BillAddress.PostalCode != null)
                    resString += "Postal Code = " + invoiceRet.BillAddress.PostalCode.GetValue() + NewLine;
                if (invoiceRet.BillAddress.Country != null)
                    resString += "Country = " + invoiceRet.BillAddress.Country.GetValue() + NewLine;
                if (invoiceRet.PONumber != null)
                    resString += "\nPO Number = " + invoiceRet.PONumber.GetValue() + NewLine;
                if (invoiceRet.TermsRef.FullName != null)
                    resString += "Terms = " + invoiceRet.TermsRef.FullName.GetValue() + NewLine;
                if (invoiceRet.DueDate != null)
                    resString += "Due Date = " + Convert.ToString(invoiceRet.DueDate.GetValue(), CultureInfo.GetCultureInfo("en-US")) + NewLine;
                if (invoiceRet.SalesTaxTotal != null)
                    resString += "Sales Tax = " + Convert.ToString(invoiceRet.SalesTaxTotal.GetValue(), CultureInfo.GetCultureInfo("en-US")) + NewLine;
                resString += "\nInvoice Line Items:" + NewLine;
                var orInvoiceLineRetList = invoiceRet.ORInvoiceLineRetList;
                var fullname = "<empty>";
                var desc = "<empty>";
                var rate = "<empty>";
                var quantity = "<empty>";
                var amount = "<empty>";
                for (var i = 0; i <= orInvoiceLineRetList.Count - 1; i++)
                {
                    if (invoiceRet.ORInvoiceLineRetList.GetAt(i).ortype == ENORInvoiceLineRet.orilrInvoiceLineRet)
                    {
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.ItemRef.FullName != null)
                            fullname = invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.ItemRef.FullName.GetValue();
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Desc != null)
                            desc = invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Desc.GetValue();
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.ORRate.Rate != null)
                            rate = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.ORRate.Rate.GetValue(), CultureInfo.GetCultureInfo("en-US"));
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Quantity != null)
                            quantity = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Quantity.GetValue(), CultureInfo.GetCultureInfo("en-US"));
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Amount != null)
                            amount = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Amount.GetValue(), CultureInfo.GetCultureInfo("en-US"));
                    }
                    else
                    {
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.ItemGroupRef.FullName != null)
                            fullname = invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.ItemGroupRef.FullName.GetValue();
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.Desc != null)
                            desc = invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.Desc.GetValue();
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.InvoiceLineRetList.GetAt(i).ORRate.Rate != null)
                            rate = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.InvoiceLineRetList.GetAt(i).ORRate.Rate.GetValue(), CultureInfo.GetCultureInfo("en-US"));
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.Quantity != null)
                            quantity = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.Quantity.GetValue(), CultureInfo.GetCultureInfo("en-US"));
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.TotalAmount != null)
                            amount = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.TotalAmount.GetValue(), CultureInfo.GetCultureInfo("en-US"));
                    }
                    resString = resString + "Fullname: " + fullname + NewLine;
                    resString = resString + "Description: " + desc + NewLine;
                    resString = resString + "Rate: " + rate + NewLine;
                    resString = resString + "Quantity: " + quantity + NewLine;
                    resString = resString + "Amount: " + amount + "\n\n";
                }
                */
                RaiseEvent(resString);
            } // if statusCode is zero
            return statusCode == 0;
        }
    }

    public class QBMsgEventArgs : EventArgs
    {
        public readonly string Msg;

        public QBMsgEventArgs(string msg)
        {
            Msg = msg;
        }
    }
}
