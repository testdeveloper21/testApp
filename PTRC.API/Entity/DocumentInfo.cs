using System;
using System.Collections.Generic;

namespace PTRC.API.Models
{
    public class DocumentInfo
    {
        public DocumentInfo()
        {
            DocumentInfos = new List<DocumentInfo>();
        }

        public string Name { get; set; }
        public string Type { get; set; }

        public string ID { get; set; }

        public string FolderPath { get; set; }

        public List<DocumentInfo> DocumentInfos { get; set; }


    }
}