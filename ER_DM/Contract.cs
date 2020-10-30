using System;
using System.Collections.Generic;
using System.Text;

namespace ER_DM
{
    class Contract
    {
        public decimal RidContract { get; set; }
        public string Contractcode { get; set; }
        public decimal RidEntityList { get; set; }
        public string Isactive { get; set; }
    }
    class ApiContractResponse
    {
        public List<Contract> Data { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }

    }
}
