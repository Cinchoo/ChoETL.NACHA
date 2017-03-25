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

            _streamReader = new StreamReader(ChoPath.GetFullPath(filePath), Configuration.GetEncoding(filePath), false, Configuration.BufferSize);
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

            _streamReader = new StreamReader(inStream, Configuration.GetEncoding(inStream), false, Configuration.BufferSize);
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
            ChoManifoldReader reader = new ChoManifoldReader(_streamReader, Configuration as ChoManifoldRecordConfiguration).WithRecordSelector(0, 1, typeof(ChoNACHABatchHeaderRecord), typeof(ChoNACHABatchControlRecord),
               typeof(ChoNACHAFileHeaderRecord), typeof(ChoNACHAFileControlRecord), typeof(ChoNACHAEntryDetailRecord), typeof(ChoNACHAAddendaRecord));

            reader.Configuration.ObjectValidationMode = ChoObjectValidationMode.ObjectLevel;
            object state = null;
            return ChoNACHAEnumeratorWrapper.BuildEnumerable<object>(() => (state = reader.Read()) != null, () => state).GetEnumerator();
        }

        public class ChoNACHAEnumeratorWrapper
        {
            public static IEnumerable<T> BuildEnumerable<T>(
                    Func<bool> moveNext, Func<T> current)
            {
                var po = new ChoEnumeratorWrapperInternal<T>(moveNext, current);
                foreach (var s in po)
                    yield return s;
            }

            private class ChoEnumeratorWrapperInternal<T>
            {
                private readonly Func<bool> _moveNext;
                private readonly Func<T> _current;
                private bool _isFileControlRecordFound = false;

                public ChoEnumeratorWrapperInternal(Func<bool> moveNext, Func<T> current)
                {
                    ChoGuard.ArgumentNotNull(moveNext, "MoveNext");
                    ChoGuard.ArgumentNotNull(current, "Current");

                    _moveNext = moveNext;
                    _current = current;
                }

                public ChoEnumeratorWrapperInternal<T> GetEnumerator()
                {
                    return this;
                }

                public bool MoveNext()
                {
                    if (_isFileControlRecordFound)
                        return false;
                    else
                        return _moveNext();
                }

                public T Current
                {
                    get
                    {
                        var ret = _current();
                        _isFileControlRecordFound = ret is ChoNACHAFileControlRecord;
                        return ret;
                    }
                }
            }
        }
    }
}
