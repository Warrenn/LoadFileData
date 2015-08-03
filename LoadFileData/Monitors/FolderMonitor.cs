using System;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using LoadFileData.Constants;
using LoadFileData.FileHandlers;
using LoadFileData.Properties;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace LoadFileData.Monitors
{
    public class FolderMonitor : IDisposable, IMonitor
    {
        private readonly IFileHandler fileHandler;
        private readonly string path;
        private IDisposable subscription;
        private FileSystemWatcher watcher;
        private IObservable<EventPattern<FileSystemEventArgs>> watcherObservable;
        private TimeSpan interval;
        private int retryCount;
        private IScheduler retryScheduler;

        public FolderMonitor(string path, IFileHandler fileHandler)
        {
            this.fileHandler = fileHandler;
            this.path = path;
        }

        public virtual void StartMonitoring(CancellationToken token)
        {
            if (watcher != null)
            {
                return;
            }
            watcher = new FileSystemWatcher(path)
            {
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.FileName
            };
            subscription = WatcherObservable.Subscribe(e =>
            {
                OnNext(e, token);
            });
        }

        public virtual void StopMonitoring()
        {
            subscription.Dispose(PolicyName.Disposable);
            watcher.Dispose(PolicyName.Disposable);
        }

        internal virtual IObservable<EventPattern<FileSystemEventArgs>> WatcherObservable
        {
            get
            {
                return watcherObservable ??
                       (watcherObservable =
                        Observable.FromEventPattern
                            <FileSystemEventHandler, FileSystemEventArgs>(
                                handler => watcher.Created += handler,
                                handler => watcher.Created -= handler));
            }
            set { watcherObservable = value; }
        }

        protected virtual void OnNext(EventPattern<FileSystemEventArgs> newFileEvent, CancellationToken token)
        {
            var fullPath = newFileEvent.EventArgs.FullPath;

            if (token.IsCancellationRequested)
            {
                StopMonitoring();
                return;
            }
            RetryScheduler.Schedule(0, TimeSpan.Zero, (state, recurse) =>
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                Stream stream;
                try
                {
                    stream = File.OpenRead(fullPath);
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref state);
                    if (state < RetryCount)
                    {
                        recurse(state, Interval);
                        return;
                    }
                    fileHandler.ReportError(fullPath, ex);
                    ExceptionPolicy.HandleException(ex, PolicyName.FolderMonitor);
                    return;
                }
                try
                {
                    fileHandler.ProcessFile(fullPath, stream, token);
                }
                catch (Exception ex)
                {
                    fileHandler.ReportError(fullPath, ex);
                    ExceptionPolicy.HandleException(ex, PolicyName.FolderMonitor);
                }
                finally
                {
                    stream.Dispose(PolicyName.Disposable);
                }
            });
            if (!token.IsCancellationRequested)
            {
                return;
            }
            StopMonitoring();
        }

        internal virtual IScheduler RetryScheduler
        {
            get { return retryScheduler ?? (retryScheduler = Scheduler.Default); }
            set { retryScheduler = value; }
        }

        internal protected virtual TimeSpan Interval
        {
            get
            {
                if (interval == default(TimeSpan))
                {
                    interval = Settings.Default.Interval;
                }
                return interval;
            }
            set { interval = value; }
        }

        internal protected virtual int RetryCount
        {
            get
            {
                if (retryCount == default(int))
                {
                    retryCount = Settings.Default.RetryCount;
                }
                return retryCount;
            }
            set { retryCount = value; }
        }

        public virtual void Dispose()
        {
            fileHandler.Dispose(PolicyName.Disposable);
            StopMonitoring();
        }
    }
}
