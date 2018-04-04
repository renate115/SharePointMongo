using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;
using System.Diagnostics;
using System.Configuration;

namespace MigrationTimerJob.Handlers
{
    public class DocumentHandler
    {

        

        public DocumentHandler(DataClasses.Document document)
        {
            if(document.Mode =="initial")
            {
                Import_Staging(document);
            }
        }

        private void Import_Staging(DataClasses.Document document)
        {
            PuttyMigrateXSDTableAdapters.StagingTableAdapter da = new PuttyMigrateXSDTableAdapters.StagingTableAdapter();
            PuttyMigrateXSD.StagingDataTable dt = new PuttyMigrateXSD.StagingDataTable();
            try
            {
                da.Insert_StagingTable(document.SharePoint_FileId1, document.SharePoint_FileId2, document.SharePoint_FileId3, document.SharePoint_FileId4, document.SharePoint_FileId5,
                    document.Mongo_FileId1, document.Mongo_FileId2, document.Mongo_FileId3, document.Mongo_FileId4, document.Mongo_FileId5, document.SharePoint_Meta, document.Mongo_Meta,
                    document.Content_Type, document.Content_Ext, document.Content_Bytes, document.Content_Size, document.UploadFilePath, document.DownloadFilePath, document.Content_title,
                    document.Content_Date, document.Import_Date);
            }
            catch (Exception ex)
            {
                //add to a new logging table 
                LogError("Import Staging:  " + ex.Message.ToString(), "error");
               

            }

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
