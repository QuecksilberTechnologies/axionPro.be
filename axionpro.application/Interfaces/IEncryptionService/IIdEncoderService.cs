using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IEncryptionService
{
    public interface IIdEncoderService
    {


            string EncodeId(long id, string tenantKey);
             long DecodeId(string? encodedId, string tenantKey);
            string EncodeString(string input, string tenantKey);
            string DecodeString(string input, string tenantKey);

        


    }
}
