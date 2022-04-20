using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesType
{
    [Serializable]
    public class ExceptionErrorInFileDecrypting : Exception
    {
        public string ID;
        public ExceptionErrorInFileDecrypting(string id) : base() => ID = id;
        public ExceptionErrorInFileDecrypting(string id,string msg) : base(msg) => ID = id;
        public override string ToString() => base.ToString() + $"something went wrong in decrypting {ID}";
    }
    [Serializable]
    public class ExceptionErrorInTypeDecrypting : Exception
    {
        public string ID;
        public ExceptionErrorInTypeDecrypting(string id) : base() => ID = id;
        public ExceptionErrorInTypeDecrypting(string id, string msg) : base(msg) => ID = id;
        public override string ToString() => base.ToString() + $"something went wrong in type decrypting {ID}";
    }
}
