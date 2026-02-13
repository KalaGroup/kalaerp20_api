using KalaERPApi.Models.Production.Canopy.Trans;
using KalaERPApi.Service;
using KalaERPApi.Service.Production.Canopy.Trans;
using System.Data;
using System.IO;
using System.Web.Http;

namespace KalaERPApi.Controllers.Production.Canopy.Trans
{
    public class CpyPrcController : ApiController
    {
        CpyPrcCon dc = new CpyPrcCon();
        CommonCon ComCon = new CommonCon();

        [HttpGet]
        [Route("CpyPrc/getCpyPrcddl")]
        public DataTable GetCpyPrcKVA(string PCCode, string MachineCode, string KVA, string Model, string PlanCode, string CatID)
        {
            return dc.getCpyPrcddl(PCCode, MachineCode, KVA, Model, PlanCode, CatID);
        }

        [HttpGet]
        [Route("CpyPrc/getCpyPrc6M")]
        public DataTable getCpyPrc6M()
        {
            return ComCon.Get6M();
        }

        [HttpGet]
        [Route("CpyPrc/getSheetPartDts")]
        public DataTable GetSheetPartDts(string PCCode, int SheetSrNo, string MachineCode, string SheetPartcode, string PlanCode, string Partcode, string CatID)
        {
            return dc.GetSheetPartDts(PCCode, SheetSrNo, MachineCode, SheetPartcode, PlanCode, Partcode, CatID);
        }

        [HttpGet]
        [Route("CpyPrc/getCpyPrcddlFab")]
        public DataTable GetCpyPrcKVAFab(string PCCode, string MachineCode, string KVA, string Model, string SuppCode)
        {
            return dc.getCpyPrcddlFab(PCCode, MachineCode, KVA, Model, SuppCode);
        }

        [HttpGet]
        [Route("CpyPrc/GetCpyKitFab")]
        public DataTable GetCpyKitFab(string PCCode, string MachineCode, string PlanCode, string PartCode, string CpyKit, string SuppCode)
        {
            return dc.GetCpyKitFab(PCCode, MachineCode, PlanCode, PartCode, CpyKit, SuppCode);
        }

        [HttpGet]
        [Route("CpyPrc/getSheetPartDtsPartCut")]
        public DataTable GetSheetPartDtsPartCut(int SheetSrNo, string MachineCode, string SheetPartcode, string PlanCode, string Partcode)
        {
            return dc.GetSheetPartDtsPartCut(SheetSrNo, MachineCode, SheetPartcode, PlanCode, Partcode);
        }

        [HttpGet]
        [Route("CpyPrc/GetTKitDts")]
        public DataTable GetTKitDts(string PCCode, string TKitID, int BatchQty, string TrnsType, string Plancode, string ProdCode)

        {
            return dc.GetTKitDts(PCCode, TKitID, BatchQty, TrnsType, Plancode, ProdCode);
        }

        [HttpGet]
        [Route("CpyPrc/GetTKitDtsPartCut")]
        public DataTable GetTKitDtsPartCut(string TKitID, int BatchQty, string TrnsType, string Plancode, string ProdCode)
        {
            return dc.GetTKitDtsPartCut(TKitID, BatchQty, TrnsType, Plancode, ProdCode);
        }

        [HttpGet]
        [Route("CpyPrc/GetCpyKit")]
        public DataTable GetCpyKit(string PCCode, string MachineCode, string PlanCode, string PartCode, string CpyKit)
        {
            return dc.GetCpyKit(PCCode, MachineCode, PlanCode, PartCode, CpyKit);
        }

        [HttpGet]
        [Route("CpyPrc/GetCpyKitPC")]
        public DataTable GetCpyKitPC(string PCCode, string MachineCode, string PlanCode, string PartCode, string CpyKit, string Kva)
        {
            return dc.GetCpyKitPC(PCCode, MachineCode, PlanCode, PartCode, CpyKit, Kva);
        }

        [HttpGet]
        [Route("CpyPrc/CpyKitDtsCPYAssly")]
        public DataTable CpyKitDtsCPYAssly(string PCCode, int PrcQty, string CpyPartCode, string PlanCode, string BOMCode, string PFBCode)
        {
            return dc.CpyKitDtsCPYAssly(PCCode, PrcQty, CpyPartCode, PlanCode, BOMCode, PFBCode);
        }


        [HttpGet]
        [Route("CpyPrc/CpyKitDtsCPYAsslyKit")]
        public DataTable CpyKitDtsCPYAsslyKit(string PCCode, int PrcQty, string CpyPartCode, string PlanCode, string BOMCode, string PFBCode)
        {
            return dc.CpyKitDtsCPYAsslyKit(PCCode, PrcQty, CpyPartCode, PlanCode, BOMCode, PFBCode);
        }



        [HttpGet]
        [Route("CpyPrc/GetCpyKitDts")]
        public DataTable GetCpyKitDts(string PCCode, int BatchQty, string CpyKitcode, string BOMCode, string PFBCode)
        {
            return dc.CpyKitDts(PCCode, BatchQty, CpyKitcode, BOMCode, PFBCode);
        }

