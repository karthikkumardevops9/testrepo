using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

//this class will call all fusion micro services in the future Moti Mashiah. in development ...
namespace MSRecordsEngine.Services
{
    public class Microservices
    {
        private readonly HttpClient _httpclient;
        public Microservices(IConfiguration config, HttpClient httpClient)
        {
            DocumentServices = new DocumentService(config, httpClient);
            _httpclient = httpClient;
        }
        public DocumentService DocumentServices { get; }
        public async Task<string> APIPOST(string url, object obj)
        {
            var content = JsonConvert.SerializeObject(obj);
            var reqContent = new StringContent(content, Encoding.UTF8, "application/json");
            var call = _httpclient.PostAsync($"{url}", reqContent);
            return await call.Result.Content.ReadAsStringAsync();
        }

    }
    //document viewer
    public class DocumentService
    {
        private static string hostService { get; set; }
        private readonly HttpClient _httpclient;
        public DocumentService(IConfiguration config, HttpClient httpClient)
        {
            hostService = config.GetSection("Microservice").GetSection("DocumentViewer").Value;
            _httpclient = httpClient;
        }
        public string SaveTempFileToCacheURL = $"{hostService}/Fusion/SaveTempFileTocache";
        private string GetStreamFlyOutFirstPageURL = $"Fusion/GetStreamFlyOutFirstPage";
        private string GetCodecInfoFromFileURL = $"Fusion/GetCodecInfoFromFile";
        private string GetCodecInfoFromFileListURL = $"Fusion/GetCodecInfoFromFileList";
        private string GetCacheLocationURL = $"Fusion/GetCacheLocation";
        private string SaveTempPDFFileToDisk = $"Fusion/SaveTempPDFFileToDisk";

        public async Task<string> SaveTempFileTocache(string url, object obj)
        {
            var content = JsonConvert.SerializeObject(obj);
            var reqContent = new StringContent(content, Encoding.UTF8, "application/json");
            var call = _httpclient.PostAsync($"{url}", reqContent);
            return await call.Result.Content.ReadAsStringAsync();
        }
        //new
        public async Task<string> SaveTempPDFfileToDisk(string filename, List<string> stringpath, string serverpath)
        {
            var model = new DocumentViewrApiModel();
            model.stringPath = stringpath;
            model.fileName = filename;
            model.serverPath = serverpath;
            var content = JsonConvert.SerializeObject(model);
            var reqContent = new StringContent(content, Encoding.UTF8, "application/json");
            var url = $"{hostService}/{this.SaveTempPDFFileToDisk}";
            var call = _httpclient.PostAsync(url, reqContent);
            return await call.Result.Content.ReadAsStringAsync();
        }
        public async Task<FileStreamResult> APIGETStreamFlyOutFirstPage(string filePath, string fullPath, bool validAttachment)
        {
            var url = $"{hostService}/{this.GetStreamFlyOutFirstPageURL}?filePath={filePath}&fullPath={fullPath}&validAttachment={validAttachment}";
            var call = _httpclient.GetAsync(url);
            var str = await call.Result.Content.ReadAsStreamAsync();

            return new FileStreamResult(str, "image/jpg");
        }
        //not in use. 
        public async Task<DocumentViewrApiModel> GetCodecInfoFromFile(string fileName, string extension)
        {
            var url = $"{hostService}/{this.GetCodecInfoFromFileURL}?fileName={fileName}&extension={extension}";
            var call = _httpclient.GetAsync(url);
            string stringResult = await call.Result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DocumentViewrApiModel>(stringResult);
        }
        public async Task<List<DocumentViewrApiModel>> GetCodecInfoFromFileList(List<string> Filepathlist)
        {
            var content = JsonConvert.SerializeObject(Filepathlist);
            var reqContent = new StringContent(content, Encoding.UTF8, "application/json");
            var url = $"{hostService}/{this.GetCodecInfoFromFileListURL}";
            var call = _httpclient.PostAsync(url, reqContent);
            string stringResult = await call.Result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DocumentViewrApiModel>>(stringResult);
        }
        public async Task<string> GetCacheLocation()
        {
            var call = _httpclient.GetAsync($"{hostService}/{this.GetCacheLocationURL}");
            return await call.Result.Content.ReadAsStringAsync();
        }
        public async Task<string> PingDocumentService()
        {
            var call = _httpclient.GetAsync($"{hostService}/api/test/ping");
            return await call.Result.Content.ReadAsStringAsync();
        }
    }
    public class DocumentViewrApiModel
    {
        public int TotalPages { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public long SizeDisk { get; set; }
        public string? FilePath { get; set; }
        public List<string> stringPath { get; set; }
        public string fileName { get; set; }
        public string serverPath { get; set; }
        public bool Ispcfile { get; set; }
    }
}