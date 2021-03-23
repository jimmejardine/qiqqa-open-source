using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Identity.Client;

namespace WPF_Template_App1.Core.Contracts.Services
{
    public interface IIdentityCacheService
    {
        void SaveMsalToken(byte[] token);

        byte[] ReadMsalToken();
    }
}
