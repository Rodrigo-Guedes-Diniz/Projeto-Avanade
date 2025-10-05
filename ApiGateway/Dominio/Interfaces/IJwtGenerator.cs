using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway.Services
{
    public interface IJwtGenerator
    {
        string GenerateToken(string userId, string userName, string userRole);
    }
}