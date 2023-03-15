using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ExclusiveCard.Managers
{
    public interface IKeyVaultManager
    {
        public Task<X509Certificate2> GetCertificate(string certificateName);

    }
}
