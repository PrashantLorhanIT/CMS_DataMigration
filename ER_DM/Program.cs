using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace ER_DM
{
    class Program
    {
        static void Main(string[] args)
        {
            //string temp = Console.ReadLine();
            //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(temp);
            //var x=  System.Convert.ToBase64String(plainTextBytes);

            //var base64EncodedBytes = System.Convert.FromBase64String(x);
            //var  y= System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

            string CoreAPIBaseURL = ConfigurationManager.AppSettings["CoreAPIBaseURL"];
            //Console.WriteLine("Enter accessToken");
            string accessToken = "Bearer " + GetToken(CoreAPIBaseURL);
            // Console.ReadLine();


            Console.WriteLine("Enter Filename");
            string Filename = Console.ReadLine();
            int type = 0;
            do
            {
                Console.WriteLine("Options");
                Console.WriteLine("1: Outgoing");
                Console.WriteLine("2: Incoming");

                type = Convert.ToInt32(Console.ReadLine());
                if (!(type == 1 || type == 2))
                {
                    Console.WriteLine("Invalid input");
                }
            } while (!(type == 1 || type == 2));
            Corr corrObj = new Corr(type);
            corrObj.EntityLists = GetEntityLists(CoreAPIBaseURL, accessToken);
            corrObj.Usermasters = GetUsermasters(CoreAPIBaseURL, accessToken);
            corrObj.Contracts = GetContracts(CoreAPIBaseURL, accessToken);
            corrObj.CoreAPIBaseURL = CoreAPIBaseURL;
            corrObj.accessToken = accessToken;
            corrObj.processFile(Filename);
            Console.ReadLine();
        }

        static List<Usermaster> GetUsermasters(string CoreAPIBaseURL, string accessToken)
        {
            List<Usermaster> entityLists = null;

            using (HttpClient client = new HttpClient())
            {
                Uri uri = new Uri(CoreAPIBaseURL + "/Usermaster/GetUsermasters");

                client.DefaultRequestHeaders.Authorization = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(accessToken);
                var GetResult = client.GetAsync(uri).GetAwaiter().GetResult();
                if (GetResult.IsSuccessStatusCode)
                {
                    ApiLoginResponse apiResponse = JsonConvert.DeserializeObject<ApiLoginResponse>(GetResult.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                    return (List<Usermaster>)apiResponse.Data;
                }


            }
            return entityLists;
        }
        static List<EntityList> GetEntityLists(string CoreAPIBaseURL, string accessToken)
        {
            List<EntityList> entityLists = null;

            using (HttpClient client = new HttpClient())
            {
                Uri uri = new Uri(CoreAPIBaseURL + "/Entity/GetExternalEntities");

                client.DefaultRequestHeaders.Authorization = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(accessToken);
                var GetResult = client.GetAsync(uri).GetAwaiter().GetResult();
                if (GetResult.IsSuccessStatusCode)
                {
                    ApiEntityResponse apiResponse = JsonConvert.DeserializeObject<ApiEntityResponse>(GetResult.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                    return (List<EntityList>)apiResponse.Data;
                }


            }
            return entityLists;
        }
        static List<Contract> GetContracts(string CoreAPIBaseURL, string accessToken)
        {
            List<Contract> entityLists = null;

            using (HttpClient client = new HttpClient())
            {
                Uri uri = new Uri(CoreAPIBaseURL + "/Contract/GetContracts");

                client.DefaultRequestHeaders.Authorization = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(accessToken);
                var GetResult = client.GetAsync(uri).GetAwaiter().GetResult();
                if (GetResult.IsSuccessStatusCode)
                {
                    ApiContractResponse apiResponse = JsonConvert.DeserializeObject<ApiContractResponse>(GetResult.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                    return (List<Contract>)apiResponse.Data;
                }


            }
            return entityLists;
        }
        static string GetToken(string CoreAPIBaseURL)
        {
            string token = "";
            Console.WriteLine("Enter Credentials");
            Console.Write("Username : ");
            string Username = Console.ReadLine();
            Console.Write("Password : ");
            string Password = Console.ReadLine();
            using (HttpClient client = new HttpClient())
            {
                Uri uri = new Uri(CoreAPIBaseURL + "/Usermaster/AuthenticateUser");
                var obj = new { Username = Username, Password = Password };
                // client.DefaultRequestHeaders.Authorization = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(accessToken);
                var GetResult = client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
                if (GetResult.IsSuccessStatusCode)
                {
                    ApiLoginResponse apiResponse = JsonConvert.DeserializeObject<ApiLoginResponse>(GetResult.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                    //var Data = JsonConvert.DeserializeObject<dynamic>(apiResponse.Data.ToString());
                    //return Data.First.token.Value;
                    return apiResponse.Data.First().Token;
                }


            }
            return token;
        }
    }



    class ApiCorrResponse
    {
        public InCorrAddResult Data { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }

    }
    public class InCorrAddResult
    {
        public decimal RidInCorr { get; set; }
        public decimal RidWorkflow { get; set; }
        public decimal RIdWorkflowTransactionInitiator { get; set; }

    }

    class Corr
    {
        // 1 OUtgoing, 2 Incoming
        int corrtype = 0;
        public Corr(int type)
        {
            corrtype = type;
            ObjectWithCorrID = new DataTable();
            ObjectWithCorrID.Columns.Add("RowId", typeof(string));
            ObjectWithCorrID.Columns.Add("ObjectId", typeof(string));
            ObjectWithCorrID.Columns.Add("RidCorr", typeof(string));
            ObjectWithCorrID.Columns.Add("CreateCorrJSONMessage", typeof(string));
            ObjectWithCorrID.Columns.Add("AttachmentIDs", typeof(string));
            ObjectWithCorrID.Columns.Add("CorrDetailIDs", typeof(string));
            ObjectWithCorrID.Columns.Add("ErrorMessage", typeof(string));
            //ObjectWithCorrID.RowChanged += ObjectWithCorrID_RowChanged;
        }


        public List<EntityList> EntityLists { get; set; }
        public List<Usermaster> Usermasters { get; set; }
        public List<Contract> Contracts { get; set; }
        public DataTable ObjectWithCorrID { get; set; }


        private List<Task> Tasks { get; set; }
        private string TempFileName = "";
        private StreamWriter sw;
        public string CoreAPIBaseURL { get; set; }
        public string accessToken { get; set; }

        public void processFile(string Filename)
        {
            //string[] Headers = "r_object_id||object_name/Correspondence Reference Number||Subject||Corr_Category||eif_project_ref||eif_type_of_doc||From_Code||eif_from||To_Code||eif_to||ecs_lc_state||eif_originator||folder_name||File Path||Rendition/Attachments||Rendition object ID||Confidential-YES/NO||Permissions/Confidentiality ||Relationship/Link||Created Date||Contract Number||Creator Name".Split("||");
            StreamReader sr = new StreamReader(Filename);
            TempFileName = Filename;
            sw = new StreamWriter(TempFileName.Replace(".txt", "_result.txt"), true);
            Tasks = new List<Task>();
            string[] Headers = sr.ReadLine().Split("||");
            int RecordCount = 1;
            int counter = 0;
            while (!sr.EndOfStream)
            {
                try
                {


                    string RowData = sr.ReadLine();
                    string[] Fields = RowData.Split("||");
                    CorrModel corrModel = new CorrModel();
                    List<Attachment> attachments = new List<Attachment>();
                    List<Corrdetail> corrdetails = new List<Corrdetail>();
                    for (int i = 0; i < Headers.Length; i++)
                    {
                        try
                        {
                            if (Headers[i].Trim() == "object_name/Correspondence Reference Number")
                            {
                                if (corrtype == 2)
                                {
                                    corrModel.SendersReferencenumber = Fields[i];
                                    corrModel.Referencenumber = "E0000-ERL-JBS-CL-XXXXX";
                                }
                                else
                                {
                                    //corrModel.SendersReferencenumber = "";
                                    corrModel.Referencenumber = Fields[i];
                                }
                            }

                            if (Headers[i].Trim() == "Subject")
                            {
                                corrModel.Subject = Fields[i];
                            }

                            if (Headers[i].Trim() == "Corr_Category")
                            {
                                corrModel.RidCorrCategory = corrtype; // incoming correspondence
                            }
                            if (Headers[i].Trim() == "eif_type_of_doc")
                            {
                                //CL-1
                                //CC-2
                                //CM-3

                                if (Fields[i].Trim().Contains("CL-"))
                                {
                                    corrModel.RidDocumenttype = 1;
                                }
                                else if (Fields[i].Trim().Contains("CC-"))
                                {
                                    corrModel.RidDocumenttype = 2;
                                }
                                else if (Fields[i].Trim().Contains("CM-"))
                                {
                                    corrModel.RidDocumenttype = 3;
                                }
                                else
                                {
                                    //ignore this record
                                    continue;
                                }

                            }
                            if (Headers[i].Trim() == "From_Code")
                            {

                                corrModel.SenderRidEntityList = EntityLists.Where(x => x.EntityCode == Fields[i]).First().RidEntityList;
                            }
                            if (Headers[i].Trim() == "To_Code")
                            {

                                corrModel.RecipientRidEntityList = EntityLists.Where(x => x.EntityCode == Fields[i]).First().RidEntityList;
                            }
                            if (Headers[i].Trim() == "ecs_lc_state")
                            {

                                corrModel.Status = "MIGRATED";
                            }
                            if (Headers[i].Trim() == "Created Date")
                            {

                                corrModel.CorrespondenceDate = DateTime.Parse(Fields[i]);
                            }
                            if (Headers[i].Trim() == "Confidential-YES/NO")
                            {
                                if (Fields[i].Trim().Contains("Y"))
                                {
                                    string[] users = Fields[i + 1].Split(",");
                                    foreach (var item in users)
                                    {
                                        try
                                        {
                                            Usermaster usermaster = Usermasters.Where(x => (x.Username) == item.Replace(" ", "")).FirstOrDefault();
                                            if (usermaster != null)
                                            {
                                                corrdetails.Add(new Corrdetail() { Duedate = null, Include = "Y", Isactive = "Y", Ismandatory = "N", RidCommunicationtype = 1, RidCommunicationDetail = null, SignatureRequired = "N", RidWorkflowstep = (corrtype == 2 ? 2 : 4), RidUsermaster = usermaster.RidUsermaster });

                                            }

                                        }
                                        catch (Exception)
                                        {

                                            Console.WriteLine("Error at Record Index(" + RecordCount + ") at Column Index while finding Condidential user in Userlist:" + item);
                                            throw;
                                        }
                                    }

                                }
                                else
                                {

                                }

                            }
                            if (Headers[i].Trim() == "Contract Number")
                            {
                                string[] temparr = Fields[i].Split("-");
                                Decimal tempContract = Contracts.Where(x => x.Contractcode == temparr[0].Trim()).First().RidContract;
                                corrModel.RidContractlist = Convert.ToDecimal(tempContract);
                            }
                            if (Headers[i].Trim() == "Creator Name")
                            {
                                //corrModel.RidUsermaster = 81;

                                Usermaster usermaster = Usermasters.Where(x => (x.Username) == Fields[i].Trim()).FirstOrDefault();
                                if (usermaster != null)
                                {
                                    corrModel.RidUsermaster = usermaster.RidUsermaster;

                                }
                                else
                                {
                                    corrModel.RidUsermaster = Usermasters.Where(x => (x.Username) == "svc.cmsadmintest").FirstOrDefault().RidUsermaster;
                                }
                                corrdetails.Add(new Corrdetail() { Duedate = null, Include = "Y", Isactive = "Y", Ismandatory = "N", RidCommunicationtype = 1, RidCommunicationDetail = null, SignatureRequired = "N", RidWorkflowstep = 1, RidUsermaster = corrModel.RidUsermaster });

                            }
                            if (Headers[i].Trim() == "r_object_id")
                            {
                                if (Fields[i].Trim().Length > 0)
                                {
                                    attachments.Add(new Attachment() { ObjectID = Fields[i], Isactive = "Y" });

                                }
                            }
                            if (Headers[i].Trim() == "Rendition object ID")
                            {
                                if (Fields[i].Trim().Length > 0)
                                {
                                    attachments.Add(new Attachment() { ObjectID = Fields[i], Isactive = "Y" });

                                }
                            }


                        }
                        catch (Exception)
                        {
                            string logMessage = "Error at Record Index(" + RecordCount + ") at Column Index" + i;
                            Console.WriteLine(logMessage);
                            DataRow dataRowError = ObjectWithCorrID.NewRow();
                            dataRowError.SetField("RowId", RecordCount);
                            dataRowError.SetField("ObjectId", Fields[0]);
                            //dataRowError.SetField("RidCorr", typeof(decimal));
                            // dataRowError.SetField("CreateCorrJSONMessage", typeof(string));
                            //dataRowError.SetField("AttachmentIDs", typeof(string));
                            //dataRowError.SetField("CorrDetailIDs", typeof(string));
                            dataRowError.SetField("ErrorMessage", logMessage);


                            ObjectWithCorrID.Rows.Add(dataRowError);

                            RecordCount++;
                            throw;
                        }


                    }


                    corrModel.Isactive = "Y";
                    corrModel.Isconfidential = "N";
                    corrModel.Isreplyrequired = "N";
                    corrModel.ApprovalsRequired = "N";
                    if (corrtype == 2)
                    {
                        corrModel.RidCommunicationType = 2;
                    }
                    else
                    {
                        corrModel.RidCommunicationType = 1;
                    }
                   
                    corrModel.RidGroupType = 1;
                    corrModel.RelationRidCorr = new int[0];

                    Task temp = null;
                    try
                    {
                        temp = Task.Run(() => MigrateRecord(RecordCount++, Fields[0], corrModel, attachments, corrdetails, corrtype));

                        Tasks.Add(temp);
                        counter++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    try
                    {
                        if (counter == 5)
                        {
                            Task.WaitAll(Tasks.ToArray());
                            Tasks.Clear();
                            counter = 0;
                            foreach (DataRow e in ObjectWithCorrID.Rows)
                            {
                                try
                                {
                                    sw.WriteLine(e[0] + "||" + e[1] + "||" + e[2] + "||" + e[3] + "||" + e[4] + "||" + e[5] + "||" + e[6]);
                                    sw.Flush();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);

                                }
                            }
                            ObjectWithCorrID.Rows.Clear();

                        }

                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                        Tasks.Clear();
                        counter = 0;
                        foreach (DataRow e in ObjectWithCorrID.Rows)
                        {
                            try
                            {
                                sw.WriteLine(e[0] + "||" + e[1] + "||" + e[2] + "||" + e[3] + "||" + e[4] + "||" + e[5] + "||" + e[6]);
                                sw.Flush();
                            }
                            catch (Exception ex2)
                            {
                                Console.WriteLine(ex2.Message);

                            }

                        }
                        ObjectWithCorrID.Rows.Clear();
                    }
                    finally
                    {

                    }
                }
                catch (Exception)
                {

                    Console.WriteLine("Skipping record");
                }
            }
            sr.Close();


            sw.Close();

        }

        private async Task MigrateRecord(int RecordCount, string ObjectId, CorrModel corrModel, List<Attachment> attachments, List<Corrdetail> corrdetails, int corrtype)
        {
            Console.WriteLine("SrNo." + (RecordCount++) + " - " + JsonConvert.SerializeObject(corrModel) + Environment.NewLine + Environment.NewLine);
            decimal result = 0;

            if (corrtype == 2)
            {
                result = await CreateInCorr(corrModel);
            }
            if (corrtype == 1)
            {
                result = await CreateOutCorr(corrModel);
            }

            DataRow dataRow = ObjectWithCorrID.NewRow();
            dataRow.SetField("RowId", RecordCount);
            dataRow.SetField("ObjectId", ObjectId);
            dataRow.SetField("RidCorr", result);
            dataRow.SetField("CreateCorrJSONMessage", JsonConvert.SerializeObject(corrModel));
            //dataRow.SetField("AttachmentIDs", typeof(string));
            // dataRow.SetField("CorrDetailIDs", typeof(string));
            dataRow.SetField("ErrorMessage", "Corr Created Successfully");

            if (result > 0 && dataRow != null)
            {

                string attachIds = "";
                int AttachSeq = 0;
                foreach (var item in attachments)
                {
                    item.RidCorr = (decimal)result;
                    item.Attachedfilename = "Attachment" + AttachSeq + ".pdf";
                    item.Attachsequence = AttachSeq++;
                    decimal attachId = await CreateAttachment(item);
                    if (attachId > 0)
                    {
                        attachIds += attachId.ToString() + ",";
                    }
                }
                dataRow.SetField("AttachmentIDs", attachIds);
                string CorrDetailIDs = "";
                foreach (var item in corrdetails)
                {
                    item.RidCorr = (decimal)result;
                    decimal CorrDetailId = await CreateCorrDetail(item);
                    if (CorrDetailId > 0)
                    {
                        CorrDetailIDs += CorrDetailId.ToString() + ",";
                    }
                }
                dataRow.SetField("CorrDetailIDs", CorrDetailIDs);
            }
            ObjectWithCorrID.Rows.Add(dataRow);
        }

        private async Task<decimal> CreateOutCorr(CorrModel corrModel)
        {
            decimal RidCorr = 0;

            using (HttpClient client = new HttpClient())
            {
                Uri uri = new Uri(CoreAPIBaseURL + "/Corr/CreateDMOutcorr");

                client.DefaultRequestHeaders.Authorization = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(accessToken);
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(corrModel), Encoding.UTF8, "application/json");
                var GetResult = (await client.PostAsync(uri, stringContent));
                if (GetResult.IsSuccessStatusCode)
                {
                    ApiCorrResponse apiResponse = JsonConvert.DeserializeObject<ApiCorrResponse>(GetResult.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                    return apiResponse.Data.RidInCorr;
                }


            }
            return RidCorr;
        }
        private async Task<decimal> CreateInCorr(CorrModel corrModel)
        {
            decimal RidCorr = 0;

            using (HttpClient client = new HttpClient())
            {
                Uri uri = new Uri(CoreAPIBaseURL + "/Corr/CreateDMIncorr");

                client.DefaultRequestHeaders.Authorization = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(accessToken);
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(corrModel), Encoding.UTF8, "application/json");
                var GetResult = (await client.PostAsync(uri, stringContent));
                if (GetResult.IsSuccessStatusCode)
                {
                    ApiCorrResponse apiResponse = JsonConvert.DeserializeObject<ApiCorrResponse>(GetResult.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                    return apiResponse.Data.RidInCorr;
                }


            }
            return RidCorr;
        }
        private async Task<decimal> CreateAttachment(Attachment attachment)
        {
            decimal RidAttachment = 0;

            using (HttpClient client = new HttpClient())
            {
                Uri uri = new Uri(CoreAPIBaseURL + "/Attachment/Create");

                client.DefaultRequestHeaders.Authorization = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(accessToken);
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(attachment), Encoding.UTF8, "application/json");
                var GetResult = await client.PostAsync(uri, stringContent);
                if (GetResult.IsSuccessStatusCode)
                {
                    ApiAttachmentResponse apiResponse = JsonConvert.DeserializeObject<ApiAttachmentResponse>(GetResult.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                    return apiResponse.Data;
                }


            }
            return RidAttachment;
        }

        private async Task<decimal> CreateCorrDetail(Corrdetail corrdetail)
        {
            decimal RidAttachment = 0;

            using (HttpClient client = new HttpClient())
            {
                Uri uri = new Uri(CoreAPIBaseURL + "/Corr/AddCorrDetail");

                client.DefaultRequestHeaders.Authorization = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(accessToken);
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(corrdetail), Encoding.UTF8, "application/json");
                var GetResult = await client.PostAsync(uri, stringContent);
                if (GetResult.IsSuccessStatusCode)
                {
                    ApiCorrdetailResponse apiResponse = JsonConvert.DeserializeObject<ApiCorrdetailResponse>(GetResult.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                    return apiResponse.Data;
                }


            }
            return RidAttachment;
        }
    }


}
