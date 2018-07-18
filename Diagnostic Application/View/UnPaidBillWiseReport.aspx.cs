﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Diagnostic_Application.BLL;
using Diagnostic_Application.DAL;
using Diagnostic_Application.Models.View_Model;
using Diagnostic_Application.View.View_Model;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Diagnostic_Application.View {
    public partial class UnPaidBillWiseReport : System.Web.UI.Page {

        private decimal total = 0;
        private int serialNo = 0;
        UnpaidBillWiseManager _unpaidBillWiseManager = new UnpaidBillWiseManager();


        protected void Page_Load(object sender, EventArgs e) {
            if (!IsPostBack) {
                LoadEmptyTestGridView();

            }

            PdfButton.Visible = false;
        }


        private void LoadEmptyTestGridView() {
            DataTable dataTable = new DataTable();
            TypeWiseReportGridView.DataSource = dataTable;
            TypeWiseReportGridView.DataBind();
        }

        protected void ShowButton_Click(object sender, EventArgs e) {


            if (FormDateTextBox.Text == string.Empty || ToDateTextBox.Text == string.Empty)
            {
                InfoMessageLabel.Text = "Please select both date";
                InfoMessageLabel.ForeColor = Color.DarkRed;
                InfoMessageLabel.Visible = true;
                LoadEmptyTestGridView();
            }
            else
            {

                InfoMessageLabel.Text = "";

                string startDate = FormDateTextBox.Text;
                string endDate = ToDateTextBox.Text;
                FormDateTextBox.Text = string.Empty;
                ToDateTextBox.Text = string.Empty;
                LoadTestGridView(startDate, endDate);
            }
        }

        private void LoadTestGridView(string startDate, string endDate){
            //throw new NotImplementedException();

            List<UnpaidBillWiseModel> testWiseReportList = _unpaidBillWiseManager.UnpaidBillReport(startDate, endDate);

            if (testWiseReportList.Count != 0){
                TypeWiseReportGridView.DataSource = testWiseReportList;
                TypeWiseReportGridView.DataBind();
                //display pdf button
                PdfButton.Visible = true;
                //total items
            }
            else{
                TypeWiseReportGridView.DataSource = null;
                TypeWiseReportGridView.DataBind();
                PdfButton.Visible = false;
            }
        }

        protected void PdfButton_Click(object sender, EventArgs e) {
            int columnsCount = TypeWiseReportGridView.HeaderRow.Cells.Count;


            PdfPTable pdfTable = new PdfPTable(columnsCount);


            foreach (TableCell gridViewHeaderCell in TypeWiseReportGridView.HeaderRow.Cells) {

                iTextSharp.text.Font font = new iTextSharp.text.Font();
                font.Color = new BaseColor(TypeWiseReportGridView.HeaderStyle.ForeColor);

                PdfPCell pdfCell = new PdfPCell(new Phrase(gridViewHeaderCell.Text, font));

                pdfTable.AddCell(pdfCell);
            }

            foreach (GridViewRow gridViewRow in TypeWiseReportGridView.Rows) {
                if (gridViewRow.RowType == DataControlRowType.DataRow) {
                    foreach (TableCell gridViewCell in gridViewRow.Cells) {
                        iTextSharp.text.Font font = new iTextSharp.text.Font();
                        font.Color = new BaseColor(TypeWiseReportGridView.RowStyle.ForeColor);

                        PdfPCell pdfCell = new PdfPCell(new Phrase(gridViewCell.Text, font));

                        pdfTable.AddCell(pdfCell);
                    }
                }
            }

            foreach (TableCell gridViewHeaderCell in TypeWiseReportGridView.FooterRow.Cells) {
                iTextSharp.text.Font font = new iTextSharp.text.Font();
                font.Color = new BaseColor(TypeWiseReportGridView.FooterStyle.ForeColor);
                PdfPCell pdfCell = new PdfPCell(new Phrase(gridViewHeaderCell.Text, font));
                pdfTable.AddCell(pdfCell);
            }


            Document pdfDocument = new Document(PageSize.A4, 20f, 10f, 10f, 10f);

            PdfWriter.GetInstance(pdfDocument, Response.OutputStream);
            string Name = "                                                     Diagnostic Center Bill Management System";
            string moduleName = "                                                          Unpaid Test Report";
            pdfDocument.Open();
            pdfDocument.Add(new Paragraph(" \n"));
            pdfDocument.Add(new Paragraph("                                                                                                                 " + DateTime.Now.ToString()));
            pdfDocument.Add(new Paragraph(Name));
            pdfDocument.Add(new Paragraph(" \n"));
            pdfDocument.Add(new Paragraph("\t" + moduleName));
            pdfDocument.Add(new Paragraph(" \n\n"));
            pdfDocument.Add(pdfTable);
            pdfDocument.Close();
            Response.ContentType = "application/pdf";
            Response.AppendHeader("content-disposition", "attachment;filename=UnpaidTesteReport.pdf");
            Response.Write(pdfDocument);
            Response.Flush();
            Response.End();
        }

        protected void TypeWiseReportGridView_RowDataBound(object sender, GridViewRowEventArgs e) {
            if (e.Row.RowType == DataControlRowType.DataRow) {
                e.Row.Cells[0].Text = (serialNo += 1).ToString();
                e.Row.Cells[1].Text = DataBinder.Eval(e.Row.DataItem, "PatientName").ToString();
                e.Row.Cells[2].Text = DataBinder.Eval(e.Row.DataItem, "BillNo").ToString();
                e.Row.Cells[3].Text = DataBinder.Eval(e.Row.DataItem, "MobileNo").ToString();
                e.Row.Cells[4].Text = DataBinder.Eval(e.Row.DataItem, "TotalAmount").ToString();

                total = total + Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "TotalAmount"));

            } else if (e.Row.RowType == DataControlRowType.Footer) {
                e.Row.Cells[0].Text = "";
                e.Row.Cells[1].Text = "";
                e.Row.Cells[2].Text = "";
                e.Row.Cells[3].Text = "Total Amount: ";
                e.Row.Cells[4].Text = total.ToString();
            }
        }
    }
}