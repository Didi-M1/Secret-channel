using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesType
{
    [Serializable]
    public class ExceptionErrorInFileDycripting : Exception
    {
        public string ID;
        public ExceptionErrorInFileDycripting(string id) : base() => ID = id;
        public ExceptionErrorInFileDycripting(string id,string msg) : base(msg) => ID = id;
        public override string ToString() => base.ToString() + $"something went wrong in Dycripting {ID}";
    }
    [Serializable]
    public class ExceptionErrorInTypeDycripting : Exception
    {
        public string ID;
        public ExceptionErrorInTypeDycripting(string id) : base() => ID = id;
        public ExceptionErrorInTypeDycripting(string id, string msg) : base(msg) => ID = id;
        public override string ToString() => base.ToString() + $"something went wrong in type Dycripting {ID}";
    }
}