        [HttpGet]
        [Route("CpyPrc/GetCpyEndPrcDts")]
        public DataTable GetCpyEndPrcDts(string PCCode, string PlanCode, string PartCode)
        {
            return dc.CpyEndPrcDts(PCCode, PlanCode, PartCode);
        }

        [HttpGet]
        [Route("CpyPrc/LoadProduct")]
        public DataTable LoadProduct(string PCCode)
        {
            return dc.LoadProduct(PCCode);
        }

        [HttpGet]
        [Route("CpyPrc/LoadMachine")]
        public DataTable LoadMachine(string PCCode)
        {
            return dc.LoadMachine(PCCode);
        }

        [HttpGet]
        [Route("CpyPrc/LoadOSSupplier")]
        public DataTable LoadOSSupplier()
        {
            return dc.LoadOSSupplier();
        }


        [HttpGet]
        [Route("CpyPrc/LoadCatID")]
        public DataTable LoadCatID(string PCCode, string PlanCode)
        {
            return dc.LoadCatID(PCCode, PlanCode);
        }

        [HttpPost]
        [Route("CpyPrc/CncSubmit")]
        public string SubmitCNC(CpyPrcCNCRequest CpyPrcCNCReq)
        {
            return dc.SubmitCNC(CpyPrcCNCReq);
        }

        [HttpPost]
        [Route("CpyPrc/PartcutSubmit")]
        public string SubmitPartCut(CpyPrcPartcutRequest cpyPrcPartcut)
        {
            return dc.SubmitPartCut(cpyPrcPartcut);
        }

        [HttpPost]
        [Route("CpyPrc/BendingSubmit")]
        public string SubmitBending(CpyPrcBendRequest cpyPrcBendReq)
        {
            return dc.SubmitBending(cpyPrcBendReq);
        }

        [HttpPost]
        [Route("CpyPrc/FabricationSubmit")]
        public string SubmitFabrication(CpyPrcFabRequest cpyPrcFabReq)
        {
            return dc.SubmitFabrication(cpyPrcFabReq);
        }

        [HttpPost]
        [Route("CpyPrc/PowderCoatingSubmit")]
        public string SubmitPowderCoating(CpyPrcPCRequest cpyPrcPCReq)
        {
            return dc.SubmitPowderCoating(cpyPrcPCReq);
        }

        [HttpPost]
        [Route("CpyPrc/SubmitCanopyAssly")]
        public string SubmitCanopyAssly(CpyPrcRequest CpyPrcReq)
        {
            return dc.SubmitCanopyAssly(CpyPrcReq);
        }

        [HttpPost()]
        [Route("CNCPrc/UploadFiles")]
        public string UploadFilesCNC()
        {
            string StrTempPath = "", filePath = "";
            int iUploadedCnt = 0;

            var request = System.Web.HttpContext.Current.Request;
            StrTempPath = "C:/TempERPFile/TempPrcCNC/" + request["FrmEcode"].ToString();

            if (Directory.Exists(StrTempPath))
            {
                Directory.GetAccessControl(StrTempPath);
            }
            else
            {
                Directory.CreateDirectory(StrTempPath);
                Directory.GetAccessControl(StrTempPath);
            }

            if (request["FileUploadType"].ToString().Trim() == "Delete")
            {
                filePath = StrTempPath + "/" + request["fileUpload"].ToString().Trim();
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return " file Deleted";
            }
            else
            {
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                var FrmEcode = request["FrmEcode"].ToString();
                //  var fileUpload = request["fileUpload"].ToString();
                // CHECK THE FILE COUNT.
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    if (hpf.ContentLength > 0)
                    {
                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                        if (!File.Exists(StrTempPath + Path.GetFileName(hpf.FileName)))
                        {
                            // SAVE THE FILES IN THE FOLDER.
                            // hpf.SaveAs(StrTempPath + "/" +  "1@@" + Path.GetFileName(hpf.FileName) );
                            hpf.SaveAs(StrTempPath + "/" + Path.GetFileName(hpf.FileName));
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                    }
                }
                // RETURN A MESSAGE.
                if (iUploadedCnt > 0)
                {
                    return iUploadedCnt + " Files Uploaded Successfully";
                }
                else
                {
                    return "Upload Failed";
                }
            }
        }

