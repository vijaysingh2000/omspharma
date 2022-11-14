using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DevExpress.Utils.FormShadow;
using oms.Model;
namespace oms.DataAccessLayer
{
    public static class MiscDL
    {
        //public static List<Tasks> GetAllTasks()
        //{
        //    List<Tasks> returnList = new List<Tasks>();

        //    returnList.Add(new Tasks() { Code = "EC", Name = "Eligibility Check" });
        //    returnList.Add(new Tasks() { Code = "OP", Name = "Order Placement" });
        //    returnList.Add(new Tasks() { Code = "OR", Name = "Order received" });
        //    returnList.Add(new Tasks() { Code = "DIS", Name = "Dispense" });
        //    returnList.Add(new Tasks() { Code = "SHP", Name = "Shipping" });
        //    returnList.Add(new Tasks() { Code = "CDEL", Name = "Confirm Delivery" });
        //    returnList.Add(new Tasks() { Code = "IINV", Name = "Invoice to Insurance" });
        //    returnList.Add(new Tasks() { Code = "IPAY", Name = "Payment Notification Ins." });
        //    returnList.Add(new Tasks() { Code = "UCDINV", Name = $"Invoice to {ApplicationEnvironmentVariables.WorkingClient.Name}" });
        //    returnList.Add(new Tasks() { Code = "UCDPAY", Name = $"Payment From {ApplicationEnvironmentVariables.WorkingClient.Name}" });

        //    return returnList;
        //}

        public static List<(int,string)> GetAddressTypes()
        {
            List<(int,string)> addressTypes = new List<(int,string)>()
            {
                (1,"Home"),
                (2,"Address 1"),
                (3,"Address 2"),
                (4,"Address 3")
            };

            return addressTypes;
        }

        public static List<(string, string)> GetAllReports()
        {
            List<(string, string)> reportList = new List<(string, string)>();

            reportList.Add(("PurchaseReport", "Purchasing Report"));
            reportList.Add(("MonthlyInvoice", "Monthly Dispense"));
            reportList.Add(("PaymentDetailed", "Payment Detailed Report"));
            reportList.Add(("PaymentSummary", "Payment Summary Report"));
            reportList.Add(("InventoryReport", "Inventory"));

            return reportList;
        }

        //public static XElement GetDefaultStepsForTask(string taskcode)
        //{
        //    XElement returnElement = new XElement("items");
            
        //    switch(taskcode)
        //    {
        //        case "EC":

        //            returnElement.Add(new XElement("item", new XAttribute("default", true), new XAttribute("state", false), new XAttribute("step", "Attach Eligibility")));

        //            break;

        //        case "OP":

        //            returnElement.Add(new XElement("item", new XAttribute("default", true), new XAttribute("state", false), new XAttribute("step", "Attach Order")));
        //            break;

        //        case "OR":

        //            returnElement.Add(new XElement("item", new XAttribute("default", true), new XAttribute("state", false), new XAttribute("step", "Attach Confirmation")));
        //            break;

        //        case "DIS":

        //            break;

        //        case "SHP":

        //            returnElement.Add(new XElement("item", new XAttribute("default", true), new XAttribute("state", false), new XAttribute("step", "Create Shipping Label")));
        //            returnElement.Add(new XElement("item", new XAttribute("default", true), new XAttribute("state", false), new XAttribute("step", "Confirm Shipping")));
        //            break;

        //        case "CDEL":

        //            returnElement.Add(new XElement("item", new XAttribute("default", true), new XAttribute("state", false), new XAttribute("step", "Confirm Delivered")));
        //            break;

        //        case "IINV":

        //            returnElement.Add(new XElement("item", new XAttribute("default", true), new XAttribute("state", false), new XAttribute("step", "Invoice Sent")));
        //            break;

        //        case "IPAY":

        //            returnElement.Add(new XElement("item", new XAttribute("default", true), new XAttribute("state", false), new XAttribute("step", "Insurance Payment Confirmed")));
        //            break;

        //        case "UCDINV":

        //            returnElement.Add(new XElement("item", new XAttribute("default", true), new XAttribute("state", false), new XAttribute("step", "Invoice Sent")));
        //            break;

        //        case "UCDPAY":

        //            break;

        //        default:
        //            {
        //                returnElement.Add(new XElement("item", new XAttribute("state", false), new XAttribute("step", "Step 1")));
        //                returnElement.Add(new XElement("item", new XAttribute("state", false), new XAttribute("step", "Step 2")));
        //                returnElement.Add(new XElement("item", new XAttribute("state", false), new XAttribute("step", "Step 3")));
        //                returnElement.Add(new XElement("item", new XAttribute("state", false), new XAttribute("step", "Step 4")));
        //                break;
        //            }
        //    }

        //    return returnElement;
        //}

        public static List<(string,string)> GetAllListItems()
        {
            List<(string,string)> returnList = new List<(string, string)>();

            returnList.Add(("drugs","Drugs"));
            returnList.Add(("insurances", "Insurances"));
            returnList.Add(("manufacturers", "Manufacturers"));
            returnList.Add(("providers", "Providers"));

            return returnList;
        }
    }
}

