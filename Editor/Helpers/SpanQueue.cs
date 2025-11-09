using System;

namespace Editor.Helpers
{
    public ref struct SpanQueue<T>
    {
        public readonly Span<T> Queue;
        public          int     current; // off by 1!!!!!!!!

        public T this[int i]
        {
            get { return Queue[i]; }
            set { Queue[i] = value; }
        }

        public SpanQueue(Span<T> queue) : this()
        {
            Queue = queue;
        }

        public T Current => Queue[current - 1];

        public T Next()
        {
            current++;
            return Queue[current - 1];
        }

        public static implicit operator ReadOnlySpan<T>(SpanQueue<T> span) => span.Queue;
        public static implicit operator Span<T>(SpanQueue<T> span)         => span.Queue;
    }
}