using System;
using System.Collections.Generic;
using System.IO;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.Encryption;
using Utilities.Files;

namespace Qiqqa.DocumentLibrary.PasswordStuff
{
    public class PasswordManager
    {
        Library library;

        public PasswordManager(Library library)
        {
            this.library = library;
        }

        public string Filename_Store
        {
            get
            {
                return library.LIBRARY_BASE_PATH + "Qiqqa.pwds";
            }
        }

        Dictionary<string, string> _passwords = null;
        Dictionary<string, string> Passwords
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

        static void WritePasswordFile(string filename, Dictionary<string, string> passwords)
        {
            Logging.Info("Writing password file.");
            SerializeFile.SaveRedundant(filename, passwords);
        }

        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Associates a password with the PDFDocument.
        /// Pass in numm or empty string to clear the password.
        /// </summary>
        /// <param name="pdf_document"></param>
        /// <param name="password"></param>
        public void AddPassword(PDFDocument pdf_document, string password)
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
        public string GetPassword(PDFDocument pdf_document)
        {
            if (Passwords.ContainsKey(pdf_document.Fingerprint))
            {
                ReversibleEncryption re = new ReversibleEncryption();
                return re.DecryptString(Passwords[pdf_document.Fingerprint]);
            }
            else
            {
                return null;
            }
        }

        public void RemovePassword(PDFDocument pdf_document)
        {
            Passwords.Remove(pdf_document.Fingerprint);
            WritePasswordFile(Filename_Store, Passwords);
        }
    }   
}

