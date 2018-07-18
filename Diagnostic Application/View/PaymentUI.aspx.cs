using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Diagnostic_Application.BLL;
using Diagnostic_Application.Models;

namespace Diagnostic_Application.View {
    public partial class PaymentUI : System.Web.UI.Page
    {

        private double amount;
        private decimal _totalAmount;
        private decimal _dueAmount;
        string connectionString = WebConfigurationManager.ConnectionStrings["DiagnosticBD"].ConnectionString;

        private PaymentManager _paymentManager = new PaymentManager();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ViewState["success"] == null)
            {
                PaymentButton.Enabled = false;
                AmountTextBox.Enabled = false;
            }
            else
            {
                PaymentButton.Enabled = true;
                AmountTextBox.Enabled = true;
            }
            
        }



        protected void SearchButton_Click(object sender, EventArgs e) {

            if (BillNoTextBox.Text == "")
            {
                InfoMessageLabel.Visible = true;
                InfoMessageLabel.Text = "Field must not be empty.";
                InfoMessageLabel.ForeColor = Color.DarkRed;
            }
            else
            {
                string billNo = BillNoTextBox.Text;
                string message1 = _paymentManager.IsBillNoExists(billNo);
                bool message2 = IsMobileNoExists(billNo);
                if (message1 == "success" || message2 == true)
                {
                    ViewState["success"] = true;
                    InfoMessageLabel.Visible = true;
                    InfoMessageLabel.Text = "Found Customer Information.";
                    InfoMessageLabel.ForeColor = Color.ForestGreen;
                    ShowPaymentInformation(billNo);
                }
                else if (message1 == "failed")
                {

                    InfoMessageLabel.Visible = true;
                    InfoMessageLabel.Text = "Bill No Not Found.";
                    InfoMessageLabel.ForeColor = Color.DarkRed;
                }

            }

            

        }

        public bool IsMobileNoExists(string mobileNo)
        {
            string query = "SELECT * FROM Patient WHERE mobile='" + mobileNo + "'";
            SqlConnection connection = new SqlConnection(connectionString);

            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            bool isBillNoExists = false;

            if (reader.HasRows)
            {
                isBillNoExists = true;
            }
            reader.Close();
            connection.Close();
            return isBillNoExists;
        }

        private void ShowPaymentInformation(string billNo)
        {
            TestEntry testEntry = _paymentManager.SearchByBill(billNo);
            Patient patient = _paymentManager.SearchPatientInfoByBillNo(billNo);

            _totalAmount = Convert.ToDecimal(testEntry.TotalAmount);
            _dueAmount = Convert.ToDecimal(patient.DueAmount.ToString());
            
            BillDateLabel.Text = patient.DueDate.ToString();
            TotalFeeLabel.Text = _totalAmount + " Taka";
            DueAmountLabel.Text = _dueAmount + " Taka";
            PaidAmountLabel.Text = (_totalAmount - _dueAmount).ToString() + " Taka";

            ViewState["DueAmount"] = _dueAmount;
            ViewState["TotalAmount"] = _totalAmount;
            ViewState["billNo"] = billNo;

            //Enable Payment Button and textbox
            PaymentButton.Enabled = true;
            AmountTextBox.Enabled = true;
        }


        protected void PaymentButton_Click(object sender, EventArgs e) {
            
            //check empty
            if (AmountTextBox.Text == "") {
                InfoMessageLabel.Visible = true;
                InfoMessageLabel.Text = "Empty Amount.";
                InfoMessageLabel.ForeColor = Color.DarkRed;
                return;
            }

            //collect the amount;
            string _billNo = (string)ViewState["billNo"];
            decimal _paidAmount = Convert.ToDecimal(AmountTextBox.Text);

            _dueAmount = (decimal) ViewState["DueAmount"];


            //check amount is greater than the dueAmount
            if (_paidAmount > _dueAmount || _paidAmount < _dueAmount)
            {
                InfoMessageLabel.Visible = true;
                InfoMessageLabel.Text = "Cannot Proced.Paid or Payment Amount mismatch.";
                InfoMessageLabel.ForeColor = Color.DarkRed;
                //return;
            }

            if (_paidAmount == _dueAmount){
                //save information with UpdateStatus = 1
                decimal _newDueAmount = _dueAmount - _paidAmount;
                string message = _paymentManager.UpdatePaymentWithStatus(_billNo, _newDueAmount, 1);
                if (message == "success"){

                    ViewState["success"] = true;
                    InfoMessageLabel.Visible = true;
                    InfoMessageLabel.Text = "Full Paid!! Updated customer information.";
                    InfoMessageLabel.ForeColor = Color.ForestGreen;
                    ShowPaymentInformation(_billNo);
                    //AmountTextBox.ReadOnly = true;
                }

                return;
            }
            //else {
            //    //everything is fine.
            //    decimal _newDueAmount = _dueAmount - _paidAmount;
            //    //check the newDueAmount
            //    string message = _paymentManager.UpdatePayment(_billNo, _newDueAmount);
            //    if (message == "success") {
            //        ViewState["success"] = true;
            //        InfoMessageLabel.Visible = true;
            //        InfoMessageLabel.Text = "Partial Paid! Update customer information.";
            //        InfoMessageLabel.ForeColor = Color.ForestGreen;
            //        ShowPaymentInformation(_billNo);
                    
            //    }
            //    AmountTextBox.Text = "";
            //    AmountTextBox.Visible = false;
            //    BillNoTextBox.Text = "";
            //    return;
            //}
            AmountTextBox.Text = "";
            AmountTextBox.Text = "";
           // BillNoTextBox.Text = "";
            return;
        }
    }
}