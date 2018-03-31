using System;

namespace TheKing.Features.Context {
    class Case {
        public string Title    { get; }
        public Action Callback { get; }

        public Case(string title, Action callback) {
            Title    = title;
            Callback = callback;
        }
    }

}
