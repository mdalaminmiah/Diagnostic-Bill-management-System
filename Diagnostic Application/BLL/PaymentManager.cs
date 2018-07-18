using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Diagnostic_Application.DAL;
using Diagnostic_Application.Models;

namespace Diagnostic_Application.BLL {
    public class PaymentManager {
        PaymentGetway _paymentGateway = new PaymentGetway();

        public TestEntry SearchByBill(string mobile){
            return _paymentGateway.SearchByBill(mobile);
        }

        public string UpdatePayment(string billNo, decimal dueAmount){
            int rowAffected =  _paymentGateway.UpdatePaymentWithStatus(billNo, dueAmount, 0);
            if (rowAffected > 0)
            {
                return "success";
            }
            else
            {
                return "failed";
            }
        }

        public bool IsExistMobileNo(string mobileNo) {
            if (SearchByBill(mobileNo) != null) {
                return true;
            } else {
                return false;
            }
        }



        public string IsBillNoExists(string billNo) {
            if (_paymentGateway.IsBillNoExists(billNo))
            {
                return "success";
            } else {
                return "failed";
            }
        }


        public Patient SearchPatientInfoByBillNo(string billNo){

            return _paymentGateway.GetPatientInfoUsingBillNo(billNo);
        }

        public string UpdatePaymentWithStatus(string billNo, decimal paidAmount, int status)
        {
            int rowAffected = _paymentGateway.UpdatePaymentWithStatus(billNo, paidAmount, status);
            //throw new NotImplementedException();
            if (rowAffected > 0)
            {
                return "success";
            }
            else
            {
                return "failed";
            }
        }
    }

}