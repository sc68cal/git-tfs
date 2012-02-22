using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Sep.Git.Tfs.Commands;
using Sep.Git.Tfs.Core;
using Sep.Git.Tfs.Core.TfsInterop;
using Sep.Git.Tfs.Test.TestHelpers;
using StructureMap.AutoMocking;

namespace Sep.Git.Tfs.Test.Commands
{
    [TestClass]
    public class RCheckinTest
    {
        private RhinoAutoMocker<Rcheckin> mocks;

        [TestInitialize]
        public void Setup()
        {
            mocks = new RhinoAutoMocker<Rcheckin>();
            mocks.Inject<TextWriter>(new StringWriter());
            mocks.Get<Globals>().Repository = mocks.Get<IGitRepository>();
        }

        [TestMethod]
        public void ShouldAssociateWithWorkItem()
        {
            WireUpMockRemote();
            mocks.Get<IGitRepository>().Stub(x => x.GetLastParentTfsCommits(null)).IgnoreArguments()
                .Return(new[] { new TfsChangesetInfo { Remote = mocks.Get<IGitTfsRemote>() } });
            mocks.ClassUnderTest.Run();

        }

        private void WireUpMockRemote()
        {
            mocks.Get<Globals>().Repository = mocks.Get<IGitRepository>();
            var remote = mocks.Get<IGitTfsRemote>();
            remote.Stub(x => x.Repository).Return(mocks.Get<IGitRepository>());
            mocks.Get<IGitRepository>().Stub(x => x.GetLastParentTfsCommits(null)).IgnoreArguments()
                .Return(new[] { new TfsChangesetInfo { Remote = remote } });
        }
    }
}
