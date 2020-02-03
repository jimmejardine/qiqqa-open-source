using System;

namespace QiqqaLegacyFileFormats          // namespace QiqqaLegacyFileFormats
{

#if SAMPLE_LOAD_CODE

   public class ApplicationConfiguration
    {
        public void SaveConfigurationRecord()
        {
            try
            {
                Logging.Info("Saving configuration");
                ObjectSerializer.SaveObject(ConfigFilenameForUser, configuration_record);
                Logging.Info("Saved configuration");
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem saving the configuration.");
            }
        }
    }

#endif

}
