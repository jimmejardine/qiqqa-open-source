using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
#if !HAS_NO_PROTOBUF
using ProtoBuf;
#endif
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Utilities.Files
{
    public class SerializeFile
    {
        private static readonly string REDUNDANT = ".redundant";

        public static void SaveRedundant(string filename, object animal_to_save)
        {
            string redundant_filename = filename + REDUNDANT;
            Save(redundant_filename, animal_to_save);
            File.Delete(filename);
            File.Move(redundant_filename, filename);
        }

        public static object LoadRedundant(string filename)
        {
            try
            {
                return Load(filename);
            }
            catch (Exception ex)
            {
                // Check if there is a redundant file to fall back on
                string redundant_filename = filename + REDUNDANT;
                if (File.Exists(redundant_filename))
                {
                    File.Delete(filename);
                    File.Move(redundant_filename, filename);
                    return Load(filename);
                }
                else
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Try use the REDUNDANT version instead...
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="animal_to_save"></param>
        public static void Save(string filename, object animal_to_save)
        {
            using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, animal_to_save);
            }
        }

        public static object LoadFromStream(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Object animal_to_load = formatter.Deserialize(stream);
            return animal_to_load;
        }

        public static object LoadFromByteArray(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                return LoadFromStream(stream);
            }
        }

        public static object Load(string filename)
        {
            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return LoadFromStream(stream);
            }
        }

        public static void SaveSafely<T>(string filename, T animal_to_save)
        {
            SaveSafely(filename, (object)animal_to_save);
        }

        public static void SaveSafely(string filename, object animal_to_save)
        {
            try
            {
                SaveRedundant(filename, animal_to_save);
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Error saving {0}", filename);
            }
        }

        public static T LoadSafely<T>(string filename)
        {
            return (T)LoadSafely(filename);
        }

        public static object LoadSafely(string filename)
        {
            try
            {
                return LoadRedundant(filename);
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Error loading {0}", filename);
                return null;
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // --- The equivalent json versions
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static void JsonSave<T>(string filename, T animal_to_save)
        {
            string json = JsonConvert.SerializeObject(animal_to_save, Formatting.Indented);
            TextSave(filename, json);
        }

        public static T JsonLoad<T>(string filename)
        {
            string json = TextLoad(filename);
            T t = JsonConvert.DeserializeObject<T>(json);
            return t;
        }

        public static T JsonLoadWithNull<T>(string filename) where T : class
        {
            try
            {
                return JsonLoad<T>(filename);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Failed to parse JSON file '{0}'", filename);
                return null;
            }
        }



        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // --- The equivalent text versions
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static void TextSave(string filename, string animal_to_save)
        {
            string redundant_filename = filename + REDUNDANT;
            TextSave_NotRedundant(redundant_filename, animal_to_save);
            File.Delete(filename);
            File.Move(redundant_filename, filename);
        }

        private static void TextSave_NotRedundant(string filename, string animal_to_save)
        {
            File.WriteAllText(filename, animal_to_save);
        }

        public static string TextLoad(string filename)
        {
            try
            {
                return TextLoad_NotRedundant(filename);
            }
            catch (Exception ex)
            {
                // Check if there is a redundant file to fall back on
                string redundant_filename = filename + REDUNDANT;
                if (File.Exists(redundant_filename))
                {
                    File.Delete(filename);
                    File.Move(redundant_filename, filename);
                    return TextLoad_NotRedundant(filename);
                }
                else
                {
                    throw ex;
                }
            }
        }

        private static string TextLoad_NotRedundant(string filename)
        {
            return File.ReadAllText(filename);
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // --- The equivalent protobuf versions
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------

#if !HAS_NO_PROTOBUF

        public static void ProtoSave<T>(string filename, T animal_to_save)
        {
            string redundant_filename = filename + REDUNDANT;
            ProtoSave_NotRedundant<T>(redundant_filename, animal_to_save);
            File.Delete(filename);
            File.Move(redundant_filename, filename);
        }

        public static byte[] ProtoSaveToByteArray<T>(T animal_to_save)
        {
            MemoryStream ms = new MemoryStream();
            Serializer.Serialize<T>(ms, animal_to_save);
            return ms.ToArray();
        }

        private static void ProtoSave_NotRedundant<T>(string filename, T animal_to_save)
        {
            using (FileStream fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Serializer.Serialize<T>(fs, animal_to_save);
            }
        }

        public static T ProtoLoadWithNull<T>(string filename) where T : class
        {
            try
            {
                return ProtoLoad<T>(filename);
            }
            catch
            {
                return null;
            }
        }

        public static T ProtoLoad<T>(string filename)
        {
            try
            {
                return ProtoLoad_NotRedundant<T>(filename);
            }
            catch (Exception ex)
            {
                // Check if there is a redundant file to fall back on
                string redundant_filename = filename + REDUNDANT;
                if (File.Exists(redundant_filename))
                {
                    File.Delete(filename);
                    File.Move(redundant_filename, filename);
                    return ProtoLoad_NotRedundant<T>(filename);
                }
                else
                {
                    throw ex;
                }
            }
        }

        private static T ProtoLoad_NotRedundant<T>(string filename)
        {
            using (FileStream fs = File.OpenRead(filename))
            {
                T animal = Serializer.Deserialize<T>(fs);
                return animal;
            }
        }

        public static T ProtoLoadFromByteArray<T>(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                T animal = Serializer.Deserialize<T>(ms);
                return animal;
            }
        }

#endif

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // --- Other
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static void SaveCompressed(string filename, object animal_to_save)
        {
            using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (GZipStream compressed_stream = new GZipStream(stream, CompressionMode.Compress))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(compressed_stream, animal_to_save);
                }
            }
        }

        public static object LoadCompressed(string filename)
        {
            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (GZipStream compressed_stream = new GZipStream(stream, CompressionMode.Decompress))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    Object animal_to_load = formatter.Deserialize(compressed_stream);
                    return animal_to_load;
                }
            }
        }
    }
}