        [HttpPost()]
        [Route("PCutPrc/UploadFiles")]
        public string UploadFilesPartCut()
        {
            string StrTempPath = "", filePath = "";
            int iUploadedCnt = 0;

            var request = System.Web.HttpContext.Current.Request;
            StrTempPath = "C:/TempERPFile/TempPrcPartCut/" + request["FrmEcode"].ToString();

            if (Directory.Exists(StrTempPath))
            {
                Directory.GetAccessControl(StrTempPath);
            }
            else
            {
                Directory.CreateDirectory(StrTempPath);
                Directory.GetAccessControl(StrTempPath);
            }

            if (request["FileUploadType"].ToString().Trim() == "Delete")
            {
                filePath = StrTempPath + "/" + request["fileUpload"].ToString().Trim();
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return " file Deleted";
            }
            else
            {
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                var FrmEcode = request["FrmEcode"].ToString();
                //  var fileUpload = request["fileUpload"].ToString();
                // CHECK THE FILE COUNT.
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    if (hpf.ContentLength > 0)
                    {
                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                        if (!File.Exists(StrTempPath + Path.GetFileName(hpf.FileName)))
                        {
                            // SAVE THE FILES IN THE FOLDER.
                            // hpf.SaveAs(StrTempPath + "/" +  "1@@" + Path.GetFileName(hpf.FileName) );
                            hpf.SaveAs(StrTempPath + "/" + Path.GetFileName(hpf.FileName));
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                    }
                }
                // RETURN A MESSAGE.
                if (iUploadedCnt > 0)
                {
                    return iUploadedCnt + " Files Uploaded Successfully";
                }
                else
                {
                    return "Upload Failed";
                }
            }
        }

        [HttpPost()]
        [Route("BendPrc/UploadFiles")]
        public string UploadFilesBend()
        {
            string StrTempPath = "", filePath = "";
            int iUploadedCnt = 0;

            var request = System.Web.HttpContext.Current.Request;
            StrTempPath = "C:/TempERPFile/TempPrcBend/" + request["FrmEcode"].ToString();

            if (Directory.Exists(StrTempPath))
            {
                Directory.GetAccessControl(StrTempPath);
            }
            else
            {
                Directory.CreateDirectory(StrTempPath);
                Directory.GetAccessControl(StrTempPath);
            }

            if (request["FileUploadType"].ToString().Trim() == "Delete")
            {
                filePath = StrTempPath + "/" + request["fileUpload"].ToString().Trim();
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return " file Deleted";
            }
            else
            {
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                var FrmEcode = request["FrmEcode"].ToString();
                //  var fileUpload = request["fileUpload"].ToString();
                // CHECK THE FILE COUNT.
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    if (hpf.ContentLength > 0)
                    {
                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                        if (!File.Exists(StrTempPath + Path.GetFileName(hpf.FileName)))
                        {
                            // SAVE THE FILES IN THE FOLDER.
                            // hpf.SaveAs(StrTempPath + "/" +  "1@@" + Path.GetFileName(hpf.FileName) );
                            hpf.SaveAs(StrTempPath + "/" + Path.GetFileName(hpf.FileName));
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                    }
                }
                // RETURN A MESSAGE.
                if (iUploadedCnt > 0)
                {
                    return iUploadedCnt + " Files Uploaded Successfully";
                }
                else
                {
                    return "Upload Failed";
                }
            }
        }

        [HttpPost()]
        [Route("FabPrc/UploadFiles")]
        public string UploadFilesFab()
        {
            string StrTempPath = "", filePath = "";
            int iUploadedCnt = 0;

            var request = System.Web.HttpContext.Current.Request;
            StrTempPath = "C:/TempERPFile/TempPrcFab/" + request["FrmEcode"].ToString();

            if (Directory.Exists(StrTempPath))
            {
                Directory.GetAccessControl(StrTempPath);
            }
            else
            {
                Directory.CreateDirectory(StrTempPath);
                Directory.GetAccessControl(StrTempPath);
            }

            if (request["FileUploadType"].ToString().Trim() == "Delete")
            {
                filePath = StrTempPath + "/" + request["fileUpload"].ToString().Trim();
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return " file Deleted";
            }
            else
            {
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                var FrmEcode = request["FrmEcode"].ToString();
                //  var fileUpload = request["fileUpload"].ToString();
                // CHECK THE FILE COUNT.
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    if (hpf.ContentLength > 0)
                    {
                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                        if (!File.Exists(StrTempPath + Path.GetFileName(hpf.FileName)))
                        {
                            // SAVE THE FILES IN THE FOLDER.
                            // hpf.SaveAs(StrTempPath + "/" +  "1@@" + Path.GetFileName(hpf.FileName) );
                            hpf.SaveAs(StrTempPath + "/" + Path.GetFileName(hpf.FileName));
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                    }
                }
                // RETURN A MESSAGE.
                if (iUploadedCnt > 0)
                {
                    return iUploadedCnt + " Files Uploaded Successfully";
                }
                else
                {
                    return "Upload Failed";
                }
            }
        }

