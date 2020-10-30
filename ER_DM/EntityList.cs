using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;


namespace ER_DM
{
    public class EntityList
    {
       
        public decimal RidEntityList { get; set; }
        public decimal RidEntityType { get; set; }
        public decimal? ParentRidEntityList { get; set; }
        public string Isactive { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
       
        public override string ToString()
        {
            return "EntityCode:" + EntityCode;
        }

        
    }
    class ApiEntityResponse
    {
        public List<EntityList> Data { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }

    }
}
