using System;
using System.Collections.Generic;
using System.Text;

namespace ER_DM
{
    public  class Attachment
    {

        public decimal RidAttachment { get; set; }
        public decimal Attachsequence { get; set; }
        public string Attachedfilename { get; set; }
        public string Isactive { get; set; }
        public string Addedby { get; set; }
        public DateTime? Addedon { get; set; }
        public string Updatedby { get; set; }
        public DateTime? Updatedon { get; set; }
        public string Documentumid { get; set; }
        public decimal? RidCorr { get; set; }
        public decimal? RidRfi { get; set; }
        public decimal? RidMom { get; set; }
        public decimal? RidTask { get; set; }

        public string HTMLString { get; set; }

        public string ObjectID { get; set; }

    }

    class ApiAttachmentResponse
    {
        public decimal Data { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }

    }
}
