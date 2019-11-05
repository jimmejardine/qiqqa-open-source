using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary;
using Utilities;
using Utilities.Encryption;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.ProcessTools;
using Utilities.Shutdownable;

namespace Qiqqa.Documents.PDF.PDFRendering
{
    public class PDFTextExtractor
    {
        public static PDFTextExtractor Instance = new PDFTextExtractor();
        private object still_running_lock = new object();
        private bool still_running;
        public bool StillRunning
        {
            get
            {
                lock (still_running_lock)
                {
                    return still_running;
                }
            }
            set
            {
                lock (still_running_lock)
                {
                    still_running = value;
                }
            }
        }

        private int NUM_OCR_THREADS;
        private Thread[] threads;

        private object queue_lock = new object();
        private Dictionary<string, Job> job_queue_group = new Dictionary<string, Job>();
        private Dictionary<string, Job> job_queue_single = new Dictionary<string, Job>();
        private HashSet<string> current_jobs_group = new HashSet<string>();
        private HashSet<string> current_jobs_single = new HashSet<string>();

        public void GetJobCounts(out int job_queue_group_count, out int job_queue_single_count)
        {
            // Get a count of how many jobs are left...
            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (queue_lock)
            {
                l1_clk.LockPerfTimerStop();
                job_queue_group_count = job_queue_group.Count;
                job_queue_single_count = job_queue_single.Count;
            }
        }

        public class Job
        {
            public PDFRenderer pdf_renderer;
            public int page;
            public int TEXT_PAGES_PER_GROUP;
            public bool force_job;
            public string language;

            public Job(PDFRenderer pdf_renderer, int page, int TEXT_PAGES_PER_GROUP)
            {
                this.pdf_renderer = pdf_renderer;
                this.page = page;
                this.TEXT_PAGES_PER_GROUP = TEXT_PAGES_PER_GROUP;

                force_job = false;
                language = "";
            }

            public override string ToString()
            {
                return String.Format("PDF:{0} Page:{1} Grouping:{2} Forced:{3} Language:{4}", pdf_renderer, page, TEXT_PAGES_PER_GROUP, force_job, language);
            }

            public void Clear()
            {
                pdf_renderer = null;
                language = String.Empty;
            }
        }

        private class NextJob : IDisposable
        {
            private PDFTextExtractor pdf_text_extractor;

            public Job job;
            public bool is_group;

            internal NextJob(PDFTextExtractor pdf_text_extractor, Job job, bool is_group, object queue_lock_reminder)
            {
                this.pdf_text_extractor = pdf_text_extractor;
                this.job = job;
                this.is_group = is_group;

                pdf_text_extractor.RecordThatJobHasStarted_LOCK(this, queue_lock_reminder);
            }

            #region --- IDisposable ------------------------------------------------------------------------

            ~NextJob()
            {
                Logging.Debug("~NextJob()");
                Dispose(false);
            }

            public void Dispose()
            {
                Logging.Debug("Disposing NextJob");
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private int dispose_count = 0;
            protected virtual void Dispose(bool disposing)
            {
                Logging.Debug("NextJob::Dispose({0}) @{1}", disposing, dispose_count);

                if (dispose_count == 0)
                {
                    // Notify that this job is done...
                    pdf_text_extractor.RecordThatJobHasCompleted(this);

                    //job?.Clear();
                }

                pdf_text_extractor = null;
                job = null;

                ++dispose_count;
            }

            #endregion

            /// <summary>
            /// Use this for getting a unique token for the job+page
            /// </summary>
            /// <param name="job"></param>
            /// <returns></returns>
            public static string GetQueuedJobToken(Job job)
            {
                return job.pdf_renderer.DocumentFingerprint + "." + job.page;
            }

            /// <summary>
            /// Use this for determining if a job group is running
            /// </summary>
            /// <param name="job"></param>
            /// <param name="is_group"></param>
            /// <returns></returns>
            public static string GetCurrentJobToken(Job job, bool is_group)
            {
                if (is_group)
                {
                    int job_group_start_page = ((job.page - 1) / job.TEXT_PAGES_PER_GROUP) * job.TEXT_PAGES_PER_GROUP + 1;
                    return job.pdf_renderer.DocumentFingerprint + "." + job_group_start_page;
                }
                else
                {
                    return job.pdf_renderer.DocumentFingerprint + "." + job.page;
                }
            }

            public override string ToString()
            {
                return String.Format("Group:{0} Job:{1}", is_group, job);
            }
        }