        [HttpPost()]
        [Route("PCPrc/UploadFiles")]
        public string UploadFilesPC()
        {
            string StrTempPath = "", filePath = "";
            int iUploadedCnt = 0;

            var request = System.Web.HttpContext.Current.Request;
            StrTempPath = "C:/TempERPFile/TempPrcPC/" + request["FrmEcode"].ToString();

            if (Directory.Exists(StrTempPath))
            {
                Directory.GetAccessControl(StrTempPath);
            }
            else
            {
                Directory.CreateDirectory(StrTempPath);
                Directory.GetAccessControl(StrTempPath);
            }

            if (request["FileUploadType"].ToString().Trim() == "Delete")
            {
                filePath = StrTempPath + "/" + request["fileUpload"].ToString().Trim();
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return " file Deleted";
            }
            else
            {
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                var FrmEcode = request["FrmEcode"].ToString();
                //  var fileUpload = request["fileUpload"].ToString();
                // CHECK THE FILE COUNT.
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    if (hpf.ContentLength > 0)
                    {
                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                        if (!File.Exists(StrTempPath + Path.GetFileName(hpf.FileName)))
                        {
                            // SAVE THE FILES IN THE FOLDER.
                            // hpf.SaveAs(StrTempPath + "/" +  "1@@" + Path.GetFileName(hpf.FileName) );
                            hpf.SaveAs(StrTempPath + "/" + Path.GetFileName(hpf.FileName));
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                    }
                }
                // RETURN A MESSAGE.
                if (iUploadedCnt > 0)
                {
                    return iUploadedCnt + " Files Uploaded Successfully";
                }
                else
                {
                    return "Upload Failed";
                }
            }
        }

        [HttpPost()]
        [Route("CpyPrc/UploadFiles")]
        public string UploadFilesCpy()
        {
            string StrTempPath = "", filePath = "";
            int iUploadedCnt = 0;

            var request = System.Web.HttpContext.Current.Request;
            StrTempPath = "C:/TempERPFile/TempPrcCpy/" + request["FrmEcode"].ToString();

            if (Directory.Exists(StrTempPath))
            {
                Directory.GetAccessControl(StrTempPath);
            }
            else
            {
                Directory.CreateDirectory(StrTempPath);
                Directory.GetAccessControl(StrTempPath);
            }

            if (request["FileUploadType"].ToString().Trim() == "Delete")
            {
                filePath = StrTempPath + "/" + request["fileUpload"].ToString().Trim();
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return " file Deleted";
            }
            else
            {
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                var FrmEcode = request["FrmEcode"].ToString();
                //  var fileUpload = request["fileUpload"].ToString();
                // CHECK THE FILE COUNT.
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    if (hpf.ContentLength > 0)
                    {
                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                        if (!File.Exists(StrTempPath + Path.GetFileName(hpf.FileName)))
                        {
                            // SAVE THE FILES IN THE FOLDER.
                            // hpf.SaveAs(StrTempPath + "/" +  "1@@" + Path.GetFileName(hpf.FileName) );
                            hpf.SaveAs(StrTempPath + "/" + Path.GetFileName(hpf.FileName));
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                    }
                }
                // RETURN A MESSAGE.
                if (iUploadedCnt > 0)
                {
                    return iUploadedCnt + " Files Uploaded Successfully";
                }
                else
                {
                    return "Upload Failed";
                }
            }
        }

        //Reverse Trans
        [HttpGet]
        [Route("CpyPrcRev/getRevPCCode")]
        public DataTable getRevPCCode(string StrTransType)
        {
            return dc.getRevPCCode(StrTransType);
        }

        [HttpGet]
        [Route("CpyPrcRev/LoadPrcDts")]
        public DataTable LoadRevPrcDts(string PCCode)
        {
            return dc.LoadRevPrcDts(PCCode);
        }

        [HttpPost]
        [Route("CpyPrcRev/SubmitRevCpyTrans")]
        public string SubmitRevCpyTrans(CpyRevRequest CpyRevReq)
        {
            return dc.SubmitRevCpyTrans(CpyRevReq);
        }

        //Reverse Trans

    }
}



//using KalaERPApi.Models.Production.Canopy.Trans;
//using KalaERPApi.Service;
//using KalaERPApi.Service.Production.Canopy.Trans;
//using System.Data;
//using System.IO;
//using System.Web.Http;

//namespace KalaERPApi.Controllers.Production.Canopy.Trans
//{
//    public class CpyPrcController : ApiController
//    {      
//        CpyPrcCon dc = new CpyPrcCon();
//        CommonCon ComCon = new CommonCon();

//        [HttpGet]
//        [Route("CpyPrc/getCpyPrcddl")]
//        public DataTable GetCpyPrcKVA(string PCCode,string MachineCode, string KVA, string Model, string PlanCode, string CatID)
//        {
//            return dc.getCpyPrcddl(PCCode, MachineCode, KVA,Model, PlanCode,CatID);
//        }

//        [HttpGet]
//        [Route("CpyPrc/getCpyPrc6M")]
//        public DataTable getCpyPrc6M()
//        {
//            return ComCon.Get6M();
//        }

//        [HttpGet]
//        [Route("CpyPrc/getSheetPartDts")]
//        public DataTable GetSheetPartDts(string PCCode, int SheetSrNo, string MachineCode, string SheetPartcode,string PlanCode, string Partcode, string CatID)
//        {
//            return dc.GetSheetPartDts(PCCode,SheetSrNo, MachineCode, SheetPartcode, PlanCode, Partcode,CatID );
//        }

