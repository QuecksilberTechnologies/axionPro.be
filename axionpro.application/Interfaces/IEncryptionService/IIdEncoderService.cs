using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IEncryptionService
{
    public interface IIdEncoderService
    {


            string EncodeId_long(long id, string tenantKey);
             long DecodeId_long(string? encodedId, string tenantKey);
             int DecodeId_int(string? encodedId, string tenantKey);
              string EncodeId_int(int id, string tenantKey);
            string EncodeString(string input, string tenantKey);
            string DecodeString(string input, string tenantKey);

        


    }
}