        private PDFTextExtractor()
        {
            ShutdownableManager.Instance.Register(Shutdown);

            // Get a sensible number of OCR processors
            NUM_OCR_THREADS = ConfigurationManager.Instance.ConfigurationRecord.System_NumOCRProcesses ?? 0;
            if (0 == NUM_OCR_THREADS)
            {
                NUM_OCR_THREADS = Environment.ProcessorCount - 1;
            }
            NUM_OCR_THREADS = Math.Max(NUM_OCR_THREADS, 1);
            NUM_OCR_THREADS = Math.Min(NUM_OCR_THREADS, Environment.ProcessorCount);
#if true // for debugging 
            NUM_OCR_THREADS = Math.Max(NUM_OCR_THREADS, 1);
#endif

            Logging.Info("Starting {0} PDFTextExtractor threads", NUM_OCR_THREADS);
            still_running = true;
            threads = new Thread[NUM_OCR_THREADS];
            for (int i = 0; i < NUM_OCR_THREADS; ++i)
            {
                threads[i] = new Thread(ThreadEntry);
                //threads[i].IsBackground = true;
                threads[i].Priority = ThreadPriority.BelowNormal;
                threads[i].Name = "PDFTextExtractor" + i;
                threads[i].Start();
            }
        }

        public void QueueJobGroup(Job job)
        {
            string token = NextJob.GetQueuedJobToken(job);

            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (queue_lock)
            {
                l1_clk.LockPerfTimerStop();

                // Only add the job if it is not already queued
                if (!job_queue_group.ContainsKey(token))
                {
                    job_queue_group[token] = job;
                }
            }
        }

        public void QueueJobSingle(Job job)
        {
            string token = NextJob.GetQueuedJobToken(job);

            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (queue_lock)
            {
                l1_clk.LockPerfTimerStop();

                // Only add the job if it is not already queued, OR if we are queuing a FORCE job, which has priority
                if (!job_queue_single.ContainsKey(token) || job.force_job)
                {
                    job_queue_single[token] = job;
                }
            }
        }

        /// <summary>
        /// NB: This must be called inside the queue_lock!
        /// </summary>
        /// <param name="nextJob"></param>
        private void RecordThatJobHasStarted_LOCK(NextJob next_job, object queue_lock_REMINDER)
        {
            string token = NextJob.GetCurrentJobToken(next_job.job, next_job.is_group);
            HashSet<string> current_jobs = next_job.is_group ? current_jobs_group : current_jobs_single;
            if (current_jobs.Contains(token))
            {
                Logging.Error("Job is already running: {0}", token);
            }
            current_jobs.Add(token);
        }

        private void RecordThatJobHasCompleted(NextJob next_job)
        {
            string token = NextJob.GetCurrentJobToken(next_job.job, next_job.is_group);

            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (queue_lock)
            {
                l1_clk.LockPerfTimerStop();
                HashSet<string> current_jobs = next_job.is_group ? current_jobs_group : current_jobs_single;
                if (!current_jobs.Contains(token))
                {
                    Logging.Error("Job is not running, so can't remove it: {0}", token);
                }
                current_jobs.Remove(token);
            }
        }

