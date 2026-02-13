using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Marketing.Trans
{
    public class EnquirySaveModel
    {
        public string EmpNo { get; set; }
        public string CompCode { get; set; }
        public string PCCode { get; set; }
        public string DomainID { get; set; }
        public string AreaID { get; set; }
        public string CustName { get; set; }
        public string RefFrom { get; set; }
        public string MktExName { get; set; }
        public string Address { get; set; }
        public string ContactPerson { get; set; }
        public string MobileNo { get; set; }
        public string EMailID { get; set; }
        public string CityID { get; set; }
        public string ActualReqQty { get; set; }
        public string PartCode_items { get; set; }
        public string Qty_items { get; set; }
        public string FStatusID { get; set; }
        public string CompititorName { get; set; }
        public string NextFDate { get; set; }
        public string FUpRemark { get; set; }

        public string ConfEMail { get; set; }
        public string ConfCustName { get; set; }
        public string ConfMobile { get; set; }
        public string EnqNo { get; set; }
        public string AssignToEmpID { get; set; }
        
    }

    public class FollowUpUpdate
    {
        public string EnqNo { get; set; }
        public string PartCode { get; set; }
        public string EmpCode { get; set; }
        public string FStatusID { get; set; }

        public string CompititorName { get; set; }

        public string NextFDate { get; set; }
        public string FUpRemark { get; set; }
        public string AssignToEmpID { get; set; }
        
    }

    public class EnquirySaveModelAngular
    {
        public string SaveType { get; set; }
        public string EnqNo { get; set; }
        public string VPCode { get; set; }
        public string VPSrNo { get; set; }
        public string UserID { get; set; }
        public string CompID { get; set; }
        public string PCCode { get; set; }
        public string DomainID { get; set; }
    
        public string CustName { get; set; }
        public string RefFrom { get; set; }
        public string AssToEmpCode { get; set; }
        public string AssToEmpName { get; set; }
        public string Address { get; set; }
        public string ContactPerson { get; set; }
        public string MobileNo { get; set; }
        public string OfficePhNo { get; set; }
        public string EMailID { get; set; }
        public string CityID { get; set; }
        public string ActualReqQty { get; set; }
        public string EnqRemark { get; set; }

        public string EnqPartCode_items { get; set; }
        public string SendQuot { get; set; }
        public string QuotPartCode_items { get; set; }  
        public string AdditionalPart_items { get; set; }  
              
        public string FStatusID { get; set; }
        public string NextFDate { get; set; }
        public string FUpRemark { get; set; }
        public string CCMailID { get; set; }
       
        public string InstallationOffer_items { get; set; }
        public string GSTInEx { get; set; }

        public string ConfCustName { get; set; }
        public string ConfMobile { get; set; }
        public string ConfEMail { get; set; }  
    }

    public class Enquiry_Submit_EnqDashboard
    {
        public string SaveType { get; set; }

        public string FlwID_Selected { get; set; }
        public string EnqPartCode_Selected { get; set; }


        public string EnqNo { get; set; }

        public string UserID { get; set; }
        public string CompID { get; set; }
        public string PCCode { get; set; }
        public string AssToEmpCode { get; set; }
        public string AssToEmpName { get; set; }

        public string CustName { get; set; }
        public string Title { get; set; }
        public string ContactPerson { get; set; }
        public string MobileNo { get; set; }

        public string EMailID { get; set; }
        public string Address { get; set; }
        public string RefFromID { get; set; }
        public string RefFrom { get; set; }

        public string DomainID { get; set; }

        public string CityID { get; set; }

        public string EnqRemark { get; set; }
        public string EnqPartCode_items { get; set; }
        public string ConfCustName { get; set; }
        public string ConfMobile { get; set; }
        public string ConfEMail { get; set; }

        public string rbldGRatingSuggestedBy { get; set; }
        public string rblloadStudyRequired { get; set; }
        public string ddlloadAnalysisStatus { get; set; }

        public string FStatusID { get; set; }
        public string NextFDate { get; set; }
        public string FlwStatusHeads { get; set; }
        public string FUpRemark { get; set; }

        public string ddlLostToWhom { get; set; }
        public string ddlLostReason { get; set; }
        public string txtLOSTToWhomDetails { get; set; }
        public string txtLOSTPrice { get; set; }

        public string SendQuot { get; set; }
        public string CCMailID { get; set; }
        public string QuotPartCode_items { get; set; }
        public string AdditionalPart_items { get; set; }
        public string GSTInEx { get; set; }
        public string InstallationOffer_items { get; set; }

        public string FollowupFeedbackfrom { get; set; }
    }



    //public string sendMKT_ENQ_GensetQuotEmail(string qtn_no, string kva, string to_mail_id, string cc_mail_id, string ass_to_emp_id, string ass_to_emp_name, 
    //    string ass_to_mob_no, string ass_to_mail_id, string cust_name, string cust_add, string model)
    //{
    //    SmtpClient sc;
    //    string Body = "";
    //    string Original_Qtn_No = qtn_no;
    //    qtn_no = qtn_no.Replace("/", "_");
    //    string strFilePath_1 = ConfigurationManager.AppSettings["Quotationfile"] + "/" + qtn_no + "_" + DateTime.Now.ToString("dd-MM-yyyy") + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".pdf";
    //    string dt_time = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

    //    string strpdf_1 = qtn_no + "_" + dt_time + ".pdf";
    //    try
    //    {
    //        LoadReport(Original_Qtn_No.Trim(), strFilePath_1.ToString().Trim(), "Q", ass_to_emp_id.Trim(), ass_to_emp_name.Trim(), ass_to_mob_no.Trim(), ass_to_mail_id.Trim(), model.Trim());

    //        if (strFilePath_1.ToString().Trim() != "")
    //        {
    //            MailMessage m = new MailMessage();
    //            sc = new SmtpClient();
    //            FileStream fs1 = new FileStream(strFilePath_1, FileMode.Open, FileAccess.Read);
    //            Attachment a1 = new Attachment(fs1, strpdf_1, MediaTypeNames.Application.Octet);

    //            m.Attachments.Add(a1);

    //            #region
    //            int flg_anubandh = 0;
    //            int flg_2kW_5kVA = 0;
    //            int flg_R550 = 0;
    //            int flg_5_To_12_kVA = 0;
    //            int flg_15_To_30_kVA = 0;
    //            int flg_40_To_160_kVA = 0;
    //            int flg_200_To_250_kVA = 0;
    //            int flg_320_1010_kVA = 0;
    //            int flg_1250_kVA_1500 = 0;

    //            int flg_2_8_To_5_5_kVA = 0;
    //            int flg_7_5_To_20_kVA = 0;
    //            int flg_NG_CPCB_IV_15_to_250_kVA = 0;
    //            int flg_OP_CPCB_IV_117_2000_kVA;


    //            string[] split_kva_item = Regex.Split(kva.Trim(), ",");
    //            if (split_kva_item.Length > 0)
    //            {
    //                for (int j = 0; j < split_kva_item.Length; j++)
    //                {
    //                    string strFilePath_2 = "", strFilePath_3 = "";
    //                    string strpdf_2 = "", strpdf_3 = "";

    //                    ////string model_type = getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + Original_Qtn_No.Trim() + "') and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "Model");
    //                    ////string Qtn_CPCB = getName("select substring(q.PartCode,15,1) as CPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "CPCB");


    //                    //string model_type = getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + Original_Qtn_No.Trim() + "') and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "Model");
    //                    //string Qtn_CPCB = getName("select substring(q.PartCode,15,1) as CPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "CPCB");
    //                    ////int Opti = Convert.ToInt32(getName("select isnull(sum(k.Qty),0) as EngCount from quotationdetails q inner join part p on q.partcode=p.PartCode inner join kitdetails k on k.kitcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and k.PartCode like '001%' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "EngCount"));
    //                    //string Opti_CPCB = getName("select top 1 substring(q.PartCode,10,1) as OptiCPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "OptiCPCB");

    //                    string model_type = getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + Original_Qtn_No.Trim() + "') and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "Model");
    //                    string Qtn_CPCB = getName("select substring(q.PartCode,15,1) as CPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "CPCB");
    //                    //int Opti = Convert.ToInt32 (getName("select isnull(sum(k.Qty),0) as EngCount from quotationdetails q inner join part p on q.partcode=p.PartCode inner join kitdetails k on k.kitcode=p.PartCode where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "' and k.PartCode like '001%' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "EngCount"));
    //                    string Opti_CPCB = getName("select top 1 substring(q.PartCode,10,1) as OptiCPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "OptiCPCB");


    //                    //if (Qtn_CPCB == "4")
    //                    //{
    //                    //if (model_type.Trim().PadRight(3).Trim() == "NG1")
    //                    if (model_type.Substring(model_type.Trim().Length - 3) == "NG1")
    //                    {
    //                        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/NG_CPCB IV+15-250_kVA.PDF";
    //                        strpdf_2 = "NG_CPCB IV+15-250_kVA.PDF";
    //                        flg_NG_CPCB_IV_15_to_250_kVA = 1;
    //                    }
    //                    else
    //                    {
    //                        if (Opti_CPCB == "5")
    //                        {
    //                            strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/OP_CPCB IV+117-2000_kVA.PDF";
    //                            strpdf_2 = "OP_CPCB IV+117-2000_kVA.PDF";
    //                            flg_OP_CPCB_IV_117_2000_kVA = 1;
    //                        }
    //                        else
    //                        {

    //                            //if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
    //                            //{
    //                            //    //strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/Kirloskar_2kW_5kVA.pdf";
    //                            //    //strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
    //                            //    //flg_2kW_5kVA = 1;
    //                            //}


    //                            //2. 8kW-5.5kVA
    //                            if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 7))
    //                            {
    //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+2.8-5.5_kVA.PDF";
    //                                strpdf_2 = "CPCB_IV+2.8-5.5_kVA.PDF";
    //                                flg_2_8_To_5_5_kVA = 1;
    //                            }
    //                            // EA Series
    //                            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 7 && Convert.ToDouble(split_kva_item[j].Trim()) <= 20))
    //                            {
    //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+7.5-20_kVA.PDF";
    //                                strpdf_2 = "CPCB_IV+7.5-20_kVA.PDF";
    //                                flg_7_5_To_20_kVA = 1;
    //                            }
    //                            //3R550
    //                            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 25 && Convert.ToDouble(split_kva_item[j].Trim()) <= 58.5))
    //                            {
    //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+25-58.5_kVA.PDF";
    //                                strpdf_2 = "CPCB_IV+25-58.5_kVA.PDF";
    //                                flg_R550 = 1;
    //                            }
    //                            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 80 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160))
    //                            {
    //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+82.5-160_kVA.PDF";
    //                                strpdf_2 = "CPCB_IV+82.5-160_kVA.PDF";
    //                                flg_15_To_30_kVA = 1;
    //                            }
    //                            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250))
    //                            {
    //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+200-250_kVA.PDF";
    //                                strpdf_2 = "CPCB_IV+200-250_kVA.PDF";
    //                                flg_40_To_160_kVA = 1;
    //                            }
    //                            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 750))
    //                            {
    //                                //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
    //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+320-750_kVA.PDF";
    //                                strpdf_2 = "CPCB_IV+320-750_kVA.PDF";
    //                                flg_320_1010_kVA = 1;
    //                            }
    //                            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 1010 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1500))
    //                            {
    //                                //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
    //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+1010-1500_kVA.PDF";
    //                                strpdf_2 = "CPCB_IV+1010-1500_kVA.PDF";
    //                                flg_320_1010_kVA = 1;
    //                            }
    //                        }
    //                    }





    //                    #region


    //                    //}
    //                    //else
    //                    //{
    //                    //    if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_2kW_5kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
    //                    //        flg_2kW_5kVA = 1;
    //                    //    }
    //                    //    // EA Series
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 12.5) && flg_5_To_12_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/iGreen-5-160kVA.pdf";
    //                    //        strpdf_2 = "Catlogue_iGreen_5_To_160_kVA.pdf";
    //                    //        flg_5_To_12_kVA = 1;
    //                    //    }
    //                    //    //3R550
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 15) && flg_R550 == 0 && model_type.Trim().Substring(0, 5).Trim() == "3R550")
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_R550_15_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_R550_15_kVA_Catlogue.pdf";
    //                    //        flg_R550 = 1;
    //                    //    }
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 30) && flg_15_To_30_kVA == 0)
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_15_To_30_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_15_To_30_kVA_Catlogue.pdf";
    //                    //        flg_15_To_30_kVA = 1;
    //                    //    }
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 40 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_40_To_160_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "6R")
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_40_To_160_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_40_To_160_kVA_Catalogue.pdf";
    //                    //        flg_40_To_160_kVA = 1;
    //                    //    }
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250) && flg_200_To_250_kVA == 0)
    //                    //    {
    //                    //        // Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_200_To_250_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_200_To_250_kVA_Catlogue.pdf";
    //                    //        flg_200_To_250_kVA = 1;
    //                    //    }
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1010) && flg_320_1010_kVA == 0)
    //                    //    {
    //                    //        // Commented by KB on 08/05/2023 as per mail from Kalpesh Sir on 06/05/2023
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_320_1010_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_320_1010_kVA_Catlogue.pdf";
    //                    //        flg_320_1010_kVA = 1;
    //                    //    }
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 1250 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1500) && flg_1250_kVA_1500 == 0)
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_1250_To_1500_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_1250_To_1500_kVA_Catlogue.pdf";
    //                    //        flg_1250_kVA_1500 = 1;
    //                    //    }
    //                    //}

    //                    //string Qtn_CPCB = getName("select substring(q.PartCode,15,1) as CPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "CPCB");

    //                    //if (Qtn_CPCB == "4")
    //                    //{
    //                    //    if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
    //                    //    {
    //                    //        //strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/Kirloskar_2kW_5kVA.pdf";
    //                    //        //strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
    //                    //        //flg_2kW_5kVA = 1;
    //                    //    }
    //                    //    // EA Series
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 20))
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+7.5-20_kVA.PDF";
    //                    //        strpdf_2 = "CPCB_IV+7.5-20_kVA.PDF.pdf";
    //                    //        flg_5_To_12_kVA = 1;
    //                    //    }
    //                    //    //3R550
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 25 && Convert.ToDouble(split_kva_item[j].Trim()) <= 58.5))
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+25-58.5_kVA.PDF";
    //                    //        strpdf_2 = "CPCB_IV+25-58.5_kVA.PDF";
    //                    //        flg_R550 = 1;
    //                    //    }
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 80 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160))
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+82.5-160_kVA.PDF";
    //                    //        strpdf_2 = "CPCB_IV+82.5-160_kVA.PDF";
    //                    //        flg_15_To_30_kVA = 1;
    //                    //    }
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250))
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+200-250_kVA.PDF";
    //                    //        strpdf_2 = "CPCB_IV+200-250_kVA.PDF";
    //                    //        flg_40_To_160_kVA = 1;
    //                    //    }
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 750))
    //                    //    {
    //                    //        //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+320-750_kVA.PDF";
    //                    //        strpdf_2 = "CPCB_IV+320-750_kVA.PDF";
    //                    //        flg_200_To_250_kVA = 1;
    //                    //    }

    //                    //}

    //                    //else
    //                    //{
    //                    //    //Stationary
    //                    //    if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && model_type.Trim().Substring(0, 2).Trim() == "CC" && flg_2kW_5kVA == 0)
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_2kW_5kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
    //                    //        flg_2kW_5kVA = 1;
    //                    //    }
    //                    //    // EA Series
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 12.5) && flg_5_To_12_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
    //                    //    {
    //                    //        //strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/iGreen-5-160kVA.pdf";
    //                    //        //strpdf_2 = "Catlogue_iGreen_5_To_160_kVA.pdf";
    //                    //        //flg_5_To_12_kVA = 1;
    //                    //    }
    //                    //    //R550
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 15) && model_type.Trim().Substring(0, 5).Trim() == "3R550" && flg_R550 == 0)
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_R550_15_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_R550_15_kVA_Catlogue.pdf";
    //                    //        flg_R550 = 1;
    //                    //    }
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 30) && flg_15_To_30_kVA == 0)
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_15_To_30_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_15_To_30_kVA_Catlogue.pdf";
    //                    //        flg_15_To_30_kVA = 1;
    //                    //    }
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 40 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_40_To_160_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "6R")
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_40_To_160_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_40_To_160_kVA_Catalogue.pdf";
    //                    //        flg_40_To_160_kVA = 1;
    //                    //    }

    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250) && flg_200_To_250_kVA == 0)
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_200_To_250_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_200_To_250_kVA_Catlogue.pdf";
    //                    //        flg_200_To_250_kVA = 1;
    //                    //    }
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1010) && flg_320_1010_kVA == 0)
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_320_1010_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_320_1010_kVA_Catlogue.pdf";
    //                    //        flg_320_1010_kVA = 1;
    //                    //    }
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 1250 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1500) && flg_1250_kVA_1500 == 0)
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_1250_To_1500_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_1250_To_1500_kVA_Catlogue.pdf";
    //                    //        flg_320_1010_kVA = 1;
    //                    //    }
    //                    //}
    //                    #endregion


    //                    if (File.Exists(strFilePath_2))
    //                    {
    //                        FileStream fs2 = new FileStream(strFilePath_2, FileMode.Open, FileAccess.Read);
    //                        Attachment a2 = new Attachment(fs2, strpdf_2, MediaTypeNames.Application.Octet);
    //                        m.Attachments.Add(a2);
    //                    }

    //                    // New Anubandh
    //                    if (Qtn_CPCB != "4")
    //                    {
    //                        if (Convert.ToDouble(split_kva_item[j].Trim()) == 125 && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() == "6R")
    //                        {
    //                            //Skip Anubandh by KB on 21/12/2023

    //                            //Skip Anubandh
    //                        }
    //                        else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
    //                        {
    //                            //Skip Anubandh by KB on 21/12/2023
    //                            //strFilePath_3 = AppDomain.CurrentDomain.BaseDirectory + "Pages/Marketing/Reports/quotation/Anubandh.pdf";
    //                            //strpdf_3 = "Anubandh.pdf";
    //                            //flg_anubandh = 1;

    //                            if (File.Exists(strFilePath_3))
    //                            {
    //                                //Skip Anubandh by KB on 21/12/2023

    //                                //FileStream fs3 = new FileStream(strFilePath_3, FileMode.Open, FileAccess.Read);
    //                                //Attachment a3 = new Attachment(fs3, strpdf_3, MediaTypeNames.Application.Octet);
    //                                //m.Attachments.Add(a3);
    //                            }
    //                        }
    //                    }
    //                    //End 
    //                }
    //            }
    //            #endregion

    //            m.From = new MailAddress("erp@kalabiz.com", "Kala Genset Pvt. Ltd. - ERP System");

    //            #region
    //            //ToMail
    //            if (to_mail_id.Length > 0 && !string.IsNullOrEmpty(to_mail_id))
    //            {
    //                string[] ToMailIDitems = Regex.Split(to_mail_id.Trim(), ",");
    //                foreach (string strTo in ToMailIDitems)
    //                {
    //                    m.To.Add(new MailAddress(strTo));
    //                }
    //            }
    //            //ToMail END

    //            //CCMail 
    //            if (cc_mail_id.Length > 0 & !string.IsNullOrEmpty(cc_mail_id))
    //            {
    //                string[] CCMailIDitems = Regex.Split(cc_mail_id.Trim(), ",");
    //                foreach (string strCC in CCMailIDitems)
    //                {
    //                    m.CC.Add(new MailAddress(strCC));
    //                }
    //            }
    //            //CCMail  END

    //            //BCCMail                 
    //            string[] BCCMailIDitems = Regex.Split("skk@kalabiz.com", ",");
    //            foreach (string strBCC in BCCMailIDitems)
    //            {
    //                m.Bcc.Add(new MailAddress(strBCC));
    //            }

    //            //BCCMail  END
    //            #endregion

    //            #region



    //            Body = "";
    //            Body += "<p>To,";
    //            Body += "<BR><span style='color:#1F497D'>" + cust_name.Trim() + " </span>";
    //            Body += "<BR><span style='color:#1F497D'>" + cust_add.Trim() + " </span></p>";

    //            Body += "<p>Subject: Proposal for " + kva.ToString().Trim() + " kVA Silent DG Set</p>";

    //            Body += "<p>KALA a popular name synonymous with power, stands tall as a market leader ";
    //            Body += "serving industrial, commercial and residential segments in the domestic as well ";
    //            Body += "as international markets.</p>";
    //            Body += "<p>KALA an IMS(Integrated Management System) Certified company has grown to be one of the biggest names in power ";
    //            Body += "generation solutions. Today, our team continues to focus on constant innovation ";
    //            Body += "and commitment towards Customer Satisfaction. We concentrate on our ";
    //            Body += "efforts in bringing the highest quality power solutions to your corporate/business/residence.</p>";


    //                Body += "<p>We as an authorized Genset Original Equipment Manufacturers (G.O.E.M's) for ";

    //                Body += "Kirloskar Oil Engines Limited-Pune, manufacture quality KOEL Green ";
    //                Body += "Generating Sets in the range of 2.1 kVA to 1500 kVA for captive and standby ";
    //                Body += "applications with options such as liquid cooled and air cooled DG sets.</p>";

    //                Body += "<p>We provide complete turnkey solutions which include site selection, load study, ";
    //                Body += "application engineering, installation, commissioning and obtaining statutory ";
    //                Body += "approvals from the Government agencies.</p>";

    //                Body += "<p>With our headquarters at Pune, We are having 3 state of the art, ultra-modern ";
    //                Body += "manufacturing and testing facilities at Chakan, Silvassa, Belgaum. We are able ";
    //                Body += "to cater to both logistic requirements as well as commercial benefits in ";
    //                Body += "taxations. We manufacture world class accoustic enclosures (Sound Proof ";
    //                Body += "Canopies) as per Central Pollution Control Board (CPCB) Norms.</p>";


    //            Body += "<p style='text-align:justify'>";
    //            Body += "Our dedicated branch offices at Pune, Chinchwad, Mumbai(Kharghar), Kolhapur, ";
    //            Body += "Solapur, Sangli, Aurangabad, Latur, Beed, Nanded, Indore, Bhopal, Gwalior, Bangalore, Belgaon, Hyderabad along with the business associates &amp; dealers are available to ";
    //            Body += "provide sales and service to our widespread customer network. Our ";
    //            Body += "marketing team is always available at your service to meet your requirements ";
    //            Body += "and satisfy all your needs 24/7. We understand the popular saying 'Customers ";
    //            Body += "have a choice' and we are grateful to you for considering us as your ";
    //            Body += "preferred choice.</p>";

    //            Body += "<p>For Genset & Commercial Details, Please find attachment.</p>";

    //            Body += "<p style='text-align: justify'><span><span style='color:#1F497D'>Thank You/Regards.</span></span></p>";

    //            Body += "<p style='text-align: justify'><span><span style='color:#1F497D'>" + ass_to_emp_name.Trim() + " <br> MailID - " + ass_to_mail_id.Trim() + " <br> MobileNo - " + ass_to_mob_no.Trim() + " <br> Toll Free No - 1800 123 0018</span></span></p>";

    //            Body += "<BR><img alt=\"\" hspace=0 src=\"cid:imageId\" align=baseline border=0>";
    //            Body += "<p style='text-align: justify'><span><span style='color:#1F497D'><b>Kala Genset Pvt ltd.</b></span></span></p>";



    //            #endregion

    //            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, null, "text/html");
    //            LinkedResource imagelink = new LinkedResource(Server.MapPath("~//images//kalalogo.JPG"), "image/png");
    //            imagelink.ContentId = "imageId";
    //            imagelink.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
    //            htmlView.LinkedResources.Add(imagelink);
    //            m.AlternateViews.Add(htmlView);

    //            m.Subject = "Proposal for " + kva.ToString().Trim() + " kVA Silent DG Set";

    //            m.IsBodyHtml = true;
    //            m.Body = Body;
    //            m.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

    //            if (!string.IsNullOrEmpty(ass_to_mail_id.ToString().Trim()))
    //            {
    //                m.ReplyTo = new MailAddress(ass_to_mail_id.ToString().Trim());
    //            }

    //            if (sc != null)
    //            {
    //                sc.Credentials = new System.Net.NetworkCredential("erp@kalabiz.com", "wanv ftwc dobq blrr");
    //                sc.Port = 587;
    //                sc.Host = "smtp.gmail.com";
    //                sc.EnableSsl = true;
    //                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
    //                sc.Timeout = 999999999;
    //                sc.ServicePoint.MaxIdleTime = 1;
    //                sc.Send(m);
    //            }

    //            m.Dispose();
    //            sc = null;

    //            fs1.Close();
    //            fs1.Dispose();
    //            //Code to Delete The Temp File
    //            if (File.Exists(strFilePath_1))
    //            {
    //                File.Delete(strFilePath_1);
    //            }
    //        }
    //        return "Successfully";
    //    }
    //    catch (Exception ex)
    //    {
    //        if (File.Exists(strFilePath_1))
    //        {
    //            File.Delete(strFilePath_1);
    //        }
    //        return "Failed" + ex.StackTrace;
    //    }
    //}



    //public void sendQuotEmailAngular(string qtn_no, string kva, string to_mail_id, string cc_mail_id, string ass_to_emp_id, string ass_to_emp_name,
    //   string ass_to_mob_no, string ass_to_mail_id, string cust_name, string cust_add, string model)
    //{
    //    string mainFolderPath = ConfigurationManager.AppSettings["Quotationfile"] + "/" + ass_to_emp_id.Trim();
    //    if (!Directory.Exists(mainFolderPath))
    //    {
    //        Directory.CreateDirectory(mainFolderPath);
    //    }

    //    //foreach (string strFile in Directory.GetFiles(mainFolderPath, "*.*"))
    //    //{
    //    //    File.Delete(strFile);
    //    //}

    //    SmtpClient sc = null;
    //    MailMessage m = null;
    //    string Body = "", DGModel = "NoniGreen";
    //    string Original_Qtn_No = qtn_no;
    //    qtn_no = qtn_no.Replace("/", "_");
    //    string str_filename = qtn_no + "_" + DateTime.Now.ToString("dd-MM-yyyy") + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".pdf";
    //    string strFilePath = mainFolderPath + "/" + str_filename;
    //    Context.Response.ContentType = "application/json; charset=utf-8";

    //    try
    //    {
    //        // iGreen KVA
    //        if (Convert.ToDouble(kva.Trim()) >= 5 && Convert.ToDouble(kva.Trim()) <= 160)
    //        {
    //            DGModel = "iGreen";
    //        } // kg
    //        else
    //        {
    //            DGModel = "NoniGreen";
    //        }

    //        LoadReport(Original_Qtn_No.Trim(), strFilePath.ToString().Trim(), "Q", ass_to_emp_id.Trim(), ass_to_emp_name.Trim(), ass_to_mob_no.Trim(), ass_to_mail_id.Trim(), DGModel.Trim());

    //        if (strFilePath.ToString().Trim() != "")
    //        {
    //            m = new MailMessage();
    //            sc = new SmtpClient();

    //            FileStream fs1 = new FileStream(strFilePath, FileMode.Open, FileAccess.Read);
    //            Attachment a1 = new Attachment(fs1, str_filename, MediaTypeNames.Application.Octet);
    //            m.Attachments.Add(a1);

    //            #region             
    //            int flg_320_1010 = 0;
    //            int flg_180_250 = 0;
    //            int flg_5_160 = 0;
    //            int flg_2_5_diesel = 0;
    //            int flg_anubandh = 0;
    //            int flg_1250_1500 = 0;

    //            //int flg_anubandh = 0;
    //            int flg_2kW_5kVA = 0;
    //            int flg_R550 = 0;
    //            int flg_5_To_12_kVA = 0;
    //            int flg_15_To_30_kVA = 0;
    //            int flg_40_To_160_kVA = 0;
    //            int flg_200_To_250_kVA = 0;
    //            int flg_320_1010_kVA = 0;
    //            int flg_1250_kVA_1500 = 0;

    //            string[] split_kva_item = Regex.Split(kva.Trim(), ",");
    //            if (split_kva_item.Length > 0)
    //            {
    //                for (int j = 0; j < split_kva_item.Length; j++)
    //                {
    //                    string strFilePath_2 = "", strFilePath_3 = "";
    //                    string strpdf_2 = "", strpdf_3 = "";

    //                    string model_type = getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + Original_Qtn_No.Trim() + "') and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "Model");
    //                    string Qtn_CPCB = getName("select substring(q.PartCode,15,1) as CPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "CPCB");
    //                    //int Opti = Convert.ToInt32(getName("select isnull(sum(k.Qty),0) as EngCount from quotationdetails q inner join part p on q.partcode=p.PartCode inner join kitdetails k on k.kitcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and k.PartCode like '001%' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "EngCount"));
    //                    string Opti_CPCB = getName("select top 1 substring(q.PartCode,10,1) as OptiCPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "OptiCPCB");

    //                    //if (Qtn_CPCB == "4")
    //                    //{
    //                    if (model_type.Substring(model_type.Length - 3) == "NG1")
    //                    {
    //                        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/NG_CPCB IV+15-250_kVA.PDF";
    //                        strpdf_2 = "NG_CPCB IV+15-250_kVA.PDF";
    //                        flg_5_To_12_kVA = 1;
    //                    }
    //                    else
    //                    {
    //                        if (Opti_CPCB == "5")
    //                        {
    //                            strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/OP_CPCB IV+117-2000_kVA.PDF";
    //                            strpdf_2 = "OP_CPCB IV+117-2000_kVA.PDF";
    //                            flg_5_To_12_kVA = 1;
    //                        }
    //                        else
    //                        {
    //                            if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
    //                            {
    //                                //strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/Kirloskar_2kW_5kVA.pdf";
    //                                //strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
    //                                //flg_2kW_5kVA = 1;
    //                            }
    //                            // EA Series
    //                            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 20))
    //                            {
    //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+7.5-20_kVA.PDF";
    //                                strpdf_2 = "CPCB_IV+7.5-20_kVA.PDF.pdf";
    //                                flg_5_To_12_kVA = 1;
    //                            }
    //                            //3R550
    //                            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 25 && Convert.ToDouble(split_kva_item[j].Trim()) <= 58.5))
    //                            {
    //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+25-58.5_kVA.PDF";
    //                                strpdf_2 = "CPCB_IV+25-58.5_kVA.PDF";
    //                                flg_R550 = 1;
    //                            }
    //                            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 80 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160))
    //                            {
    //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+82.5-160_kVA.PDF";
    //                                strpdf_2 = "CPCB_IV+82.5-160_kVA.PDF";
    //                                flg_15_To_30_kVA = 1;
    //                            }
    //                            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250))
    //                            {
    //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+200-250_kVA.PDF";
    //                                strpdf_2 = "CPCB_IV+200-250_kVA.PDF";
    //                                flg_40_To_160_kVA = 1;
    //                            }
    //                            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 750))
    //                            {
    //                                //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
    //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+320-750_kVA.PDF";
    //                                strpdf_2 = "CPCB_IV+320-750_kVA.PDF";
    //                                flg_200_To_250_kVA = 1;
    //                            }
    //                        }
    //                    }

    //                    //}
    //                    //else
    //                    //{
    //                    //    if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_2kW_5kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
    //                    //        flg_2kW_5kVA = 1;
    //                    //    }
    //                    //    // EA Series
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 12.5) && flg_5_To_12_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/iGreen-5-160kVA.pdf";
    //                    //        strpdf_2 = "Catlogue_iGreen_5_To_160_kVA.pdf";
    //                    //        flg_5_To_12_kVA = 1;
    //                    //    }
    //                    //    //3R550
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 15) && flg_R550 == 0 && model_type.Trim().Substring(0, 5).Trim() == "3R550")
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_R550_15_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_R550_15_kVA_Catlogue.pdf";
    //                    //        flg_R550 = 1;
    //                    //    }
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 30) && flg_15_To_30_kVA == 0)
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_15_To_30_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_15_To_30_kVA_Catlogue.pdf";
    //                    //        flg_15_To_30_kVA = 1;
    //                    //    }
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 40 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_40_To_160_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "6R")
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_40_To_160_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_40_To_160_kVA_Catalogue.pdf";
    //                    //        flg_40_To_160_kVA = 1;
    //                    //    }
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250) && flg_200_To_250_kVA == 0)
    //                    //    {
    //                    //        // Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_200_To_250_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_200_To_250_kVA_Catlogue.pdf";
    //                    //        flg_200_To_250_kVA = 1;
    //                    //    }
    //                    //        // Commented by KB on 01/07/2024 as Catalog is not required for 1000 KVA
    //                    //    //else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1010) && flg_320_1010_kVA == 0)
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) < 1000) && flg_320_1010_kVA == 0)
    //                    //    {
    //                    //        // Commented by KB on 08/05/2023 as per mail from Kalpesh Sir on 06/05/2023
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_320_1010_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_320_1010_kVA_Catlogue.pdf";
    //                    //        flg_320_1010_kVA = 1;
    //                    //    }
    //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 1250 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1500) && flg_1250_kVA_1500 == 0)
    //                    //    {
    //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_1250_To_1500_kVA.pdf";
    //                    //        strpdf_2 = "Kirloskar_1250_To_1500_kVA_Catlogue.pdf";
    //                    //        flg_1250_kVA_1500 = 1;
    //                    //    }
    //                    //}
    //                    //if (Convert.ToDouble(split_kva_item[j].Trim()) == 125 && model_type.Trim().Substring(0, 2).Trim() == "6R")
    //                    // {
    //                    //     //Dont attach catlouge
    //                    // }
    //                    // else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1010) && flg_320_1010 == 0)
    //                    // {
    //                    //     // Commented by KB as per mail from Kalpesh
    //                    //     strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/320-1010-kVA.pdf";
    //                    //     strpdf_2 = "320-1010-kVA-Catlogue.pdf";
    //                    //     flg_320_1010 = 1;
    //                    // }
    //                    //else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 1250 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1500) && flg_1250_1500 == 0)
    //                    //{
    //                    //    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/New_Green_750KVA_to_1500kVA.pdf";
    //                    //    strpdf_2 = "1250-1500-kVA-Catlogue.pdf";
    //                    //    flg_1250_1500 = 1;
    //                    //}
    //                    // else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 180 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250) && flg_180_250 == 0)
    //                    // {
    //                    //     strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/180-250-kVA.pdf";
    //                    //     strpdf_2 = "180-250-kVA-Catalogue.pdf";
    //                    //     flg_180_250 = 1;
    //                    // }
    //                    // // New Catgouge
    //                    // else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_5_160 == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC") // EA Series
    //                    // {
    //                    //     strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/iGreen-5-160kVA.pdf";
    //                    //     strpdf_2 = "iGreen_5_To_160_kVA_Catlogue.pdf";
    //                    //     flg_5_160 = 1;
    //                    // }
    //                    // else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && model_type.Trim().Substring(0, 2).Trim() == "CC" && flg_2_5_diesel == 0) // Petrol
    //                    // {
    //                    //     strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/3-5-kVA-KCC.pdf";

    //                    //     strpdf_2 = "3-5-kVA-KCC-Catlogue.pdf";
    //                    //     flg_2_5_diesel = 1;
    //                    // }

    //                    if (File.Exists(strFilePath_2))
    //                    {
    //                        FileStream fs2 = new FileStream(strFilePath_2, FileMode.Open, FileAccess.Read);
    //                        Attachment a2 = new Attachment(fs2, strpdf_2, MediaTypeNames.Application.Octet);
    //                        m.Attachments.Add(a2);
    //                    }

    //                    // New Anubandh
    //                    if (Convert.ToDouble(split_kva_item[j].Trim()) == 125 && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() == "6R")
    //                    {
    //                        //Skip Anubandh for 6R
    //                    }
    //                    else
    //                    if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
    //                    {
    //                        //Skip Anubandh by KB 21/12/2023
    //                        //strFilePath_3 = AppDomain.CurrentDomain.BaseDirectory + "quotation/Anubandh.pdf";
    //                        //strpdf_3 = "Anubandh.pdf";
    //                        //flg_anubandh = 1;

    //                        if (File.Exists(strFilePath_3))
    //                        {
    //                            //Skip Anubandh by KB 21/12/2023
    //                            //FileStream fs3 = new FileStream(strFilePath_3, FileMode.Open, FileAccess.Read);
    //                            //Attachment a3 = new Attachment(fs3, strpdf_3, MediaTypeNames.Application.Octet);
    //                            //m.Attachments.Add(a3);
    //                        }
    //                    }
    //                    //End 
    //                }
    //            }
    //            #endregion

    //            m.From = new MailAddress("erp@kalabiz.com", "Kala Genset Pvt. Ltd. - ERP System");

    //            #region
    //            //ToMail
    //            if (to_mail_id.Length > 5 && !string.IsNullOrEmpty(to_mail_id))
    //            {
    //                string[] ToMailIDitems = Regex.Split(to_mail_id.Trim(), ",");
    //                foreach (string strTo in ToMailIDitems)
    //                {
    //                    m.To.Add(new MailAddress(strTo));
    //                }
    //            }
    //            //ToMail END

    //            //CCMail 
    //            if (cc_mail_id.Length > 5 & !string.IsNullOrEmpty(cc_mail_id))
    //            {
    //                string[] CCMailIDitems = Regex.Split(cc_mail_id.Trim(), ",");
    //                foreach (string strCC in CCMailIDitems)
    //                {
    //                    m.CC.Add(new MailAddress(strCC));
    //                }
    //            }
    //            //CCMail  END

    //            //BCCMail                 
    //            string[] BCCMailIDitems = Regex.Split("adg@kalabiz.com", ",");
    //            foreach (string strBCC in BCCMailIDitems)
    //            {
    //                m.Bcc.Add(new MailAddress(strBCC));
    //            }

    //            //BCCMail  END
    //            #endregion

    //            #region
    //            Body = "";
    //            Body += "<p>To,";
    //            Body += "<BR><span style='color:#1F497D'>" + cust_name.Trim() + " </span>";
    //            Body += "<BR><span style='color:#1F497D'>" + cust_add.Trim() + " </span></p>";

    //            Body += "<p>Subject: Proposal for " + kva.ToString().Trim() + " kVA Silent DG Set</p>";

    //            Body += "<p>KALA a popular name synonymous with power, stands tall as a market leader ";
    //            Body += "serving industrial, commercial and residential segments in the domestic as well ";
    //            Body += "as international markets.</p>";

    //            Body += "<p>KALA an IMS(Integrated Management System) Certified company has grown to be one of the biggest names in power ";
    //            Body += "generation solutions. Today, our team continues to focus on constant innovation ";
    //            Body += "and commitment towards Customer Satisfaction. We concentrate on our ";
    //            Body += "efforts in bringing the highest quality power solutions to your corporate/business/residence.</p>";

    //            Body += "<p>We as an authorized Genset Original Equipment Manufacturers (G.O.E.M's) for ";
    //            Body += "Kirloskar Oil Engines Limited-Pune, manufacture quality KOEL Green ";
    //            Body += "Generating Sets in the range of 2.1 KVA to 1500 KVA for captive and standby ";
    //            Body += "applications with options such as liquid cooled and air cooled DG sets.</p>";

    //            Body += "<p>We provide complete turnkey solutions which include site selection, load study, ";
    //            Body += "application engineering, installation, commissioning and obtaining statutory ";
    //            Body += "approvals from the Government agencies.</p>";

    //            Body += "<p>With our headquarters at Pune, We are having 3 state of the art, ultra-modern ";
    //            Body += "manufacturing and testing facilities at Chakan, Silvassa, Belgaum. We are able ";
    //            Body += "to cater to both logistic requirements as well as commercial benefits in ";
    //            Body += "taxations. We manufacture world class accoustic enclosures (Sound Proof ";
    //            Body += "Canopies) as per Central Pollution Control Board (CPCB) Norms.</p>";

    //            Body += "<p style='text-align:justify'>";
    //            Body += "Our dedicated branch offices at Pune, Mumbai(Kharghar), Kolhapur, ";
    //            Body += "Solapur,Indore,Bhopal and Bangalore, Belgaon, Hyderabad along with the business associates &amp; dealers are available to ";
    //            Body += "provide sales and service to our widespread customer network. Our ";
    //            Body += "marketing team is always available at your service to meet your requirements ";
    //            Body += "and satisfy all your needs 24/7. We understand the popular saying 'Customers ";
    //            Body += "have a choice' and we are grateful to you for considering us as your ";
    //            Body += "preferred choice.</p>";

    //            Body += "<p>For Genset Details & Commercial, Please find attachment.</p>";

    //            Body += "<p style='text-align: justify'><span><span style='color:#1F497D'>Thank You/Regards.</span></span></p>";

    //            Body += "<p style='text-align: justify'><span><span style='color:#1F497D'>" + ass_to_emp_name.Trim() + " <br> MailID - " + ass_to_mail_id.Trim() + " <br> MobileNo - " + ass_to_mob_no.Trim() + " <br> Toll Free No - 1800 123 0018</span></span></p>";

    //            Body += "<BR><img alt=\"\" hspace=0 src=\"cid:imageId\" align=baseline border=0>";

    //            if (Original_Qtn_No.Trim().Substring(10, 2).Trim() == "16")
    //            {
    //                Body += "<p style='text-align: justify'><span><span style='color:#1F497D'><b>Kala International LLC</b></span></span></p>";
    //            }
    //            else
    //            {
    //                Body += "<p style='text-align: justify'><span><span style='color:#1F497D'><b>Kala Genset Pvt ltd.</b></span></span></p>";
    //            }

    //            #endregion

    //            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, null, "text/html");
    //            LinkedResource imagelink = new LinkedResource(Server.MapPath("~//images//kalalogo.JPG"), "image/png");
    //            imagelink.ContentId = "imageId";
    //            imagelink.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
    //            htmlView.LinkedResources.Add(imagelink);
    //            m.AlternateViews.Add(htmlView);

    //            m.Subject = "Proposal for " + kva.ToString().Trim() + " kVA Silent DG Set";

    //            m.IsBodyHtml = true;
    //            m.Body = Body;
    //            m.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

    //            if (ass_to_mail_id.Length > 5 && !string.IsNullOrEmpty(ass_to_mail_id.ToString().Trim()))
    //            {
    //                m.ReplyTo = new MailAddress(ass_to_mail_id.ToString().Trim());
    //            }

    //            if (sc != null)
    //            {
    //                sc.Credentials = new System.Net.NetworkCredential("erp@kalabiz.com", "wanv ftwc dobq blrr");
    //                sc.Port = 587;
    //                sc.Host = "smtp.gmail.com";
    //                sc.EnableSsl = true;
    //                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
    //                sc.Timeout = 999999999;
    //                sc.ServicePoint.MaxIdleTime = 1;
    //                sc.Send(m);
    //            }

    //            m.Dispose();
    //            sc = null;
    //            m = null;

    //            fs1.Close();
    //            fs1.Dispose();
    //        }
    //        Context.Response.Write("Success");
    //    }
    //    catch (Exception ex)
    //    {

    //        Context.Response.Write("Failed StackTrace" + ex.StackTrace + ", Message" + ex.Message);
    //    }
    //}




}