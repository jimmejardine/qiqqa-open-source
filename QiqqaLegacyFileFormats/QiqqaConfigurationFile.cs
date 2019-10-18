using System;

namespace QiqqaLegacyFileFormats
{
    public class ApplicationConfigurationX
    {
 public void SaveConfigurationRecord()
        {
#if false
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
#endif
        }
    }
}
