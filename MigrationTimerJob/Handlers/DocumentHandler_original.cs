using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using MongoDB.Driver.Wrappers;
using MongoDB.Driver.Linq;
using System.Diagnostics;


namespace Utilities.Handlers
{
    public class DocumentHandler
    {
        DbContext db;
        public string ErrorMessage { get; set; }
        IMongoDatabase mongoDb;
        public DocumentHandler()
        {
            db = new DbContext();
        }

        public List<DataClasses.Document> GetDocuments()
        {
            return db.database.GetCollection<DataClasses.Document>("document").FindAll().ToList();
        }


        //public DataTable GetDocmentListByPolicyHolder(string phid)
        //{
        //    PierceMainTableAdapters.MongoDocumentLinkTableAdapter da = new PierceMainTableAdapters.MongoDocumentLinkTableAdapter();
        //    PierceMain.MongoDocumentLinkDataTable dt = new PierceMain.MongoDocumentLinkDataTable();
        //    try
        //    {
        //        da.FillBy(dt, phid);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessage = ex.Message.ToString();
        //    }

        //    return dt;
        //}

        public string GetOneBucketDocumentById(string fileid)
        {
            SetupMongoDb();
            string newid = string.Empty;
            try
            {
                
                ObjectId obj = new ObjectId(fileid);
                //var ObjectId = new database.ob('59c2b7df8dff1e3488c16c97');
                var query = Query.EQ("_id", obj);
                //var query = Query.EQ("_id",);
                // db.document.findOne(query);
                //var dbCollection = db.database.GetCollection("pdfs.files");
                //var entity = dbCollection.Find(query);
                ////var intcount = entity.Count();
                //if (entity !=null)
                //{
                //    Debug.WriteLine(entity.ToString());

                //}

                //var fileids = db.database.GetCollection("pdfs.files").Find(query);


                var bucket = new GridFSBucket(mongoDb, new GridFSBucketOptions { BucketName = "pdfs" });

                var fileBytes = bucket.DownloadAsBytes(obj);
                var ctr = fileBytes.Count();
               // Debug.WriteLine(fileBytes);
                string splitstring = fileBytes.ToString();
                
                Array arrSplit = splitstring.Split(',');
                ctr = arrSplit.Length;
                //string unformatted = arrSplit.GetValue(0).ToString();
                //Array arrsplit2 = unformatted.Split(':');
                //string unformatted2 = arrsplit2.GetValue(1).ToString();
                //int len = unformatted2.Length;
                //int stoplen = len - 13;
                //newid = unformatted2.Substring(11, stoplen);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
            }



            return newid;


        }
        public string GetOneDocumentById(string fileid)
        {

            string newid = string.Empty;
            try
            {
                //ObjectId obj = new ObjectId("59c2b7df8dff1e3488c16c97");
                //var ObjectId = db.database.ObjectId('59c2b7df8dff1e3488c16c97');
                //var query = Query.EQ("_id", obj;
                var query = Query.EQ("FileId", fileid);
                // db.document.findOne(query);
                var dbCollection = db.database.GetCollection("document");
                var entity = dbCollection.FindOne(query);

                entity.ToString();
                string splitstring = entity.ToString();
                Array arrSplit = splitstring.Split(',');
                string unformatted = arrSplit.GetValue(0).ToString();
                Array arrsplit2 = unformatted.Split(':');
                string unformatted2 = arrsplit2.GetValue(1).ToString();
                int len = unformatted2.Length;
                int stoplen = len - 13;
                newid = unformatted2.Substring(11, stoplen);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
            }


          
            return newid;


        }
        public DataClasses.Document GetOneDocumentDetailByFileId(DataClasses.Document document)
        {
            string fileContents = string.Empty;
            ObjectId obj = new ObjectId(document.FileId);
            document.Entity = "";

            string location = string.Empty;
            try
            {
                location = System.Configuration.ConfigurationManager.AppSettings["DownloadLocation"].ToString();
            }
            catch
            {
                location = @"C:\Temp\";
            }
            var query = Query.EQ("_id", obj);
            var file = db.database.GridFS.FindOne(query);
            string name = file.Name;
            var newFileName = name;
            try
            {
                
                Array arrSplit = name.Split('\\');
                int intlen = arrSplit.Length;
                string filename = arrSplit.GetValue(intlen - 1).ToString();
                newFileName = location + filename;
                document.DocumentTitle = "Your file is located here: " + newFileName;
               
            }
            catch(Exception ex)
            {
                document.DocumentTitle = "Unable to handle the file name - section 001";
                document.Entity = string.Empty;
                ErrorMessage = ex.Message.ToString(); 
            }
           
            
           
           
            try
            {
                using (var stream = file.OpenRead())
                {

                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);

                    using (var newFs = new FileStream(newFileName, FileMode.Create))
                    {
                        newFs.Write(bytes, 0, bytes.Length);

                    }
                }

                document.nResult = true;
                document.Entity = newFileName;


            }
            catch (Exception ex)
            {

                document.nResult = false;
                document.DocumentTitle = "Unable to download this document (original file name and location: " + name;
                document.Entity = string.Empty;
                ErrorMessage = ex.Message.ToString();
            }

