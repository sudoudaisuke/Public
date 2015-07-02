using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEImageDebugSample {
    class EnumerableExtention<T> : IEnumerable<T> {
        private readonly IEnumerable<T> _self;
        private readonly Func<Exception, bool> _onError;

        public EnumerableExtention(IEnumerable<T> target, Func<Exception, bool> onError) {
            if (target == null) {
                throw new ArgumentNullException("target");
            }
            _self = target;
            _onError = onError;
        }

        public IEnumerator<T> GetEnumerator() {
            var enumrator = new EnumeratorExtention<T>(_self.GetEnumerator(), _onError);
            return enumrator;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            var enumrator = new EnumeratorExtention<T>(_self.GetEnumerator(), _onError);
            return enumrator;
        }
    }

    class EnumeratorExtention<T> : IEnumerator<T> {
        private readonly IEnumerator<T> _self;
        private readonly Func<Exception, bool> _onError;

        public EnumeratorExtention(IEnumerator<T> target, Func<Exception, bool> onError) {
            if (target == null) {
                throw new ArgumentNullException("target");
            }
            _self = target;
            _onError = onError;
        }

        public T Current {
            get {
                return _self.Current;
            }
        }

        public void Dispose() {
            _self.Dispose();
        }

        object System.Collections.IEnumerator.Current {
            get {
                return _self.Current;
            }
        }

        public bool MoveNext() {
            try {
                return _self.MoveNext();
            } catch (Exception ex) {
                if (_onError != null) {
                    var br = _onError(ex);
                    if (!br) {
                        throw;
                    }
                }
                return _self.MoveNext();
            }
        }

        public void Reset() {
            _self.Reset();
        }
    }
}
