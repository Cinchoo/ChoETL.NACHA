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

        public ChoNACHAReader(string filePath, Encoding encoding = null, int bufferSize = 2048)
        {
            ChoGuard.ArgumentNotNullOrEmpty(filePath, "FilePath");
            Init();

            _streamReader = new StreamReader(ChoPath.GetFullPath(filePath), encoding == null ? Encoding.Default : encoding, false, bufferSize);
            _closeStreamOnDispose = true;
        }

        public ChoNACHAReader(StreamReader streamReader)
        {
            ChoGuard.ArgumentNotNull(streamReader, "StreamReader");
            Init();

            _streamReader = streamReader;
        }

        public ChoNACHAReader(Stream inStream, Encoding encoding = null, int bufferSize = 2048)
        {
            ChoGuard.ArgumentNotNull(inStream, "Stream");
            Init();

            _streamReader = new StreamReader(inStream, encoding == null ? Encoding.Default : encoding, false, bufferSize);
            _closeStreamOnDispose = true;
        }

        private void Init()
        {
            _enumerator = new Lazy<IEnumerator>(() => GetEnumerator());
        }

        public object Read()
        {
            if (_enumerator.Value.MoveNext())
                return _enumerator.Value.Current;
            else
                return null;
        }

        public void Dispose()
        {
            if (_closeStreamOnDispose)
                _streamReader.Dispose();
        }

        public IEnumerator GetEnumerator()
        {
            ChoManifoldReader reader = new ChoManifoldReader(_streamReader).WithRecordSelector(0, 1, typeof(ChoBatchHeaderRecord), typeof(ChoBatchControlRecord),
               typeof(ChoFileHeaderRecord), typeof(ChoFileControlRecord), typeof(ChoEntryDetailRecord), typeof(ChoAddendaRecord));

            bool batchEnd = false;
            object state = null;
            return ChoEnumeratorWrapper.BuildEnumerable<object>(() =>
            {
                state = reader.Read();
                batchEnd = state is ChoBatchControlRecord;

                return state != null && !batchEnd;
            }, () => state).GetEnumerator();
        }
    }
}
