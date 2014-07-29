file-lock
=========

A simple file-locking implementation in C#.

## How File Locks Work ##

Take from "[Easy Mode: Synchronizing Multiple Processes with File Locks](http://blog.markedup.com/2014/07/easy-mode-synchronizing-multiple-processes-with-file-locks/ "Easy Mode: Synchronizing Multiple Processes with File Locks")" on the [MarkedUp Analytics blog](http://blog.markedup.com/ "MarkedUp Analytics and In-app Marketing Blog").

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

