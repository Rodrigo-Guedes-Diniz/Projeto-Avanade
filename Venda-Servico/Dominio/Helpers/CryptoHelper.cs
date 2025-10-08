using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace VendaServico.Dominio.Helpers
{
    public class CryptoHelper
    {
        public static SymmetricSecurityKey BuildKeyFromConfig(string secret)
        {
            if (string.IsNullOrWhiteSpace(secret))
                throw new ArgumentNullException(nameof(secret));

            try
            {
                // tenta tratar como Base64 (quando a chave está armazenada como base64)
                var bytes = Convert.FromBase64String(secret);
                return new SymmetricSecurityKey(bytes);
            }
            catch
            {
                // se não for Base64, usa UTF8 (texto puro)
                return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            }
        }
    }
}