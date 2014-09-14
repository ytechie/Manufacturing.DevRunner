using System;
using System.Reactive.Linq;
using System.Reflection;
using Bootstrap.Extensions.StartupTasks;
using Microsoft.FluentMessaging;
using Manufacturing.Framework.Datasource;
using Manufacturing.Framework.Dto;
using Microsoft.Practices.ServiceLocation;
using WorkerRoleWithSBQueue1.Configuration;
using Manufacturing.Framework.Repository.Interface;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Manufacturing.Framework.Model;
using System.Data.Entity;
using System.Data.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure;
using log4net;
using System.Text;

namespace Manufacturing.DevRunner.Services
{
    class DeviceDataBlobWriter : IStartupTask
    {

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private readonly IBlobRepository _repository;
        private readonly IDatasourceRecordSerializer _serializer;

        private readonly CloudConfiguration _config;

        private readonly string _connectionString;

        private CloudStorageAccount _storageAccount;


        public DeviceDataBlobWriter(CloudConfiguration configuration, IDatasourceRecordSerializer serializer)
        {
            _config = configuration;
            //_repository = repository;
            _serializer = serializer;

            _connectionString = _config.DataStorageConfiguration.GetConnectionString();

            // Retrieve storage account from connection string.
            _storageAccount = CloudStorageAccount.Parse(_connectionString);
            
            // Create the blob client.
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();

            
        }

        public void Run()
        {

            var rx = QueueFramework
                .FromTopicSubscription(
                _config.ReceiveQueue.GetConnectionString(), _config.ReceiveQueue.QueueName, "blobwriter")
                    .WithPrefetchCount(1000)
                .WithMaxConcurrency(1)
                
                .OutputToReactive(_serializer as DatasourceRecordSerializer);

            Log.DebugFormat("DeviceDataBlobWriter Starting");

            //TODO: What if we die while filling the buffer?
            rx.Timestamp().Buffer(1000).Do(message => Log.Debug("writing message")).Subscribe(messages =>
                {
                    //write to blob storeage
                    
                    // Create file name
                    var firstTimeStamp = messages[0].Timestamp;
                    var filename = firstTimeStamp.Year + "\\" + String.Format("{0:MM}", firstTimeStamp) 
                        + "\\" + String.Format("{0:dd}", firstTimeStamp)
                        + "\\" + String.Format("{0:HH}", firstTimeStamp)
                        + "\\" + String.Format("{0:mm}", firstTimeStamp) 
                        + "-" + String.Format("{0:ss}", firstTimeStamp) 
                        + "-" + String.Format("{0:ffff}", firstTimeStamp);


                    CloudBlockBlob blockBlob = new CloudBlockBlob(new Uri(_storageAccount.BlobEndpoint + 
                        _config.DataStorageConfiguration.RawDataContainer + "/" + filename), _storageAccount.Credentials);

                    blockBlob.Properties.ContentType = "text/plain";
                    blockBlob.Properties.ContentEncoding = "utf8";

                     using (MemoryStream ms = new MemoryStream())
                        {
                            byte[] dataToWrite;

                            //Header
                            dataToWrite = Encoding.UTF8.GetBytes("Rx Timestamp, Datasource ID, DataType, EncodedDataType, IntervalSeconds, Source Timestamp, Value\r\n");
                            ms.Write(dataToWrite, 0, dataToWrite.Length);

                            //Body
                            foreach (var m in messages)
                            {

                                dataToWrite = Encoding.UTF8.GetBytes(String.Format("{0:yyyy/MM/dd HH:mm:ss.ffff}", m.Timestamp) + "," + m.Value.DatasourceId + "," + m.Value.DataType + "," + m.Value.EncodedDataType + "," + m.Value.IntervalSeconds + "," + String.Format("{0:yyyy/MM/dd HH:mm:ss.ffff}", m.Value.Timestamp) + "," + m.Value.GetDecimalValue() + "\r\n");
                                ms.Write(dataToWrite, 0, dataToWrite.Length);
                            }
                            ms.Position = 0;
                            blockBlob.UploadFromStream(ms);
                        }

                });
        }

        public void Reset()
        {
        }
    }
}
