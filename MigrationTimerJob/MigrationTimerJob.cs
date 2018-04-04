using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;
using System.Diagnostics;
using System.Configuration;

namespace MigrationTimerJob
{
    public class MigrationTimerJob :SPJobDefinition
    {
        string BaseURL = string.Empty;
        string strUrl = string.Empty;
       public MigrationTimerJob():base()  
{
        }

        public MigrationTimerJob(string jobName, SPService service) : base(jobName, service, null, SPJobLockType.None)  
{
            this.Title = "Migration Timer Job";
        }


        public MigrationTimerJob(string jobName, SPWebApplication webapp) : base(jobName, webapp, null, SPJobLockType.ContentDatabase)  
{
            this.Title = "Migration Timer Job";
        }

        public override void Execute(Guid contentDbId)
        {

            try
            {

                string listName = string.Empty;


                strUrl = "http://putty08/pm";   // System.Configuration.ConfigurationManager.AppSettings["PMURL"].ToString();
                BaseURL = "http://putty08/";   // System.Configuration.ConfigurationManager.AppSettings["BaseURL"].ToString();
                Main();



            }

            catch (Exception ex)
            {

                LogError("Execute Method Mongo Migration Job" + ex.Message.ToString(), "error");

            }




        }
        protected bool Main()
        {
            bool nResult = true;
            DataClasses.Document document =new DataClasses.Document();
            string dtIn = DateTime.Now.ToString("yyyy-MM-dd");



            using (SPSite site = new SPSite(strUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPList list = web.Lists["PolicyHolders"];
                    SPQuery query = new SPQuery();
                    query.RowLimit = 50;
                    SPListItemCollection items = list.GetItems(query);
                 
                    foreach (SPListItem item in items)
                    {


                        SPAttachmentCollection attachments = item.Attachments;
                        foreach (string fileName in attachments)
                        {
                            SPFile file = web.GetFile(attachments.UrlPrefix + fileName);
                            byte[] bArray = file.OpenBinary();

                            document.BucketName = "pdfs";
                            document.CommentID = "";
                            document.Comments = "";
                            document.Content_Bytes = bArray;
                            document.Content_Ext = "pdf";
                            document.Content_Size = 0;
                            document.Content_title = item["Title"].ToString();
                            document.Content_Type = "";
                            document.DocumentId = item.ID.ToString();
                            document.DocumentTitle = "";
                            document.DownloadFilePath = fileName;
                            document.FileId = "";
                            document.Mode = "initial";
                            document.Mongo_FileId1 = "";
                            document.Mongo_FileId2 = "";
                            document.Mongo_FileId3 = "";
                            document.Mongo_FileId4 = "";
                            document.Mongo_FileId5 = "";
                            document.Mongo_Meta = "";
                            document.NumFiles = 1;
                            document.PHID = item["PolicyHolder_ID"].ToString();
                            document.PMID = "";
                            document.SharePoint_FileId1 = item["PolicyHolder_SocialSecurityNumbe"].ToString();
                            document.SharePoint_FileId2 = item["PolicyHolder_LastName"].ToString();
                            document.SharePoint_FileId3 = item["PolicyHolder_FirstName"].ToString();
                            document.SharePoint_FileId4 = item["PolicyHolder_ID"].ToString();
                            document.SharePoint_FileId5 = "";
                            document.SharePoint_Meta = "";
                            document.UploadFilePath = "migration";
                            document.Content_Date = Convert.ToDateTime(dtIn);
                            document.DocumentDate= Convert.ToDateTime(dtIn);
                            document.Import_Date= Convert.ToDateTime(dtIn);
                            Handlers.DocumentHandler handler = new Handlers.DocumentHandler(document);
                            ClearDocumentProperties(document);
                            

                            //document.BucketName = "pdfs";
                            //document.FileExt = "pdf";
                            //document.NumFiles = 1;
                            //document.UploadFilePath = fileName;
                            //document.Content = bArray;
                        }
                    }

                }
            }

            return nResult;

        }
        private DataClasses.Document ClearDocumentProperties(DataClasses.Document document)
        {
            document.BucketName = "";
            document.CommentID = "";
            document.Comments = "";
            document.Content_Bytes = null;
            document.Content_Ext = "";
            document.Content_Size = 0;
            document.Content_title = "";
            document.Content_Type = "";
            document.DocumentId = "";
            document.DocumentTitle = "";
            document.DownloadFilePath = "";
            document.FileId = "";
            document.Mode = "";
            document.Mongo_FileId1 = "";
            document.Mongo_FileId2 = "";
            document.Mongo_FileId3 = "";
            document.Mongo_FileId4 = "";
            document.Mongo_FileId5 = "";
            document.Mongo_Meta = "";
            document.NumFiles = 0;
            document.PHID = "";
            document.PMID = "";
            document.SharePoint_FileId1 = "";
            document.SharePoint_FileId2 = "";
            document.SharePoint_FileId3 = "";
            document.SharePoint_FileId4 = "";
            document.SharePoint_FileId5 = "";
            document.SharePoint_Meta = "";
            document.UploadFilePath = "";




            return document;

        }
        public void LogError(string ErrorMessage, string type)
        {

            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                if (!EventLog.SourceExists("Pierce Mongo Migration Job"))
                {
                    EventLog.CreateEventSource("Pierce Mongo Migration Job", "Pierce Mongo Migration Job");
                }
                if (type == "error")
                {
                    EventLog.WriteEntry("Pierce Mongo Migration Job",
                                                ErrorMessage,
                                                EventLogEntryType.Information);
                }

                if (type == "error")
                {
                    EventLog.WriteEntry("Pierce Mongo Migration Job",
                                               ErrorMessage,
                                               EventLogEntryType.Error);
                }

                SPDiagnosticsService diagSvc = SPDiagnosticsService.Local;
                diagSvc.WriteTrace(0,
                    new SPDiagnosticsCategory("Pierce Mongo Migration Job",
                        TraceSeverity.Monitorable,
                        EventSeverity.Error),
                    TraceSeverity.Monitorable,
                    "An exception occurred: {0}",
                    new object[] { ErrorMessage });
            });

        }
    }
}
