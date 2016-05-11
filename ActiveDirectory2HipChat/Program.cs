﻿using ActiveDirectory2HipChat.Processors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace ActiveDirectory2HipChat
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetLogger("Ad2HipChat");

        static void Main(string[] args)
        {
            // Start Logging
            LoggerConfig.StartLogging();

            Logger.Info("Starting Ad2HipChat Integration");

            Logger.Trace("Building task factory");
            var tasks = new List<Task>();

            Logger.Trace("Defining new cancellation token for all processors");
            var cancellationToken = new CancellationTokenSource();

            Logger.Trace("Adding AD Processor");
            tasks.Add(Task.Factory.StartNew(() => new AdProcessor().Run(cancellationToken), cancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default));

            Logger.Trace("Adding HipChat Processor");
            tasks.Add(Task.Factory.StartNew(() => new HipchatProcessor().Run(cancellationToken), cancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default));

            // Fire them all up and wait for them to complete... this shouldn't happen, otherwise the loops are broken.
            Logger.Trace("Running the task factory... firing the missles");
            Task.WaitAll(tasks.ToArray());

            Logger.Info("Stopping Ad2HipChat Integration");

            Logger.Trace("Cancelling all running cooperative tasks");
            cancellationToken.Cancel();

            Logger.Info("Ad2HipChat Integration Stopped");
        }
    }
}
