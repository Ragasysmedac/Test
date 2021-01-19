using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XAutomateMVC.Models
{
    public class FileUpload
    {
       
            public int Id { get; set; }
            public string Name { get; set; }
            public string FileType { get; set; }
            public string Extension { get; set; }
            public string Description { get; set; }
            public string UploadedBy { get; set; }
            public DateTime? CreatedOn { get; set; }
        
    }
}