//        [HttpGet]
//        [Route("CpyPrc/getCpyPrcddlFab")]
//        public DataTable GetCpyPrcKVAFab(string PCCode, string MachineCode, string KVA, string Model, string SuppCode)
//        {
//            return dc.getCpyPrcddlFab(PCCode, MachineCode, KVA, Model, SuppCode);
//        } 

//        [HttpGet]
//        [Route("CpyPrc/GetCpyKitFab")]
//        public DataTable GetCpyKitFab(string PCCode, string MachineCode, string PlanCode, string PartCode, string CpyKit, string SuppCode)
//        {
//            return dc.GetCpyKitFab(PCCode, MachineCode, PlanCode, PartCode, CpyKit, SuppCode);
//        }

//        [HttpGet]
//        [Route("CpyPrc/getSheetPartDtsPartCut")]
//        public DataTable GetSheetPartDtsPartCut(int SheetSrNo, string MachineCode, string SheetPartcode, string PlanCode, string Partcode)
//        {
//            return dc.GetSheetPartDtsPartCut(SheetSrNo, MachineCode, SheetPartcode, PlanCode, Partcode);
//        }

//        [HttpGet]
//        [Route("CpyPrc/GetTKitDts")]
//        public DataTable GetTKitDts(string PCCode, string TKitID,int BatchQty, string TrnsType, string Plancode, string ProdCode)

//        {
//            return dc.GetTKitDts( PCCode, TKitID, BatchQty, TrnsType,Plancode,ProdCode);
//        }

//        [HttpGet]
//        [Route("CpyPrc/GetTKitDtsPartCut")]
//        public DataTable GetTKitDtsPartCut(string TKitID, int BatchQty, string TrnsType, string Plancode, string ProdCode)
//        {
//            return dc.GetTKitDtsPartCut(TKitID, BatchQty, TrnsType, Plancode, ProdCode);
//        }

//        [HttpGet]
//        [Route("CpyPrc/GetCpyKit")]
//        public DataTable GetCpyKit(string PCCode, string MachineCode, string PlanCode, string PartCode, string CpyKit)
//        {
//            return dc.GetCpyKit(PCCode, MachineCode, PlanCode, PartCode, CpyKit);
//        }

//        [HttpGet]
//        [Route("CpyPrc/GetCpyKitPC")]
//        public DataTable GetCpyKitPC(string PCCode, string MachineCode, string PlanCode, string PartCode, string CpyKit)
//        {
//            return dc.GetCpyKitPC(PCCode, MachineCode, PlanCode, PartCode, CpyKit);
//        }

//        [HttpGet]
//        [Route("CpyPrc/CpyKitDtsCPYAssly")]
//        public DataTable CpyKitDtsCPYAssly( string PCCode,int PrcQty, string CpyPartCode, string PlanCode, string BOMCode, string PFBCode)
//        {
//            return dc.CpyKitDtsCPYAssly( PCCode, PrcQty, CpyPartCode,PlanCode, BOMCode, PFBCode);
//        }


//        [HttpGet]
//        [Route("CpyPrc/CpyKitDtsCPYAsslyKit")]
//        public DataTable CpyKitDtsCPYAsslyKit(string PCCode, int PrcQty, string CpyPartCode, string PlanCode, string BOMCode, string PFBCode)
//        {
//            return dc.CpyKitDtsCPYAsslyKit(PCCode, PrcQty, CpyPartCode, PlanCode, BOMCode, PFBCode);
//        }



//        [HttpGet]
//        [Route("CpyPrc/GetCpyKitDts")]
//        public DataTable GetCpyKitDts(string PCCode, int BatchQty, string CpyKitcode, string BOMCode, string PFBCode)
//        {
//            return dc.CpyKitDts(PCCode, BatchQty, CpyKitcode, BOMCode, PFBCode);
//        }

//        [HttpGet]
//        [Route("CpyPrc/GetCpyEndPrcDts")]
//        public DataTable GetCpyEndPrcDts(string PCCode, string PlanCode, string PartCode)
//        {
//            return dc.CpyEndPrcDts(PCCode, PlanCode, PartCode);
//        }

//        [HttpGet]
//        [Route("CpyPrc/LoadProduct")]
//        public DataTable LoadProduct(string PCCode)
//        {
//            return dc.LoadProduct(PCCode);
//        }

//        [HttpGet]
//        [Route("CpyPrc/LoadMachine")]
//        public DataTable LoadMachine(string PCCode)
//        {
//            return dc.LoadMachine(PCCode);
//        }

//        [HttpGet]
//        [Route("CpyPrc/LoadOSSupplier")]
//        public DataTable LoadOSSupplier()
//        {
//            return dc.LoadOSSupplier();
//        }


//        [HttpGet]
//        [Route("CpyPrc/LoadCatID")]
//        public DataTable LoadCatID(string PCCode , string PlanCode)
//        {
//            return dc.LoadCatID(PCCode, PlanCode);
//        }

