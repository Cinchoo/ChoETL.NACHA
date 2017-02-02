using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoETL.NACHA
{
    public class ChoNACHAWriter : IDisposable
    {
        private StreamWriter _streamWriter;
        private bool _closeStreamOnDispose = false;
        private ChoManifoldWriter _writer;

        public ChoNACHAConfiguration Configuration
        {
            get;
            private set;
        }

        public ChoNACHAWriter(string filePath, ChoNACHAConfiguration configuration = null)
        {
            ChoGuard.ArgumentNotNullOrEmpty(filePath, "FilePath");
            Configuration = configuration;
            Init();

            _streamWriter = new StreamWriter(ChoPath.GetFullPath(filePath), false, Configuration.Encoding, Configuration.BufferSize);
            _closeStreamOnDispose = true;
        }

        public ChoNACHAWriter(StreamWriter streamWriter, ChoNACHAConfiguration configuration = null)
        {
            ChoGuard.ArgumentNotNull(streamWriter, "StreamWriter");
            Configuration = configuration;
            Init();
            _streamWriter = streamWriter;
        }

        public ChoNACHAWriter(Stream inStream, ChoNACHAConfiguration configuration = null)
        {
            ChoGuard.ArgumentNotNull(inStream, "Stream");
            Configuration = configuration;
            Init();

            _streamWriter = new StreamWriter(inStream, Configuration.Encoding, Configuration.BufferSize);
            _closeStreamOnDispose = true;
        }

        private void Init()
        {
            if (Configuration == null)
                Configuration = new ChoNACHAConfiguration();

            _writer = new ChoManifoldWriter(_streamWriter, Configuration as ChoManifoldRecordConfiguration).WithRecordSelector(0, 1, typeof(ChoBatchHeaderRecord), typeof(ChoBatchControlRecord),
               typeof(ChoFileHeaderRecord), typeof(ChoFileControlRecord), typeof(ChoEntryDetailRecord), typeof(ChoAddendaRecord));
        }

        public void Write(IEnumerable<ChoEntryDetailRecord> records)
        {
            _writer.Write(records);
        }

        public void Write(ChoEntryDetailRecord record)
        {
            _writer.Write(record);
        }

        public static string ToText(IEnumerable<ChoEntryDetailRecord> records, ChoNACHAConfiguration configuration = null)
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var parser = new ChoNACHAWriter(writer, configuration))
            {
                parser.Write(records);

                writer.Flush();
                stream.Position = 0;

                return reader.ReadToEnd();
            }
        }

        public static string ToText(ChoEntryDetailRecord record, ChoNACHAConfiguration configuration = null)
        {
            return ToText(ChoEnumerable.AsEnumerable(record), configuration);
        }

        public void Dispose()
        {
            if (_closeStreamOnDispose)
                _streamWriter.Dispose();
        }
    }
}
