using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.IO;
using System;
namespace MvcProject
{
    public class AuthorizationRequirement:IAuthorizationRequirement
    {
        public bool ValidateAvailableSpace()
        {
            return DriveInfo.GetDrives()[0].AvailableFreeSpace > 1024 * 1024 * 1024;
        }
    }
}