        private bool IsSimilarJobRunning(Job job, bool is_group, object queue_lock_REMINDER)
        {
            // Check if a similar group job is running
            if (current_jobs_group.Contains(NextJob.GetCurrentJobToken(job, true))) return true;

            // If this is a single job, check if a similar single job is running
            if (!is_group)
            {
                if (current_jobs_single.Contains(NextJob.GetCurrentJobToken(job, false))) return true;
            }

            // No similar job is running...
            return false;
        }

        private DateTime ocr_disabled_next_notification_time = DateTime.MinValue;
        private const double TARGET_RATIO = 1.0;
        // add noise to the ratio to ensure that the status update, which lists the counts, shows the activity by the numbers going up and down as the user watches
        private int prev_ocr_count = 0;
        private int prev_textify_count = 0;

        private NextJob GetNextJob()
        {
            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (queue_lock)
            {
                l1_clk.LockPerfTimerStop();

                // Check if OCR is disabled
                if (!(ConfigurationManager.Instance.ConfigurationRecord.Library_OCRDisabled
                    || ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks))
                {
                    int ocr_count = job_queue_single.Count;
                    int textify_count = job_queue_group.Count;
                    double current_ratio = (ocr_count + 1) * (1.0 / (textify_count + 1));

                    // noise the target ratio: choose such that the user will observe the numbers changing most often
                    //					
                    // if the current_ratio is below par, we want to pick a textify job first,
                    // but if its number has been bumped by +1 since last time, we go for another job first instead
                    // as otherwise the user would observe a 'stuck count' while a lot of work is being done.
                    // (and vice versa of course for current_ratio above par)
                    //
                    if (current_ratio <= TARGET_RATIO)
                    {
                        if (prev_textify_count == textify_count - 1)
                        {
                            current_ratio = TARGET_RATIO + 0.1;
                        }
                    }
                    else
                    {
                        if (prev_ocr_count == ocr_count - 1)
                        {
                            current_ratio = TARGET_RATIO - 0.1;
                        }
                    }

                    prev_textify_count = textify_count;
                    prev_ocr_count = ocr_count;

                    if (current_ratio <= TARGET_RATIO)
                    {
                        // First look for any GROUP jobs                
                        foreach (var pair in job_queue_group)
                        {
                            Job job = pair.Value;
                            if (!IsSimilarJobRunning(job, true, queue_lock))
                            {
                                job_queue_group.Remove(pair.Key);
                                return new NextJob(this, job, true, queue_lock);
                            }
                        }
                    }

                    // Don't bother with the sophistication when the numbers get large:
                    if (100 >= ocr_count)
                    {
                        // First look for any SINGLE 1st pages - these get priority
                        foreach (var pair in job_queue_single)
                        {
                            Job job = pair.Value;
                            if (!IsSimilarJobRunning(job, false, queue_lock))
                            {
                                if (1 == job.page)
                                {
                                    job_queue_single.Remove(pair.Key);
                                    return new NextJob(this, job, false, queue_lock);
                                }
                            }
                        }

                        // Look for any first 3 pages (this is where they will probably already be reading by now, so get them early)
                        foreach (var pair in job_queue_single)
                        {
                            Job job = pair.Value;
                            if (!IsSimilarJobRunning(job, false, queue_lock))
                            {
                                if (3 >= job.page)
                                {
                                    job_queue_single.Remove(pair.Key);
                                    return new NextJob(this, job, false, queue_lock);
                                }
                            }
                        }

                        // Look for any last 3 pages (we want the bibliographies in case they click on "make cross references")
                        foreach (var pair in job_queue_single)
                        {
                            Job job = pair.Value;
                            if (!IsSimilarJobRunning(job, false, queue_lock))
                            {
                                if (job.pdf_renderer.PageCount - 3 <= job.page)
                                {
                                    job_queue_single.Remove(pair.Key);
                                    return new NextJob(this, job, false, queue_lock);
                                }
                            }
                        }

                        // Look for any first 10 pages (this is where we process the short PDFs before the long PDFs)
                        foreach (var pair in job_queue_single)
                        {
                            Job job = pair.Value;
                            if (!IsSimilarJobRunning(job, false, queue_lock))
                            {
                                if (10 >= job.page)
                                {
                                    job_queue_single.Remove(pair.Key);
                                    return new NextJob(this, job, false, queue_lock);
                                }
                            }
                        }

                        // Look for any last 10 pages (this is where we process the short PDFs before the long PDFs)
                        foreach (var pair in job_queue_single)
                        {
                            Job job = pair.Value;
                            if (!IsSimilarJobRunning(job, false, queue_lock))
                            {
                                if (job.pdf_renderer.PageCount - 10 <= job.page)
                                {
                                    job_queue_single.Remove(pair.Key);
                                    return new NextJob(this, job, false, queue_lock);
                                }
                            }
                        }
                    }

                    // Otherwise get the most recently added job
                    //
                    // (in a large queue, that is: just grab the first available)
                    foreach (var pair in job_queue_single)
                    {
                        Job job = pair.Value;
                        if (!IsSimilarJobRunning(job, false, queue_lock))
                        {
                            job_queue_single.Remove(pair.Key);
                            return new NextJob(this, job, false, queue_lock);
                        }
                    }

                    // And when there's nothing else to do, clean up the GROUP queue remainder anyway
                    foreach (var pair in job_queue_group)
                    {
                        Job job = pair.Value;
                        if (!IsSimilarJobRunning(job, true, queue_lock))
                        {
                            job_queue_group.Remove(pair.Key);
                            return new NextJob(this, job, true, queue_lock);
                        }
                    }
                }
            }

            // Check if OCR is disabled
            if (ConfigurationManager.Instance.ConfigurationRecord.Library_OCRDisabled
                || ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks)
            {
                int job_queue_group_count;
                int job_queue_single_count;
                GetJobCounts(out job_queue_group_count, out job_queue_single_count);

                int job_queue_total_count = job_queue_group_count + job_queue_single_count;
                // Only state that the OCR is disabled if there is something to be OCRed
                if (0 < job_queue_single_count)
                {
                    // perform frequent happening time check outside lock:
                    if (DateTime.UtcNow.Subtract(ocr_disabled_next_notification_time).TotalMilliseconds > 0)
                    {
                        StatusManager.Instance.UpdateStatus("PDFOCR", String.Format("OCR is disabled (pending: {0} page(s) to textify and {1} page(s) to OCR)", job_queue_group_count, job_queue_single_count));
                        ocr_disabled_next_notification_time = DateTime.UtcNow.AddSeconds(5);
                    }
                }
            }

            // There are no jobs! Or we're not allowed to return any jobs if disabled!
            return null;
        }

