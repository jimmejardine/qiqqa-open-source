using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Expedition
{

#if SAMPLE_LOAD_CODE

    public static class ExpeditionBuilder
    {
        private const int MAX_TOPIC_ITERATIONS = 30;

        public static ExpeditionDataSource BuildExpeditionDataSource(Library library, int num_topics, bool add_autotags, bool add_tags)
        {
            // Initialise the datasource
            ExpeditionDataSource data_source = new ExpeditionDataSource();

            data_source.date_created = DateTime.UtcNow;

            //...

            return data_source;
        }
    }

#endif

}
