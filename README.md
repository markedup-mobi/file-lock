file-lock
=========

A simple file-locking implementation in C#.

## How File Locks Work ##

Taken from "[Easy Mode: Synchronizing Multiple Processes with File Locks](http://blog.markedup.com/2014/07/easy-mode-synchronizing-multiple-processes-with-file-locks/ "Easy Mode: Synchronizing Multiple Processes with File Locks")" on the [MarkedUp Analytics blog](http://blog.markedup.com/ "MarkedUp Analytics and In-app Marketing Blog").

The concept of a file lock is simple and it’s fairly ubiquitous in the real-world – anyone who’s done a lot of work with Ruby (Gemfile.lock) or Linux has used a file lock at some point in time.

> A file lock is implemented by having multiple process look for a file with the same name at a well-known location on the file system – if the file exists, then each process knows that the lock has been held at some point in time. The lock is acquired when the file is written to disk and the lock is released when it is deleted.
> 
> Typically the lock file will contain some data about the ID of the process that last held the lock and the time when the process acquired it – that way processes can have “re-entrant locking” and lock timeouts, in the event that a process was forcibly killed before it had a chance to explicitly release the lock.

Here’s an illustrated example:

![MarkedUp File Lock Example Step 1](http://markedupblog.blob.core.windows.net/wordpress/2014/07/file-lock-example.png)

Process 1 acquires the lock by writing markedup.lock to the file system – and then it does its work against the shared resource. Process 2 attempted to acquire the lock but since the file is already in use, it has to wait until Process 1 releases the lock.

![MarkedUp File Lock Example Step 2](http://markedupblog.blob.core.windows.net/wordpress/2014/07/file-lock-example-step-2.png)

Process 1 releases the lock by deleting the markedup.lock file. Process 2 attempts to acquire the lock later and discovers that the markedup.lock file is missing, so it successfully acquires the lock by recreating the markedup.lock file and does its work against the shared resource.

Read our full blog post on "[Easy Mode: Synchronizing Multiple Processes with File Locks](http://blog.markedup.com/2014/07/easy-mode-synchronizing-multiple-processes-with-file-locks/ "Easy Mode: Synchronizing Multiple Processes with File Locks")" for more information.

## Using file-lock ##

Install the [FileLock NuGet package](https://www.nuget.org/packages/FileLock/ "MarkedUp C# File Lock package") via the following command in Visual Studio:

    PM> Install-Package FileLock

Once that's done, you create a reference to a file lock via the following code snippet:

    //creates a file lock with a 2-minute timeout
    var fileLock = SimpleFileLock.Create("path/to/lock/name.{extension}", TimeSpan.FromMinutes(2));

Note: this **does not create the lock file itself**, it only gives you a reference to it.

To attempt to acquire the lock, use the `TryAcquireLock()` method:

    if (fileLock.TryAcquireLock())
    {
    	//Lock acquired - do your work here
    }
    else
    {
    	//Failed to acquire lock - SpinWait or do something else
    }

`TryAcquireLock()` will return `true` under if and only if:

* The lockfile you specified does not exist; or
* The lockfile you specified exists, and it is already owned by the calling process; or
* The lockfile you specified exists, but it has timed out in accordance with the timeout value to specified.

Once you're finished with your work, release the lock via `fLock.ReleaseLock()` - which will delete the file.


## License ##
file-lock is licensed under Apache V2.0 - see [License](LICENSE) for details.

## Local Builds ##
file-lock uses [FAKE](https://github.com/fsharp/FAKE "FAKE - F# build system") to build its release locally. 

After making your changes, just run `build.cmd` and check the results in the `\bin\FileLock` folder.

If you want to do a local NuGet packge, make sure you edit [RELEASE+NOTES.md](RELEASE_NOTES.md) and add a new entry at the top of the file with a new version number before you run the build. This will increment the version number of the NuGet package produced when `build.cmd` runs.

## Contributing ##
We accept pull requests, and have you have any questions please feel free to leave an issue here or [contact MarkedUp Analytics on Twitter](https://twitter.com/markedupmobi "MarkedUp Analytics and In-app Marketing on Twitter").
