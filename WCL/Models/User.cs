using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WCL.Models
{
    class User
    {
        #region Constructor
        public User() 
        {
            IsNullError = true;
        }   
        #endregion

        #region Properties
        public long id { get; set; }
        public string? username { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? RepeatedPassword { get; set; }
        public string? phone { get; set; }
        #endregion

        #region Validation
        public bool IsNullError { get; set; }
        public string ValidateFields(string field)
        {
            string result = string.Empty;
            
            switch (field)
            {
                case nameof(username)    when string.IsNullOrEmpty(username):     result = "Требуется ввести имя";     break;
                case nameof(firstName)   when string.IsNullOrEmpty(firstName):    result = "Требуется ввести имя";     break;
                case nameof(lastName)    when string.IsNullOrEmpty(lastName):     result = "Требуется ввести фамилию"; break;
                case nameof(email)       when string.IsNullOrEmpty(email):        result = "Требуется ввести почту";   break;
                case nameof(password)    when string.IsNullOrEmpty(password):     result = "Требуется ввести пароль";  break;
                case nameof(RepeatedPassword) when password != RepeatedPassword: result = "Требуется ввести одинаковые";  break;
                case nameof(RepeatedPassword) when string.IsNullOrEmpty(RepeatedPassword): result = "Требуется повторно ввести пароль";  break;
                case nameof(phone) when string.IsNullOrEmpty(phone): result = "Требуется ввести номер телефона"; break;
            }

            IsNullError = string.IsNullOrEmpty(result);
            return result;
        }

        #endregion
    }
}
