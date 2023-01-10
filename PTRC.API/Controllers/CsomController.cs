using PTRC.API.Attributes;
using System.Web.Http;
using PTRC.API.Service;
using System;
using PTRC.API.Models;
using System.Web.Http.Description;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using System.Linq;
using System.Net;
using System.Configuration;
using System.Web;
using System.Web.Http.Cors;

namespace PTRC.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    [ErrorLogAttribute]
    [Route("api/[controller]")]
   
    public class CsomController : ApiController
    {
        private readonly ILoggerManager _loggermanager;
        private readonly string _username ;
        private readonly string _password ;
        private readonly string _domain ;
        private readonly string _baseurl ;
        private readonly string _rootdoclibid;

        public CsomController(ILoggerManager loggermanager)
        {
            _loggermanager = loggermanager;
            _username = ConfigurationManager.AppSettings["CSOMUser"].ToString();
            _password= ConfigurationManager.AppSettings["CSOMPassword"].ToString();
            _domain = ConfigurationManager.AppSettings["CSOMDomain"].ToString();
            _baseurl= ConfigurationManager.AppSettings["CSOMUrl"].ToString();
            _rootdoclibid= ConfigurationManager.AppSettings["RootDocLibId"].ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderpath"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResponse))]
        [Route("GetDocuments")]
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetDocuments(string folderpath=null)
        {
            folderpath = "/PowertrainDocuments/Product_Selector_POC";
            var response = new ApiResponse();
            try
            {
                List<DocumentInfo> items = new List<DocumentInfo>();
                using (var ctx = new ClientContext(_baseurl))
                {
                    ctx.ExecutingWebRequest += clientContext_ExecutingWebRequest;
                    ctx.Credentials = new NetworkCredential(_username, _password, _domain);
                    ctx.Load(ctx.Web, a => a.Lists);
                    List list = ctx.Web.Lists.GetById(new Guid(_rootdoclibid));
                    ctx.ExecuteQuery();


                    IterateListItems(folderpath, ctx, list, items);
                    response.DocumentInfo = items;
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _loggermanager.LogError(ex.StackTrace);
                response.ErrorOccurred = true;
                response.IsValid = false;
                return Ok(response);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderpath"></param>
        /// <param name="file"></param>
        /// <param name="fileproperty"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResponse))]
        [Route("PostDocuments")]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult PostDocuments(string folderpath)
        {
            folderpath = @"/PowertrainDocuments/Product_Selector_POC";
            var response = new ApiResponse();
            var v = System.Web.HttpContext.Current.Request.Files;
            var cc = System.Web.HttpContext.Current.Request.InputStream;
            try
            {
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count != 1)
                {
                    response.ErrorOccurred = false;
                    response.IsValid = false;
                    return Ok(response);
                }
                else
                {
                    using (ClientContext CContext = new ClientContext(_baseurl))
                    {
                        CContext.ExecutingWebRequest += clientContext_ExecutingWebRequest;
                        CContext.Credentials = new NetworkCredential(_username, _password, _domain); ;
                        Web web = CContext.Web;
                        FileCreationInformation newFile = new FileCreationInformation();
                        newFile.ContentStream = httpRequest.Files[0].InputStream;
                        newFile.Url = httpRequest.Files[0].FileName;
                        newFile.Overwrite = true;
                        

                        List DocumentLibrary = web.Lists.GetById(new Guid(_rootdoclibid));
                        Folder Clientfolder = DocumentLibrary.RootFolder.Folders.GetByUrl(folderpath);
                        Microsoft.SharePoint.Client.File uploadFile = Clientfolder.Files.Add(newFile); 
                        CContext.Load(DocumentLibrary);
                        CContext.Load(uploadFile);
                        CContext.ExecuteQuery();                        
                        
                    }
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                _loggermanager.LogError(ex.StackTrace);
                response.ErrorOccurred = true;
                response.IsValid = false;
                return Ok(response);
            }
        }
        /// <summary>
        /// Enable FORM Based Authentication  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clientContext_ExecutingWebRequest(object sender, WebRequestEventArgs e)
        {
            e.WebRequestExecutor.WebRequest.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
        }

        private  void IterateListItems(string requestedFolder, ClientContext ctx, List list, List<DocumentInfo> documentInfos)
        {
            List<ListItem> items = new List<ListItem>();


            ListItemCollectionPosition position = null;
            int rowLimit = 100;
            var camlQuery = new CamlQuery();
            camlQuery.ViewXml = @"<View><RowLimit>" + rowLimit + @"</RowLimit></View>";
            camlQuery.FolderServerRelativeUrl = requestedFolder;

            do
            {
                ListItemCollection listItems = null;
                camlQuery.ListItemCollectionPosition = position;
                listItems = list.GetItems(camlQuery);
                ctx.Load(listItems);
                ctx.ExecuteQuery();
                position = listItems.ListItemCollectionPosition;
                items.AddRange(listItems.ToList());
            }
            while (position != null);

            foreach (var item in items)
            {
                DocumentInfo documentInfo = new DocumentInfo();
                documentInfo.Name = Convert.ToString(item.FieldValues["FileLeafRef"]);
                documentInfo.ID = Convert.ToString(item.FieldValues["ID"]);
                documentInfo.Type = item.FileSystemObjectType.ToString();
                documentInfo.DocumentInfos = null;
                if (documentInfo.Type.ToLower().Equals("folder"))
                {
                    documentInfo.FolderPath = Convert.ToString(item.FieldValues["FileRef"]);
                    IterateListItems(documentInfo.FolderPath, ctx, list, documentInfo.DocumentInfos);
                }
                
                documentInfos.Add(documentInfo);

            }

        }

    }
}