            //using (var fs = file.OpenRead())
            //{
            //    var bytes = new byte[fs.Length];
            //    fs.Read(bytes, 0, (int)fs.Length);

            //    var stream = new MemoryStream(bytes);
            //    // var res = Encoding.UTF8.GetString(stream.GetBuffer(), 0, stream.GetBuffer().Length);
            //    StreamReader reader = new StreamReader(stream);
            //    string text = reader.ReadToEnd();
            //    fileContents = text;
            //}
           
          
            return document;
       

        }
        public void SaveRetrieve()
        {
            //var id = "ObjectId(59c2b7df8dff1e3488c16c97)";
            ObjectId obj = new ObjectId("59c2b7df8dff1e3488c16c97");
            ////var ObjectId = db.database.ObjectId('59c2b7df8dff1e3488c16c97');
            var query = Query.EQ("_id", obj);
            //// db.document.findOne(query);
            var dbCollection = db.database.GetCollection("document");
          
            var entity = dbCollection.FindOne(query);

            //return entity.ToString();
            var localFilename = @"C:\temp\test99.txt";
            var remoteFilename = "test99";

            var gridFS = db.database.GridFS;
           var gridFSS = dbCollection.Database.GridFS;
            string name = gridFS.DatabaseName;
            var fileInfo = gridFS.Upload(localFilename, remoteFilename);
          
            //gridFS.Download(localFilename, remoteFilename);
            //gridFSS.Download(localFilename, query);
            //gridFSS.Download(localFilename, "BDTemp.txt");

        }
        //public bool InsertMongoDBLink(DataClasses.Document document)
        //{
        //    bool nResult = true;
        //    string dtIn = DateTime.Now.ToString("yyyy-MM-dd");


        //    DateTime dtDate = Convert.ToDateTime(dtIn);
        //    PierceMainTableAdapters.MongoDocumentLinkTableAdapter da = new PierceMainTableAdapters.MongoDocumentLinkTableAdapter();

        //    try
        //    {
        //        da.InsertMongoDocumentLink(document.PHID, document.CommentID, document.FileId, document.DocumentId, document.DocumentTitle, document.UploadFilePath, App.GlobalVars.username, document.DocumentDate,document.DocumentType,document.FileId);
               


        //    }
        //    catch (Exception ex)
        //    {

        //        nResult = false;
        //        if (ex.Message.Contains("UNIQUE"))
        //        {

                  
        //        }
        //        else
        //        {
        //            ErrorMessage = ex.Message.ToString();
        //        }

        //    }


        //    return nResult;


        //}
        public void ClearCollections()
        {
            //var collection = db.database.GetCollection("document");
            //collection.RemoveAll();
            //var collectionString = mongoDb.GetCollection("stringBasedPics");

            // collectionBin.RemoveAll();
            // collectionString.DeleteMany(new BsonDocument());
        }
        private  void SetupMongoDb()
        {
            string hostName = "localhost";
            int portNumber = 27017;
            string databaseName = "PiercePolicyManager";

            var clientSettings = new MongoClientSettings()
            {
                Server = new MongoServerAddress(hostName, portNumber),
                MinConnectionPoolSize = 1,
                MaxConnectionPoolSize = 1500,
                ConnectTimeout = new TimeSpan(0, 0, 30),
                SocketTimeout = new TimeSpan(0, 1, 30),
                WaitQueueTimeout = new TimeSpan(0, 1, 0)
            };

            mongoDb = new MongoClient(clientSettings).GetDatabase(databaseName);
        }
        //private  void RetrieveFromGridBucketFS()
        //{
        //    SetupMongoDb();
        //    var bucket = new GridFSBucket(mongoDb, new GridFSBucketOptions
        //    {
        //        BucketName = "pdfs",



        //    });
        //    var filesIds = mongoDb.GetCollection<DataClasses.Document>("pictures.files").Find(new BsonDocument()).ToEnumerable().Select(doc => doc.GetElement("_id").Value);

