using System;
using System.Collections.Generic;
using System.Text;

namespace ER_DM
{
    public class Corrdetail
    {
        public decimal RidCorrdetail { get; set; }
        public decimal RidCorr { get; set; }
        public decimal RidWorkflowstep { get; set; }
        public decimal? RidUsermaster { get; set; }
        public string Ismandatory { get; set; }
        public string Isactive { get; set; }
        public DateTime? Duedate { get; set; }
        public decimal RidCommunicationtype { get; set; }
        public decimal? RidCommunicationDetail { get; set; }
        public string SignatureRequired { get; set; }
        public string Include { get; set; }
       
    }
    class ApiCorrdetailResponse
    {
        public decimal Data { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }

    }
}
