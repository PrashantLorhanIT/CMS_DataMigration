using System;
using System.Collections.Generic;
using System.Text;

namespace ER_DM
{
    class Usermaster
    {
        public decimal RidUsermaster { get; set; }
        public string Username { get; set; }
        public string Isaduser { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Middlename { get; set; }
        public string Email { get; set; }
        public decimal RidRoles { get; set; }
        public decimal RidLanguage { get; set; }
        public string Isactive { get; set; }
        public string Addedby { get; set; }
        public DateTime? Addedon { get; set; }
        public string Updatedby { get; set; }

        public DateTime? Updatedon { get; set; }
        public string Isaccountenabled { get; set; }
        public string Appointedas { get; set; }

        public string ProfileImage { get; set; }
        public decimal? RidEntityList { get; set; }
        public decimal? RidRootEntityList { get; set; }

        public string Token { get; set; }
        public string RefreshToken { get; set; }


    
    }

    class ApiLoginResponse
    {
        public List<Usermaster> Data { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }

    }
}
