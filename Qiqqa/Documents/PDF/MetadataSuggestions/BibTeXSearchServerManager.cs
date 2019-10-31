using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Random;

namespace Qiqqa.Documents.PDF.MetadataSuggestions
{
    class BibTeXSearchServerManager
    {
        class ServerRecord
        {
            public string url;
            public double latency_ms = 1000;
            public DateTime search_failure_backoff_time = DateTime.MinValue;
        }

        private Dictionary<string, ServerRecord> server_records = new Dictionary<string, ServerRecord>();

        public BibTeXSearchServerManager()
        {
            for (int i = 1; i <= 4; ++i)
            {
                // WHOIS search1.bibtexsearch.com ==> owned by James Jardine. See also www.jimme.net
                string url = String.Format(WebsiteAccess.Url_BibTeXSearchServerN, i);
                EnsureServerRecordExists(url);
            }
        }

        private void EnsureServerRecordExists(string url)
        {
            if (!server_records.ContainsKey(url))
            {
                ServerRecord server_record = new ServerRecord();
                server_record.url = url;
                server_records[server_record.url] = server_record;
            }
        }

        private IEnumerable<ServerRecord> GetWorkingServerRecords()
        {
            var NOW = DateTime.UtcNow;
            return server_records.Values.Where(o => o.search_failure_backoff_time < NOW);
        }

        public bool MustBackoff()
        {
            return !GetWorkingServerRecords().Any();
        }

        public string GetServerUrl()
        {
            if (!String.IsNullOrEmpty(RegistrySettings.Instance.Read(RegistrySettings.BibTeXSearchSearchUrl))) return RegistrySettings.Instance.Read(RegistrySettings.BibTeXSearchSearchUrl);

            IEnumerable<ServerRecord> working_server_records = GetWorkingServerRecords();

            if (!working_server_records.Any())
            {
                throw new GenericException("There are no viable bibtexsearch servers...");
            }

            // Get the minimum latency
            double minimum_latency_ms = Double.MaxValue;
            foreach (var server_record in working_server_records)
            {
                minimum_latency_ms = Math.Min(minimum_latency_ms, server_record.latency_ms);
            }

            // Get the total normalised latency
            double total_normalised_latency_ms = 0;
            foreach (var server_record in working_server_records)
            {
                total_normalised_latency_ms += (minimum_latency_ms / server_record.latency_ms);
            }

            // Get a random offset latency and random record
            double cutoff_normalised_latency_ms = RandomAugmented.Instance.NextDouble(total_normalised_latency_ms);
            foreach (var server_record in working_server_records)
            {
                cutoff_normalised_latency_ms -= (minimum_latency_ms / server_record.latency_ms);
                if (0 >= cutoff_normalised_latency_ms) return server_record.url;
            }

            // If we get here there was double precision error and we want the last one...
            return working_server_records.Last().url;
        }

        public void ReportError(string url)
        {
            EnsureServerRecordExists(url);
            server_records[url].search_failure_backoff_time = DateTime.UtcNow.AddMinutes(10);
        }

        public void ReportLatency(string url, double latency_ms)
        {
            EnsureServerRecordExists(url);

            // Speedups affect the moving average faster than slowdowns
            if (latency_ms < server_records[url].latency_ms)
            {
                server_records[url].latency_ms = 0.2 * server_records[url].latency_ms + 0.8 * latency_ms;
            }
            else
            {
                server_records[url].latency_ms = 0.5 * server_records[url].latency_ms + 0.5 * latency_ms;
            }

            if (1 > server_records[url].latency_ms)
            {
                Logging.Warn("Reported bibtexsearch latency is sub millisecond.  Weird...");
                server_records[url].latency_ms = 1;
            }
        }
    }
}
