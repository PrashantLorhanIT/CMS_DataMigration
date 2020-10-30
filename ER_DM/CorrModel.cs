using System;
using System.Collections.Generic;
using System.Text;

namespace ER_DM
{
    public class CorrModel
    {
        public CorrModel()
        {

        }
        public int[] RelationRidCorr { get; set; }
        //public decimal RidCorr { get; set; }
        public decimal? RidCorrCategory { get; set; }
        public string Referencenumber { get; set; }
        public string TransmittalReferencenumber { get; set; }
        public string SendersReferencenumber { get; set; }
        public DateTime? CorrespondenceDate { get; set; }
        public string Status { get; set; }
        public decimal? RidContractlist { get; set; }
        public string Isreplyrequired { get; set; }
        public DateTime? Replyrequiredbydate { get; set; }
        public string Isconfidential { get; set; }
        public string Subject { get; set; }
        public string Bodycontent { get; set; }
        public decimal? RidDocumenttype { get; set; }
        public decimal? RidCorrtemplate { get; set; }
        public decimal? RidUsermaster { get; set; }
        public string Isactive { get; set; }
        public string ApprovalsRequired { get; set; }
        public decimal? SenderRidEntityList { get; set; }
        public decimal? RecipientRidEntityList { get; set; }
        public decimal? RidCommunicationType { get; set; }
        public decimal? RidGroupType { get; set; }


        //public override string ToString()
        //{
        //    return "RidCorr:" + RidCorr;
        //}
    }
}
