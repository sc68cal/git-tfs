
## Git-Tfs & the Index

### Background

Git-Tfs replaced GitSharp with [LibGit2Sharp][libgit2sharp pr], because
the GitSharp project had become stagnant, and [memory][issue8] [issues][issue22] remained
unfixed, which forced one the maintainers of Git-Tfs, Matt Burke,
to fork the project and [carry his own patches][gitsharp patches]
(they were eventually upstreamed, but years after the fact).

The work to replace GitSharp with LibGit2Sharp was relatively
straight forward. Git-Tfs used classes in-tree to represent [Git 
objects][GitTfs.GitCommit]
with private members that mapped back to the GitSharp classes returned
from GitSharp API calls. Replacing GitSharp was just a matter of finding
the correct LibGit2Sharp equivalents and updating the Git-Tfs classes.

The original structure of the project was not updated to fully
utilize the APIs that LibGit2Sharp provided at the time,
as well as subsequent additions.

### Git-Tfs and the Index

Git-Tfs still uses a small amount of shell commands, which requires the
use of the git binary. One of the main portions of code that still relies
on shell commands, is the indexing code. In order to import
changesets from TFS into the Git repository, without affecting the working
directroy, Git-Tfs launches the git binary and populates the
[`GIT_INDEX_FILE` environment variable][WithTemporaryIndex], which is 
[honored by Git][GitEnvironment].

Git-Tfs uses LibGit2Sharp (and GitSharp before LibGit2Sharp) to [hash and
insert][hashandinsert] files fetched from TFS, and the hashes of the files
are written into the temporary index file.

### The Next Steps

By replacing `WithTemporaryIndex` with calls to LibGit2Sharp's
[TreeDefinition][TreeDefinition] API, we can programatically build
up trees and create commits with the LibGit2Sharp API.

There are advantages to this change:

* Better performance, by not creating new processes and executing Git in a
  subcommand
* Better maintainability: The code will be more explicit about how commits are
  created


[ObjectDatabase]: https://github.com/libgit2/libgit2sharp/blob/vNext/LibGit2Sharp/ObjectDatabase.cs
[CreateTree]: https://github.com/libgit2/libgit2sharp/blob/vNext/LibGit2Sharp/ObjectDatabase.cs#L178
[TreeEntry]: https://github.com/libgit2/libgit2sharp/blob/vNext/LibGit2Sharp/TreeEntry.cs
[WithTemporaryIndex]: https://github.com/git-tfs/git-tfs/blob/b2c892d0b2d49e649980e6ec79cb490cb5c30d99/GitTfs/Core/GitTfsRemote.cs#L681
[GitEnvironment]: http://git-scm.com/2010/04/11/environment.html
[libgit2sharp pr]: https://github.com/git-tfs/git-tfs/pull/176
[gitsharp patches]: https://github.com/spraints/GitSharp/compare/9d6b1ab...c5f6e71
[issue8]: https://github.com/git-tfs/git-tfs/issue/8
[issue22]: https://github.com/git-tfs/git-tfs/pull/22
[hashandinsert]: https://github.com/git-tfs/git-tfs/blob/master/GitTfs/Core/GitRepository.cs#L462
[GitTfs.GitCommit]: https://github.com/git-tfs/git-tfs/blob/0b32e0aa38c86c4e4c536f92ffbf0a0ca8aa1793/GitTfs/Core/GitCommit.cs
[TreeDefinition]: https://github.com/libgit2/libgit2sharp/blob/vNext/LibGit2Sharp.Tests/ObjectDatabaseFixture.cs#L154-L360
