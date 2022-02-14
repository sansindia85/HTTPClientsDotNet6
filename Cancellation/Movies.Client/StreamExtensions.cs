//using System;
//using System.IO;
//using System.Text;
//using Newtonsoft.Json;

//namespace Movies.Client
//{
//    internal static class StreamExtensions
//    {
//        internal static T ReadAndDeserializeFromJson<T>(this Stream stream)
//        {
//            if (stream == null)
//            {
//                throw new ArgumentNullException(nameof(stream));
//            }

//            if (!stream.CanRead)
//            {
//                throw new NotSupportedException("Can't read from this stream.");
//            }

//            using (var streamReader = new StreamReader(stream))
//            {
//                using (var jsonTextReader = new JsonTextReader(streamReader))
//                {
//                    var jsonSerializer = new JsonSerializer();
//                    return jsonSerializer.Deserialize<T>(jsonTextReader);
//                }
//            }

//        }

//        internal static void SerializeToJsonAndWrite<T>(this Stream stream, T objectToWrite)
//        {
//            if (stream == null)
//            {
//                throw new ArgumentNullException(nameof(stream));
//            }

//            if (!stream.CanRead)
//            {
//                throw new NotSupportedException("Can't read from this stream.");
//            }

//            //The higher the size of the buffer. The more memory is required.
//            using (var streamWriter =
//                   new StreamWriter(stream, new UTF8Encoding(), 8192, true))
//            {
//                using (var jsonTextWriter = new JsonTextWriter(streamWriter))
//                {
//                    var jsonSerializer = new JsonSerializer();
//                    jsonSerializer.Serialize(jsonTextWriter, objectToWrite);
//                    jsonTextWriter.Flush(); //If we dont this then we will end up with an empty or incomplete stream
//                }
//            }
//        }
//    }
//}
