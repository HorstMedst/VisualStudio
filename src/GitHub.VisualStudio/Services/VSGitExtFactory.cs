﻿extern alias TF14;
extern alias TF15;

using System;
using System.ComponentModel.Composition;
using GitHub.Logging;
using Serilog;
using VSGitExt14 = TF14.GitHub.VisualStudio.Base.VSGitExt;
using VSGitExt15 = TF15.GitHub.VisualStudio.Base.VSGitExt;
using Microsoft.VisualStudio.Shell;

namespace GitHub.Services
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class VSGitExtFactory
    {
        static readonly ILogger log = LogManager.ForContext<VSGitExtFactory>();

        readonly IGitHubServiceProvider serviceProvider;

        [ImportingConstructor]
        public VSGitExtFactory(IGitHubServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        [Export(typeof(IVSGitExt))]
        public IVSGitExt VSGitExt => serviceProvider.GetService<IVSGitExt>();

        public static IVSGitExt Create(string dteVersion, IAsyncServiceProvider sp)
        {
            // DTE.Version always ends with ".0" even for later minor versions.
            switch (dteVersion)
            {
                case "14.0":
                    return new Lazy<IVSGitExt>(() => new VSGitExt14(sp.GetServiceAsync)).Value;
                case "15.0":
                    return new Lazy<IVSGitExt>(() => new VSGitExt15(sp.GetServiceAsync)).Value;
                default:
                    log.Error("There is no IVSGitExt implementation for DTE version {Version}", dteVersion);
                    return null;
            }
        }
    }
}