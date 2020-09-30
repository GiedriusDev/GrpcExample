using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api
{
    public static class Config
    {
        public static string ServerAddress = string.Empty;

        public static void Initialize(IConfiguration configuration)
        {
            ServerAddress = configuration["ServerAddress"];
        }
    }
}