//        [HttpPost]
//        [Route("CpyPrc/CncSubmit")]
//        public string SubmitCNC(CpyPrcCNCRequest CpyPrcCNCReq)
//        {
//            return dc.SubmitCNC(CpyPrcCNCReq);
//        }

//        [HttpPost]
//        [Route("CpyPrc/PartcutSubmit")]
//        public string SubmitPartCut(CpyPrcPartcutRequest cpyPrcPartcut)
//        {
//            return dc.SubmitPartCut(cpyPrcPartcut);
//        }

//        [HttpPost]
//        [Route("CpyPrc/BendingSubmit")]
//        public string SubmitBending(CpyPrcBendRequest cpyPrcBendReq)
//        {
//            return dc.SubmitBending(cpyPrcBendReq);
//        }

//        [HttpPost]
//        [Route("CpyPrc/FabricationSubmit")]
//        public string SubmitFabrication(CpyPrcFabRequest cpyPrcFabReq)
//        {
//            return dc.SubmitFabrication(cpyPrcFabReq);
//        }

//        [HttpPost]
//        [Route("CpyPrc/PowderCoatingSubmit")]
//        public string SubmitPowderCoating(CpyPrcPCRequest cpyPrcPCReq)
//        {
//            return dc.SubmitPowderCoating(cpyPrcPCReq);
//        }

//        [HttpPost]
//        [Route("CpyPrc/SubmitCanopyAssly")]
//        public string SubmitCanopyAssly(CpyPrcRequest CpyPrcReq)
//        {
//            return dc.SubmitCanopyAssly(CpyPrcReq);
//        }

//        [HttpPost()]
//        [Route("CNCPrc/UploadFiles")]
//        public string UploadFilesCNC()
//        {
//            string StrTempPath = "", filePath = "";
//            int iUploadedCnt = 0;

//            var request = System.Web.HttpContext.Current.Request;
//            StrTempPath = "C:/TempERPFile/TempPrcCNC/" + request["FrmEcode"].ToString();

//            if (Directory.Exists(StrTempPath))
//            {
//                Directory.GetAccessControl(StrTempPath);
//            }
//            else
//            {
//                Directory.CreateDirectory(StrTempPath);
//                Directory.GetAccessControl(StrTempPath);
//            }

//            if (request["FileUploadType"].ToString().Trim() == "Delete")
//            {
//                filePath = StrTempPath + "/" + request["fileUpload"].ToString().Trim();
//                if (File.Exists(filePath))
//                {
//                    File.Delete(filePath);
//                }
//                return " file Deleted";
//            }
//            else
//            {
//                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
//                var FrmEcode = request["FrmEcode"].ToString();
//                //  var fileUpload = request["fileUpload"].ToString();
//                // CHECK THE FILE COUNT.
//                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
//                {
//                    System.Web.HttpPostedFile hpf = hfc[iCnt];
//                    if (hpf.ContentLength > 0)
//                    {
//                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
//                        if (!File.Exists(StrTempPath + Path.GetFileName(hpf.FileName)))
//                        {
//                            // SAVE THE FILES IN THE FOLDER.
//                            // hpf.SaveAs(StrTempPath + "/" +  "1@@" + Path.GetFileName(hpf.FileName) );
//                            hpf.SaveAs(StrTempPath + "/" + Path.GetFileName(hpf.FileName));
//                            iUploadedCnt = iUploadedCnt + 1;
//                        }
//                    }
//                }
//                // RETURN A MESSAGE.
//                if (iUploadedCnt > 0)
//                {
//                    return iUploadedCnt + " Files Uploaded Successfully";
//                }
//                else
//                {
//                    return "Upload Failed";
//                }
//            }
//        }

//        [HttpPost()]
//        [Route("PCutPrc/UploadFiles")]
//        public string UploadFilesPartCut()
//        {
//            string StrTempPath = "", filePath = "";
//            int iUploadedCnt = 0;

//            var request = System.Web.HttpContext.Current.Request;
//            StrTempPath = "C:/TempERPFile/TempPrcPartCut/" + request["FrmEcode"].ToString();

//            if (Directory.Exists(StrTempPath))
//            {
//                Directory.GetAccessControl(StrTempPath);
//            }
//            else
//            {
//                Directory.CreateDirectory(StrTempPath);
//                Directory.GetAccessControl(StrTempPath);
//            }

