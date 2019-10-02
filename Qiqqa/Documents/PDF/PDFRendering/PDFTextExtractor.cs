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
using Utilities.Misc;
using Utilities.ProcessTools;
using Utilities.Shutdownable;

namespace Qiqqa.Documents.PDF.PDFRendering
{
    public class PDFTextExtractor
    {
        public static PDFTextExtractor Instance = new PDFTextExtractor();

        bool still_running;
        int NUM_OCR_THREADS;
        Thread[] threads;

        private object queue_lock = new object();
        Dictionary<string, Job> job_queue_group = new Dictionary<string, Job>();
        Dictionary<string, Job> job_queue_single = new Dictionary<string, Job>();

        HashSet<string> current_jobs_group = new HashSet<string>();
        HashSet<string> current_jobs_single = new HashSet<string>();

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

                this.force_job = false;
                this.language = "";
            }

            public override string ToString()
            {
                return String.Format("PDF:{0} Page:{1}", pdf_renderer, page);
            }

            public void Clear()
            {
                this.pdf_renderer = null;
                this.language = String.Empty;
            }
        }

        class NextJob : IDisposable
        {
            PDFTextExtractor pdf_text_extractor;

            public Job job;
            public bool is_group;

            internal NextJob(PDFTextExtractor pdf_text_extractor, Job job, bool is_group, object queue_lock_reminder)
            {
                this.pdf_text_extractor = pdf_text_extractor;
                this.job = job;
                this.is_group = is_group;

                pdf_text_extractor.RecordThatJobHasStarted_LOCK(this, queue_lock_reminder);
            }

            ~NextJob()
            {
                Logging.Info("~NextJob()");
                Dispose(false);
            }

            public void Dispose()
            {
                Logging.Info("Disposing NextJob");
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private int dispose_count = 0;
            private void Dispose(bool disposing)
            {
                Logging.Debug("NextJob::Dispose({0}) @{1}", disposing ? "true" : "false", ++dispose_count);
                if (disposing)
                {
                    // Notify that this job is done...
                    pdf_text_extractor.RecordThatJobHasCompleted(this);

                    //job?.Clear();
                }

                pdf_text_extractor = null;
                job = null;

                // Get rid of unmanaged resources 
            }

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
            /// <param name="force_is_group"></param>
            /// <returns></returns>
            public static string GetCurrentJobToken(Job job, bool is_group, bool force_is_group)
            {
                if (force_is_group || is_group)
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

        PDFTextExtractor()
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

            Logging.Info("Starting {0} PDFTextExtractor threads", NUM_OCR_THREADS);
            still_running = true;
            threads = new Thread[NUM_OCR_THREADS];
            for (int i = 0; i < NUM_OCR_THREADS; ++i)
            {
                threads[i] = new Thread(ThreadEntry);
                threads[i].Priority = ThreadPriority.BelowNormal;
                threads[i].Name = "PDFTextExtractor";
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
            string token = NextJob.GetCurrentJobToken(next_job.job, next_job.is_group, false);
            HashSet<string> current_jobs = next_job.is_group ? current_jobs_group : current_jobs_single;
            if (current_jobs.Contains(token))
            {
                Logging.Error("Job is already running: " + token);
            }
            current_jobs.Add(token);
        }

        private void RecordThatJobHasCompleted(NextJob next_job)
        {
            string token = NextJob.GetCurrentJobToken(next_job.job, next_job.is_group, false);

            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (queue_lock)
            {
                l1_clk.LockPerfTimerStop();
                HashSet<string> current_jobs = next_job.is_group ? current_jobs_group : current_jobs_single;
                if (!current_jobs.Contains(token))
                {
                    Logging.Error("Job is not running, so can't remove it: " + token);
                }
                current_jobs.Remove(token);
            }
        }

        private bool IsSimilarJobRunning(Job job, bool is_group, object queue_lock_REMINDER)
        {
            // Check if a similar group job is running
            if (current_jobs_group.Contains(NextJob.GetCurrentJobToken(job, is_group, true))) return true;

            // If this is a single job, check if a similar single job is running
            if (!is_group)
            {
                if (current_jobs_single.Contains(NextJob.GetCurrentJobToken(job, is_group, false))) return true;
            }

            // No similar job is running...
            return false;
        }


        private DateTime ocr_disabled_next_notification_time = DateTime.MinValue;
        private NextJob GetNextJob()
        {
            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (queue_lock)
            {
                l1_clk.LockPerfTimerStop();

                // Check if OCR is disabled
                if (!ConfigurationManager.Instance.ConfigurationRecord.Library_OCRDisabled)
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

                    // Don't bother with these single page priorities when the queue is large/huge:
                    if (200 >= job_queue_single.Count)
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
                }
            }

            // Check if OCR is disabled
            if (ConfigurationManager.Instance.ConfigurationRecord.Library_OCRDisabled)
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
                        StatusManager.Instance.UpdateStatus("PDFOCR", String.Format("OCR is disabled ({0} page(s) still to OCR)", job_queue_single_count));
                        ocr_disabled_next_notification_time = DateTime.UtcNow.AddSeconds(5);
                    }
                }
            }

            // There are no jobs! Or we're not allowed to return any jobs if disabled!
            return null;
        }

        void ThreadEntry()
        {
            bool did_some_ocr_since_last_iteration = false;

            while (still_running)
            {
                // Get a count of how many jobs are left...
                int job_queue_group_count;
                int job_queue_single_count;
                GetJobCounts(out job_queue_group_count, out job_queue_single_count);

                int job_queue_total_count = job_queue_group_count + job_queue_single_count;

                if ((0 < job_queue_group_count || 0 < job_queue_single_count) && Library.IsBusyAddingPDFs)
                {
                    StatusManager.Instance.UpdateStatus("PDFOCR", "OCR paused while adding documents.");
                    Thread.Sleep(1000);
                    continue;
                }

                using (NextJob next_job = GetNextJob())
                {
                    if (null != next_job)
                    {
                        did_some_ocr_since_last_iteration = true;

                        Logging.Info("Doing OCR for job '{0}'", next_job.job);
                        StatusManager.Instance.UpdateStatus("PDFOCR", String.Format("{0} page(s) to textify and {1} page(s) to OCR.", job_queue_group_count, job_queue_single_count), 1, job_queue_total_count);

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
            string check_failed_group_token = NextJob.GetCurrentJobToken(next_job.job, next_job.is_group, false);
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

                int SECONDS_TO_WAIT = 60;  // MAKE SURE THIS NUMBER IS LARGER THAN THE NUMBER IN THE ACTUAL QiqqaOCR so that QiqqaOCR has time to finish up...!

                if (CheckOCRProcessSuccess(ocr_parameters, SECONDS_TO_WAIT))
                {
                    next_job.job.pdf_renderer.StorePageTextGroup(next_job.job.page, next_job.job.TEXT_PAGES_PER_GROUP, temp_ocr_result_filename);
                }
                else
                {
                    // If the group fails, then we queue it up for single OCR attempts...
                    string new_failed_group_token = NextJob.GetCurrentJobToken(next_job.job, next_job.is_group, false);
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

            int SECONDS_TO_WAIT = 210;  // MAKE SURE THIS NUMBER IS LARGER THAN THE NUMBER IN THE ACTUAL QiqqaOCR so that QiqqaOCR has time to finish up...!

            if (CheckOCRProcessSuccess(ocr_parameters, SECONDS_TO_WAIT))
            {
                next_job.job.pdf_renderer.StorePageTextSingle(next_job.job.page, temp_ocr_result_filename);
            }
            else
            {
                Logging.Error("Couldn't even perform OCR on the page, so giving up for " + next_job.job);

                // Store an empty file so we don't queue forever...

            }
        }

        bool CheckOCRProcessSuccess(string ocr_parameters, int SECONDS_TO_WAIT)
        {
            // Fire up the process            
            using (Process process = ProcessSpawning.SpawnChildProcess("QiqqaOCR.exe", ocr_parameters, ProcessPriorityClass.BelowNormal))
            {
                DateTime process_start_time = DateTime.UtcNow;

                using (ProcessOutputReader process_output_reader = new ProcessOutputReader(process))
                {
                    // Wait a few minutes for the OCR process to exit
                    while (true)
                    {
                        if (!still_running)
                        {
                            break;
                        }
                        if (process.WaitForExit(500))
                        {
                            break;
                        }
                        if (DateTime.UtcNow.Subtract(process_start_time).TotalSeconds >= SECONDS_TO_WAIT)
                        {
                            break;
                        }
                    }

                    // Check that we had a clean exit
                    if (!process.HasExited || 0 != process.ExitCode)
                    {
                        Logging.Error("There was a problem while running OCR with parameters: {0}", ocr_parameters);
                        Logging.Info("Parameters: {0}", ocr_parameters);
                        Logging.Info(process_output_reader.GetOutputsDumpString());

                        if (!process.HasExited)
                        {
                            try
                            {
                                process.Kill();
                            }
                            catch (Exception ex)
                            {
                                Logging.Error(ex, "There was a problem killing the OCR process");
                            }
                        }

                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        void Shutdown()
        {
            Logging.Info("Stopping PDFTextExtractor threads");
            still_running = false;
            for (int i = 0; i < NUM_OCR_THREADS; ++i)
            {
                threads[i].Join();
            }
            Logging.Info("Stopped PDFTextExtractor");
        }
    }
}