        //    foreach (var id in filesIds)
        //    {
        //        var fileBytes = bucket.DownloadAsBytes(id);
        //        fileBytes = null;
        //    }
        //}
        public DataClasses.Document SaveFilesToGridFSBinary( int numFiles, byte[] content, string fileName, DataClasses.Document document)
        {
            SetupMongoDb();
            fileName = document.UploadFilePath;
            string MongoFileId = string.Empty;

            var bucket = new GridFSBucket(mongoDb, new GridFSBucketOptions
            {
                BucketName = document.BucketName
                

            
            });

            for (int i = 0; i < numFiles; ++i)
            {
                string targetFileName = $"{fileName.Substring(0, fileName.Length - ".pdf".Length)}{i}.pdf";
                int chunkSize = content.Length <= 1048576 ? 51200 : 1048576;
               var id= bucket.UploadFromBytes(targetFileName, content, new GridFSUploadOptions { ChunkSizeBytes = chunkSize });
                document.FileId = "";
                document.DocumentId = id.ToString();
                //InsertMongoDBLink(document);

            }

            return document;
        }

        public bool SaveBinary(byte[] content,int numDocs, DataClasses.Document document)
        {

            bool nResult = true;
            var collection = db.database.GetCollection("document");
            BsonDocument baseDoc = new BsonDocument();
            baseDoc.SetElement(new BsonElement("pdfContent", content));
            for (int i = 0; i < numDocs; ++i)
            {
                var guid = Guid.NewGuid();
                baseDoc.SetElement(new BsonElement("_id", guid));
                baseDoc.SetElement(new BsonElement("filename", document.UploadFilePath));
                baseDoc.SetElement(new BsonElement("title", document.DocumentTitle));
                collection.Insert(baseDoc);
                document.FileId = guid.ToString();
                document.DocumentId = GetOneDocumentById(guid.ToString());
               // InsertMongoDBLink(document);

                
            }


            return nResult;
        }
        //public bool Save(DataClasses.Document document, bool nComments)
        //{
        //    bool nContinue = true;
        //    document.CommentID = "";
        //    //get document extension
        //    string documentfilepath = string.Empty;
        //    string extension = string.Empty;
        //    bool nKnownType = false;
        //    documentfilepath = document.UploadFilePath.ToString();
           
        //    try
        //    {
        //        Array arrSplit = documentfilepath.Split('.');
        //        int count = arrSplit.Length;
        //        if (count > 0)
        //        {
        //            extension = arrSplit.GetValue(count - 1).ToString().ToLower();
        //        }

        //        if (extension == "pdf")
        //        {
        //            document.DocumentType = "PDF";
        //            nKnownType = true;

        //        }
        //        if (extension == "doc" || extension == "docx" || extension == "doc")
        //        {
        //            document.DocumentType = "WORD";
        //            nKnownType = true;

        //        }
        //        if (extension == "txt")
        //        {
        //            document.DocumentType = "TEXT";
        //            nKnownType = true;

        //        }

        //        if (extension == "xl" || extension == "xls")
        //        {
        //            document.DocumentType = "EXCEL";
        //            nKnownType = true;

        //        }

        //        if (!nKnownType)
        //        {
        //            document.DocumentType = "UNKNOWN";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessage = "Error on Split: " + ex.Message.ToString();
        //    }
            


        //    DataClasses.Communication communication = new DataClasses.Communication();
        //    try
        //    {
        //        if (nComments)
        //        {

        //            //get a comment ID for later saving
        //            MainDataLogic mdl = new PiercePolicyManager.MainDataLogic();
        //            int nextid = mdl.GetAppIDAndUpdate("communication");
        //            communication.CommentID = "PHC" + nextid;
        //            communication.CommType = "Document";

        //            document.CommentID = communication.CommentID;
        //        }

        //        //Save the document meta data 
             
        //        using (var fs = new FileStream(document.UploadFilePath, FileMode.Open))
        //        {
        //            try
        //            {
        //                var gridFsInfo = db.database.GridFS.Upload(fs, document.UploadFilePath);
        //                var fileId = gridFsInfo.Id;
        //                document.FileId = fileId.ToString();

        //                db.database.GetCollection<DataClasses.Document>("document").Insert(document);

        //                document.DocumentId = GetOneDocumentById(fileId.ToString());

        //                //create the mongo DB link 

        //                InsertMongoDBLink(document);
        //            }
        //            catch (Exception ex)
        //            {
        //                nContinue = false;
        //                ErrorMessage = ErrorMessage + " Error on Save: " + ex.Message.ToString();

        //            }


        //        }
        //        if (nContinue && nComments)
        //        {

        //            //create the comment record 

        //            communication.Comments = document.Comments;
        //            communication.DocumentId = document.DocumentId;
        //            communication.PHID = App.GlobalVars.PHID;
        //            communication.DocumentTitle = document.DocumentTitle;
        //            communication.CreatedBy = App.GlobalVars.username;
        //            communication.CommType = "Document";
        //            communication.FileId = document.FileId;
        //            CommunicationHandler comm = new CommunicationHandler();
        //            comm.InsertCommunication(communication);


        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessage = "Save: Communications/File" + ex.Message.ToString();
        //    }
        //    return true;
        //}
}
}
