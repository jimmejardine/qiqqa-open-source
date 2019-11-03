#if false

using System;

namespace Qiqqa.Synchronisation
{
    internal class SyncParameters
    {
        int library_size_maximum;
        int file_size_maximum;

        public SyncParameters(int library_size_maximum, int file_size_maximum)
        {
            this.library_size_maximum = library_size_maximum;
            this.file_size_maximum = file_size_maximum;
        }

        internal double SYNCABLE_FILE_SIZE_MAXIMUM
        {
            get
            {
                return file_size_maximum * 1024 * 1024;
            }
        }

        internal double LIBRARY_SIZE_MAXIMUM
        {
            get
            {
                return library_size_maximum * 1024 * 1024;
            }
        }

        internal double LIBRARY_SIZE_WARNING
        {
            get
            {
                return 0.75 * LIBRARY_SIZE_MAXIMUM;
            }
        }

        internal string LIBRARY_SIZE_MAXIMUM_IN_MB
        {
            get
            {
                return String.Format("{0}Mb", library_size_maximum);
            }
        }

        internal string SYNCABLE_FILE_SIZE_MAXIMUM_IN_MB
        {
            get
            {
                return String.Format("{0}Mb", file_size_maximum);
            }
        }
    }
}

#endif
