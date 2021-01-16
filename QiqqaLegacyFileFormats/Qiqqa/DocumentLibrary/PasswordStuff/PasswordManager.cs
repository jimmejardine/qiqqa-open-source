using System;
using System.Collections.Generic;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace QiqqaLegacyFileFormats          // namespace Qiqqa.DocumentLibrary.PasswordStuff
{

#if SAMPLE_LOAD_CODE

    public class PasswordManager
    {
        private TypedWeakReference<WebLibraryDetail> web_library_detail;
        public WebLibraryDetail LibraryRef => web_library_detail?.TypedTarget;

        public PasswordManager(WebLibraryDetail web_library_detail)
        {
            this.web_library_detail = new TypedWeakReference<WebLibraryDetail>(web_library_detail);
        }

        public string Filename_Store => Path.GetFullPath(Path.Combine(LibraryRef.LIBRARY_BASE_PATH, @"Qiqqa.pwds"));

        private Dictionary<string, string> _passwords = null;

        private Dictionary<string, string> Passwords
        {
            get
            {
                // Check that it is already loaded
                if (null == _passwords)
                {
                    // Attempt to load it
                    try
                    {
                        if (File.Exists(Filename_Store))
                        {
                            _passwords = (Dictionary<string, string>)SerializeFile.LoadRedundant(Filename_Store);
                            return _passwords;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Warn(ex, "There was a problem loading the password file, so starting a new one...");
                    }

                    // If we get here there are no passwords...
                    return new Dictionary<string, string>();
                }

                return _passwords;
            }
        }

        // ------------------------------------------------------------------------------------------------------------

        private void WritePasswordFile(string filename, Dictionary<string, string> passwords)
        {
            Logging.Info("Writing password file {0}.", filename);
            SerializeFile.SaveRedundant(filename, passwords);
        }

        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Associates a password with the PDFDocument.
        /// Pass in numm or empty string to clear the password.
        /// </summary>
        /// <param name="pdf_document"></param>
        /// <param name="password"></param>
        public void AddPassword(PDFDocument_ThreadUnsafe pdf_document, string password)
        {
            if (null == pdf_document)
            {
                Logging.Warn("Can't associate a password with a null PDFDocument.");
            }

            if (String.IsNullOrEmpty(password))
            {
                RemovePassword(pdf_document);
            }
            else
            {
                ReversibleEncryption re = new ReversibleEncryption();
                Passwords[pdf_document.Fingerprint] = re.EncryptString(password);
                WritePasswordFile(Filename_Store, Passwords);
            }
        }

        /// <summary>
        /// Gets the password associated with the PDFDocument.  Returns null if there is no associated password.
        /// </summary>
        /// <param name="pdf_document"></param>
        /// <returns></returns>
        public string GetPassword(PDFDocument_ThreadUnsafe pdf_document)
        {
            string fingerprint = pdf_document.Fingerprint;
            if (Passwords.ContainsKey(fingerprint))
            {
                ReversibleEncryption re = new ReversibleEncryption();
                return re.DecryptString(Passwords[fingerprint]);
            }
            else
            {
                return null;
            }
        }

        public void RemovePassword(PDFDocument_ThreadUnsafe pdf_document)
        {
            Passwords.Remove(pdf_document.Fingerprint);
            WritePasswordFile(Filename_Store, Passwords);
        }
    }

#endif

}