        /// <summary>
        /// Flush the queue. Use this call to discard pending work items when the application is terminating.
        /// </summary>
        private void FlushAllJobs()
        {
            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (queue_lock)
            {
                l1_clk.LockPerfTimerStop();

                job_queue_group.Clear();
                job_queue_single.Clear();
            }
        }

        private void ThreadEntry()
        {
            bool did_some_ocr_since_last_iteration = false;

            while (true)
            {
                if (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown || !StillRunning)
                {
                    int job_queue_group_count;
                    int job_queue_single_count;
                    GetJobCounts(out job_queue_group_count, out job_queue_single_count);

                    Logging.Debug特("PDFTextExtractor: shutting down and flushing the queue ({0} + {1} items discarded)", job_queue_group_count, job_queue_single_count);

                    FlushAllJobs();
                    break;
                }

                // If this library is busy, skip it for now
                if (Library.IsBusyAddingPDFs)
                {
                    // Get a count of how many jobs are left...
                    int job_queue_group_count;
                    int job_queue_single_count;
                    GetJobCounts(out job_queue_group_count, out job_queue_single_count);

                    int job_queue_total_count = job_queue_group_count + job_queue_single_count;

                    if (0 < job_queue_group_count || 0 < job_queue_single_count)
                    {
                        did_some_ocr_since_last_iteration = true;
                        StatusManager.Instance.UpdateStatus("PDFOCR", "OCR paused while adding documents.");
                    }
                    Thread.Sleep(1000);
                    continue;
                }

                if (ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks)
                {
                    Logging.Debug特("OCR/Textify daemons are forced to sleep via Configuration::DisableAllBackgroundTasks");
                    Thread.Sleep(1000);
                    continue;
                }

                using (NextJob next_job = GetNextJob())
                {
                    if (null != next_job)
                    {
                        did_some_ocr_since_last_iteration = true;

                        Logging.Info("Doing OCR for job '{0}'", next_job.job);

                        long clk_duration;
                        {
                            Stopwatch clk = new Stopwatch();
                            clk.Start();

                            // Relinquish control to the UI thread to make sure responsiveness remains tolerable at 100% CPU load.
                            Utilities.GUI.WPFDoEvents.WaitForUIThreadActivityDone();

                            clk_duration = clk.ElapsedMilliseconds;
                        }

                        // The call above can take quite a while to complete, so check all abort/delay checks once again, just in case...:
                        if (false
                            || Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown || !StillRunning
                            || clk_duration > 100
                            || Library.IsBusyAddingPDFs
                            || ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks
                            )
                        {
                            Logging.Warn("Recheck job queue after WaitForUIThreadActivityDone took {0}ms or shutdown/dealy signals were detected: {1}/{2}/{3}/{4}.",
                                clk_duration,
                                (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown || !StillRunning) ? "+Shutdown+" : "-SD-",
                                clk_duration > 100 ? "+UI-wait+" : "-UI-",
                                Library.IsBusyAddingPDFs ? "+PDFAddPending+" : "-PDF-",
                                ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks ? "+DisableBackgroundTasks+" : "-DB-"
                             );

                            // push the job onto the queue and start from the beginning:
                            if (next_job.is_group)
                            {
                                QueueJobGroup(next_job.job);
                            }
                            else
                            {
                                QueueJobSingle(next_job.job);
                            }
                            continue;
                        }
                        else
                        {
                            // Get a count of how many jobs are left...
                            int job_queue_group_count;
                            int job_queue_single_count;
                            GetJobCounts(out job_queue_group_count, out job_queue_single_count);

                            // nitpick: we'll be one off in the counts as we have the current job as well, but I'm fine with an incidental 0/0/99% report.
                            int job_queue_total_count = job_queue_group_count + job_queue_single_count + 1;

                            StatusManager.Instance.UpdateStatus("PDFOCR", String.Format("{0} page(s) to textify and {1} page(s) to OCR.", job_queue_group_count, job_queue_single_count), 1, job_queue_total_count);
                        }

                        // If the text has somehow appeared before we get to process it (perhaps two requests for the same job)
                        if (!next_job.job.force_job && null != next_job.job.pdf_renderer.GetOCRText(next_job.job.page, false))
                        {
                            Logging.Info("Job '{0}' is redundant as text exists", next_job.job);
                            continue;
                        }

                        string temp_ocr_result_filename = TempFile.GenerateTempFilename("txt");

                        try
                        {
                            if (next_job.is_group)
                            {
                                ProcessNextJob_Group(next_job, temp_ocr_result_filename);
                            }
                            else
                            {
                                ProcessNextJob_Single(next_job, temp_ocr_result_filename);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.Error(ex, "There was a problem processing job {0}", next_job.job);
                        }
                        finally
                        {
                            try
                            {
                                // (it's okay to try to delete the tempfiles when we're terminating; the rest of the job has been skipped)
                                File.Delete(temp_ocr_result_filename);
                            }
                            catch (Exception ex)
                            {
                                Logging.Error(ex, "There was a problem deleting the temporary OCR file {0}", temp_ocr_result_filename);
                            }
                        }
                    }
                    else
                    {
                        if (did_some_ocr_since_last_iteration)
                        {
                            did_some_ocr_since_last_iteration = false;
                            StatusManager.Instance.ClearStatus("PDFOCR");
                        }

                        Thread.Sleep(500);
                    }
                }
            }
        }

        private HashSet<string> failed_pdf_group_tokens = new HashSet<string>();
        private void ProcessNextJob_Group(NextJob next_job, string temp_ocr_result_filename)
        {
            // Check that this PDF has not failed before
            string check_failed_group_token = NextJob.GetCurrentJobToken(next_job.job, next_job.is_group);
            if (!failed_pdf_group_tokens.Contains(check_failed_group_token))
            {
                // Build up the page numbers string
                string page_numbers_string;
                {
                    int page_range_start = ((next_job.job.page - 1) / next_job.job.TEXT_PAGES_PER_GROUP) * next_job.job.TEXT_PAGES_PER_GROUP + 1;
                    int page_range_end = page_range_start + next_job.job.TEXT_PAGES_PER_GROUP - 1;
                    page_range_end = Math.Min(page_range_end, next_job.job.pdf_renderer.PageCount);

                    StringBuilder sb = new StringBuilder();
                    for (int page = page_range_start; page <= page_range_end; ++page)
                    {
                        sb.Append(page);
                        sb.Append(',');
                    }
                    page_numbers_string = sb.ToString();
                    page_numbers_string = page_numbers_string.TrimEnd(',');
                }

                string ocr_parameters =
                    ""
                    + "GROUP"
                    + " "
                    + '"' + next_job.job.pdf_renderer.PDFFilename + '"'
                    + " "
                    + page_numbers_string
                    + " "
                    + '"' + temp_ocr_result_filename + '"'
                    + " "
                    + '"' + ReversibleEncryption.Instance.EncryptString(next_job.job.pdf_renderer.PDFUserPassword) + '"'
                    + " "
                    + '"' + next_job.job.language + '"'
                    ;

                if (CheckOCRProcessSuccess(ocr_parameters))
                {
                    next_job.job.pdf_renderer.StorePageTextGroup(next_job.job.page, next_job.job.TEXT_PAGES_PER_GROUP, temp_ocr_result_filename);
                }
                else
                {
                    // If the group fails, then we queue it up for single OCR attempts...
                    string new_failed_group_token = NextJob.GetCurrentJobToken(next_job.job, next_job.is_group);
                    failed_pdf_group_tokens.Add(new_failed_group_token);

                    // ... and queue it up for single OCR attempts.
                    QueueJobSingle(next_job.job);
                }
            }
            else
            {
                // Queue previously failed attempts on this PDF file for single OCR attempts.
                QueueJobSingle(next_job.job);
            }
        }

        private void ProcessNextJob_Single(NextJob next_job, string temp_ocr_result_filename)
        {
            string ocr_parameters =
                ""
                + "SINGLE"
                + " "
                + '"' + next_job.job.pdf_renderer.PDFFilename + '"'
                + " "
                + next_job.job.page
                + " "
                + '"' + temp_ocr_result_filename + '"'
                + " "
                + '"' + ReversibleEncryption.Instance.EncryptString(next_job.job.pdf_renderer.PDFUserPassword) + '"'
                + " "
                + '"' + next_job.job.language + '"'
                ;

            if (CheckOCRProcessSuccess(ocr_parameters))
            {
                next_job.job.pdf_renderer.StorePageTextSingle(next_job.job.page, temp_ocr_result_filename);
            }
            else
            {
                Logging.Error("Couldn't even perform OCR on the page, so giving up for {0}", next_job.job);

                // Store an empty file so we don't queue forever... (but only if this is not due to the application terminating)
            }
        }

        // STDOUT/STDERR
        private bool CheckOCRProcessSuccess(string ocr_parameters)
        {
            // Fire up the process            
            using (Process process = ProcessSpawning.SpawnChildProcess("QiqqaOCR.exe", ocr_parameters, ProcessPriorityClass.BelowNormal))
            {
                Stopwatch clk = Stopwatch.StartNew();
                long duration = 0;

                using (ProcessOutputReader process_output_reader = new ProcessOutputReader(process))
                {
                    // Wait a few minutes for the OCR process to exit
                    while (true)
                    {
                        duration = clk.ElapsedMilliseconds;

                        if (!Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown && !StillRunning)
                        {
                            break;
                        }
                        if (process.WaitForExit(1000))
                        {
                            break;
                        }
                        if (duration >= Constants.MAX_WAIT_TIME_MS_FOR_QIQQA_OCR_TASK_TO_TERMINATE + Constants.EXTRA_TIME_MS_FOR_WAITING_ON_QIQQA_OCR_TASK_TERMINATION)
                        {
                            break;
                        }
                    }

                    // Check that we had a clean exit
                    if (!process.HasExited || 0 != process.ExitCode)
                    {
                        bool has_exited = process.HasExited;
                        
                        if (!has_exited)
                        {
                            try
                            {
                                process.Kill();

                                // wait for the completion signal; this also helps to collect all STDERR output of the application (even while it was KILLED)
                                process.WaitForExit(1000);
                            }
                            catch (Exception ex)
                            {
                                Logging.Error(ex, "There was a problem killing the OCR process after timeout ({0} ms)", duration);
                            }
                        }

                        Logging.Error("There was a problem while running OCR with parameters: {0}\n--- Exit Code: {1}\n--- {3}\n{2}", ocr_parameters, process.ExitCode, process_output_reader.GetOutputsDumpString(), (has_exited ? $"Exit code: {process.ExitCode}" : $"Timeout: {duration} ms"));

                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        private void Shutdown()
        {
            Logging.Info("Stopping PDFTextExtractor threads");
            StillRunning = false;

            int job_queue_group_count;
            int job_queue_single_count;
            GetJobCounts(out job_queue_group_count, out job_queue_single_count);

            Logging.Debug特("PDFTextExtractor::Shutdown: flushing the queue ({0} + {1} items discarded)", job_queue_group_count, job_queue_single_count);
            FlushAllJobs();

            SafeThreadPool.QueueUserWorkItem(o =>
            {
                Logging.Info("+Stopping PDFTextExtractor threads (async)");

                bool[] done = new bool[NUM_OCR_THREADS];
                Stopwatch clk = Stopwatch.StartNew();

                while (true)
                {
                    int cnt = 0;

                    for (int i = 0; i < NUM_OCR_THREADS; ++i)
                    {
                        if (!done[i])
                        {
                            cnt++;
                            if (threads[i].Join(150))
                            {
                                done[i] = true;
                                threads[i] = null;
                                cnt--;
                            }
                        }
                    }
                    Logging.Info("Stopping PDFTextExtractor threads (async): {0} threads are pending.", cnt);
                    if (cnt == 0)
                    {
                        break;
                    }

                    // abort the threads if they're taking way too long:
                    if (clk.ElapsedMilliseconds >= Constants.MAX_WAIT_TIME_MS_AT_PROGRAM_SHUTDOWN)
                    {
                        for (int i = 0; i < NUM_OCR_THREADS; ++i)
                        {
                            if (!done[i])
                            {
                                Logging.Error("Stopping PDFTextExtractor threads (async): timeout ({1} sec), hence ABORTing PDF/OCR thread {0}.", i, Constants.MAX_WAIT_TIME_MS_AT_PROGRAM_SHUTDOWN / 1000);
                                threads[i].Abort();
                            }
                        }
                    }

                    WPFDoEvents.WaitForUIThreadActivityDone();
                }

                Logging.Info("-Stopping PDFTextExtractor threads (async) --> all done!");
            });

            Logging.Info("Stopped PDFTextExtractor");
        }
    }
}
