// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Memory.Internals
{
    internal class SharedArrayPoolBuffer<T> : ManagedBufferBase<T>, IRefCounted
        where T : struct
    {
        private readonly int lengthInBytes;
        private LifetimeGuard lifetimeGuard;

        public SharedArrayPoolBuffer(int lengthInElements)
        {
            this.lengthInBytes = lengthInElements * Unsafe.SizeOf<T>();
            this.Array = ArrayPool<byte>.Shared.Rent(this.lengthInBytes);
            this.lifetimeGuard = new LifetimeGuard(this.Array);
        }

        public byte[] Array { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (this.Array == null)
            {
                return;
            }

            this.lifetimeGuard.Dispose();
            this.Array = null;
        }

        public override Span<T> GetSpan()
        {
            this.CheckDisposed();
            return MemoryMarshal.Cast<byte, T>(this.Array.AsSpan(0, this.lengthInBytes));
        }

        protected override object GetPinnableObject() => this.Array;

        public void AddRef()
        {
            this.CheckDisposed();
            this.lifetimeGuard.AddRef();
        }

        public void ReleaseRef() => this.lifetimeGuard.ReleaseRef();

        [Conditional("DEBUG")]
        private void CheckDisposed()
        {
            if (this.Array == null)
            {
                throw new ObjectDisposedException("SharedArrayPoolBuffer");
            }
        }

        private sealed class LifetimeGuard : RefCountedMemoryLifetimeGuard
        {
            private byte[] array;

            public LifetimeGuard(byte[] array) => this.array = array;

            protected override void Release()
            {
                // If this is called by a finalizer, we will end storing the first array of this bucket
                // on the thread local storage of the finalizer thread.
                // This is not ideal, but subsequent leaks will end up returning arrays to per-cpu buckets,
                // meaning likely a different bucket than it was rented from,
                // but this is PROBABLY better than not returning the arrays at all.
                ArrayPool<byte>.Shared.Return(this.array);
                this.array = null;
            }
        }
    }
}
