using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;



namespace MigrationTimerJob.DataClasses
{
    public class Document
    {
        public Document() {
         
        }

       
        public string DocumentTitle { get; set;}
        public string UploadFilePath { get; set; }
        public string DownloadFilePath { get; set; }
        public string DocumentId { get; set; }
        public string Comments { get; set; }
        public string FileId { get; set; }
        public string PMID { get; set; }
        public string PHID { get; set; }
        public string CommentID { get; set; }
  
        public bool nResult { get; set; }
       
        public DateTime DocumentDate { get; set; }
       
        public int NumFiles { get; set; }
        public string BucketName { get; set; }
       
        public string SharePoint_FileId1 { get; set; }
        public string SharePoint_FileId2 { get; set; }
        public string SharePoint_FileId3 { get; set; }
        public string SharePoint_FileId4 { get; set; }
        public string SharePoint_FileId5 { get; set; }

        public string Mongo_FileId1 { get; set; }
        public string Mongo_FileId2 { get; set; }
        public string Mongo_FileId3 { get; set; }
        public string Mongo_FileId4 { get; set; }
        public string Mongo_FileId5 { get; set; }

        public string SharePoint_Meta { get; set; }
        public string Mongo_Meta { get; set; }

        public string Content_Type { get; set; }
        public string Content_Ext { get; set; }
        public string Content_title { get; set; }
        public byte[] Content_Bytes { get; set; }
        public int Content_Size { get; set; }
        public DateTime Content_Date { get; set; }
        public DateTime Import_Date { get; set; }
        public string Mode { get; set; }



    }
}