//            if (request["FileUploadType"].ToString().Trim() == "Delete")
//            {
//                filePath = StrTempPath + "/" + request["fileUpload"].ToString().Trim();
//                if (File.Exists(filePath))
//                {
//                    File.Delete(filePath);
//                }
//                return " file Deleted";
//            }
//            else
//            {
//                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
//                var FrmEcode = request["FrmEcode"].ToString();
//                //  var fileUpload = request["fileUpload"].ToString();
//                // CHECK THE FILE COUNT.
//                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
//                {
//                    System.Web.HttpPostedFile hpf = hfc[iCnt];
//                    if (hpf.ContentLength > 0)
//                    {
//                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
//                        if (!File.Exists(StrTempPath + Path.GetFileName(hpf.FileName)))
//                        {
//                            // SAVE THE FILES IN THE FOLDER.
//                            // hpf.SaveAs(StrTempPath + "/" +  "1@@" + Path.GetFileName(hpf.FileName) );
//                            hpf.SaveAs(StrTempPath + "/" + Path.GetFileName(hpf.FileName));
//                            iUploadedCnt = iUploadedCnt + 1;
//                        }
//                    }
//                }
//                // RETURN A MESSAGE.
//                if (iUploadedCnt > 0)
//                {
//                    return iUploadedCnt + " Files Uploaded Successfully";
//                }
//                else
//                {
//                    return "Upload Failed";
//                }
//            }
//        }

//        [HttpPost()]
//        [Route("BendPrc/UploadFiles")]
//        public string UploadFilesBend()
//        {
//            string StrTempPath = "", filePath = "";
//            int iUploadedCnt = 0;

//            var request = System.Web.HttpContext.Current.Request;
//            StrTempPath = "C:/TempERPFile/TempPrcBend/" + request["FrmEcode"].ToString();

//            if (Directory.Exists(StrTempPath))
//            {
//                Directory.GetAccessControl(StrTempPath);
//            }
//            else
//            {
//                Directory.CreateDirectory(StrTempPath);
//                Directory.GetAccessControl(StrTempPath);
//            }

//            if (request["FileUploadType"].ToString().Trim() == "Delete")
//            {
//                filePath = StrTempPath + "/" + request["fileUpload"].ToString().Trim();
//                if (File.Exists(filePath))
//                {
//                    File.Delete(filePath);
//                }
//                return " file Deleted";
//            }
//            else
//            {
//                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
//                var FrmEcode = request["FrmEcode"].ToString();
//                //  var fileUpload = request["fileUpload"].ToString();
//                // CHECK THE FILE COUNT.
//                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
//                {
//                    System.Web.HttpPostedFile hpf = hfc[iCnt];
//                    if (hpf.ContentLength > 0)
//                    {
//                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
//                        if (!File.Exists(StrTempPath + Path.GetFileName(hpf.FileName)))
//                        {
//                            // SAVE THE FILES IN THE FOLDER.
//                            // hpf.SaveAs(StrTempPath + "/" +  "1@@" + Path.GetFileName(hpf.FileName) );
//                            hpf.SaveAs(StrTempPath + "/" + Path.GetFileName(hpf.FileName));
//                            iUploadedCnt = iUploadedCnt + 1;
//                        }
//                    }
//                }
//                // RETURN A MESSAGE.
//                if (iUploadedCnt > 0)
//                {
//                    return iUploadedCnt + " Files Uploaded Successfully";
//                }
//                else
//                {
//                    return "Upload Failed";
//                }
//            }
//        }

//        [HttpPost()]
//        [Route("FabPrc/UploadFiles")]
//        public string UploadFilesFab()
//        {
//            string StrTempPath = "", filePath = "";
//            int iUploadedCnt = 0;

//            var request = System.Web.HttpContext.Current.Request;
//            StrTempPath = "C:/TempERPFile/TempPrcFab/" + request["FrmEcode"].ToString();

//            if (Directory.Exists(StrTempPath))
//            {
//                Directory.GetAccessControl(StrTempPath);
//            }
//            else
//            {
//                Directory.CreateDirectory(StrTempPath);
//                Directory.GetAccessControl(StrTempPath);
//            }

//            if (request["FileUploadType"].ToString().Trim() == "Delete")
//            {
//                filePath = StrTempPath + "/" + request["fileUpload"].ToString().Trim();
//                if (File.Exists(filePath))
//                {
//                    File.Delete(filePath);
//                }
//                return " file Deleted";
//            }
//            else
//            {
//                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
//                var FrmEcode = request["FrmEcode"].ToString();
//                //  var fileUpload = request["fileUpload"].ToString();
//                // CHECK THE FILE COUNT.
//                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
//                {
//                    System.Web.HttpPostedFile hpf = hfc[iCnt];
//                    if (hpf.ContentLength > 0)
//                    {
//                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
//                        if (!File.Exists(StrTempPath + Path.GetFileName(hpf.FileName)))
//                        {
//                            // SAVE THE FILES IN THE FOLDER.
//                            // hpf.SaveAs(StrTempPath + "/" +  "1@@" + Path.GetFileName(hpf.FileName) );
//                            hpf.SaveAs(StrTempPath + "/" + Path.GetFileName(hpf.FileName));
//                            iUploadedCnt = iUploadedCnt + 1;
//                        }
//                    }
//                }
//                // RETURN A MESSAGE.
//                if (iUploadedCnt > 0)
//                {
//                    return iUploadedCnt + " Files Uploaded Successfully";
//                }
//                else
//                {
//                    return "Upload Failed";
//                }
//            }
//        }

