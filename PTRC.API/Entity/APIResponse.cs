using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTRC.API.Models
{
    public class ApiResponse
    {
        public ApiResponse() {
            DocumentInfo = new List<DocumentInfo>();
        }
        public bool ErrorOccurred { get; set; }
        public bool IsValid { get; set; } = true;
        public string Message { get; set; }
        public List<DocumentInfo> DocumentInfo { get; set; }
    }
}