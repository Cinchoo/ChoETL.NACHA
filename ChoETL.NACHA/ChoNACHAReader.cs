using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoETL.NACHA
{
    public class ChoNACHAReader : IDisposable, IEnumerable
    {
        private StreamReader _streamReader;
        private bool _closeStreamOnDispose = false;
        private Lazy<IEnumerator> _enumerator = null;

        public ChoNACHAConfiguration Configuration
        {
            get;
            private set;
        }

        public ChoNACHAReader(string filePath, ChoNACHAConfiguration configuration = null)
        {
            ChoGuard.ArgumentNotNullOrEmpty(filePath, "FilePath");
            Configuration = configuration;
            Init();

            _streamReader = new StreamReader(ChoPath.GetFullPath(filePath), Configuration.Encoding, false, Configuration.BufferSize);
            _closeStreamOnDispose = true;
        }

        public ChoNACHAReader(StreamReader streamReader, ChoNACHAConfiguration configuration = null)
        {
            ChoGuard.ArgumentNotNull(streamReader, "StreamReader");
            Configuration = configuration;
            Init();

            _streamReader = streamReader;
        }

        public ChoNACHAReader(Stream inStream, ChoNACHAConfiguration configuration = null)
        {
            ChoGuard.ArgumentNotNull(inStream, "Stream");
            Configuration = configuration;
            Init();

            _streamReader = new StreamReader(inStream, Configuration.Encoding, false, Configuration.BufferSize);
            _closeStreamOnDispose = true;
        }

        private void Init()
        {
            _enumerator = new Lazy<IEnumerator>(() => GetEnumerator());
            if (Configuration == null)
                Configuration = new ChoNACHAConfiguration();
        }

        public object Read()
        {
            if (_enumerator.Value.MoveNext())
                return _enumerator.Value.Current;
            else
                return null;
        }

        public static ChoNACHAReader LoadText(string inputText, ChoNACHAConfiguration configuration = null)
        {
            var r = new ChoNACHAReader(inputText.ToStream(), configuration);
            r._closeStreamOnDispose = true;

            return r;
        }

        public void Dispose()
        {
            if (_closeStreamOnDispose)
                _streamReader.Dispose();
        }

        public IEnumerator GetEnumerator()
        {
            ChoManifoldReader reader = new ChoManifoldReader(_streamReader, Configuration as ChoManifoldRecordConfiguration).WithRecordSelector(0, 1, typeof(ChoBatchHeaderRecord), typeof(ChoBatchControlRecord),
               typeof(ChoFileHeaderRecord), typeof(ChoFileControlRecord), typeof(ChoEntryDetailRecord), typeof(ChoAddendaRecord));

            object state = null;
            return ChoEnumeratorWrapper.BuildEnumerable<object>(() => (state = reader.Read()) != null, () => state).GetEnumerator();
        }
    }
}
