using System.Web.Http;
using System.Data;
using KalaERPApi.Service;
using System.Text;
using KalaERPApi.Service.Marketing.Trans;
using KalaERPApi.Models.Marketing.Trans;
using System.Text.RegularExpressions;
using System.Net;
using System;
using Antlr.Runtime.Tree;
using System.Linq;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace KalaERPApi.Controllers.Marketing.Plan
{
    public class EnquiryController : ApiController
    {
        StringBuilder sb = new StringBuilder();
        CommonCon dc = new CommonCon();
        CommonCon cf = new CommonCon();
        DataTable ds = new DataTable();
        EnquiryService ser = new EnquiryService();
        DataSet dsFillData = new DataSet();
        DataSet dsFilltemp = new DataSet();

        string emailStatus = "";
        //Enq Dashboard report  
        #region


        [HttpGet]
        [Route("Enquiry/GetEnquiryCard")]
        public DataTable GetEnquiryCard(string Type, string fromDate, string ToDate, string BranchCode, string Empode)
        {
            sb.Remove(0, sb.Length);
            sb.Append("exec [MKT_ENQ_ERP_dashBoard] '" + Type.Trim() + "','" + fromDate.Trim() + "','" + ToDate.Trim() + "','" + BranchCode.Trim() + "','" + Empode.Trim() + "' ");
            return dc.procDT(sb.ToString(), "tbl_EnquiryCard");
        }


        [HttpGet]
        [Route("Enquiry/getGetBranchList")]
        public DataTable getGetBranchList(string Type, string userId, string CompId, string PCCode)
        {
            sb.Remove(0, sb.Length);

            if (Type.Trim() == "A" || Type.Trim() == "P"
          || PCCode.Trim() == "07.011"
          || userId.Trim() == "07250005" || userId.Trim() == "07250006"
          ) //Admin
            {
                sb.Append(" Select E.Branchcode as BranchCode,P.PCName as BranchName from Enquiry E WITH (NOLOCK)  INNER JOIN ProfitCenter P WITH (NOLOCK)  on e.Branchcode =P.PCCode  where   E.CompanyCode='" + CompId.ToString().Trim() + "' and  E.Active='1'  group by E.Branchcode,P.PCName Union All select '0' as Branchcode ,'ALL' as BranchName order by BranchName ");
            }
            else //User 
            {
                sb.Append(" Select E.ProfitCenter as BranchCode,P.PCName as BranchName from EMPLOYEE E WITH (NOLOCK)  INNER JOIN ProfitCenter P WITH (NOLOCK)  on E.ProfitCenter =P.PCCode where E.Active='1'and E.Ecode='" + userId.Trim() + "'  group by E.ProfitCenter,P.PCName  order by BranchName ");
            }
            return dc.procDT(sb.ToString(), "tbl_Branch");

        }

        [HttpGet]
        [Route("Enquiry/getGetEmployeeList")]
        public DataTable getGetEmployeeList(string Type, string userId, string CompId, string PCCode)
        {
            sb.Remove(0, sb.Length);

            if (Type.Trim() == "A" || Type.Trim() == "P"
          || PCCode.Trim() == "07.011"
          || userId.Trim() == "07250005" || userId.Trim() == "07250006"
          ) //Admin
            {
                sb.Append(" EXEC LoadAssignToName '" + PCCode.Trim() + "'  ,'0' ");
            }
            else //User 
            {
                sb.Append(" Select EM.Ecode as Code,EM.Fname + ' ' + EM.Lname + ' --> ' + EM.Ecode as NAME from  EMPLOYEE EM WITH (NOLOCK)  where EM.Active='1'  and EM.Ecode='" + userId.Trim() + "'   group by EM.Ecode,EM.Fname,EM.Lname order by NAME  ");
            }
            return dc.procDT(sb.ToString(), "tbl_Emp");

        }

        [HttpGet]
        [Route("Enquiry/MktEmpdtlsList")]
        public DataTable MktEmpdtlsList(string Type, string userId, string CompId, string PCCode)
        {
            sb.Remove(0, sb.Length);
            sb.Append(" Select EM.Ecode as ECode,EM.Fname + ' ' + EM.Lname + ' --> ' + EM.Ecode as EName from  EMPLOYEE EM WITH (NOLOCK)  where EM.Active='1'  and EM.Ecode='" + userId.Trim() + "'   group by EM.Ecode,EM.Fname,EM.Lname order by ENAME  ");

            return dc.procDT(sb.ToString(), "tbl_Emp");
        }

        [HttpGet]
        [Route("Enquiry/GetEnquiryData_Card_Cnt")]
        public DataTable GetEnquiryData_Card_Cnt(string LoginuserId, string LoginUserType, string LoginCompId, String ddlSearchBy, String txtFrmDate, String txtToDate, string ddlBranchCode, string ddlEmployeeCode)
        {
            sb.Remove(0, sb.Length);
            if (ddlSearchBy.Trim() == "ENQStatus_ALL_CNT" || ddlSearchBy.Trim() == "")
            {
                if (LoginuserId.Trim() == "07250005" || LoginuserId.Trim() == "07250006") // For Help Desk
                {
                    // Show C2 To C4 ,KGD ENQ
                    sb.Append(" EXEC [MKT_ENQ_ERP_dashBoard_Final] 'ENQDtlsWise','HelpDesk' ,'ENQStatus_ALL_CNT',NULL,NULL,'" + ddlBranchCode.Trim() + "','" + ddlEmployeeCode.Trim() + "' ");
                }
                else
                {
                    sb.Append(" EXEC [MKT_ENQ_ERP_dashBoard_Final]  'ENQDtlsWise','KALA' ,'ENQStatus_ALL_CNT',NULL,NULL,'" + ddlBranchCode.Trim() + "','" + ddlEmployeeCode.Trim() + "' ");
                }
            }
            else
            {
                if (LoginuserId.Trim() == "07250005" || LoginuserId.Trim() == "07250006") // For Help Desk
                {
                    // Show C2 To C4 ,KGD ENQ
                    sb.Append(" EXEC [MKT_ENQ_ERP_dashBoard_Final]  'ENQDtlsWise','HelpDesk', '" + ddlSearchBy.Trim() + "','" + txtFrmDate.Trim() + "','" + txtToDate.Trim() + "','" + ddlBranchCode.Trim() + "','" + ddlEmployeeCode.Trim() + "' ");
                }
                else
                {
                    sb.Append(" EXEC [MKT_ENQ_ERP_dashBoard_Final]  'ENQDtlsWise','KALA', '" + ddlSearchBy.Trim() + "','" + txtFrmDate.Trim() + "','" + txtToDate.Trim() + "','" + ddlBranchCode.Trim() + "','" + ddlEmployeeCode.Trim() + "'");
                }
            }
            return dc.procDT(sb.ToString(), "tbl_EnquiryData_Card_Cnt");
        }


        [HttpGet]
        [Route("Enquiry/GetEnquiryGridData_Dtls")]
        public DataTable GetEnquiryGridData_Dtls(string LoginuserId, string LoginUserType, string LoginCompId, String ddlSearchBy, String txtFrmDate, String txtToDate, string ddlBranchCode, string ddlEmployeeCode, string strFStatus)
        {
            // ddlSearchBy = "ENQStatus_ALL_CNT";

            sb.Remove(0, sb.Length);

            if (ddlSearchBy.Trim() == "ENQStatus_ALL_CNT" || ddlSearchBy.Trim() == "")
            {
                if (LoginuserId.Trim() == "07250005" || LoginuserId.Trim() == "07250006") // For Help Desk
                {
                    // Show C2 To C4 ,KGD ENQ
                    sb.Append(" EXEC [MKT_ENQ_ERP_dashBoard_Dtls_Final] 'HelpDesk' , 'ENQStatus_ALL_CNT',NULL,NULL,'" + ddlBranchCode.Trim() + "','" + ddlEmployeeCode.Trim() + "','" + strFStatus.Trim() + "','0'");
                }
                else
                {
                    sb.Append(" EXEC [MKT_ENQ_ERP_dashBoard_Dtls_Final] 'KALA' , 'ENQStatus_ALL_CNT',NULL,NULL,'" + ddlBranchCode.Trim() + "','" + ddlEmployeeCode.Trim() + "','" + strFStatus.Trim() + "','0'");
                }
            }
            else
            {
                if (LoginuserId.Trim() == "07250005" || LoginuserId.Trim() == "07250006") // For Help Desk
                {
                    // Show C2 To C4 ,KGD ENQ
                    sb.Append(" EXEC [MKT_ENQ_ERP_dashBoard_Dtls_Final] 'HelpDesk' ,'" + ddlSearchBy.Trim() + "','" + txtFrmDate.Trim() + "','" + txtToDate.Trim() + "','" + ddlBranchCode.Trim() + "','" + ddlEmployeeCode.Trim() + "','" + strFStatus.Trim() + "','0'");
                }
                else
                {
                    sb.Append(" EXEC [MKT_ENQ_ERP_dashBoard_Dtls_Final] 'KALA' ,'" + ddlSearchBy.Trim() + "','" + txtFrmDate.Trim() + "','" + txtToDate.Trim() + "','" + ddlBranchCode.Trim() + "','" + ddlEmployeeCode.Trim() + "','" + strFStatus.Trim() + "','0'");
                }
            }
            return dc.procDT(sb.ToString(), "tbl_EnquiryData_Card_Cnt");
        }



        [HttpGet]
        [Route("Enquiry/GetEnquiry_View")]
        public DataTable GetEnquiry_View(string CompId, string EnqNo, string PartCode)
        {
            sb.Remove(0, sb.Length);
            sb.Append(" EXEC rptEnquiryReport '" + CompId.Trim() + "','0','0','0','" + EnqNo.ToString().Trim() + "','0','" + PartCode.Trim() + "','0','0','0','0','0','0','0','0','0'");
            return dc.procDT(sb.ToString(), "tbl_GetEnquiry_View");
        }


        [HttpGet]
        [Route("Enquiry/Check_Qtn_Send_Y_Or_N")]
        public DataTable Check_Qtn_Send_Y_Or_N(string EnqNo)
        {
            sb.Remove(0, sb.Length);
            sb.Append(" select isnull(count(QtnNo),0) as Cnt from quotation where enqno='" + EnqNo.ToString().Trim() + "' and Active='1' ");
            return dc.procDT(sb.ToString(), "tbl_Check_Qtn_Send_Y_Or_N");
        }


        #endregion
        //Enq Dashboard report 

        //Enq Dashboard Trans Form Query
        #region 


        //[HttpGet]
        //[Route("Enquiry/GetProductRangeAngular")]
        //public DataTable GetProductRangeAngular(string KVA, string Phase, string Model, string Panel, string Type)
        //{
        //    sb.Remove(0, sb.Length);
        //    sb.Append("exec [getKVAPhaseModel_spAngular] '" + KVA.Trim() + "','" + Phase.Trim() + "','" + Model.Trim() + "','" + Panel.Trim() + "','" + Type.Trim() + "' ");
        //    return dc.procDT(sb.ToString(), "tbl_ProductRange");
        //}


        [HttpGet]
        [Route("Enquiry/getKVAPhaseModel_EnqDashboard")]
        public DataTable getKVAPhaseModel_EnqDashboard_spAngular(string KVA, string Phase, string Model, string Panel, string Type, String ReqEnqNo, string ReqPartCode)
        {
            sb.Remove(0, sb.Length);
            sb.Append(" exec [getKVAPhaseModel_EnqDashboard_spAngular] '" + KVA.Trim() + "','" + Phase.Trim() + "','" + Model.Trim() + "','" + Panel.Trim() + "','" + Type.Trim() + "' ");

            dsFillData = null;
            dsFillData = dc.procDS(sb.ToString(), "tbl_KVAPhaseModel_EnqDashboard");

            for (int i = 0; (i < dsFillData.Tables["tbl_KVAPhaseModel_EnqDashboard"].Rows.Count); i++)
            {
                //DO NOT Change code -- SKK (Code check Do not insert Duplicate Partcode in ENQ when It Updated)
                if (ReqEnqNo != "null" && ReqEnqNo != "0" && Type.Trim() == "PartDesc")
                {
                    sb.Remove(0, sb.Length);
                    sb.Append(" Select  P.PartDesc + ' --> ' + P.PartCode as PartDesc  ,P.PartCode  from EnquiryDetails E inner join Part P On E.Partcode=P.PartCode where E.ENQNO=  '" + ReqEnqNo + "' ");
                    dsFilltemp = null;
                    dsFilltemp = dc.procDS(sb.ToString(), "EnquiryDetails_Part");
                    if (dsFilltemp.Tables["EnquiryDetails_Part"].Rows.Count > 0)
                    {
                        for (int P = 0; (P < dsFilltemp.Tables["EnquiryDetails_Part"].Rows.Count); P++)
                        {
                            //Do not rermove  // For Followup  Part Enable 
                            if (dsFilltemp.Tables["EnquiryDetails_Part"].Rows[P]["PartCode"].ToString().Trim() != ReqPartCode.ToString().Trim())
                            {
                                if (dsFillData.Tables["tbl_KVAPhaseModel_EnqDashboard"].Rows[i]["PartCode"].ToString().Trim() == dsFilltemp.Tables["EnquiryDetails_Part"].Rows[P]["PartCode"].ToString().Trim())
                                {
                                    dsFillData.Tables["tbl_KVAPhaseModel_EnqDashboard"].Rows.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
            }

            return dsFillData.Tables["tbl_KVAPhaseModel_EnqDashboard"];
        }


        [HttpGet]
        [Route("Enquiry/GetCity_EnqDashboard")] // OK
        public DataTable GetCity_EnqDashboard(string BranchCode)
        {
            //sb.Remove(0, sb.Length);
            //sb.Append("Select CT.CName,CT.CName+'(' +ST.SAliseName + ') --> '+ cast(CT.CID as varchar) as CityName,cast(CT.CID as varchar) as CityCode from City CT inner join State ST on CT.StateID=ST.SID ");
            //sb.Append("where CT.Active = '1' and CT.Auth = '1' and ST.Active = '1' and ST.Auth = '1' and st.countryid = '01' ");


            sb.Remove(0, sb.Length);
            sb.Append(" Select CT.CName + ' --> ' + D.DName  + ' --> ' +  ST.SName   + ' --> ' +  C.Cname   as CityName ");
            sb.Append(" ,C.CID  + ' --> ' + cast(ST.SID   as varchar)   + ' --> ' +  cast(CT.CID as varchar)  as CityCode  ");
            sb.Append(" from City  CT inner join District D on CT.DistrictID=D.DID   ");
            sb.Append(" inner join State ST on CT.StateID=ST.SID  inner join Country C on  ST.CountryID=C.CID  ");
            sb.Append(" where D.Active='1' and D.Auth='1' and CT.Active='1'  ");
            sb.Append(" and CT.Auth='1' and ST.Active='1' and ST.Auth='1' and  C.Active='1' and C.Auth='1'  ");


            if (BranchCode.Trim().Substring(0, 2).Trim() == "07")
            {
                if (BranchCode.Trim() == "07.008"  //Pune
                    || BranchCode.Trim() == "07.003"  // HO
                     || BranchCode.Trim() == "07.028" //CP
                      || BranchCode.Trim() == "07.019" //Kolhapur
                       || BranchCode.Trim() == "07.010" //Mkt Corporate
                        || BranchCode.Trim() == "07.045" // Sangli
                        || BranchCode.Trim() == "07.044" // Satara
                        || BranchCode.Trim() == "07.020" // Solapur

                    ) //MH
                {
                    sb.Append("  and c.cid='01' and ST.SID='1' ");
                }
                else if (BranchCode.Trim() == "07.005" //Indore
                    || BranchCode.Trim() == "07.014" //Bhopal
                    || BranchCode.Trim() == "07.022") // MP
                {
                    sb.Append("  and c.cid='01' and ST.SID='6' ");
                }
                else if (BranchCode.Trim() == "07.014") // GOA
                {
                    sb.Append("  and c.cid='01' and ST.SID='24' ");
                }
                else if (BranchCode.Trim() == "07.021" //Belgaum
                    ||
                    BranchCode.Trim() == "07.039" //Gulberga
                  ) // KA
                {
                    sb.Append("  and c.cid='01' and ST.SID='3' ");
                }
                else
                {
                    sb.Append("  and c.cid='01' ");
                }
            }
            else
            {
                sb.Append("  and c.cid='01' ");
            }
            sb.Append(" order by CT.CName,D.DName,ST.SName,C.CName ");
            return dc.procDT(sb.ToString(), "tbl_CityName");
        }


        [HttpGet]
        [Route("Enquiry/GetSegment_EnqDashboard")]// OK
        public DataTable GetSegment_EnqDashboard()
        {
            sb.Remove(0, sb.Length);
            sb.Append("exec GetSegment_EnqDashboard_sp");
            return dc.procDT(sb.ToString(), "tbl_Sector");
        }

        [HttpGet]
        [Route("Enquiry/GetTitlePrefixList_EnqDashboard")] // OK
        public DataTable GetTitlePrefixList_EnqDashboard()
        {
            sb.Remove(0, sb.Length);
            sb.Append("select PrefixCode,Prefix from Salutation_Mst where Active='1'  AND PrefixCode <>'01' order by PrefixCode");
            return dc.procDT(sb.ToString(), "tbl_TitlePrefixList");
        }

        [HttpGet]
        [Route("Enquiry/GetReferenceFromList_EnqDashboard")] // OK
        public DataTable GetReferenceFromList_EnqDashboard()
        {
            sb.Remove(0, sb.Length);
            sb.Append("select ReferenceName,REFCode From Reference_EnqMaster  Where Active='1' and Auth='1'  AND REFCode <>'01' order by ReferenceName");
            return dc.procDT(sb.ToString(), "tbl_Reference");
        }

        [HttpGet]
        [Route("Enquiry/GetFollowUpStatus_EnqDashboard")] // OK
        public DataTable GetFollowUpStatus_EnqDashboard(string LoginUserID)
        {
            sb.Remove(0, sb.Length);
            if (LoginUserID.Trim() == "07250005" || LoginUserID.Trim() == "07250006") //MKT Survay

            {
                sb.Append(" select StatusCode,StatusID,StatusDescription from FollowupStatus where Active=1 and StatusID NOT IN ('04','0','00','08','09','10','13','14','15','16','17') order by StatusCode Asc ");

            }
            else //KALA Emp
            {
                sb.Append(" select StatusCode,StatusID,StatusDescription from FollowupStatus where Active=1 and StatusID NOT IN('0','00','08','09','10','13','14','15','16','17') order by StatusCode Asc ");

            }
            return dc.procDT(sb.ToString(), "tbl_FollowUpStatus");
        }


        [HttpGet]
        [Route("Enquiry/GetFollowupRemarkHeads_EnqDashboard")] // OK
        public DataTable GetFollowupRemarkHeads_EnqDashboard()
        {
            sb.Remove(0, sb.Length);
            sb.Append(" select EnqFollowupRemark,Code From EnqFollowupRemarkMaster  Where Active='1' and Auth='1'  AND  Code <>'01'  order by EnqFollowupRemark ");
            return dc.procDT(sb.ToString(), "tbl_GetFollowupRemarkHeads_EnqDashboard");
        }


        [HttpGet]
        [Route("Enquiry/GetLostToWhomList")]
        public DataTable GetLostToWhomList() // OK
        {
            sb.Remove(0, sb.Length);
            sb.Append("select MfgName +' ('+ rtrim(MfgType) + ') ' as MfgName,id,rtrim(MfgType) as MfgType from GenratorManufacturer  " +
                    " union all select 'Other' as MfgName,'0' as id,'0' as MfgType order by MfgType,MfgName ");
            return dc.procDT(sb.ToString(), "WhomList");
        }

        [HttpGet]
        [Route("Enquiry/GetLostReasonList")]
        public DataTable GetLostReasonList() // OK
        {

            sb.Remove(0, sb.Length);
            sb.Append("Select ELID,ELNAME from EnqLossMaster where Active='1'");
            return dc.procDT(sb.ToString(), "tbl_EnqLos");

        }

        [HttpGet]
        [Route("Enquiry/GetEnq_SerchByCustomerDetails")]
        public DataTable GetEnq_SerchByCustomerDetails(string CustName, string MktUserCode) // OK
        {

            sb.Remove(0, sb.Length);
            //  sb.Append("select EnqNo , convert(varchar(10),dt,103)as Date,CustName,Address,MobileNo,EMailID as EnqInfo From Enquiry where active=1  and (CustName like 'SATISH%' and Assigntoempid='3497') order by EnqNo desc ");
            sb.Append("select EnqNo + ' #Date: ' + convert(varchar(10),dt,103)+ ' #Cust: ' +CustName+ ' #Address: ' +Address+ ' #Mobile: ' +MobileNo+ ' #EMail: ' +EMailID as ENQINFO,EnqNo From Enquiry where active=1 and (CustName like '" + CustName.Trim() + "%' and Assigntoempid='" + MktUserCode.Trim() + "') order by EnqNo desc ");
            return dc.procDT(sb.ToString(), "tbl_CustDetails");

        }

        #endregion
        //Enq Dashboard Trans Form Query


        //[HttpGet]
        //[Route("Enquiry/GetGenratorManufacturer")]
        //public DataTable GetGenratorManufacturer()
        //{
        //    sb.Remove(0, sb.Length);
        //    sb.Append("select MfgName +'('+ rtrim(MfgType) + ')' as MfgName,id from GenratorManufacturer ");
        //    sb.Append("union all ");
        //    sb.Append("select 'Other' as MfgName,0 as id order by id,MfgName");
        //    return dc.procDT(sb.ToString(), "tbl_GenratorManufacturer");
        //}


        [HttpGet]
        [Route("Enquiry/GetSegment")]
        public DataTable GetSegment()
        {
            sb.Remove(0, sb.Length);
            sb.Append("exec getSector_sp");
            return dc.procDT(sb.ToString(), "tbl_Sector");
        }



        [HttpGet]
        [Route("Enquiry/GetEmpName")]
        public DataTable GetEmpName(string LoginType, string ECode, string PCCode)
        {
            sb.Remove(0, sb.Length);
            sb.Append("select e1.FName+' '+e1.Lname as EName,e1.ECode from employee e1 where empstatus='P' ");
            if (LoginType.Trim() == "A")
            {
                sb.Append("and e1.DeptName like '%Marketing%' or e1.ECode in(select e.ECode from Employee e ");
                sb.Append("inner join Loginmst l on e.ecode=l.name where l.LoginType='A' and e.EmpStatus='P' and l.active='1') ");
            }
            else
            {
                sb.Append("and ((e1.ProfitCenter in (Select PCcode from ProfitCenter where HODECode = '" + ECode.Trim() + "')) OR e1.ECode = '" + ECode.Trim() + "') ");
            }
            sb.Append("and e1.CompanyCode='07' order by e1.FName");
            return dc.procDT(sb.ToString(), "tbl_Employee");
        }

        [HttpGet]
        [Route("Enquiry/GetCityName")]
        public DataTable GetCityName(string StateID)
        {
            sb.Remove(0, sb.Length);
            sb.Append("Select CT.CName,CT.CName+'(' +ST.SAliseName + ') --> '+ cast(CT.CID as varchar) as CityName,cast(CT.CID as varchar) as CityCode from City CT inner join State ST on CT.StateID=ST.SID ");
            sb.Append("where CT.Active = '1' and CT.Auth = '1' and ST.Active = '1' and ST.Auth = '1' and st.countryid = '01' ");
            if (StateID.Trim() == "0") //KFress
            {
                sb.Append(" and ST.SID in('1','11') ");
            }
            else if (StateID.Trim() == "1") // MH & KT
            {
                sb.Append(" and DistrictID <> '0' and ST.SID  in('1','3') ");
            }
            else if (StateID.Trim() != "0")
            {
                sb.Append(" and DistrictID <> '0' and ST.SID='" + StateID.Trim() + "' ");
            }

            sb.Append("order by ST.SName,CT.CName");
            return dc.procDT(sb.ToString(), "tbl_CityName");
        }

        [HttpGet]
        [Route("Enquiry/GetProductRange")]
        public DataTable GetProductRange(string KVA, string Phase, string Model, string Panel, string Type)
        {
            sb.Remove(0, sb.Length);
            sb.Append("exec [getKVAPhaseModel_spApps] '" + KVA.Trim() + "','" + Phase.Trim() + "','" + Model.Trim() + "','" + Panel.Trim() + "','" + Type.Trim() + "' ");
            return dc.procDT(sb.ToString(), "tbl_ProductRange");
        }

        //[HttpGet]
        //[Route("Enquiry/GetProductRangeAngular")]
        //public DataTable GetProductRangeAngular(string KVA, string Phase, string Model, string Panel, string Type)
        //{
        //    sb.Remove(0, sb.Length);
        //    sb.Append("exec [getKVAPhaseModel_spAngular] '" + KVA.Trim() + "','" + Phase.Trim() + "','" + Model.Trim() + "','" + Panel.Trim() + "','" + Type.Trim() + "' ");
        //    return dc.procDT(sb.ToString(), "tbl_ProductRange");
        //}

        [HttpGet]
        [Route("Enquiry/chkEnqAlreadyReg")]
        public DataTable chkEnqAlreadyReg(string Search_Text, string Search_Type, string Comp_Code)
        {
            sb.Remove(0, sb.Length);
            sb.Append("EXEC [EnqFeedCheckSP_New] '" + Search_Text.Trim() + "','" + Search_Type.Trim() + "','" + Comp_Code.Trim() + "'");
            return dc.procDT(sb.ToString(), "tbl_EnquiryLog");
        }

        [HttpGet]
        [Route("Enquiry/getPndEnquiryList")]
        public DataTable getPndEnquiryList(string comp_code, string emp_code, string src_type, string from_dt, string to_dt, string flw_type)
        {
            string[] user_items = null;
            try
            {
                if (emp_code.Length <= 10)
                {
                    string user_data = dc.getName("select l.logintype+'-->'+e.deptname as user_data from loginmst l inner join employee e on e.ecode=l.name where e.ecode='" + emp_code.Trim() + "'", "loginmst", "user_data");
                    user_items = Regex.Split(user_data, "-->");
                }
                sb.Remove(0, sb.Length);
                sb.Append("select top 500 e.EnqNo,convert(varchar(10),e.dt,105) as EnqDt,e.CustName,e.Address,e.ContactPerson,e.RefFrom,e.MobileNo,e.EMailID,Pc.PCname,ed.Qty,");
                sb.Append("ed.PartCode,PT.PartDesc,Pt.KVA,Pt.Phase,Pt.Model,Pt.cfm as Panel,e.AssignToEmpID,EName = case e.Assigntoempid when '0' then 'Pending' else Em.fname+' '+Em.lname end,");
                sb.Append("FromMailID= case em.CompMailID when 'NIL' then '' else em.CompMailID  end,em.CMobileNo as EmpMobileNo,FS1.StatusCode+'('+fs1.StatusDescription +')-->'+efd.FStatusID as FStatus,");
                sb.Append("convert(varchar(10),efd.NextFDate,105) as FDate,efd.Remark as FRemark,c.CName+'('+ST.SAliseName +') --> '+ cast(C.CID as varchar) as City,s.SName+'-->'+e.DomainID as Segment,");
                sb.Append("QuotSend=case isnull((select Top 1 QtnNo from Quotation where EnqNo=e.EnqNo and active = '1'),'N') when 'N' then 'No' else 'Yes' end ");
                sb.Append("from Enquiry e inner join enquirydetails ed on e.enqno=ed.enqno inner join Sector s on s.SID=e.DomainID inner join City c on c.CID = e.City ");
                sb.Append("inner join State ST on C.StateID=ST.SID inner join EnquiryFollowupDetails efd on (ed.FID=efd.FID) inner join FollowupStatus FS1 on efd.FStatusID= FS1.StatusID ");
                sb.Append("inner join part Pt on ed.partcode = pt.Partcode inner join ProfitCenter Pc on e.Branchcode =Pc.PCCode ");
                sb.Append("inner join Employee Em on e.Assigntoempid=Em.ecode where e.active=1 and e.CompanyCode='" + comp_code.Trim() + "' ");
                if (src_type.Trim() == "E")
                {
                    sb.Append("and e.dt>='" + from_dt.Trim() + " 00:00:00' and e.dt<='" + to_dt.Trim() + " 23:59:59' ");
                }
                else if (src_type.Trim() == "F")
                {
                    sb.Append("and efd.NextFDate>='" + from_dt.Trim() + " 00:00:00' and efd.NextFDate<='" + to_dt.Trim() + " 23:59:59' ");
                }
                if (flw_type.Trim() != "0")
                {
                    sb.Append("and efd.FStatusID in ('" + flw_type.Trim() + "') ");
                }
                sb.Append("and efd.FStatusID in ('01','02','06','07') ");
                if (emp_code.Length <= 10)
                {
                    if (user_items[0].Trim() == "U" && (emp_code.Trim() == "0111" || emp_code.Trim() == "0124")) // Kala User Emp
                    {
                        sb.Append("and Em.DeptName like '" + user_items[1].Trim() + "%' ");
                    }
                    else if (user_items[0].Trim() == "U" && (emp_code.Trim() != "0111" || emp_code.Trim() != "0124")) // Kala User HOD             
                    {
                        sb.Append("and Em.ecode='" + emp_code.Trim() + "' ");
                    }
                }
                else
                {
                    sb.Append("and Em.ecode='" + emp_code.Trim() + "' ");
                }
                sb.Append("group by e.EnqNo,e.dt,e.CustName,e.Address,e.ContactPerson,e.RefFrom,e.MobileNo,e.EMailID,Pc.PCname,ed.Qty,");
                sb.Append("ed.PartCode,PT.PartDesc,Pt.KVA,Pt.Phase,Pt.Model,Pt.cfm,e.AssignToEmpID,e.Assigntoempid,Em.fname,Em.lname,");
                sb.Append("em.CompMailID,em.CMobileNo,FS1.StatusCode,convert(varchar(10),efd.NextFDate,105),efd.Remark,c.CName,s.SName,");
                sb.Append("efd.FStatusID,fs1.StatusDescription,e.DomainID,ST.SAliseName,C.CID order by e.enqno desc");
                return dc.procDT(sb.ToString(), "tbl_PndFollowupList");
            }
            catch
            {
                throw;
            }
        }


        [HttpGet]
        [Route("Enquiry/GetInstOffer")]
        public DataTable GetInstOffer(string KVA, string Type)
        {
            sb.Remove(0, sb.Length);
            sb.Append("exec ENQIntsLoadData '" + KVA.Trim() + "','" + Type.Trim() + "'");
            return dc.procDT(sb.ToString(), "tbl_InstOffer");
        }

        // return "S" ;
        [HttpPost]
        [Route("Enquiry/Submit_EnqDashboard")]
        public string Submit_EnqDashboard([FromBody] Enquiry_Submit_EnqDashboard EnquirySubmitEnqDashboard)
        {
           return ser.Submit_EnqDashboard(EnquirySubmitEnqDashboard);
        
        }

        [HttpGet]
        [Route("Enquiry/GetEnquiryPrevious")]
        public DataTable GetEnquiryPrevious(string EnqNo, string PartCode, string Type)
        {
            sb.Remove(0, sb.Length);
            sb.Append("exec EnquiryPreviousEnqDashboard_sp '" + EnqNo.Trim() + "','" + PartCode.Trim() + "','" + Type.Trim() + "'");
            return dc.procDT(sb.ToString(), "tbl_EnquiryPrevious");
        }

        [HttpPost]
        [Route("Enquiry/Submit")]
        public string Submit([FromBody] EnquirySaveModel enquirySaveModel)
        {
            return ser.Submit(enquirySaveModel);
        }

        [HttpPost]
        [Route("Enquiry/UpdateEnquiry")]
        public string UpdateEnquiry([FromBody] EnquirySaveModel enquirySaveModel)
        {
            return ser.UpdateEnquiry(enquirySaveModel);
        }

        [HttpGet]
        [Route("Enquiry/getExistingCustList")]
        public DataTable getExistingCustomerList(string emp_code, string cust_name)
        {
            try
            {
                string loginType = dc.getName("select LoginType from LoginMst where name='" + emp_code.Trim() + "'", "tbl_LoginMst", "LoginType");

                sb.Remove(0, sb.Length);
                sb.Append("select EnqNo,EnqNo + ' #Date: ' + convert(varchar(10),dt,103)+ ' #Cust: ' +CustName+ ' #Address: ' +Address+ ' #Mobile: ' +MobileNo+ ' #EMail: ' +EMailID as ENQINFO ");
                if (loginType.Trim() == "A")
                {
                    sb.Append("From Enquiry where active=1 and CustName like '" + cust_name.Trim() + "%' ");
                }
                else
                {
                    sb.Append("From Enquiry where active=1 and (CustName like '" + cust_name.Trim() + "%' and Assigntoempid='" + emp_code.Trim() + "') ");
                }
                sb.Append("order by EnqNo desc");

                return dc.procDT(sb.ToString(), "tbl_ExistingCustList");
            }
            catch
            {
                throw;
            }
        }

        [HttpGet]
        [Route("Enquiry/getExistingCustListDetails")]
        public DataTable getExistingCustomerListDetails(string enq_no)
        {
            try
            {
                return dc.procDT("EXEC getPRVENQINFO '" + enq_no.Trim() + "'", "tbl_ExistingCustListDetails");
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        [Route("Enquiry/SubmitAngular")]
        public string SubmitAngular([FromBody] EnquirySaveModelAngular enquirySaveModel)
        {
            return ser.SubmitAngular(enquirySaveModel);
        }

        [HttpPost]
        [Route("Enquiry/UpdateFlwUp")]
        public string UpdateFlwUp([FromBody] FollowUpUpdate followUpUpdate)
        {
            return ser.UpdateFollowUp(followUpUpdate);
        }

        [HttpGet]
        [Route("Enquiry/SendPreviousQuot")]

        // Added by KB on 23/01/2026 
        public string SendPreviousQuot(string QtnNo, string To_EMailID, string AssToEmpCode, string CCMailID, string CustName, string Address)
        {
            emailStatus = "Failed";

            //if (string.IsNullOrEmpty(CCMailID))
            //{
            //    ccMailID = "";
            //}

            try
            {
                string str_ccMailID = CCMailID.Trim();
                //string kvaitems = dc.getName("select STUFF((SELECT (',' + cast(kVA as varchar)) AS kVA from Quotation q inner join QuotationDetails qd on q.QtnNo = qd.QtnNo " +
                //" inner join part p on p.PartCode = qd.PartCode where q.Active = '1' and q.QtnNo = '" + QtnNo.Trim() + "' GROUP BY kVA ORDER BY kVA " +
                //" FOR XML PATH(''), TYPE ).value('.', 'VARCHAR(MAX)') ,1,1,'') as kVA", "tbl_Quotation", "kVA");

                DataSet ds = dc.procDS("select FName+' '+LName as ename,cmobileno,compmailID,ProfitCenter from employee where ecode='" + AssToEmpCode.Trim() + "'", "tbl_employee");


                if (string.IsNullOrEmpty(str_ccMailID))
                {
                    if (!string.IsNullOrEmpty(ds.Tables["tbl_employee"].Rows[0]["compmailID"].ToString().Trim()))
                    {
                        str_ccMailID = ds.Tables["tbl_employee"].Rows[0]["compmailID"].ToString().Trim();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(ds.Tables["tbl_employee"].Rows[0]["compmailID"].ToString().Trim()))
                    {
                        str_ccMailID += "," + ds.Tables["tbl_employee"].Rows[0]["compmailID"].ToString().Trim();
                    }
                }

                string StrQKAV = "";
                string iGreen = "N", KG = "N";
                DataSet ds_Qtn_KVA = dc.procDS("select cast(kVA as varchar) AS kVA , Model from Quotation q inner join QuotationDetails qd on q.QtnNo = qd.QtnNo " +
                    " inner join part p on p.PartCode = qd.PartCode where q.Active = '1' and q.QtnNo = '" + QtnNo.Trim() + "'  " +
                    " GROUP BY  kVA,Model  ORDER BY kVA ", "tbl_Quotation");

                for (int i = 0; (i < ds_Qtn_KVA.Tables["tbl_Quotation"].Rows.Count); i++)
                {
                    //SATISH

                    if (string.IsNullOrEmpty(StrQKAV))
                    {
                        StrQKAV = ds_Qtn_KVA.Tables["tbl_Quotation"].Rows[i]["kVA"].ToString().Trim();
                    }
                    else
                    {
                        StrQKAV += "," + ds_Qtn_KVA.Tables["tbl_Quotation"].Rows[i]["kVA"].ToString().Trim();
                    }


                    // 125 6R(kg)
                    if (Convert.ToDouble(ds_Qtn_KVA.Tables["tbl_Quotation"].Rows[i]["kVA"].ToString().Trim()) == 125 && ds_Qtn_KVA.Tables["tbl_Quotation"].Rows[i]["Model"].ToString().Trim().Substring(0, 2).Trim() == "6R")
                    {
                        //KG = "Y";
                    } // iGreen KVA
                    else if ((Convert.ToDouble(ds_Qtn_KVA.Tables["tbl_Quotation"].Rows[i]["kVA"].ToString().Trim()) >= 5 && Convert.ToDouble(ds_Qtn_KVA.Tables["tbl_Quotation"].Rows[i]["kVA"].ToString().Trim()) <= 160)
                        && ds_Qtn_KVA.Tables["tbl_Quotation"].Rows[i]["Model"].ToString().Trim().Substring(0, 2).Trim() != "CC")
                    {
                        iGreen = "Y";
                    } // kg
                    // Added by KB on 22/01/2026 for 12 Months Warranty
                    else if ((Convert.ToDouble(ds_Qtn_KVA.Tables["tbl_Quotation"].Rows[i]["kVA"].ToString().Trim()) >= 7.5)
                        && ds_Qtn_KVA.Tables["tbl_Quotation"].Rows[i]["Model"].ToString().Trim().Substring(0, 2).Trim() == "CC")
                    {
                        iGreen = "W";
                    }
                    else
                    {
                        //KG = "Y";
                    }
                }

                if (!string.IsNullOrEmpty(StrQKAV))
                {

                    string strQuotkVA = "";
                    string[] ch = Regex.Split(StrQKAV, ",");
                    string[] chNew = new string[ch.Length];
                    for (int i = 0; i < ch.Length; i++)
                    {
                        chNew[i] = ch[i].ToString();
                    }
                    string[] distVal = chNew.Distinct().ToArray();
                    foreach (string c in distVal)
                    {
                        if (strQuotkVA.Trim() == "")
                        {
                            strQuotkVA = c.ToString();
                        }
                        else if (strQuotkVA.Trim() != "")
                        {
                            strQuotkVA = strQuotkVA + "," + c.ToString();
                        }
                    }


                    //Send Quotation Mail
                    if (iGreen == "Y" && KG == "Y")
                    {
                        // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditorResponse", "alert('You Cannot Quot iGreen & Non iGreen Product within same Quotation...!')", true);
                    }
                    else
                    {
                        if (iGreen.Trim() == "Y")
                        {
                            sendQuotEmail(ds.Tables["tbl_employee"].Rows[0]["ProfitCenter"].ToString().Trim(), QtnNo.Trim(), strQuotkVA.Trim(), To_EMailID.Trim(), str_ccMailID.Trim(), AssToEmpCode.Trim(), ds.Tables["tbl_employee"].Rows[0]["ename"].ToString().Trim(), ds.Tables["tbl_employee"].Rows[0]["cmobileno"].ToString().Trim(), ds.Tables["tbl_employee"].Rows[0]["compmailID"].ToString().Trim(), CustName.Trim(), Address.Trim(), "iGreen");
                        }
                        else if (iGreen.Trim() == "W")
                        {
                            sendQuotEmail(ds.Tables["tbl_employee"].Rows[0]["ProfitCenter"].ToString().Trim(), QtnNo.Trim(), strQuotkVA.Trim(), To_EMailID.Trim(), str_ccMailID.Trim(), AssToEmpCode.Trim(), ds.Tables["tbl_employee"].Rows[0]["ename"].ToString().Trim(), ds.Tables["tbl_employee"].Rows[0]["cmobileno"].ToString().Trim(), ds.Tables["tbl_employee"].Rows[0]["compmailID"].ToString().Trim(), CustName.Trim(), Address.Trim(), "Warranty");
                        }
                        else
                        {
                            sendQuotEmail(ds.Tables["tbl_employee"].Rows[0]["ProfitCenter"].ToString().Trim(), QtnNo.Trim(), strQuotkVA.Trim(), To_EMailID.Trim(), str_ccMailID.Trim(), AssToEmpCode.Trim(), ds.Tables["tbl_employee"].Rows[0]["ename"].ToString().Trim(), ds.Tables["tbl_employee"].Rows[0]["cmobileno"].ToString().Trim(), ds.Tables["tbl_employee"].Rows[0]["compmailID"].ToString().Trim(), CustName.Trim(), Address.Trim(), "NoniGreen");
                        }
                    }


                    //sendQuotEmail(QtnNo.Trim(), strQuotkVA.Trim(), To_EMailID.Trim(), ccMailID.Trim(), AssToEmpCode.Trim(), ds.Tables["tbl_employee"].Rows[0]["ename"].ToString().Trim(), ds.Tables["tbl_employee"].Rows[0]["cmobileno"].ToString().Trim(), ds.Tables["tbl_employee"].Rows[0]["compmailid"].ToString().Trim(), CustName.Trim(), Address.Trim(), "NoniGreen");

                    //string url = "https://www.kalapms.com/QuotApi/Service.asmx/sendQuotEmailAngular?qtn_no=" + QtnNo.Trim() + "&kva=" + kvaitems.Trim() + "&to_mail_id=" + To_EMailID.Trim()
                    //+ "&cc_mail_id=" + ccMailID.Trim() + "&ass_to_emp_id=" + AssToEmpCode.Trim() + "&ass_to_emp_name=" + ds.Tables["tbl_employee"].Rows[0]["ename"].ToString().Trim()
                    //+ "&ass_to_mob_no=" + ds.Tables["tbl_employee"].Rows[0]["cmobileno"].ToString().Trim() + "&ass_to_mail_id=" + ds.Tables["tbl_employee"].Rows[0]["compmailid"].ToString().Trim()
                    //+ "&cust_name=" + CustName.Trim() + "&cust_add=" + Address.Trim();

                    //HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);
                    //Request.Method = "GET";
                    //Request.KeepAlive = true;
                    //HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
                    //if (Response.StatusCode == HttpStatusCode.OK)
                    //{
                    //    emailStatus = "Mail Send Successfully ";
                    //}
                    //Response.Close();
                }
            }
            catch (Exception ex)
            {
                emailStatus = "Mail Send Failed StackTrace " + ex.StackTrace + ", Message " + ex.Message;
                //emailStatus = "Mail Send Failed StackTrace " + ex.StackTrace + ", Message " + ex.Message;
            }
            return emailStatus;
        }

        // Added by KB on 23/01/2026 
        //    string strFilePath_1 = ConfigurationManager.AppSettings["Quotationfile"] + "/" + "QTN_24-25_07013431_10-03-2025_16_21_53.pdf";
        protected void sendQuotEmail(string qtn_AssToEmpPCCode, string qtn_no, string kva, string to_mail_id, string cc_mail_id, string ass_to_emp_id, string ass_to_emp_name, string ass_to_mob_no,
            string ass_to_mail_id, string cust_name, string cust_add, string model)
        {


            SmtpClient sc;
            string Body = "";
            string Original_Qtn_No = qtn_no;
            qtn_no = qtn_no.Replace("/", "_");
            string strFilePath_1 = ConfigurationManager.AppSettings["Quotationfile"] + "/" + qtn_no + "_" + DateTime.Now.ToString("dd-MM-yyyy") + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".pdf";

            string dt_time = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            string strpdf_1 = qtn_no + "_" + dt_time + ".pdf";
            try
            {
                LoadReport(Original_Qtn_No.Trim(), strFilePath_1.ToString().Trim(), "Q", ass_to_emp_id.Trim(), ass_to_emp_name.Trim(), ass_to_mob_no.Trim(), ass_to_mail_id.Trim(), model.Trim());

                if (strFilePath_1.ToString().Trim() != "")
                {
                    MailMessage m = new MailMessage();
                    sc = new SmtpClient();
                    FileStream fs1 = new FileStream(strFilePath_1, FileMode.Open, FileAccess.Read);
                    Attachment a1 = new Attachment(fs1, strpdf_1, MediaTypeNames.Application.Octet);
                    m.Attachments.Add(a1);

                    #region
                    //int flg_anubandh = 0;
                    //int flg_2kW_5kVA = 0;
                    //int flg_R550 = 0;
                    //int flg_5_To_12_kVA = 0;
                    //int flg_15_To_30_kVA = 0;
                    //int flg_40_To_160_kVA = 0;
                    //int flg_200_To_250_kVA = 0;
                    //int flg_320_1010_kVA = 0;
                    //int flg_1250_kVA_1500 = 0;

                    //int flg_2_8_To_5_5_kVA = 0;
                    //int flg_7_5_To_20_kVA = 0;
                    //int flg_NG_CPCB_IV_15_to_250_kVA = 0;
                    //int flg_OP_CPCB_IV_117_2000_kVA;

                    //string[] split_kva_item = Regex.Split(kva.Trim(), ",");
                    //if (split_kva_item.Length > 0)
                    //{
                    //    for (int j = 0; j < split_kva_item.Length; j++)
                    //    {
                    //        string strFilePath_2 = "", strFilePath_3 = "";
                    //        string strpdf_2 = "", strpdf_3 = "";
                    //        string model_type = dc.getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + Original_Qtn_No.Trim() + "') and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "Model");
                    //        string Qtn_CPCB = dc.getName("select substring(q.PartCode,15,1) as CPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "CPCB");
                    //        //int Opti = Convert.ToInt32 (clsCommonFunctions.getName("select isnull(sum(k.Qty),0) as EngCount from quotationdetails q inner join part p on q.partcode=p.PartCode inner join kitdetails k on k.kitcode=p.PartCode where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "' and k.PartCode like '001%' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "EngCount"));
                    //        string Opti_CPCB = dc.getName("select top 1 substring(q.PartCode,10,1) as OptiCPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "OptiCPCB");

                    //        //if (Qtn_CPCB == "4")
                    //        //{
                    //        //if (model_type.Trim().PadRight(3).Trim() == "NG1")
                    //        if (model_type.Substring(model_type.Trim().Length - 3) == "NG1")
                    //        {
                    //            strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/NG_CPCB IV+15-250_kVA.PDF";
                    //            strpdf_2 = "NG_CPCB IV+15-250_kVA.PDF";
                    //            flg_NG_CPCB_IV_15_to_250_kVA = 1;
                    //        }
                    //        else
                    //        {
                    //            if (Opti_CPCB == "5")
                    //            {
                    //                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/OP_CPCB IV+117-2000_kVA.PDF";
                    //                strpdf_2 = "OP_CPCB IV+117-2000_kVA.PDF";
                    //                flg_OP_CPCB_IV_117_2000_kVA = 1;
                    //            }
                    //            else
                    //            {

                    //                //if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
                    //                //{
                    //                //    //strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/Kirloskar_2kW_5kVA.pdf";
                    //                //    //strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
                    //                //    //flg_2kW_5kVA = 1;
                    //                //}


                    //                //2. 8kW-5.5kVA
                    //                if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 7))
                    //                {
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+2.8-5.5_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+2.8-5.5_kVA.PDF";
                    //                    flg_2_8_To_5_5_kVA = 1;
                    //                }
                    //                // EA Series
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 7 && Convert.ToDouble(split_kva_item[j].Trim()) <= 20))
                    //                {
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+7.5-20_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+7.5-20_kVA.PDF";
                    //                    flg_7_5_To_20_kVA = 1;
                    //                }
                    //                //3R550
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 25 && Convert.ToDouble(split_kva_item[j].Trim()) <= 58.5))
                    //                {
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+25-58.5_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+25-58.5_kVA.PDF";
                    //                    flg_R550 = 1;
                    //                }
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 80 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160))
                    //                {
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+82.5-160_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+82.5-160_kVA.PDF";
                    //                    flg_15_To_30_kVA = 1;
                    //                }
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250))
                    //                {
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+200-250_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+200-250_kVA.PDF";
                    //                    flg_40_To_160_kVA = 1;
                    //                }
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 750))
                    //                {
                    //                    //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+320-750_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+320-750_kVA.PDF";
                    //                    flg_320_1010_kVA = 1;
                    //                }
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 1010 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1500))
                    //                {
                    //                    //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+1010-1500_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+1010-1500_kVA.PDF";
                    //                    flg_320_1010_kVA = 1;
                    //                }
                    //            }
                    //        }

                    //        //}

                    //        // Commented by KB on 23/07/2024 as CPCB 2 is Excluded
                    //        //else
                    //        //{
                    //        //    if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_2kW_5kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
                    //        //        flg_2kW_5kVA = 1;
                    //        //    }
                    //        //    // EA Series
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 12.5) && flg_5_To_12_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/iGreen-5-160kVA.pdf";
                    //        //        strpdf_2 = "Catlogue_iGreen_5_To_160_kVA.pdf";
                    //        //        flg_5_To_12_kVA = 1;
                    //        //    }
                    //        //    //3R550
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 15) && flg_R550 == 0 && model_type.Trim().Substring(0, 5).Trim() == "3R550")
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_R550_15_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_R550_15_kVA_Catlogue.pdf";
                    //        //        flg_R550 = 1;
                    //        //    }
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 30) && flg_15_To_30_kVA == 0)
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_15_To_30_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_15_To_30_kVA_Catlogue.pdf";
                    //        //        flg_15_To_30_kVA = 1;
                    //        //    }
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 40 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_40_To_160_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "6R")
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_40_To_160_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_40_To_160_kVA_Catalogue.pdf";
                    //        //        flg_40_To_160_kVA = 1;
                    //        //    }
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250) && flg_200_To_250_kVA == 0)
                    //        //    {
                    //        //        // Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_200_To_250_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_200_To_250_kVA_Catlogue.pdf";
                    //        //        flg_200_To_250_kVA = 1;
                    //        //    }
                    //        //    //else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1010) && flg_320_1010_kVA == 0)
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) < 1000) && flg_320_1010_kVA == 0)
                    //        //    {
                    //        //        // Commented by KB on 08/05/2023 as per mail from Kalpesh Sir on 06/05/2023
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_320_1010_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_320_1010_kVA_Catlogue.pdf";
                    //        //        flg_320_1010_kVA = 1;
                    //        //    }
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 1250 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1500) && flg_1250_kVA_1500 == 0)
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_1250_To_1500_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_1250_To_1500_kVA_Catlogue.pdf";
                    //        //        flg_1250_kVA_1500 = 1;
                    //        //    }
                    //        //}
                    //        // Commented by KB on 23/07/2024

                    //        if (File.Exists(strFilePath_2))
                    //        {
                    //            FileStream fs2 = new FileStream(strFilePath_2, FileMode.Open, FileAccess.Read);
                    //            Attachment a2 = new Attachment(fs2, strpdf_2, MediaTypeNames.Application.Octet);
                    //            m.Attachments.Add(a2);
                    //        }

                    //        // New Anubandh
                    //        if (Qtn_CPCB != "4")
                    //        {
                    //            if (Convert.ToDouble(split_kva_item[j].Trim()) == 125 && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() == "6R")
                    //            {
                    //                //Skip Anubandh by KB on 21/12/2023

                    //                //Skip Anubandh
                    //            }
                    //            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
                    //            {
                    //                //Skip Anubandh by KB on 21/12/2023
                    //                //strFilePath_3 = AppDomain.CurrentDomain.BaseDirectory + "quotation/quotation/Anubandh.pdf";
                    //                //strpdf_3 = "Anubandh.pdf";
                    //                //flg_anubandh = 1;

                    //                if (File.Exists(strFilePath_3))
                    //                {
                    //                    //Skip Anubandh by KB on 21/12/2023

                    //                    //FileStream fs3 = new FileStream(strFilePath_3, FileMode.Open, FileAccess.Read);
                    //                    //Attachment a3 = new Attachment(fs3, strpdf_3, MediaTypeNames.Application.Octet);
                    //                    //m.Attachments.Add(a3);
                    //                }
                    //            }
                    //        }
                    //        //End 
                    //    }
                    //}
                    #endregion


                    #region
                    int flg_anubandh = 0;
                    //int flg_2kW_5kVA = 0;
                    //int flg_R550 = 0;
                    //int flg_5_To_12_kVA = 0;
                    //int flg_15_To_30_kVA = 0;
                    //int flg_40_To_160_kVA = 0;
                    //int flg_200_To_250_kVA = 0;
                    //int flg_320_1010_kVA = 0;
                    //int flg_1250_kVA_1500 = 0;

                    //int flg_2_8_To_5_5_kVA = 0;
                    //int flg_7_5_To_20_kVA = 0;
                    //int flg_NG_CPCB_IV_15_to_250_kVA = 0;
                    //int flg_OP_CPCB_IV_117_2000_kVA;

                    int flg_NG_CPCB_IV_15_to_250_kVA = 0;
                    int flg_2_To_7_kVA = 0;
                    int flg_7_To_20_kVA = 0;
                    int flg_25_To_58_5_kVA = 0;
                    int flg_80_To_160_kVA = 0;
                    int flg_200_To_250_kVA = 0;
                    int flg_320_TO_750_kVA = 0;
                    int flg_1010_TO_1500_kVA = 0;
                    int flg_OP_CPCB_IV_117_To_2000_kVA = 0;

                    //GK Product 

                    int flg_7_5_To_25_kVA_GK = 0;

                    string strProc = "";
                    strProc += " Select  QD.QtnNo,QD.Qty,QD.BasicPrice, ";
                    strProc += " QD.Partcode,PT.PartDesc,Pt.KVA,Pt.Phase,Pt.Model,Pt.cfm , substring(QD.PartCode,15,1) as CPCB ,substring(QD.PartCode,10,1) as OptiCPCB ";
                    strProc += " ,PT.rating from Quotation Q inner join QuotationDetails  QD on Q.QtnNo=QD.QtnNo inner join Part  PT on PT.PartCode=QD.PartCode ";
                    strProc += " WHERE Q.QtNNo='" + Original_Qtn_No.Trim() + "' ";
                    DataSet ds = dc.procDS(strProc, "QQ1");



                    // Prv Ouotation Details
                    if (ds.Tables["QQ1"].Rows.Count > 0)
                    {
                        for (int i = 0; (i < ds.Tables["QQ1"].Rows.Count); i++)
                        {
                            string str_FilePath = "";
                            string str_pdf_Name = "";


                            string Str_KVA = ds.Tables["QQ1"].Rows[i]["KVA"].ToString().Trim();
                            string model_type = ds.Tables["QQ1"].Rows[i]["Model"].ToString().Trim();
                            string Qtn_CPCB = ds.Tables["QQ1"].Rows[i]["CPCB"].ToString().Trim();
                            string Opti_CPCB = ds.Tables["QQ1"].Rows[i]["OptiCPCB"].ToString().Trim();

                            //GK Product
                            string Str_Product_GKOrNOt = ds.Tables["QQ1"].Rows[i]["rating"].ToString().Trim();

                            //string model_type = clsCommonFunctions.getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "') and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "Model");
                            //string Qtn_CPCB = clsCommonFunctions.getName("select substring(q.PartCode,15,1) as CPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "CPCB");

                            ////int Opti = Convert.ToInt32 (clsCommonFunctions.getName("select isnull(sum(k.Qty),0) as EngCount from quotationdetails q inner join part p on q.partcode=p.PartCode inner join kitdetails k on k.kitcode=p.PartCode where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "' and k.PartCode like '001%' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "EngCount"));
                            //string Opti_CPCB = clsCommonFunctions.getName("select top 1 substring(q.PartCode,10,1) as OptiCPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "OptiCPCB");



                            //if (Qtn_CPCB == "4")
                            //{
                            //if (model_type.Trim().PadRight(3).Trim() == "NG1")
                            if (model_type.Substring(model_type.Trim().Length - 3).Trim() == "NG1")
                            {

                                if (flg_NG_CPCB_IV_15_to_250_kVA == 0)
                                {

                                    str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/NG_CPCB IV+15-250_kVA.PDF";
                                    str_pdf_Name = "NG_CPCB IV+15-250_kVA.PDF";
                                    if (File.Exists(str_FilePath))
                                    {
                                        FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                        Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                        m.Attachments.Add(a2);
                                    }
                                }

                                flg_NG_CPCB_IV_15_to_250_kVA = flg_NG_CPCB_IV_15_to_250_kVA + 1;
                            }
                            else
                            {


                                if (Opti_CPCB.Trim() == "5") //OPTIPrime
                                {
                                    if (flg_OP_CPCB_IV_117_To_2000_kVA == 0)
                                    {
                                        str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/OP_CPCB IV+117-2000_kVA.PDF";
                                        str_pdf_Name = "OP_CPCB IV+117-2000_kVA.PDF";
                                        if (File.Exists(str_FilePath))
                                        {
                                            FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                            Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                            m.Attachments.Add(a2);
                                        }
                                    }

                                    flg_OP_CPCB_IV_117_To_2000_kVA = flg_OP_CPCB_IV_117_To_2000_kVA + 1;


                                }
                                else
                                {


                                    //GK Product
                                    if (Str_Product_GKOrNOt.Trim() == "GK")
                                    {


                                        if ((Convert.ToDouble(Str_KVA.Trim()) >= 7.5 && Convert.ToDouble(Str_KVA.Trim()) <= 25))
                                        {
                                            if (flg_7_5_To_25_kVA_GK == 0)
                                            {
                                                str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV_GK+7.5-25_kVA.PDF";
                                                str_pdf_Name = "CPCB_IV_GK+7.5-25_kVA.PDF";
                                                if (File.Exists(str_FilePath))
                                                {
                                                    FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                                    Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                                    m.Attachments.Add(a2);
                                                }
                                            }

                                            flg_7_5_To_25_kVA_GK = flg_7_5_To_25_kVA_GK + 1;
                                        }



                                    }

                                    else

                                    {


                                        //if ((Convert.ToDouble(Str_KVA.Trim()) >= 2 && Convert.ToDouble(Str_KVA.Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
                                        //{
                                        //    //str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/Kirloskar_2kW_5kVA.pdf";
                                        //    //str_pdf_Name = "Kirloskar_2kW_5kVA_Catlogue.pdf";
                                        //    //flg_2kW_5kVA = 1;
                                        //}



                                        if ((Convert.ToDouble(Str_KVA.Trim()) >= 2 && Convert.ToDouble(Str_KVA.Trim()) <= 7))
                                        {
                                            if (flg_2_To_7_kVA == 0)
                                            {
                                                str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+2.8-5.5_kVA.PDF";
                                                str_pdf_Name = "CPCB_IV+2.8-5.5_kVA.PDF";
                                                if (File.Exists(str_FilePath))
                                                {
                                                    FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                                    Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                                    m.Attachments.Add(a2);
                                                }
                                            }

                                            flg_2_To_7_kVA = flg_2_To_7_kVA + 1;
                                        }
                                        // EA Series
                                        else if ((Convert.ToDouble(Str_KVA.Trim()) >= 7 && Convert.ToDouble(Str_KVA.Trim()) <= 20))
                                        {
                                            if (flg_7_To_20_kVA == 0)
                                            {
                                                str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+7.5-20_kVA.PDF";
                                                str_pdf_Name = "CPCB_IV+7.5-20_kVA.PDF";
                                                if (File.Exists(str_FilePath))
                                                {
                                                    FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                                    Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                                    m.Attachments.Add(a2);
                                                }
                                            }

                                            flg_7_To_20_kVA = flg_7_To_20_kVA + 1;
                                        }

                                        else if ((Convert.ToDouble(Str_KVA.Trim()) >= 25 && Convert.ToDouble(Str_KVA.Trim()) <= 58.5))
                                        {
                                            if (flg_25_To_58_5_kVA == 0)
                                            {
                                                str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+25-58.5_kVA.PDF";
                                                str_pdf_Name = "CPCB_IV+25-58.5_kVA.PDF";
                                                if (File.Exists(str_FilePath))
                                                {
                                                    FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                                    Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                                    m.Attachments.Add(a2);
                                                }
                                            }
                                            flg_25_To_58_5_kVA = flg_25_To_58_5_kVA + 1;
                                        }
                                        else if ((Convert.ToDouble(Str_KVA.Trim()) >= 80 && Convert.ToDouble(Str_KVA.Trim()) <= 160))
                                        {
                                            if (flg_80_To_160_kVA == 0)
                                            {
                                                str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+82.5-160_kVA.PDF";
                                                str_pdf_Name = "CPCB_IV+82.5-160_kVA.PDF";
                                                if (File.Exists(str_FilePath))
                                                {
                                                    FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                                    Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                                    m.Attachments.Add(a2);
                                                }
                                            }
                                            flg_80_To_160_kVA = flg_80_To_160_kVA + 1;
                                        }
                                        else if ((Convert.ToDouble(Str_KVA.Trim()) >= 200 && Convert.ToDouble(Str_KVA.Trim()) <= 250))
                                        {
                                            if (flg_200_To_250_kVA == 0)
                                            {
                                                str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+200-250_kVA.PDF";
                                                str_pdf_Name = "CPCB_IV+200-250_kVA.PDF";
                                                if (File.Exists(str_FilePath))
                                                {
                                                    FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                                    Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                                    m.Attachments.Add(a2);
                                                }
                                            }
                                            flg_200_To_250_kVA = flg_200_To_250_kVA + 1;
                                        }
                                        else if ((Convert.ToDouble(Str_KVA.Trim()) >= 320 && Convert.ToDouble(Str_KVA.Trim()) <= 750))
                                        {
                                            if (flg_320_TO_750_kVA == 0)
                                            {
                                                //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
                                                str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+320-750_kVA.PDF";
                                                str_pdf_Name = "CPCB_IV+320-750_kVA.PDF";
                                                if (File.Exists(str_FilePath))
                                                {
                                                    FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                                    Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                                    m.Attachments.Add(a2);
                                                }
                                            }
                                            flg_320_TO_750_kVA = flg_320_TO_750_kVA + 1;
                                        }
                                        else if ((Convert.ToDouble(Str_KVA.Trim()) >= 1010 && Convert.ToDouble(Str_KVA.Trim()) <= 1500))
                                        {
                                            if (flg_1010_TO_1500_kVA == 0)
                                            {
                                                //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
                                                str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+1010-1500_kVA.PDF";
                                                str_pdf_Name = "CPCB_IV+1010-1500_kVA.PDF";
                                                if (File.Exists(str_FilePath))
                                                {
                                                    FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                                    Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                                    m.Attachments.Add(a2);
                                                }
                                            }
                                            flg_1010_TO_1500_kVA = flg_1010_TO_1500_kVA + 1;
                                        }
                                    }
                                }
                            }


                            // New Anubandh
                            if (Qtn_CPCB != "4")
                            {
                                if (Convert.ToDouble(Str_KVA.Trim()) == 125 && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() == "6R")
                                {
                                    //Skip Anubandh by KB on 21/12/2023

                                    //Skip Anubandh
                                }
                                else if ((Convert.ToDouble(Str_KVA.Trim()) >= 5 && Convert.ToDouble(Str_KVA.Trim()) <= 160) && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
                                {
                                    //Skip Anubandh by KB on 21/12/2023
                                    //strFilePath_3 = AppDomain.CurrentDomain.BaseDirectory + "Pages/Marketing/Reports/quotation/Anubandh.pdf";
                                    //strpdf_3 = "Anubandh.pdf";
                                    //flg_anubandh = 1;

                                    // if (File.Exists(strFilePath_3))
                                    // {
                                    //Skip Anubandh by KB on 21/12/2023

                                    //FileStream fs3 = new FileStream(strFilePath_3, FileMode.Open, FileAccess.Read);
                                    //Attachment a3 = new Attachment(fs3, strpdf_3, MediaTypeNames.Application.Octet);
                                    //m.Attachments.Add(a3);
                                    //}
                                }
                            }
                            //End 
                        }
                    }


                    #region

                    // string[] split_kva_item = Regex.Split(kva.Trim(), ",");
                    //if (split_kva_item.Length > 0)
                    //{
                    //    for (int j = 0; j < split_kva_item.Length; j++)
                    //    {
                    //        string strFilePath_2 = "", strFilePath_3 = "";
                    //        string strpdf_2 = "", strpdf_3 = "";
                    //        string model_type = cls.getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + Original_Qtn_No.Trim() + "') and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "Model");
                    //        string Qtn_CPCB = cls.getName("select substring(q.PartCode,15,1) as CPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "CPCB");
                    //        //int Opti = Convert.ToInt32 (clsCommonFunctions.getName("select isnull(sum(k.Qty),0) as EngCount from quotationdetails q inner join part p on q.partcode=p.PartCode inner join kitdetails k on k.kitcode=p.PartCode where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "' and k.PartCode like '001%' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "EngCount"));
                    //        string Opti_CPCB = cls.getName("select top 1 substring(q.PartCode,10,1) as OptiCPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "OptiCPCB");

                    //        //if (Qtn_CPCB == "4")
                    //        //{
                    //        //if (model_type.Trim().PadRight(3).Trim() == "NG1")
                    //        if (model_type.Substring(model_type.Trim().Length - 3) == "NG1")
                    //        {
                    //            strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/NG_CPCB IV+15-250_kVA.PDF";
                    //            strpdf_2 = "NG_CPCB IV+15-250_kVA.PDF";
                    //            flg_NG_CPCB_IV_15_to_250_kVA = 1;
                    //        }
                    //        else
                    //        {
                    //            if (Opti_CPCB == "5")
                    //            {
                    //                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/OP_CPCB IV+117-2000_kVA.PDF";
                    //                strpdf_2 = "OP_CPCB IV+117-2000_kVA.PDF";
                    //                flg_OP_CPCB_IV_117_2000_kVA = 1;
                    //            }
                    //            else
                    //            {

                    //                //if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
                    //                //{
                    //                //    //strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/Kirloskar_2kW_5kVA.pdf";
                    //                //    //strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
                    //                //    //flg_2kW_5kVA = 1;
                    //                //}


                    //                //2. 8kW-5.5kVA
                    //                if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 7))
                    //                {
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+2.8-5.5_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+2.8-5.5_kVA.PDF";
                    //                    flg_2_8_To_5_5_kVA = 1;
                    //                }
                    //                // EA Series
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 7 && Convert.ToDouble(split_kva_item[j].Trim()) <= 20))
                    //                {
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+7.5-20_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+7.5-20_kVA.PDF";
                    //                    flg_7_5_To_20_kVA = 1;
                    //                }
                    //                //3R550
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 25 && Convert.ToDouble(split_kva_item[j].Trim()) <= 58.5))
                    //                {
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+25-58.5_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+25-58.5_kVA.PDF";
                    //                    flg_R550 = 1;
                    //                }
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 80 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160))
                    //                {
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+82.5-160_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+82.5-160_kVA.PDF";
                    //                    flg_15_To_30_kVA = 1;
                    //                }
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250))
                    //                {
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+200-250_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+200-250_kVA.PDF";
                    //                    flg_40_To_160_kVA = 1;
                    //                }
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 750))
                    //                {
                    //                    //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+320-750_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+320-750_kVA.PDF";
                    //                    flg_320_1010_kVA = 1;
                    //                }
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 1010 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1500))
                    //                {
                    //                    //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+1010-1500_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+1010-1500_kVA.PDF";
                    //                    flg_320_1010_kVA = 1;
                    //                }
                    //            }
                    //        }

                    //        //}

                    //        // Commented by KB on 23/07/2024 as CPCB 2 is Excluded
                    //        //else
                    //        //{
                    //        //    if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_2kW_5kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
                    //        //        flg_2kW_5kVA = 1;
                    //        //    }
                    //        //    // EA Series
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 12.5) && flg_5_To_12_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/iGreen-5-160kVA.pdf";
                    //        //        strpdf_2 = "Catlogue_iGreen_5_To_160_kVA.pdf";
                    //        //        flg_5_To_12_kVA = 1;
                    //        //    }
                    //        //    //3R550
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 15) && flg_R550 == 0 && model_type.Trim().Substring(0, 5).Trim() == "3R550")
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_R550_15_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_R550_15_kVA_Catlogue.pdf";
                    //        //        flg_R550 = 1;
                    //        //    }
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 30) && flg_15_To_30_kVA == 0)
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_15_To_30_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_15_To_30_kVA_Catlogue.pdf";
                    //        //        flg_15_To_30_kVA = 1;
                    //        //    }
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 40 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_40_To_160_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "6R")
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_40_To_160_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_40_To_160_kVA_Catalogue.pdf";
                    //        //        flg_40_To_160_kVA = 1;
                    //        //    }
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250) && flg_200_To_250_kVA == 0)
                    //        //    {
                    //        //        // Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_200_To_250_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_200_To_250_kVA_Catlogue.pdf";
                    //        //        flg_200_To_250_kVA = 1;
                    //        //    }
                    //        //    //else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1010) && flg_320_1010_kVA == 0)
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) < 1000) && flg_320_1010_kVA == 0)
                    //        //    {
                    //        //        // Commented by KB on 08/05/2023 as per mail from Kalpesh Sir on 06/05/2023
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_320_1010_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_320_1010_kVA_Catlogue.pdf";
                    //        //        flg_320_1010_kVA = 1;
                    //        //    }
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 1250 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1500) && flg_1250_kVA_1500 == 0)
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_1250_To_1500_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_1250_To_1500_kVA_Catlogue.pdf";
                    //        //        flg_1250_kVA_1500 = 1;
                    //        //    }
                    //        //}
                    //        // Commented by KB on 23/07/2024

                    //        if (File.Exists(strFilePath_2))
                    //        {
                    //            FileStream fs2 = new FileStream(strFilePath_2, FileMode.Open, FileAccess.Read);
                    //            Attachment a2 = new Attachment(fs2, strpdf_2, MediaTypeNames.Application.Octet);
                    //            m.Attachments.Add(a2);
                    //        }

                    //        // New Anubandh
                    //        if (Qtn_CPCB != "4")
                    //        {
                    //            if (Convert.ToDouble(split_kva_item[j].Trim()) == 125 && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() == "6R")
                    //            {
                    //                //Skip Anubandh by KB on 21/12/2023

                    //                //Skip Anubandh
                    //            }
                    //            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
                    //            {
                    //                //Skip Anubandh by KB on 21/12/2023
                    //                //strFilePath_3 = AppDomain.CurrentDomain.BaseDirectory + "quotation/quotation/Anubandh.pdf";
                    //                //strpdf_3 = "Anubandh.pdf";
                    //                //flg_anubandh = 1;

                    //                if (File.Exists(strFilePath_3))
                    //                {
                    //                    //Skip Anubandh by KB on 21/12/2023

                    //                    //FileStream fs3 = new FileStream(strFilePath_3, FileMode.Open, FileAccess.Read);
                    //                    //Attachment a3 = new Attachment(fs3, strpdf_3, MediaTypeNames.Application.Octet);
                    //                    //m.Attachments.Add(a3);
                    //                }
                    //            }
                    //        }
                    //        //End 
                    //    }
                    //}
                    #endregion
                    #endregion


                    m.From = new MailAddress("erp@kalabiz.com", "Kala Genset Pvt. Ltd. - ERP System");

                    #region
                    //ToMail
                    if (to_mail_id.Length > 0 && !string.IsNullOrEmpty(to_mail_id))
                    {
                        string[] ToMailIDitems = Regex.Split(to_mail_id.Trim(), ",");
                        foreach (string strTo in ToMailIDitems)
                        {
                            m.To.Add(new MailAddress(strTo));
                        }
                    }
                    //ToMail END


                    //Add sales.support7@kalabiz.com and Indore and bhopal HOD EmailID IN CC
                    if (qtn_AssToEmpPCCode.Trim() == "07.005")//  Indore 
                    {
                        if (string.IsNullOrEmpty(cc_mail_id))
                        {
                            cc_mail_id = "vijay.nikam@kalabiz.com" + "," + "sales.indore@kalabiz.com";
                        }
                        else
                        {
                            cc_mail_id = cc_mail_id.ToString().Trim() + "," + "vijay.nikam@kalabiz.com" + "," + "sales.indore@kalabiz.com" + "," + "vaishakh.pathak@kalabiz.com";
                        }
                    }
                    if (qtn_AssToEmpPCCode.Trim() == "07.014")// Bhopal
                    {

                        if (string.IsNullOrEmpty(cc_mail_id))
                        {
                            cc_mail_id = "vijay.nikam@kalabiz.com" + "," + "sales.bhopal@kalabiz.com";
                        }
                        else
                        {
                            cc_mail_id = cc_mail_id.ToString().Trim() + "," + "vijay.nikam@kalabiz.com" + "," + "sales.bhopal@kalabiz.com";
                        }



                    }


                    //CCMail 
                    if (cc_mail_id.Length > 0 & !string.IsNullOrEmpty(cc_mail_id))
                    {
                        string[] CCMailIDitems = Regex.Split(cc_mail_id.Trim(), ",");
                        foreach (string strCC in CCMailIDitems)
                        {
                            m.CC.Add(new MailAddress(strCC));
                        }
                    }




                    //CCMail  END

                    //BCCMail                 
                    string[] BCCMailIDitems = Regex.Split("skk@kalabiz.com", ",");
                    foreach (string strBCC in BCCMailIDitems)
                    {
                        m.Bcc.Add(new MailAddress(strBCC));
                    }

                    //BCCMail  END
                    #endregion

                    #region
                    Body = "";
                    Body += "<p>To,";
                    Body += "<BR><span style='color:#1F497D'>" + cust_name.Trim() + " </span>";
                    Body += "<BR><span style='color:#1F497D'>" + cust_add.Trim() + " </span></p>";

                    Body += "<p>Subject: Proposal for " + kva.ToString().Trim() + " kVA Silent DG Set</p>";

                    Body += "<p>KALA a popular name synonymous with power, stands tall as a market leader ";
                    Body += "serving industrial, commercial and residential segments in the domestic as well ";
                    Body += "as international markets.</p>";
                    Body += "<p>KALA an IMS(Integrated Management System) Certified company has grown to be one of the biggest names in power ";
                    Body += "generation solutions. Today, our team continues to focus on constant innovation ";
                    Body += "and commitment towards Customer Satisfaction. We concentrate on our ";
                    Body += "efforts in bringing the highest quality power solutions to your corporate/business/residence.</p>";

                    Body += "<p>We as an authorized Genset Original Equipment Manufacturers (G.O.E.M's) for ";

                    Body += "Kirloskar Oil Engines Limited-Pune, manufacture quality KOEL Green ";
                    Body += "Generating Sets in the range of 2.1 kVA to 1500 kVA for captive and standby ";
                    Body += "applications with options such as liquid cooled and air cooled DG sets.</p>";

                    Body += "<p>We provide complete turnkey solutions which include site selection, load study, ";
                    Body += "application engineering, installation, commissioning and obtaining statutory ";
                    Body += "approvals from the Government agencies.</p>";

                    Body += "<p>With our headquarters at Pune, We are having 3 state of the art, ultra-modern ";
                    Body += "manufacturing and testing facilities at Chakan, Silvassa, Belgaum. We are able ";
                    Body += "to cater to both logistic requirements as well as commercial benefits in ";
                    Body += "taxations. We manufacture world class accoustic enclosures (Sound Proof ";
                    Body += "Canopies) as per Central Pollution Control Board (CPCB) Norms.</p>";

                    Body += "<p style='text-align:justify'>";
                    Body += "Our dedicated branch offices at Pune, Chinchwad, Mumbai(Kharghar), Kolhapur, ";
                    Body += "Solapur, Sangli, Aurangabad, Latur, Beed, Nanded, Indore, Bhopal, Gwalior, Bangalore, Belgaon, Hyderabad along with the business associates &amp; dealers are available to ";
                    Body += "provide sales and service to our widespread customer network. Our ";
                    Body += "marketing team is always available at your service to meet your requirements ";
                    Body += "and satisfy all your needs 24/7. We understand the popular saying 'Customers ";
                    Body += "have a choice' and we are grateful to you for considering us as your ";
                    Body += "preferred choice.</p>";


                    Body += "<p>For Genset & Commercial Details, Please find attachment.</p>";

                    Body += "<p style='text-align: justify'><span><span style='color:#1F497D'>Thank You/Regards.</span></span></p>";

                    Body += "<p style='text-align: justify'><span><span style='color:#1F497D'>" + ass_to_emp_name.Trim() + " <br> MailID - " + ass_to_mail_id.Trim() + " <br> MobileNo - " + ass_to_mob_no.Trim() + " <br> Toll Free No - 1800 123 0018</span></span></p>";

                    Body += "<BR><img alt=\"\" hspace=0 src=\"cid:imageId\" align=baseline border=0>";

                    //if (Original_Qtn_No.Trim().Substring(10, 2).Trim() == "16")
                    //{
                    //    Body += "<p style='text-align: justify'><span><span style='color:#1F497D'><b>Kala International LLC</b></span></span></p>";
                    //}
                    //else
                    //{
                    Body += "<p style='text-align: justify'><span><span style='color:#1F497D'><b>Kala Genset Pvt ltd.</b></span></span></p>";
                    // }

                    #endregion

                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, null, "text/html");

                    //  var filePath = Path.Combine("~//images//kalalogo.JPG", "images", "kalalogo.JPG");

                    LinkedResource imagelink = new LinkedResource(AppDomain.CurrentDomain.BaseDirectory + "images/kalalogo.JPG", "image/jpeg");

                    //  LinkedResource imagelink = new LinkedResource(Server.MapPath("~//images//kalalogo.JPG"), "image/png");
                    imagelink.ContentId = "imageId";
                    imagelink.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                    htmlView.LinkedResources.Add(imagelink);
                    m.AlternateViews.Add(htmlView);

                    m.Subject = "Proposal for " + kva.ToString().Trim() + " kVA Silent DG Set";

                    m.IsBodyHtml = true;
                    m.Body = Body;
                    m.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                    if (!string.IsNullOrEmpty(ass_to_mail_id.ToString().Trim()))
                    {
                        m.ReplyTo = new MailAddress(ass_to_mail_id.ToString().Trim());
                    }

                    if (sc != null)
                    {
                        //sc.Credentials = new System.Net.NetworkCredential("erp@kalabiz.com", "wanv ftwc dobq blrr");
                        sc.Credentials = new System.Net.NetworkCredential("erp@kalabiz.com", "wanv ftwc dobq blrr");
                        sc.Port = 587;
                        sc.Host = "smtp.gmail.com";
                        sc.EnableSsl = true;
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                        sc.Timeout = 999999999;
                        sc.ServicePoint.MaxIdleTime = 1;
                        sc.Send(m);
                    }

                    m.Dispose();
                    sc = null;

                    fs1.Close();
                    fs1.Dispose();
                    //Code to Delete The Temp File
                    //if (File.Exists(strFilePath_1))
                    //{
                    //    File.Delete(strFilePath_1);
                    //}
                }
                //  return "Successfully";
                emailStatus = "Email Send Successfully";
            }
            catch (Exception ex)
            {
                //if (File.Exists(strFilePath_1))
                //{
                //    File.Delete(strFilePath_1);
                //}
                //  return "Failed" + ex.StackTrace;
                emailStatus = "Mail Sending Error,Please Try Again";
                //return "Failed";
            }
        }

        // Added by KB on 23/01/2026 
        protected void LoadReport(string QtnNo, string ReportName, string loadRptType, string AssignToEmpID, string AssignToEmpName, string AssignToMobileNo, string AssignToMailID, string Model)
        {
            var rpt = new ReportDocument();
            if (loadRptType.ToString().Trim() == "Q")
            {
                // iGreen

                int Qtn_CPCB = Convert.ToInt32(dc.getName("select count(qtnno) as CPCB from quotationdetails where qtnno = '" + QtnNo + "' and substring(PartCode,15,1) ='4'", "QuotationDetails", "CPCB"));
                string model_Gas = dc.getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + QtnNo + "')", "QuotationDetails", "Model");


                //if (Qtn_CPCB == 0)
                //{
                //    if (Model.Trim() == "iGreen")
                //    {
                //        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAiGreen.rpt");
                //    }
                //    else
                //    {
                //        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALA.rpt");
                //    }
                //}
                //else if (Qtn_CPCB > 0)
                //{

                if (model_Gas.Substring(model_Gas.Trim().Length - 3) == "NG1")
                {
                    rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAGasDGCPCBIV.rpt");

                }
                else
                {
                    if (Model.Trim() == "iGreen")
                    {
                        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAiGreenCPCBIV.rpt");
                    }
                    // Added by KB on 22/01/2026 as for 7.5 KVA CCModel we require Warranty as 12 Months
                    else if (Model.Trim() == "Warranty") 
                    {
                        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAiGreenCPCBIV1yrWarranty.rpt");
                    }

                    else
                    {

                        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAiGreenCPCBIV.rpt");

                        //rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "Pages/Marketing/Reports/quotation/KALACPCBIV.rpt");
                    }
                }


                //}
                rpt.SetDatabaseLogon("sa", "3HMungiIMR", "Localhost", "ERP");
                rpt.SetParameterValue(0, QtnNo.ToString().Trim());
                rpt.SetParameterValue(1, AssignToEmpName.Trim());
                rpt.SetParameterValue(2, AssignToMobileNo.Trim());
                rpt.SetParameterValue(3, AssignToMailID.Trim());
                rpt.SetParameterValue(4, "'" + QtnNo.ToString().Trim() + "'");
                rpt.ExportToDisk(ExportFormatType.PortableDocFormat, ReportName.ToString().Trim());
                rpt.Close();
            }
            else if (loadRptType.ToString().Trim() == "I")
            {
                rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAInstallation.rpt");
                rpt.SetDatabaseLogon("sa", "3HMungiIMR", "Localhost", "ERP");
                rpt.SetParameterValue(0, QtnNo.ToString().Trim());
                rpt.SetParameterValue(1, AssignToEmpName.Trim());
                rpt.SetParameterValue(2, AssignToMobileNo.Trim());
                rpt.SetParameterValue(3, AssignToMailID.Trim());
                rpt.ExportToDisk(ExportFormatType.PortableDocFormat, ReportName.ToString().Trim());
                rpt.Close();
            }
        }





        //Commented by KB on 23/01/2026
        //public string SendPreviousQuot(string QtnNo, string To_EMailID, string AssToEmpCode, string CCMailID, string CustName, string Address)
        //{
        //    emailStatus = "Failed";

        //    //if (string.IsNullOrEmpty(CCMailID))
        //    //{
        //    //    ccMailID = "";
        //    //}

        //    try
        //    {
        //        string str_ccMailID = CCMailID.Trim();
        //        //string kvaitems = dc.getName("select STUFF((SELECT (',' + cast(kVA as varchar)) AS kVA from Quotation q inner join QuotationDetails qd on q.QtnNo = qd.QtnNo " +
        //        //" inner join part p on p.PartCode = qd.PartCode where q.Active = '1' and q.QtnNo = '" + QtnNo.Trim() + "' GROUP BY kVA ORDER BY kVA " +
        //        //" FOR XML PATH(''), TYPE ).value('.', 'VARCHAR(MAX)') ,1,1,'') as kVA", "tbl_Quotation", "kVA");

        //        DataSet ds = dc.procDS("select FName+' '+LName as ename,cmobileno,compmailID,ProfitCenter from employee where ecode='" + AssToEmpCode.Trim() + "'", "tbl_employee");


        //        if (string.IsNullOrEmpty(str_ccMailID))
        //        {
        //            if (!string.IsNullOrEmpty(ds.Tables["tbl_employee"].Rows[0]["compmailID"].ToString().Trim()))
        //            {
        //                str_ccMailID =  ds.Tables["tbl_employee"].Rows[0]["compmailID"].ToString().Trim();
        //            }
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(ds.Tables["tbl_employee"].Rows[0]["compmailID"].ToString().Trim()))
        //            {
        //                str_ccMailID += "," + ds.Tables["tbl_employee"].Rows[0]["compmailID"].ToString().Trim();
        //            }
        //        }

        //        string StrQKAV = "";
        //        string iGreen = "N", KG = "N";
        //        DataSet ds_Qtn_KVA = dc.procDS("select cast(kVA as varchar) AS kVA , Model from Quotation q inner join QuotationDetails qd on q.QtnNo = qd.QtnNo " +
        //            " inner join part p on p.PartCode = qd.PartCode where q.Active = '1' and q.QtnNo = '" + QtnNo.Trim() + "'  " +
        //            " GROUP BY  kVA,Model  ORDER BY kVA ", "tbl_Quotation");

        //        for (int i = 0; (i < ds_Qtn_KVA.Tables["tbl_Quotation"].Rows.Count); i++)
        //        {
        //                 //SATISH

        //                if (string.IsNullOrEmpty(StrQKAV))
        //                {
        //                    StrQKAV = ds_Qtn_KVA.Tables["tbl_Quotation"].Rows[i]["kVA"].ToString().Trim();
        //                }
        //                else
        //                {
        //                    StrQKAV += "," + ds_Qtn_KVA.Tables["tbl_Quotation"].Rows[i]["kVA"].ToString().Trim();
        //                }


        //            // 125 6R(kg)
        //            if (Convert.ToDouble(ds_Qtn_KVA.Tables["tbl_Quotation"].Rows[i]["kVA"].ToString().Trim()) == 125 && ds_Qtn_KVA.Tables["tbl_Quotation"].Rows[i]["Model"].ToString().Trim().Substring(0, 2).Trim() == "6R")
        //            {
        //                //KG = "Y";
        //            } // iGreen KVA
        //            else if ((Convert.ToDouble(ds_Qtn_KVA.Tables["tbl_Quotation"].Rows[i]["kVA"].ToString().Trim()) >= 5 && Convert.ToDouble(ds_Qtn_KVA.Tables["tbl_Quotation"].Rows[i]["kVA"].ToString().Trim()) <= 160)
        //                && ds_Qtn_KVA.Tables["tbl_Quotation"].Rows[i]["Model"].ToString().Trim().Substring(0, 2).Trim() != "CC")
        //            {
        //                iGreen = "Y";
        //            } // kg
        //            else
        //            {
        //                //KG = "Y";
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(StrQKAV))
        //        {

        //            string strQuotkVA = "";
        //            string[] ch = Regex.Split(StrQKAV, ",");
        //            string[] chNew = new string[ch.Length];
        //            for (int i = 0; i < ch.Length; i++)
        //            {
        //                chNew[i] = ch[i].ToString();
        //            }
        //            string[] distVal = chNew.Distinct().ToArray();
        //            foreach (string c in distVal)
        //            {
        //                if (strQuotkVA.Trim() == "")
        //                {
        //                    strQuotkVA = c.ToString();
        //                }
        //                else if (strQuotkVA.Trim() != "")
        //                {
        //                    strQuotkVA = strQuotkVA + "," + c.ToString();
        //                }
        //            }


        //            //Send Quotation Mail
        //            if (iGreen == "Y" && KG == "Y")
        //            {
        //                // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditorResponse", "alert('You Cannot Quot iGreen & Non iGreen Product within same Quotation...!')", true);
        //            }
        //            else
        //            {
        //                if (iGreen.Trim() == "Y")
        //                {
        //                    sendQuotEmail(ds.Tables["tbl_employee"].Rows[0]["ProfitCenter"].ToString().Trim(),QtnNo.Trim(), strQuotkVA.Trim(), To_EMailID.Trim(), str_ccMailID.Trim(), AssToEmpCode.Trim(), ds.Tables["tbl_employee"].Rows[0]["ename"].ToString().Trim(), ds.Tables["tbl_employee"].Rows[0]["cmobileno"].ToString().Trim(), ds.Tables["tbl_employee"].Rows[0]["compmailID"].ToString().Trim(), CustName.Trim(), Address.Trim(), "iGreen");
        //                }
        //                else
        //                {
        //                    sendQuotEmail(ds.Tables["tbl_employee"].Rows[0]["ProfitCenter"].ToString().Trim(),QtnNo.Trim(), strQuotkVA.Trim(), To_EMailID.Trim(), str_ccMailID.Trim(), AssToEmpCode.Trim(), ds.Tables["tbl_employee"].Rows[0]["ename"].ToString().Trim(), ds.Tables["tbl_employee"].Rows[0]["cmobileno"].ToString().Trim(), ds.Tables["tbl_employee"].Rows[0]["compmailID"].ToString().Trim(), CustName.Trim(), Address.Trim(), "NoniGreen");
        //                }
        //            }


        //            //sendQuotEmail(QtnNo.Trim(), strQuotkVA.Trim(), To_EMailID.Trim(), ccMailID.Trim(), AssToEmpCode.Trim(), ds.Tables["tbl_employee"].Rows[0]["ename"].ToString().Trim(), ds.Tables["tbl_employee"].Rows[0]["cmobileno"].ToString().Trim(), ds.Tables["tbl_employee"].Rows[0]["compmailid"].ToString().Trim(), CustName.Trim(), Address.Trim(), "NoniGreen");

        //            //string url = "https://www.kalapms.com/QuotApi/Service.asmx/sendQuotEmailAngular?qtn_no=" + QtnNo.Trim() + "&kva=" + kvaitems.Trim() + "&to_mail_id=" + To_EMailID.Trim()
        //            //+ "&cc_mail_id=" + ccMailID.Trim() + "&ass_to_emp_id=" + AssToEmpCode.Trim() + "&ass_to_emp_name=" + ds.Tables["tbl_employee"].Rows[0]["ename"].ToString().Trim()
        //            //+ "&ass_to_mob_no=" + ds.Tables["tbl_employee"].Rows[0]["cmobileno"].ToString().Trim() + "&ass_to_mail_id=" + ds.Tables["tbl_employee"].Rows[0]["compmailid"].ToString().Trim()
        //            //+ "&cust_name=" + CustName.Trim() + "&cust_add=" + Address.Trim();

        //            //HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);
        //            //Request.Method = "GET";
        //            //Request.KeepAlive = true;
        //            //HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
        //            //if (Response.StatusCode == HttpStatusCode.OK)
        //            //{
        //            //    emailStatus = "Mail Send Successfully ";
        //            //}
        //            //Response.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        emailStatus = "Mail Send Failed StackTrace " + ex.StackTrace + ", Message " + ex.Message;
        //        //emailStatus = "Mail Send Failed StackTrace " + ex.StackTrace + ", Message " + ex.Message;
        //    }
        //    return emailStatus;
        //}

        ////    string strFilePath_1 = ConfigurationManager.AppSettings["Quotationfile"] + "/" + "QTN_24-25_07013431_10-03-2025_16_21_53.pdf";
        ////Commented by KB on 23/01/2026
        //protected void sendQuotEmail(string qtn_AssToEmpPCCode, string qtn_no, string kva, string to_mail_id, string cc_mail_id, string ass_to_emp_id, string ass_to_emp_name, string ass_to_mob_no,
        //    string ass_to_mail_id, string cust_name, string cust_add, string model)
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
        //            //int flg_anubandh = 0;
        //            //int flg_2kW_5kVA = 0;
        //            //int flg_R550 = 0;
        //            //int flg_5_To_12_kVA = 0;
        //            //int flg_15_To_30_kVA = 0;
        //            //int flg_40_To_160_kVA = 0;
        //            //int flg_200_To_250_kVA = 0;
        //            //int flg_320_1010_kVA = 0;
        //            //int flg_1250_kVA_1500 = 0;

        //            //int flg_2_8_To_5_5_kVA = 0;
        //            //int flg_7_5_To_20_kVA = 0;
        //            //int flg_NG_CPCB_IV_15_to_250_kVA = 0;
        //            //int flg_OP_CPCB_IV_117_2000_kVA;

        //            //string[] split_kva_item = Regex.Split(kva.Trim(), ",");
        //            //if (split_kva_item.Length > 0)
        //            //{
        //            //    for (int j = 0; j < split_kva_item.Length; j++)
        //            //    {
        //            //        string strFilePath_2 = "", strFilePath_3 = "";
        //            //        string strpdf_2 = "", strpdf_3 = "";
        //            //        string model_type = dc.getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + Original_Qtn_No.Trim() + "') and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "Model");
        //            //        string Qtn_CPCB = dc.getName("select substring(q.PartCode,15,1) as CPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "CPCB");
        //            //        //int Opti = Convert.ToInt32 (clsCommonFunctions.getName("select isnull(sum(k.Qty),0) as EngCount from quotationdetails q inner join part p on q.partcode=p.PartCode inner join kitdetails k on k.kitcode=p.PartCode where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "' and k.PartCode like '001%' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "EngCount"));
        //            //        string Opti_CPCB = dc.getName("select top 1 substring(q.PartCode,10,1) as OptiCPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "OptiCPCB");

        //            //        //if (Qtn_CPCB == "4")
        //            //        //{
        //            //        //if (model_type.Trim().PadRight(3).Trim() == "NG1")
        //            //        if (model_type.Substring(model_type.Trim().Length - 3) == "NG1")
        //            //        {
        //            //            strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/NG_CPCB IV+15-250_kVA.PDF";
        //            //            strpdf_2 = "NG_CPCB IV+15-250_kVA.PDF";
        //            //            flg_NG_CPCB_IV_15_to_250_kVA = 1;
        //            //        }
        //            //        else
        //            //        {
        //            //            if (Opti_CPCB == "5")
        //            //            {
        //            //                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/OP_CPCB IV+117-2000_kVA.PDF";
        //            //                strpdf_2 = "OP_CPCB IV+117-2000_kVA.PDF";
        //            //                flg_OP_CPCB_IV_117_2000_kVA = 1;
        //            //            }
        //            //            else
        //            //            {

        //            //                //if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
        //            //                //{
        //            //                //    //strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/Kirloskar_2kW_5kVA.pdf";
        //            //                //    //strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
        //            //                //    //flg_2kW_5kVA = 1;
        //            //                //}


        //            //                //2. 8kW-5.5kVA
        //            //                if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 7))
        //            //                {
        //            //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+2.8-5.5_kVA.PDF";
        //            //                    strpdf_2 = "CPCB_IV+2.8-5.5_kVA.PDF";
        //            //                    flg_2_8_To_5_5_kVA = 1;
        //            //                }
        //            //                // EA Series
        //            //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 7 && Convert.ToDouble(split_kva_item[j].Trim()) <= 20))
        //            //                {
        //            //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+7.5-20_kVA.PDF";
        //            //                    strpdf_2 = "CPCB_IV+7.5-20_kVA.PDF";
        //            //                    flg_7_5_To_20_kVA = 1;
        //            //                }
        //            //                //3R550
        //            //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 25 && Convert.ToDouble(split_kva_item[j].Trim()) <= 58.5))
        //            //                {
        //            //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+25-58.5_kVA.PDF";
        //            //                    strpdf_2 = "CPCB_IV+25-58.5_kVA.PDF";
        //            //                    flg_R550 = 1;
        //            //                }
        //            //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 80 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160))
        //            //                {
        //            //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+82.5-160_kVA.PDF";
        //            //                    strpdf_2 = "CPCB_IV+82.5-160_kVA.PDF";
        //            //                    flg_15_To_30_kVA = 1;
        //            //                }
        //            //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250))
        //            //                {
        //            //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+200-250_kVA.PDF";
        //            //                    strpdf_2 = "CPCB_IV+200-250_kVA.PDF";
        //            //                    flg_40_To_160_kVA = 1;
        //            //                }
        //            //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 750))
        //            //                {
        //            //                    //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
        //            //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+320-750_kVA.PDF";
        //            //                    strpdf_2 = "CPCB_IV+320-750_kVA.PDF";
        //            //                    flg_320_1010_kVA = 1;
        //            //                }
        //            //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 1010 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1500))
        //            //                {
        //            //                    //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
        //            //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+1010-1500_kVA.PDF";
        //            //                    strpdf_2 = "CPCB_IV+1010-1500_kVA.PDF";
        //            //                    flg_320_1010_kVA = 1;
        //            //                }
        //            //            }
        //            //        }

        //            //        //}

        //            //        // Commented by KB on 23/07/2024 as CPCB 2 is Excluded
        //            //        //else
        //            //        //{
        //            //        //    if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
        //            //        //    {
        //            //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_2kW_5kVA.pdf";
        //            //        //        strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
        //            //        //        flg_2kW_5kVA = 1;
        //            //        //    }
        //            //        //    // EA Series
        //            //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 12.5) && flg_5_To_12_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
        //            //        //    {
        //            //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/iGreen-5-160kVA.pdf";
        //            //        //        strpdf_2 = "Catlogue_iGreen_5_To_160_kVA.pdf";
        //            //        //        flg_5_To_12_kVA = 1;
        //            //        //    }
        //            //        //    //3R550
        //            //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 15) && flg_R550 == 0 && model_type.Trim().Substring(0, 5).Trim() == "3R550")
        //            //        //    {
        //            //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_R550_15_kVA.pdf";
        //            //        //        strpdf_2 = "Kirloskar_R550_15_kVA_Catlogue.pdf";
        //            //        //        flg_R550 = 1;
        //            //        //    }
        //            //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 30) && flg_15_To_30_kVA == 0)
        //            //        //    {
        //            //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_15_To_30_kVA.pdf";
        //            //        //        strpdf_2 = "Kirloskar_15_To_30_kVA_Catlogue.pdf";
        //            //        //        flg_15_To_30_kVA = 1;
        //            //        //    }
        //            //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 40 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_40_To_160_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "6R")
        //            //        //    {
        //            //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_40_To_160_kVA.pdf";
        //            //        //        strpdf_2 = "Kirloskar_40_To_160_kVA_Catalogue.pdf";
        //            //        //        flg_40_To_160_kVA = 1;
        //            //        //    }
        //            //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250) && flg_200_To_250_kVA == 0)
        //            //        //    {
        //            //        //        // Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
        //            //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_200_To_250_kVA.pdf";
        //            //        //        strpdf_2 = "Kirloskar_200_To_250_kVA_Catlogue.pdf";
        //            //        //        flg_200_To_250_kVA = 1;
        //            //        //    }
        //            //        //    //else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1010) && flg_320_1010_kVA == 0)
        //            //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) < 1000) && flg_320_1010_kVA == 0)
        //            //        //    {
        //            //        //        // Commented by KB on 08/05/2023 as per mail from Kalpesh Sir on 06/05/2023
        //            //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_320_1010_kVA.pdf";
        //            //        //        strpdf_2 = "Kirloskar_320_1010_kVA_Catlogue.pdf";
        //            //        //        flg_320_1010_kVA = 1;
        //            //        //    }
        //            //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 1250 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1500) && flg_1250_kVA_1500 == 0)
        //            //        //    {
        //            //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_1250_To_1500_kVA.pdf";
        //            //        //        strpdf_2 = "Kirloskar_1250_To_1500_kVA_Catlogue.pdf";
        //            //        //        flg_1250_kVA_1500 = 1;
        //            //        //    }
        //            //        //}
        //            //        // Commented by KB on 23/07/2024

        //            //        if (File.Exists(strFilePath_2))
        //            //        {
        //            //            FileStream fs2 = new FileStream(strFilePath_2, FileMode.Open, FileAccess.Read);
        //            //            Attachment a2 = new Attachment(fs2, strpdf_2, MediaTypeNames.Application.Octet);
        //            //            m.Attachments.Add(a2);
        //            //        }

        //            //        // New Anubandh
        //            //        if (Qtn_CPCB != "4")
        //            //        {
        //            //            if (Convert.ToDouble(split_kva_item[j].Trim()) == 125 && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() == "6R")
        //            //            {
        //            //                //Skip Anubandh by KB on 21/12/2023

        //            //                //Skip Anubandh
        //            //            }
        //            //            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
        //            //            {
        //            //                //Skip Anubandh by KB on 21/12/2023
        //            //                //strFilePath_3 = AppDomain.CurrentDomain.BaseDirectory + "quotation/quotation/Anubandh.pdf";
        //            //                //strpdf_3 = "Anubandh.pdf";
        //            //                //flg_anubandh = 1;

        //            //                if (File.Exists(strFilePath_3))
        //            //                {
        //            //                    //Skip Anubandh by KB on 21/12/2023

        //            //                    //FileStream fs3 = new FileStream(strFilePath_3, FileMode.Open, FileAccess.Read);
        //            //                    //Attachment a3 = new Attachment(fs3, strpdf_3, MediaTypeNames.Application.Octet);
        //            //                    //m.Attachments.Add(a3);
        //            //                }
        //            //            }
        //            //        }
        //            //        //End 
        //            //    }
        //            //}
        //            #endregion


        //            #region
        //            int flg_anubandh = 0;
        //            //int flg_2kW_5kVA = 0;
        //            //int flg_R550 = 0;
        //            //int flg_5_To_12_kVA = 0;
        //            //int flg_15_To_30_kVA = 0;
        //            //int flg_40_To_160_kVA = 0;
        //            //int flg_200_To_250_kVA = 0;
        //            //int flg_320_1010_kVA = 0;
        //            //int flg_1250_kVA_1500 = 0;

        //            //int flg_2_8_To_5_5_kVA = 0;
        //            //int flg_7_5_To_20_kVA = 0;
        //            //int flg_NG_CPCB_IV_15_to_250_kVA = 0;
        //            //int flg_OP_CPCB_IV_117_2000_kVA;

        //            int flg_NG_CPCB_IV_15_to_250_kVA = 0;
        //            int flg_2_To_7_kVA = 0;
        //            int flg_7_To_20_kVA = 0;
        //            int flg_25_To_58_5_kVA = 0;
        //            int flg_80_To_160_kVA = 0;
        //            int flg_200_To_250_kVA = 0;
        //            int flg_320_TO_750_kVA = 0;
        //            int flg_1010_TO_1500_kVA = 0;
        //            int flg_OP_CPCB_IV_117_To_2000_kVA = 0;


        //            string strProc = "";
        //            strProc += " Select  QD.QtnNo,QD.Qty,QD.BasicPrice, ";
        //            strProc += " QD.Partcode,PT.PartDesc,Pt.KVA,Pt.Phase,Pt.Model,Pt.cfm , substring(QD.PartCode,15,1) as CPCB ,substring(QD.PartCode,10,1) as OptiCPCB ";
        //            strProc += " from Quotation Q inner join QuotationDetails  QD on Q.QtnNo=QD.QtnNo inner join Part  PT on PT.PartCode=QD.PartCode ";
        //            strProc += " WHERE Q.QtNNo='" + Original_Qtn_No.Trim() + "' ";
        //            DataSet ds = dc.procDS(strProc, "QQ1");



        //            // Prv Ouotation Details
        //            if (ds.Tables["QQ1"].Rows.Count > 0)
        //            {
        //                for (int i = 0; (i < ds.Tables["QQ1"].Rows.Count); i++)
        //                {
        //                    string str_FilePath = "";
        //                    string str_pdf_Name = "";


        //                    string Str_KVA = ds.Tables["QQ1"].Rows[i]["KVA"].ToString().Trim();
        //                    string model_type = ds.Tables["QQ1"].Rows[i]["Model"].ToString().Trim();
        //                    string Qtn_CPCB = ds.Tables["QQ1"].Rows[i]["CPCB"].ToString().Trim();
        //                    string Opti_CPCB = ds.Tables["QQ1"].Rows[i]["OptiCPCB"].ToString().Trim();

        //                    //string model_type = clsCommonFunctions.getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "') and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "Model");
        //                    //string Qtn_CPCB = clsCommonFunctions.getName("select substring(q.PartCode,15,1) as CPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "CPCB");

        //                    ////int Opti = Convert.ToInt32 (clsCommonFunctions.getName("select isnull(sum(k.Qty),0) as EngCount from quotationdetails q inner join part p on q.partcode=p.PartCode inner join kitdetails k on k.kitcode=p.PartCode where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "' and k.PartCode like '001%' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "EngCount"));
        //                    //string Opti_CPCB = clsCommonFunctions.getName("select top 1 substring(q.PartCode,10,1) as OptiCPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "OptiCPCB");



        //                    //if (Qtn_CPCB == "4")
        //                    //{
        //                    //if (model_type.Trim().PadRight(3).Trim() == "NG1")
        //                    if (model_type.Substring(model_type.Trim().Length - 3).Trim() == "NG1")
        //                    {

        //                        if (flg_NG_CPCB_IV_15_to_250_kVA == 0)
        //                        {

        //                            str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/NG_CPCB IV+15-250_kVA.PDF";
        //                            str_pdf_Name = "NG_CPCB IV+15-250_kVA.PDF";
        //                            if (File.Exists(str_FilePath))
        //                            {
        //                                FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
        //                                Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
        //                                m.Attachments.Add(a2);
        //                            }
        //                        }

        //                        flg_NG_CPCB_IV_15_to_250_kVA = flg_NG_CPCB_IV_15_to_250_kVA + 1;
        //                    }
        //                    else
        //                    {


        //                        if (Opti_CPCB.Trim() == "5") //OPTIPrime
        //                        {
        //                            if (flg_OP_CPCB_IV_117_To_2000_kVA == 0)
        //                            {
        //                                str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/OP_CPCB IV+117-2000_kVA.PDF";
        //                                str_pdf_Name = "OP_CPCB IV+117-2000_kVA.PDF";
        //                                if (File.Exists(str_FilePath))
        //                                {
        //                                    FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
        //                                    Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
        //                                    m.Attachments.Add(a2);
        //                                }
        //                            }

        //                            flg_OP_CPCB_IV_117_To_2000_kVA = flg_OP_CPCB_IV_117_To_2000_kVA + 1;


        //                        }
        //                        else
        //                        {

        //                            //if ((Convert.ToDouble(Str_KVA.Trim()) >= 2 && Convert.ToDouble(Str_KVA.Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
        //                            //{
        //                            //    //str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/Kirloskar_2kW_5kVA.pdf";
        //                            //    //str_pdf_Name = "Kirloskar_2kW_5kVA_Catlogue.pdf";
        //                            //    //flg_2kW_5kVA = 1;
        //                            //}



        //                            if ((Convert.ToDouble(Str_KVA.Trim()) >= 2 && Convert.ToDouble(Str_KVA.Trim()) <= 7))
        //                            {
        //                                if (flg_2_To_7_kVA == 0)
        //                                {
        //                                    str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+2.8-5.5_kVA.PDF";
        //                                    str_pdf_Name = "CPCB_IV+2.8-5.5_kVA.PDF";
        //                                    if (File.Exists(str_FilePath))
        //                                    {
        //                                        FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
        //                                        Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
        //                                        m.Attachments.Add(a2);
        //                                    }
        //                                }

        //                                flg_2_To_7_kVA = flg_2_To_7_kVA + 1;
        //                            }
        //                            // EA Series
        //                            else if ((Convert.ToDouble(Str_KVA.Trim()) >= 7 && Convert.ToDouble(Str_KVA.Trim()) <= 20))
        //                            {
        //                                if (flg_7_To_20_kVA == 0)
        //                                {
        //                                    str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+7.5-20_kVA.PDF";
        //                                    str_pdf_Name = "CPCB_IV+7.5-20_kVA.PDF";
        //                                    if (File.Exists(str_FilePath))
        //                                    {
        //                                        FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
        //                                        Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
        //                                        m.Attachments.Add(a2);
        //                                    }
        //                                }

        //                                flg_7_To_20_kVA = flg_7_To_20_kVA + 1;
        //                            }

        //                            else if ((Convert.ToDouble(Str_KVA.Trim()) >= 25 && Convert.ToDouble(Str_KVA.Trim()) <= 58.5))
        //                            {
        //                                if (flg_25_To_58_5_kVA == 0)
        //                                {
        //                                    str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+25-58.5_kVA.PDF";
        //                                    str_pdf_Name = "CPCB_IV+25-58.5_kVA.PDF";
        //                                    if (File.Exists(str_FilePath))
        //                                    {
        //                                        FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
        //                                        Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
        //                                        m.Attachments.Add(a2);
        //                                    }
        //                                }
        //                                flg_25_To_58_5_kVA = flg_25_To_58_5_kVA + 1;
        //                            }
        //                            else if ((Convert.ToDouble(Str_KVA.Trim()) >= 80 && Convert.ToDouble(Str_KVA.Trim()) <= 160))
        //                            {
        //                                if (flg_80_To_160_kVA == 0)
        //                                {
        //                                    str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+82.5-160_kVA.PDF";
        //                                    str_pdf_Name = "CPCB_IV+82.5-160_kVA.PDF";
        //                                    if (File.Exists(str_FilePath))
        //                                    {
        //                                        FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
        //                                        Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
        //                                        m.Attachments.Add(a2);
        //                                    }
        //                                }
        //                                flg_80_To_160_kVA = flg_80_To_160_kVA + 1;
        //                            }
        //                            else if ((Convert.ToDouble(Str_KVA.Trim()) >= 200 && Convert.ToDouble(Str_KVA.Trim()) <= 250))
        //                            {
        //                                if (flg_200_To_250_kVA == 0)
        //                                {
        //                                    str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+200-250_kVA.PDF";
        //                                    str_pdf_Name = "CPCB_IV+200-250_kVA.PDF";
        //                                    if (File.Exists(str_FilePath))
        //                                    {
        //                                        FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
        //                                        Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
        //                                        m.Attachments.Add(a2);
        //                                    }
        //                                }
        //                                flg_200_To_250_kVA = flg_200_To_250_kVA + 1;
        //                            }
        //                            else if ((Convert.ToDouble(Str_KVA.Trim()) >= 320 && Convert.ToDouble(Str_KVA.Trim()) <= 750))
        //                            {
        //                                if (flg_320_TO_750_kVA == 0)
        //                                {
        //                                    //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
        //                                    str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+320-750_kVA.PDF";
        //                                    str_pdf_Name = "CPCB_IV+320-750_kVA.PDF";
        //                                    if (File.Exists(str_FilePath))
        //                                    {
        //                                        FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
        //                                        Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
        //                                        m.Attachments.Add(a2);
        //                                    }
        //                                }
        //                                flg_320_TO_750_kVA = flg_320_TO_750_kVA + 1;
        //                            }
        //                            else if ((Convert.ToDouble(Str_KVA.Trim()) >= 1010 && Convert.ToDouble(Str_KVA.Trim()) <= 1500))
        //                            {
        //                                if (flg_1010_TO_1500_kVA == 0)
        //                                {
        //                                    //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
        //                                    str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+1010-1500_kVA.PDF";
        //                                    str_pdf_Name = "CPCB_IV+1010-1500_kVA.PDF";
        //                                    if (File.Exists(str_FilePath))
        //                                    {
        //                                        FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
        //                                        Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
        //                                        m.Attachments.Add(a2);
        //                                    }
        //                                }
        //                                flg_1010_TO_1500_kVA = flg_1010_TO_1500_kVA + 1;
        //                            }
        //                        }
        //                    }


        //                    // New Anubandh
        //                    if (Qtn_CPCB != "4")
        //                    {
        //                        if (Convert.ToDouble(Str_KVA.Trim()) == 125 && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() == "6R")
        //                        {
        //                            //Skip Anubandh by KB on 21/12/2023

        //                            //Skip Anubandh
        //                        }
        //                        else if ((Convert.ToDouble(Str_KVA.Trim()) >= 5 && Convert.ToDouble(Str_KVA.Trim()) <= 160) && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
        //                        {
        //                            //Skip Anubandh by KB on 21/12/2023
        //                            //strFilePath_3 = AppDomain.CurrentDomain.BaseDirectory + "Pages/Marketing/Reports/quotation/Anubandh.pdf";
        //                            //strpdf_3 = "Anubandh.pdf";
        //                            //flg_anubandh = 1;

        //                            // if (File.Exists(strFilePath_3))
        //                            // {
        //                            //Skip Anubandh by KB on 21/12/2023

        //                            //FileStream fs3 = new FileStream(strFilePath_3, FileMode.Open, FileAccess.Read);
        //                            //Attachment a3 = new Attachment(fs3, strpdf_3, MediaTypeNames.Application.Octet);
        //                            //m.Attachments.Add(a3);
        //                            //}
        //                        }
        //                    }
        //                    //End 
        //                }
        //            }


        //            #region

        //            // string[] split_kva_item = Regex.Split(kva.Trim(), ",");
        //            //if (split_kva_item.Length > 0)
        //            //{
        //            //    for (int j = 0; j < split_kva_item.Length; j++)
        //            //    {
        //            //        string strFilePath_2 = "", strFilePath_3 = "";
        //            //        string strpdf_2 = "", strpdf_3 = "";
        //            //        string model_type = cls.getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + Original_Qtn_No.Trim() + "') and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "Model");
        //            //        string Qtn_CPCB = cls.getName("select substring(q.PartCode,15,1) as CPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "CPCB");
        //            //        //int Opti = Convert.ToInt32 (clsCommonFunctions.getName("select isnull(sum(k.Qty),0) as EngCount from quotationdetails q inner join part p on q.partcode=p.PartCode inner join kitdetails k on k.kitcode=p.PartCode where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "' and k.PartCode like '001%' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "EngCount"));
        //            //        string Opti_CPCB = cls.getName("select top 1 substring(q.PartCode,10,1) as OptiCPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "OptiCPCB");

        //            //        //if (Qtn_CPCB == "4")
        //            //        //{
        //            //        //if (model_type.Trim().PadRight(3).Trim() == "NG1")
        //            //        if (model_type.Substring(model_type.Trim().Length - 3) == "NG1")
        //            //        {
        //            //            strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/NG_CPCB IV+15-250_kVA.PDF";
        //            //            strpdf_2 = "NG_CPCB IV+15-250_kVA.PDF";
        //            //            flg_NG_CPCB_IV_15_to_250_kVA = 1;
        //            //        }
        //            //        else
        //            //        {
        //            //            if (Opti_CPCB == "5")
        //            //            {
        //            //                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/OP_CPCB IV+117-2000_kVA.PDF";
        //            //                strpdf_2 = "OP_CPCB IV+117-2000_kVA.PDF";
        //            //                flg_OP_CPCB_IV_117_2000_kVA = 1;
        //            //            }
        //            //            else
        //            //            {

        //            //                //if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
        //            //                //{
        //            //                //    //strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/Kirloskar_2kW_5kVA.pdf";
        //            //                //    //strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
        //            //                //    //flg_2kW_5kVA = 1;
        //            //                //}


        //            //                //2. 8kW-5.5kVA
        //            //                if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 7))
        //            //                {
        //            //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+2.8-5.5_kVA.PDF";
        //            //                    strpdf_2 = "CPCB_IV+2.8-5.5_kVA.PDF";
        //            //                    flg_2_8_To_5_5_kVA = 1;
        //            //                }
        //            //                // EA Series
        //            //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 7 && Convert.ToDouble(split_kva_item[j].Trim()) <= 20))
        //            //                {
        //            //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+7.5-20_kVA.PDF";
        //            //                    strpdf_2 = "CPCB_IV+7.5-20_kVA.PDF";
        //            //                    flg_7_5_To_20_kVA = 1;
        //            //                }
        //            //                //3R550
        //            //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 25 && Convert.ToDouble(split_kva_item[j].Trim()) <= 58.5))
        //            //                {
        //            //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+25-58.5_kVA.PDF";
        //            //                    strpdf_2 = "CPCB_IV+25-58.5_kVA.PDF";
        //            //                    flg_R550 = 1;
        //            //                }
        //            //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 80 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160))
        //            //                {
        //            //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+82.5-160_kVA.PDF";
        //            //                    strpdf_2 = "CPCB_IV+82.5-160_kVA.PDF";
        //            //                    flg_15_To_30_kVA = 1;
        //            //                }
        //            //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250))
        //            //                {
        //            //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+200-250_kVA.PDF";
        //            //                    strpdf_2 = "CPCB_IV+200-250_kVA.PDF";
        //            //                    flg_40_To_160_kVA = 1;
        //            //                }
        //            //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 750))
        //            //                {
        //            //                    //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
        //            //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+320-750_kVA.PDF";
        //            //                    strpdf_2 = "CPCB_IV+320-750_kVA.PDF";
        //            //                    flg_320_1010_kVA = 1;
        //            //                }
        //            //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 1010 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1500))
        //            //                {
        //            //                    //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
        //            //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+1010-1500_kVA.PDF";
        //            //                    strpdf_2 = "CPCB_IV+1010-1500_kVA.PDF";
        //            //                    flg_320_1010_kVA = 1;
        //            //                }
        //            //            }
        //            //        }

        //            //        //}

        //            //        // Commented by KB on 23/07/2024 as CPCB 2 is Excluded
        //            //        //else
        //            //        //{
        //            //        //    if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
        //            //        //    {
        //            //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_2kW_5kVA.pdf";
        //            //        //        strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
        //            //        //        flg_2kW_5kVA = 1;
        //            //        //    }
        //            //        //    // EA Series
        //            //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 12.5) && flg_5_To_12_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
        //            //        //    {
        //            //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/iGreen-5-160kVA.pdf";
        //            //        //        strpdf_2 = "Catlogue_iGreen_5_To_160_kVA.pdf";
        //            //        //        flg_5_To_12_kVA = 1;
        //            //        //    }
        //            //        //    //3R550
        //            //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 15) && flg_R550 == 0 && model_type.Trim().Substring(0, 5).Trim() == "3R550")
        //            //        //    {
        //            //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_R550_15_kVA.pdf";
        //            //        //        strpdf_2 = "Kirloskar_R550_15_kVA_Catlogue.pdf";
        //            //        //        flg_R550 = 1;
        //            //        //    }
        //            //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 30) && flg_15_To_30_kVA == 0)
        //            //        //    {
        //            //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_15_To_30_kVA.pdf";
        //            //        //        strpdf_2 = "Kirloskar_15_To_30_kVA_Catlogue.pdf";
        //            //        //        flg_15_To_30_kVA = 1;
        //            //        //    }
        //            //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 40 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_40_To_160_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "6R")
        //            //        //    {
        //            //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_40_To_160_kVA.pdf";
        //            //        //        strpdf_2 = "Kirloskar_40_To_160_kVA_Catalogue.pdf";
        //            //        //        flg_40_To_160_kVA = 1;
        //            //        //    }
        //            //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250) && flg_200_To_250_kVA == 0)
        //            //        //    {
        //            //        //        // Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
        //            //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_200_To_250_kVA.pdf";
        //            //        //        strpdf_2 = "Kirloskar_200_To_250_kVA_Catlogue.pdf";
        //            //        //        flg_200_To_250_kVA = 1;
        //            //        //    }
        //            //        //    //else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1010) && flg_320_1010_kVA == 0)
        //            //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) < 1000) && flg_320_1010_kVA == 0)
        //            //        //    {
        //            //        //        // Commented by KB on 08/05/2023 as per mail from Kalpesh Sir on 06/05/2023
        //            //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_320_1010_kVA.pdf";
        //            //        //        strpdf_2 = "Kirloskar_320_1010_kVA_Catlogue.pdf";
        //            //        //        flg_320_1010_kVA = 1;
        //            //        //    }
        //            //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 1250 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1500) && flg_1250_kVA_1500 == 0)
        //            //        //    {
        //            //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_1250_To_1500_kVA.pdf";
        //            //        //        strpdf_2 = "Kirloskar_1250_To_1500_kVA_Catlogue.pdf";
        //            //        //        flg_1250_kVA_1500 = 1;
        //            //        //    }
        //            //        //}
        //            //        // Commented by KB on 23/07/2024

        //            //        if (File.Exists(strFilePath_2))
        //            //        {
        //            //            FileStream fs2 = new FileStream(strFilePath_2, FileMode.Open, FileAccess.Read);
        //            //            Attachment a2 = new Attachment(fs2, strpdf_2, MediaTypeNames.Application.Octet);
        //            //            m.Attachments.Add(a2);
        //            //        }

        //            //        // New Anubandh
        //            //        if (Qtn_CPCB != "4")
        //            //        {
        //            //            if (Convert.ToDouble(split_kva_item[j].Trim()) == 125 && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() == "6R")
        //            //            {
        //            //                //Skip Anubandh by KB on 21/12/2023

        //            //                //Skip Anubandh
        //            //            }
        //            //            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
        //            //            {
        //            //                //Skip Anubandh by KB on 21/12/2023
        //            //                //strFilePath_3 = AppDomain.CurrentDomain.BaseDirectory + "quotation/quotation/Anubandh.pdf";
        //            //                //strpdf_3 = "Anubandh.pdf";
        //            //                //flg_anubandh = 1;

        //            //                if (File.Exists(strFilePath_3))
        //            //                {
        //            //                    //Skip Anubandh by KB on 21/12/2023

        //            //                    //FileStream fs3 = new FileStream(strFilePath_3, FileMode.Open, FileAccess.Read);
        //            //                    //Attachment a3 = new Attachment(fs3, strpdf_3, MediaTypeNames.Application.Octet);
        //            //                    //m.Attachments.Add(a3);
        //            //                }
        //            //            }
        //            //        }
        //            //        //End 
        //            //    }
        //            //}
        //            #endregion
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


        //            //Add sales.support7@kalabiz.com and Indore and bhopal HOD EmailID IN CC
        //            if (qtn_AssToEmpPCCode.Trim() == "07.005")//  Indore 
        //            {
        //                if (string.IsNullOrEmpty(cc_mail_id))
        //                {
        //                    cc_mail_id = "vijay.nikam@kalabiz.com" + "," + "sales.indore@kalabiz.com";
        //                }
        //                else
        //                {
        //                    cc_mail_id = cc_mail_id.ToString().Trim() + "," + "vijay.nikam@kalabiz.com" + "," + "sales.indore@kalabiz.com" + "," + "vaishakh.pathak@kalabiz.com";
        //                }
        //            }
        //            if (qtn_AssToEmpPCCode.Trim() == "07.014")// Bhopal
        //            {

        //                if (string.IsNullOrEmpty(cc_mail_id))
        //                {
        //                    cc_mail_id = "vijay.nikam@kalabiz.com" + "," + "sales.bhopal@kalabiz.com";
        //                }
        //                else
        //                {
        //                    cc_mail_id = cc_mail_id.ToString().Trim() + "," + "vijay.nikam@kalabiz.com" + "," + "sales.bhopal@kalabiz.com";
        //                }



        //            }


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

        //            Body += "<p>We as an authorized Genset Original Equipment Manufacturers (G.O.E.M's) for ";

        //            Body += "Kirloskar Oil Engines Limited-Pune, manufacture quality KOEL Green ";
        //            Body += "Generating Sets in the range of 2.1 kVA to 1500 kVA for captive and standby ";
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

        //            //if (Original_Qtn_No.Trim().Substring(10, 2).Trim() == "16")
        //            //{
        //            //    Body += "<p style='text-align: justify'><span><span style='color:#1F497D'><b>Kala International LLC</b></span></span></p>";
        //            //}
        //            //else
        //            //{
        //            Body += "<p style='text-align: justify'><span><span style='color:#1F497D'><b>Kala Genset Pvt ltd.</b></span></span></p>";
        //            // }

        //            #endregion

        //            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, null, "text/html");

        //            //  var filePath = Path.Combine("~//images//kalalogo.JPG", "images", "kalalogo.JPG");

        //            LinkedResource imagelink = new LinkedResource(AppDomain.CurrentDomain.BaseDirectory + "images/kalalogo.JPG", "image/jpeg");

        //            //  LinkedResource imagelink = new LinkedResource(Server.MapPath("~//images//kalalogo.JPG"), "image/png");
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
        //                //sc.Credentials = new System.Net.NetworkCredential("erp@kalabiz.com", "wanv ftwc dobq blrr");
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
        //            //if (File.Exists(strFilePath_1))
        //            //{
        //            //    File.Delete(strFilePath_1);
        //            //}
        //        }
        //        //  return "Successfully";
        //        emailStatus = "Email Send Successfully";
        //    }
        //    catch (Exception ex)
        //    {
        //        //if (File.Exists(strFilePath_1))
        //        //{
        //        //    File.Delete(strFilePath_1);
        //        //}
        //        //  return "Failed" + ex.StackTrace;
        //        emailStatus = "Mail Sending Error,Please Try Again";
        //        //return "Failed";
        //    }
        //}

        ////Commented by KB on 23/01/2026
        //protected void LoadReport(string QtnNo, string ReportName, string loadRptType, string AssignToEmpID, string AssignToEmpName, string AssignToMobileNo, string AssignToMailID, string Model)
        //{
        //    var rpt = new ReportDocument();
        //    if (loadRptType.ToString().Trim() == "Q")
        //    {
        //        // iGreen

        //        int Qtn_CPCB = Convert.ToInt32(dc.getName("select count(qtnno) as CPCB from quotationdetails where qtnno = '" + QtnNo + "' and substring(PartCode,15,1) ='4'", "QuotationDetails", "CPCB"));
        //        string model_Gas = dc.getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + QtnNo + "')", "QuotationDetails", "Model");


        //        //if (Qtn_CPCB == 0)
        //        //{
        //        //    if (Model.Trim() == "iGreen")
        //        //    {
        //        //        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAiGreen.rpt");
        //        //    }
        //        //    else
        //        //    {
        //        //        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALA.rpt");
        //        //    }
        //        //}
        //        //else if (Qtn_CPCB > 0)
        //        //{

        //        if (model_Gas.Substring(model_Gas.Trim().Length - 3) == "NG1")
        //        {
        //            rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAGasDGCPCBIV.rpt");

        //        }
        //        else
        //        {
        //            if (Model.Trim() == "iGreen")
        //            {
        //                rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAiGreenCPCBIV.rpt");
        //            }

        //            else
        //            {

        //                rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAiGreenCPCBIV.rpt");

        //                //rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "Pages/Marketing/Reports/quotation/KALACPCBIV.rpt");
        //            }
        //        }


        //        //}
        //        rpt.SetDatabaseLogon("sa", "3HMungiIMR", "Localhost", "ERP");
        //        rpt.SetParameterValue(0, QtnNo.ToString().Trim());
        //        rpt.SetParameterValue(1, AssignToEmpName.Trim());
        //        rpt.SetParameterValue(2, AssignToMobileNo.Trim());
        //        rpt.SetParameterValue(3, AssignToMailID.Trim());
        //        rpt.SetParameterValue(4, "'" + QtnNo.ToString().Trim() + "'");
        //        rpt.ExportToDisk(ExportFormatType.PortableDocFormat, ReportName.ToString().Trim());
        //        rpt.Close();
        //    }
        //    else if (loadRptType.ToString().Trim() == "I")
        //    {
        //        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAInstallation.rpt");
        //        rpt.SetDatabaseLogon("sa", "3HMungiIMR", "Localhost", "ERP");
        //        rpt.SetParameterValue(0, QtnNo.ToString().Trim());
        //        rpt.SetParameterValue(1, AssignToEmpName.Trim());
        //        rpt.SetParameterValue(2, AssignToMobileNo.Trim());
        //        rpt.SetParameterValue(3, AssignToMailID.Trim());
        //        rpt.ExportToDisk(ExportFormatType.PortableDocFormat, ReportName.ToString().Trim());
        //        rpt.Close();
        //    }
        //}


        //protected void sendQuotEmail(string qtn_no, string kva, string to_mail_id, string cc_mail_id, string ass_to_emp_id, string ass_to_emp_name, string ass_to_mob_no, string ass_to_mail_id, string cust_name, string cust_add, string model)
        //{
        //    SmtpClient sc;
        //    string Body = "";
        //    string Original_Qtn_No = qtn_no;
        //    qtn_no = qtn_no.Replace("/", "_");
        //    string strFilePath_1 = ConfigurationManager.AppSettings["Quotationfile"] + "/" + qtn_no + "_" + DateTime.Now.ToString("dd-MM-yyyy") + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".pdf";
        //    string dt_time = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        //    //   string strFilePath_1 = ConfigurationManager.AppSettings["Quotationfile"] + "/" + "QTN_24-25_07013431_10-03-2025_16_21_53.pdf";
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

        //            string[] split_kva_item = Regex.Split(kva.Trim(), ",");
        //            if (split_kva_item.Length > 0)
        //            {
        //                for (int j = 0; j < split_kva_item.Length; j++)
        //                {
        //                    string strFilePath_2 = "", strFilePath_3 = "";
        //                    string strpdf_2 = "", strpdf_3 = "";

        //                    //string model_type = getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + Original_Qtn_No.Trim() + "') and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "Model");
        //                    //string Qtn_CPCB = getName("select substring(q.PartCode,15,1) as CPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "CPCB");


        //                    string model_type = dc.getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + Original_Qtn_No.Trim() + "') and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "Model");
        //                    string Qtn_CPCB = dc.getName("select substring(q.PartCode,15,1) as CPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "CPCB");
        //                    //int Opti = Convert.ToInt32(getName("select isnull(sum(k.Qty),0) as EngCount from quotationdetails q inner join part p on q.partcode=p.PartCode inner join kitdetails k on k.kitcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and k.PartCode like '001%' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "EngCount"));
        //                    string Opti_CPCB = dc.getName("select top 1 substring(q.PartCode,10,1) as OptiCPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "OptiCPCB");

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
        //                    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
        //                    {
        //                        //strFilePath_3 = AppDomain.CurrentDomain.BaseDirectory + "quotation/Anubandh.pdf";
        //                        //strpdf_3 = "Anubandh.pdf";
        //                        //flg_anubandh = 1;

        //                        if (File.Exists(strFilePath_3))
        //                        {
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
        //            Body += "Generating Sets in the range of 2.1 KVA to 1010 KVA for captive and standby ";
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

        //            //  var filePath = Path.Combine("~//images//kalalogo.JPG", "images", "kalalogo.JPG");

        //            LinkedResource imagelink = new LinkedResource(AppDomain.CurrentDomain.BaseDirectory + "images/kalalogo.JPG", "image/jpeg");

        //            //  LinkedResource imagelink = new LinkedResource(Server.MapPath("~//images//kalalogo.JPG"), "image/png");
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
        //            //if (File.Exists(strFilePath_1))
        //            //{
        //            //    File.Delete(strFilePath_1);
        //            //}
        //        }
        //        //  return "Successfully";
        //        emailStatus = "Successfully";
        //    }
        //    catch (Exception ex)
        //    {
        //        //if (File.Exists(strFilePath_1))
        //        //{
        //        //    File.Delete(strFilePath_1);
        //        //}
        //        //  return "Failed" + ex.StackTrace;
        //        emailStatus = "Failed";
        //        //return "Failed";
        //    }
        //}

        //protected void LoadReport(string QtnNo, string ReportName, string loadRptType, string AssignToEmpID, string AssignToEmpName, string AssignToMobileNo, string AssignToMailID, string Model)
        //{
        //    var rpt = new ReportDocument();
        //    if (loadRptType.ToString().Trim() == "Q")
        //    {
        //        // iGreen

        //        int Qtn_CPCB = Convert.ToInt32(dc.getName("select count(qtnno) as CPCB from quotationdetails where qtnno = '" + QtnNo + "' and substring(PartCode,15,1) ='4'", "QuotationDetails", "CPCB"));
        //        string model_Gas = dc.getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + QtnNo + "')", "QuotationDetails", "Model");


        //        //if (Qtn_CPCB == 0)
        //        //{
        //        //    if (Model.Trim() == "iGreen")
        //        //    {
        //        //        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAiGreen.rpt");
        //        //    }
        //        //    else
        //        //    {
        //        //        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALA.rpt");
        //        //    }
        //        //}
        //        //else if (Qtn_CPCB > 0)
        //        //{
        //        if (model_Gas.Substring(model_Gas.Length - 3) == "NG1")
        //        {
        //            rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAGasDGCPCBIV.rpt");
        //        }
        //        else
        //        {
        //            if (Model.Trim() == "iGreen")
        //            {
        //                rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAiGreenCPCBIV.rpt");
        //            }
        //            else
        //            {

        //                rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAiGreenCPCBIV.rpt");

        //                //rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALACPCBIV.rpt");
        //            }
        //        }
        //        //}
        //        rpt.SetDatabaseLogon("sa", "3HMungiIMR", "Localhost", "ERP");
        //        rpt.SetParameterValue(0, QtnNo.ToString().Trim());
        //        rpt.SetParameterValue(1, AssignToEmpName.Trim());
        //        rpt.SetParameterValue(2, AssignToMobileNo.Trim());
        //        rpt.SetParameterValue(3, AssignToMailID.Trim());
        //        rpt.SetParameterValue(4, "'" + QtnNo.ToString().Trim() + "'");
        //        rpt.ExportToDisk(ExportFormatType.PortableDocFormat, ReportName.ToString().Trim());
        //        rpt.Close();
        //    }
        //    else if (loadRptType.ToString().Trim() == "I")
        //    {
        //        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAInstallation.rpt");
        //        rpt.SetDatabaseLogon("sa", "3HMungiIMR", "Localhost", "ERP");
        //        rpt.SetParameterValue(0, QtnNo.ToString().Trim());
        //        rpt.SetParameterValue(1, AssignToEmpName.Trim());
        //        rpt.SetParameterValue(2, AssignToMobileNo.Trim());
        //        rpt.SetParameterValue(3, AssignToMailID.Trim());
        //        rpt.ExportToDisk(ExportFormatType.PortableDocFormat, ReportName.ToString().Trim());
        //        rpt.Close();
        //    }
        //}



        [HttpGet]
        [Route("Enquiry/GetCustomerDetails")]
        public DataTable GetCustomerDetails(string CustName) // OK
        {
            sb.Remove(0, sb.Length);
            //sb.Append("  select EnqNo + ' #Date: ' + convert(varchar(10),dt,103)+ ' #Cust: ' +CustName+ ' #Address: ' +Address+ ' #Mobile: ' +MobileNo+ ' #EMail: ' +EMailID as ENQINFO,EnqNo From Enquiry" +
            //    " where active=1 and(CustName like '" + CustName.Trim() + "%' and Assigntoempid = '3497') order by EnqNo desc ");
            sb.Append("   select EnqNo + ' #Date: ' + convert(varchar(10),dt,103)+ ' #Cust: ' +CustName+ ' #Address: ' +Address+ ' #Mobile: ' +MobileNo+ ' #EMail: ' +EMailID+ ' #Title: ' +Title + ' #Address: ' +Address+ ' #RefFromHead: ' +RefFromHead+ ' #RefFrom: ' +RefFrom+ ' #DomainID: ' +DomainID + ' #City: ' +City as ENQINFO,EnqNo  From Enquiry" +
                " where active=1 and(CustName like 'satish' and Assigntoempid = '3497') order by EnqNo desc ");
            return dc.procDT(sb.ToString(), "tbl_CustDetails");
        }
    }
}
