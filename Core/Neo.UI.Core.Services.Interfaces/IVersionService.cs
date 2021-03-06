﻿using System;
using Neo.UI.Core.Data;

namespace Neo.UI.Core.Services.Interfaces
{
    public interface IVersionService
    {
        Version CurrentVersion { get; }

        Version LatestVersion { get; }

        bool UpdateIsRequired { get; }

        ReleaseInfo GetLatestReleaseInfo();
    }
}