//        [HttpPost()]
//        [Route("PCPrc/UploadFiles")]
//        public string UploadFilesPC()
//        {
//            string StrTempPath = "", filePath = "";
//            int iUploadedCnt = 0;

//            var request = System.Web.HttpContext.Current.Request;
//            StrTempPath = "C:/TempERPFile/TempPrcPC/" + request["FrmEcode"].ToString();

//            if (Directory.Exists(StrTempPath))
//            {
//                Directory.GetAccessControl(StrTempPath);
//            }
//            else
//            {
//                Directory.CreateDirectory(StrTempPath);
//                Directory.GetAccessControl(StrTempPath);
//            }

//            if (request["FileUploadType"].ToString().Trim() == "Delete")
//            {
//                filePath = StrTempPath + "/" + request["fileUpload"].ToString().Trim();
//                if (File.Exists(filePath))
//                {
//                    File.Delete(filePath);
//                }
//                return " file Deleted";
//            }
//            else
//            {
//                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
//                var FrmEcode = request["FrmEcode"].ToString();
//                //  var fileUpload = request["fileUpload"].ToString();
//                // CHECK THE FILE COUNT.
//                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
//                {
//                    System.Web.HttpPostedFile hpf = hfc[iCnt];
//                    if (hpf.ContentLength > 0)
//                    {
//                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
//                        if (!File.Exists(StrTempPath + Path.GetFileName(hpf.FileName)))
//                        {
//                            // SAVE THE FILES IN THE FOLDER.
//                            // hpf.SaveAs(StrTempPath + "/" +  "1@@" + Path.GetFileName(hpf.FileName) );
//                            hpf.SaveAs(StrTempPath + "/" + Path.GetFileName(hpf.FileName));
//                            iUploadedCnt = iUploadedCnt + 1;
//                        }
//                    }
//                }
//                // RETURN A MESSAGE.
//                if (iUploadedCnt > 0)
//                {
//                    return iUploadedCnt + " Files Uploaded Successfully";
//                }
//                else
//                {
//                    return "Upload Failed";
//                }
//            }
//        }

//        [HttpPost()]
//        [Route("CpyPrc/UploadFiles")]
//        public string UploadFilesCpy()
//        {
//            string StrTempPath = "", filePath = "";
//            int iUploadedCnt = 0;

//            var request = System.Web.HttpContext.Current.Request;
//            StrTempPath = "C:/TempERPFile/TempPrcCpy/" + request["FrmEcode"].ToString();

//            if (Directory.Exists(StrTempPath))
//            {
//                Directory.GetAccessControl(StrTempPath);
//            }
//            else
//            {
//                Directory.CreateDirectory(StrTempPath);
//                Directory.GetAccessControl(StrTempPath);
//            }

//            if (request["FileUploadType"].ToString().Trim() == "Delete")
//            {
//                filePath = StrTempPath + "/" + request["fileUpload"].ToString().Trim();
//                if (File.Exists(filePath))
//                {
//                    File.Delete(filePath);
//                }
//                return " file Deleted";
//            }
//            else
//            {
//                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
//                var FrmEcode = request["FrmEcode"].ToString();
//                //  var fileUpload = request["fileUpload"].ToString();
//                // CHECK THE FILE COUNT.
//                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
//                {
//                    System.Web.HttpPostedFile hpf = hfc[iCnt];
//                    if (hpf.ContentLength > 0)
//                    {
//                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
//                        if (!File.Exists(StrTempPath + Path.GetFileName(hpf.FileName)))
//                        {
//                            // SAVE THE FILES IN THE FOLDER.
//                            // hpf.SaveAs(StrTempPath + "/" +  "1@@" + Path.GetFileName(hpf.FileName) );
//                            hpf.SaveAs(StrTempPath + "/" + Path.GetFileName(hpf.FileName));
//                            iUploadedCnt = iUploadedCnt + 1;
//                        }
//                    }
//                }
//                // RETURN A MESSAGE.
//                if (iUploadedCnt > 0)
//                {
//                    return iUploadedCnt + " Files Uploaded Successfully";
//                }
//                else
//                {
//                    return "Upload Failed";
//                }
//            }
//        }

//        //Reverse Trans
//        [HttpGet]
//        [Route("CpyPrcRev/getRevPCCode")]
//        public DataTable getRevPCCode(string StrTransType)
//        {
//            return dc.getRevPCCode(StrTransType);
//        }

//        [HttpGet]
//        [Route("CpyPrcRev/LoadPrcDts")]
//        public DataTable LoadRevPrcDts(string PCCode)
//        {
//            return dc.LoadRevPrcDts(PCCode);
//        }

//        [HttpPost]
//        [Route("CpyPrcRev/SubmitRevCpyTrans")]
//        public string SubmitRevCpyTrans(CpyRevRequest CpyRevReq)
//        {
//            return dc.SubmitRevCpyTrans(CpyRevReq);
//        }

//        //Reverse Trans

//    }
//}
