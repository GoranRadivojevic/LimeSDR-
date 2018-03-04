using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PowerSDR
{
    public unsafe sealed class UnsafeBuffer : IDisposable
    {
        private readonly GCHandle _handle;
        private void* _ptr;
        private int _length;
        private Array _buffer;

        private UnsafeBuffer(Array buffer, int realLength, bool aligned)
        {
            _buffer = buffer;
            _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
            _ptr = (void*)_handle.AddrOfPinnedObject();
            if (aligned)
            {
                _ptr = (void*)(((long)_ptr + 15) & ~15);
            }
            _length = realLength;
        }

        ~UnsafeBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_handle.IsAllocated)
            {
                _handle.Free();
            }
            _buffer = null;
            _ptr = null;
            _length = 0;
            GC.SuppressFinalize(this);
        }

        public void* Address
        {
            get { return _ptr; }
        }

        public int Length
        {
            get { return _length; }
        }

        public static implicit operator void*(UnsafeBuffer unsafeBuffer)
        {
            return unsafeBuffer.Address;
        }

        public static UnsafeBuffer Create(int size)
        {
            return Create(1, size, true);
        }

        public static UnsafeBuffer Create(int length, int sizeOfElement)
        {
            return Create(length, sizeOfElement, true);
        }

        public static UnsafeBuffer Create(int length, int sizeOfElement, bool aligned)
        {
            var buffer = new byte[length * sizeOfElement + (aligned ? 16 : 0)];
            return new UnsafeBuffer(buffer, length, aligned);
        }

        public static UnsafeBuffer Create(Array buffer)
        {
            return new UnsafeBuffer(buffer, buffer.Length, false);
        }
    }

    public class Decimator
    {
        public struct Complex
        {
            public float Real;
            public float Imag;

            public Complex(float real, float imaginary)
            {
                Real = real;
                Imag = imaginary;
            }

            public Complex(Complex c)
            {
                Real = c.Real;
                Imag = c.Imag;
            }

            public static Complex ComplexAdd(Complex xy, Complex uv)
            {
                return new Complex(xy.Real + uv.Real, xy.Imag + uv.Imag);
            }

            public static Complex ComplexSubtract(Complex xy, Complex uv)
            {
                return new Complex(xy.Real - uv.Real, xy.Imag - uv.Imag);
            }

            public float Modulus()
            {
                return (float)Math.Sqrt(ModulusSquared());
            }

            public float ModulusSquared()
            {
                return Real * Real + Imag * Imag;
            }

            public float Argument()
            {
                return (float)Math.Atan2(Imag, Real);
            }

            public float FastArgument()
            {
                return (float)Math.Atan2((float)Imag, (float)Real);
            }

            public Complex Conjugate()
            {
                return new Complex(Real, -Imag);
            }

            public override string ToString()
            {
                return string.Format("real {0}, imag {1}", Real, Imag);
            }

            public static bool operator ==(Complex leftHandSide, Complex rightHandSide)
            {
                if (leftHandSide.Real != rightHandSide.Real)
                {
                    return false;
                }
                return (leftHandSide.Imag == rightHandSide.Imag);
            }

            public static bool operator !=(Complex leftHandSide, Complex rightHandSide)
            {
                if (leftHandSide.Real != rightHandSide.Real)
                {
                    return true;
                }
                return (leftHandSide.Imag != rightHandSide.Imag);
            }

            public static Complex operator +(Complex a, Complex b)
            {
                return new Complex(a.Real + b.Real, a.Imag + b.Imag);
            }

            public static Complex operator -(Complex a, Complex b)
            {
                return new Complex(a.Real - b.Real, a.Imag - b.Imag);
            }

            public static Complex operator *(Complex a, Complex b)
            {
                return new Complex(a.Real * b.Real - a.Imag * b.Imag,
                                   a.Imag * b.Real + a.Real * b.Imag);
            }

            public static Complex operator *(Complex a, float b)
            {
                return new Complex(a.Real * b, a.Imag * b);
            }

            public static Complex operator /(Complex a, Complex b)
            {
                var dn = b.Real * b.Real + b.Imag * b.Imag;
                dn = 1.0f / dn;
                var re = (a.Real * b.Real + a.Imag * b.Imag) * dn;
                var im = (a.Imag * b.Real - a.Real * b.Imag) * dn;
                return new Complex(re, im);
            }

            public static Complex operator /(Complex a, float b)
            {
                b = 1f / b;
                return new Complex(a.Real * b, a.Imag * b);
            }

            public static Complex operator ~(Complex a)
            {
                return a.Conjugate();
            }

            public static implicit operator Complex(float d)
            {
                return new Complex(d, 0);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Real.GetHashCode() * 397) ^ Imag.GetHashCode();
                }
            }

            public bool Equals(Complex obj)
            {
                return obj.Real == Real && obj.Imag == Imag;
            }

            public override bool Equals(object obj)
            {
                if (obj.GetType() != typeof(Complex)) return false;
                return Equals((Complex)obj);
            }
        }

        public unsafe interface IFilter
        {
            void Process(float* buffer, int length);
            void ProcessInterleaved(float* buffer, int length);
        }

#if !__MonoCS__
        [StructLayout(LayoutKind.Sequential, Pack = 16)]
#endif
        public unsafe sealed class FirFilter : IDisposable, IFilter
        {
            [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
            public static extern void* Memcpy(void* dest, void* src, int len);

            private const double Epsilon = 1e-6;
            private const int CircularBufferSize = 4;

            private float* _coeffPtr;
            private UnsafeBuffer _coeffBuffer;

            private float* _queuePtr;
            private UnsafeBuffer _queueBuffer;

            private int _queueSize;
            private int _offset;
            private bool _isSymmetric;
            private bool _isSparse;

            public FirFilter()
                : this(new float[0])
            {
            }

            public FirFilter(float[] coefficients)
            {
                SetCoefficients(coefficients);
            }

            ~FirFilter()
            {
                Dispose();
            }

            public void Dispose()
            {
                _coeffBuffer = null;
                _queueBuffer = null;
                _coeffPtr = null;
                _queuePtr = null;
                GC.SuppressFinalize(this);
            }

            public int Length
            {
                get { return _queueSize; }
            }

            public void SetCoefficients(float[] coefficients)
            {
                if (coefficients == null)
                {
                    return;
                }

                if (_coeffBuffer == null || coefficients.Length != _queueSize)
                {
                    _queueSize = coefficients.Length;
                    _offset = _queueSize * (CircularBufferSize - 1);

                    _coeffBuffer = UnsafeBuffer.Create(_queueSize, sizeof(float));
                    _coeffPtr = (float*)_coeffBuffer;

                    _queueBuffer = UnsafeBuffer.Create(_queueSize * CircularBufferSize, sizeof(float));
                    _queuePtr = (float*)_queueBuffer;
                }

                for (var i = 0; i < _queueSize; i++)
                {
                    _coeffPtr[i] = coefficients[i];
                }

                _isSymmetric = true;
                _isSparse = true;

                if (_queueSize % 2 != 0)
                {
                    var halfLen = _queueSize / 2;

                    for (var i = 0; i < halfLen; i++)
                    {
                        var j = _queueSize - 1 - i;
                        if (Math.Abs(_coeffPtr[i] - _coeffPtr[j]) > Epsilon)
                        {
                            _isSymmetric = false;
                            _isSparse = false;
                            break;
                        }
                        if (i % 2 != 0)
                        {
                            _isSparse = _coeffPtr[i] == 0f && _coeffPtr[j] == 0f;
                        }
                    }
                }
            }

            private void ProcessSymmetricKernel(float* buffer, int length)
            {
                for (var n = 0; n < length; n++)
                {
                    var queue = _queuePtr + _offset;

                    queue[0] = buffer[n];

                    var acc = 0.0f;

                    var halfLen = _queueSize / 2;
                    var len = halfLen;

                    var ptr1 = _coeffPtr;
                    var ptr2 = queue;
                    var ptr3 = queue + _queueSize - 1;

                    if (len >= 4)
                    {
                        do
                        {
                            acc += ptr1[0] * (ptr2[0] + ptr3[0])
                                 + ptr1[1] * (ptr2[1] + ptr3[-1])
                                 + ptr1[2] * (ptr2[2] + ptr3[-2])
                                 + ptr1[3] * (ptr2[3] + ptr3[-3]);

                            ptr1 += 4;
                            ptr2 += 4;
                            ptr3 -= 4;
                        } while ((len -= 4) >= 4);
                    }
                    while (len-- > 0)
                    {
                        acc += *ptr1++ * (*ptr2++ + *ptr3--);
                    }
                    acc += queue[halfLen] * _coeffPtr[halfLen];

                    if (--_offset < 0)
                    {
                        _offset = _queueSize * (CircularBufferSize - 1);
                        Memcpy(_queuePtr + _offset + 1, _queuePtr, (_queueSize - 1) * sizeof(float));
                    }

                    buffer[n] = acc;
                }
            }

            private void ProcessSymmetricKernelInterleaved(float* buffer, int length)
            {
                length <<= 1;
                for (var n = 0; n < length; n += 2)
                {
                    var queue = _queuePtr + _offset;

                    queue[0] = buffer[n];

                    var acc = 0.0f;

                    var halfLen = _queueSize / 2;
                    var len = halfLen;

                    var ptr1 = _coeffPtr;
                    var ptr2 = queue;
                    var ptr3 = queue + _queueSize - 1;

                    if (len >= 4)
                    {
                        do
                        {
                            acc += ptr1[0] * (ptr2[0] + ptr3[0])
                                 + ptr1[1] * (ptr2[1] + ptr3[-1])
                                 + ptr1[2] * (ptr2[2] + ptr3[-2])
                                 + ptr1[3] * (ptr2[3] + ptr3[-3]);

                            ptr1 += 4;
                            ptr2 += 4;
                            ptr3 -= 4;
                        } while ((len -= 4) >= 4);
                    }
                    while (len-- > 0)
                    {
                        acc += *ptr1++ * (*ptr2++ + *ptr3--);
                    }
                    acc += queue[halfLen] * _coeffPtr[halfLen];

                    if (--_offset < 0)
                    {
                        _offset = _queueSize * (CircularBufferSize - 1);
                        Memcpy(_queuePtr + _offset + 1, _queuePtr, (_queueSize - 1) * sizeof(float));
                    }

                    buffer[n] = acc;
                }
            }

            private void ProcessSparseSymmetricKernel(float* buffer, int length)
            {
                for (var n = 0; n < length; n++)
                {
                    var queue = _queuePtr + _offset;
                    queue[0] = buffer[n];

                    var acc = 0.0f;

                    var halfLen = _queueSize / 2;
                    var len = halfLen;

                    var ptr1 = _coeffPtr;
                    var ptr2 = queue;
                    var ptr3 = queue + _queueSize - 1;

                    if (len >= 8)
                    {
                        do
                        {
                            acc += ptr1[0] * (ptr2[0] + ptr3[0])
                                 + ptr1[2] * (ptr2[2] + ptr3[-2])
                                 + ptr1[4] * (ptr2[4] + ptr3[-4])
                                 + ptr1[6] * (ptr2[6] + ptr3[-6]);

                            ptr1 += 8;
                            ptr2 += 8;
                            ptr3 -= 8;
                        } while ((len -= 8) >= 8);
                    }
                    if (len >= 4)
                    {
                        acc += ptr1[0] * (ptr2[0] + ptr3[0])
                                + ptr1[2] * (ptr2[2] + ptr3[-2]);
                        ptr1 += 4;
                        ptr2 += 4;
                        ptr3 -= 4;
                        len -= 4;
                    }
                    while (len-- > 0)
                    {
                        acc += *ptr1++ * (*ptr2++ + *ptr3--);
                    }
                    acc += queue[halfLen] * _coeffPtr[halfLen];

                    if (--_offset < 0)
                    {
                        _offset = _queueSize * (CircularBufferSize - 1);
                        Memcpy(_queuePtr + _offset + 1, _queuePtr, (_queueSize - 1) * sizeof(float));
                    }

                    buffer[n] = acc;
                }
            }

            private void ProcessSparseSymmetricKernelInterleaved(float* buffer, int length)
            {
                length <<= 1;
                for (var n = 0; n < length; n += 2)
                {
                    var queue = _queuePtr + _offset;
                    queue[0] = buffer[n];

                    var acc = 0.0f;

                    var halfLen = _queueSize / 2;
                    var len = halfLen;

                    var ptr1 = _coeffPtr;
                    var ptr2 = queue;
                    var ptr3 = queue + _queueSize - 1;

                    if (len >= 8)
                    {
                        do
                        {
                            acc += ptr1[0] * (ptr2[0] + ptr3[0])
                                 + ptr1[2] * (ptr2[2] + ptr3[-2])
                                 + ptr1[4] * (ptr2[4] + ptr3[-4])
                                 + ptr1[6] * (ptr2[6] + ptr3[-6]);

                            ptr1 += 8;
                            ptr2 += 8;
                            ptr3 -= 8;
                        } while ((len -= 8) >= 8);
                    }
                    if (len >= 4)
                    {
                        acc += ptr1[0] * (ptr2[0] + ptr3[0])
                             + ptr1[2] * (ptr2[2] + ptr3[-2]);
                        ptr1 += 4;
                        ptr2 += 4;
                        ptr3 -= 4;
                        len -= 4;
                    }
                    while (len-- > 0)
                    {
                        acc += *ptr1++ * (*ptr2++ + *ptr3--);
                    }
                    acc += queue[halfLen] * _coeffPtr[halfLen];

                    if (--_offset < 0)
                    {
                        _offset = _queueSize * (CircularBufferSize - 1);
                        Memcpy(_queuePtr + _offset + 1, _queuePtr, (_queueSize - 1) * sizeof(float));
                    }

                    buffer[n] = acc;
                }
            }

            private void ProcessStandard(float* buffer, int length)
            {
                for (var n = 0; n < length; n++)
                {
                    var queue = _queuePtr + _offset;
                    queue[0] = buffer[n];

                    var acc = 0.0f;

                    var len = _queueSize;
                    var ptr1 = queue;
                    var ptr2 = _coeffPtr;
                    if (len >= 4)
                    {
                        do
                        {
                            acc += ptr1[0] * ptr2[0]
                                 + ptr1[1] * ptr2[1]
                                 + ptr1[2] * ptr2[2]
                                 + ptr1[3] * ptr2[3];
                            ptr1 += 4;
                            ptr2 += 4;
                        } while ((len -= 4) >= 4);
                    }
                    while (len-- > 0)
                    {
                        acc += *ptr1++ * *ptr2++;
                    }

                    if (--_offset < 0)
                    {
                        _offset = _queueSize * (CircularBufferSize - 1);
                        Memcpy(_queuePtr + _offset + 1, _queuePtr, (_queueSize - 1) * sizeof(float));
                    }

                    buffer[n] = acc;
                }
            }

            private void ProcessStandardInterleaved(float* buffer, int length)
            {
                length <<= 1;
                for (var n = 0; n < length; n += 2)
                {
                    var queue = _queuePtr + _offset;
                    queue[0] = buffer[n];

                    var acc = 0.0f;

                    var len = _queueSize;
                    var ptr1 = queue;
                    var ptr2 = _coeffPtr;
                    if (len >= 4)
                    {
                        do
                        {
                            acc += ptr1[0] * ptr2[0]
                                 + ptr1[1] * ptr2[1]
                                 + ptr1[2] * ptr2[2]
                                 + ptr1[3] * ptr2[3];
                            ptr1 += 4;
                            ptr2 += 4;
                        } while ((len -= 4) >= 4);
                    }
                    while (len-- > 0)
                    {
                        acc += *ptr1++ * *ptr2++;
                    }

                    if (--_offset < 0)
                    {
                        _offset = _queueSize * (CircularBufferSize - 1);
                        Memcpy(_queuePtr + _offset + 1, _queuePtr, (_queueSize - 1) * sizeof(float));
                    }

                    buffer[n] = acc;
                }
            }

            public void Process(float* buffer, int length)
            {
                if (_isSparse)
                {
                    ProcessSparseSymmetricKernel(buffer, length);
                }
                else if (_isSymmetric)
                {
                    ProcessSymmetricKernel(buffer, length);
                }
                else
                {
                    ProcessStandard(buffer, length);
                }
            }

            public void ProcessInterleaved(float* buffer, int length)
            {
                if (_isSparse)
                {
                    ProcessSparseSymmetricKernelInterleaved(buffer, length);
                }
                else if (_isSymmetric)
                {
                    ProcessSymmetricKernelInterleaved(buffer, length);
                }
                else
                {
                    ProcessStandardInterleaved(buffer, length);
                }
            }
        }

        public enum DecimationFilterType
        {
            Fast,
            Baseband,
            Audio
        }

        public static class DecimationKernels
        {
            #region Constants

            public const float Cic3Max = 0.5f - 0.4985f;
            public const float Hb11TapMax = 0.5f - 0.475f;
            public const float Hb15TapMax = 0.5f - 0.451f;
            public const float Hb19TapMax = 0.5f - 0.428f;
            public const float Hb23TapMax = 0.5f - 0.409f;
            public const float Hb27TapMax = 0.5f - 0.392f;
            public const float Hb31TapMax = 0.5f - 0.378f;
            public const float Hb35TapMax = 0.5f - 0.366f;
            public const float Hb39TapMax = 0.5f - 0.356f;
            public const float Hb43TapMax = 0.5f - 0.347f;
            public const float Hb47TapMax = 0.5f - 0.340f;
            public const float Hb51TapMax = 0.5f - 0.333f;

            public static readonly float[] Kernel11 =
            {
                0.0060431029837374152f,
                0.0f,
                -0.049372515458761493f,
                0.0f,
                0.29332944952052842f,
                0.5f,
                0.29332944952052842f,
                0.0f,
                -0.049372515458761493f,
                0.0f,
                0.0060431029837374152f
            };

            public static readonly float[] Kernel15 =
            {
                -0.001442203300285281f,
                0.0f,
                0.013017512802724852f,
                0.0f,
                -0.061653278604903369f,
                0.0f,
                0.30007792316024057f,
                0.5f,
                0.30007792316024057f,
                0.0f,
                -0.061653278604903369f,
                0.0f,
                0.013017512802724852f,
                0.0f,
                -0.001442203300285281f
            };

            public static readonly float[] Kernel19 =
            {
                0.00042366527106480427f,
                0.0f,
                -0.0040717333369021894f,
                0.0f,
                0.019895653881950692f,
                0.0f,
                -0.070740034412329067f,
                0.0f,
                0.30449249772844139f,
                0.5f,
                0.30449249772844139f,
                0.0f,
                -0.070740034412329067f,
                0.0f,
                0.019895653881950692f,
                0.0f,
                -0.0040717333369021894f,
                0.0f,
                0.00042366527106480427f
            };

            public static readonly float[] Kernel23 =
            {
                -0.00014987651418332164f,
                0.0f,
                0.0014748633283609852f,
                0.0f,
                -0.0074416944990005314f,
                0.0f,
                0.026163522731980929f,
                0.0f,
                -0.077593699116544707f,
                0.0f,
                0.30754683719791986f,
                0.5f,
                0.30754683719791986f,
                0.0f,
                -0.077593699116544707f,
                0.0f,
                0.026163522731980929f,
                0.0f,
                -0.0074416944990005314f,
                0.0f,
                0.0014748633283609852f,
                0.0f,
                -0.00014987651418332164f
            };

            public static readonly float[] Kernel27 =
            {
                0.000063730426952664685f,
                0.0f,
                -0.00061985193978569082f,
                0.0f,
                0.0031512504783365756f,
                0.0f,
                -0.011173151342856621f,
                0.0f,
                0.03171888754393197f,
                0.0f,
                -0.082917863582770729f,
                0.0f,
                0.3097770473566307f,
                0.5f,
                0.3097770473566307f,
                0.0f,
                -0.082917863582770729f,
                0.0f,
                0.03171888754393197f,
                0.0f,
                -0.011173151342856621f,
                0.0f,
                0.0031512504783365756f,
                0.0f,
                -0.00061985193978569082f,
                0.0f,
                0.000063730426952664685f
            };

            public static readonly float[] Kernel31 =
            {
                -0.000030957335326552226f,
                0.0f,
                0.00029271992847303054f,
                0.0f,
                -0.0014770381124258423f,
                0.0f,
                0.0052539088990950535f,
                0.0f,
                -0.014856378748476874f,
                0.0f,
                0.036406651919555999f,
                0.0f,
                -0.08699862567952929f,
                0.0f,
                0.31140967076042625f,
                0.5f,
                0.31140967076042625f,
                0.0f,
                -0.08699862567952929f,
                0.0f,
                0.036406651919555999f,
                0.0f,
                -0.014856378748476874f,
                0.0f,
                0.0052539088990950535f,
                0.0f,
                -0.0014770381124258423f,
                0.0f,
                0.00029271992847303054f,
                0.0f,
                -0.000030957335326552226f
            };

            public static readonly float[] Kernel35 =
            {
                0.000017017718072971716f,
                0.0f,
                -0.00015425042851962818f,
                0.0f,
                0.00076219685751140838f,
                0.0f,
                -0.002691614694785393f,
                0.0f,
                0.0075927497927344764f,
                0.0f,
                -0.018325727896057686f,
                0.0f,
                0.040351004914363969f,
                0.0f,
                -0.090198224668969554f,
                0.0f,
                0.31264689763504327f,
                0.5f,
                0.31264689763504327f,
                0.0f,
                -0.090198224668969554f,
                0.0f,
                0.040351004914363969f,
                0.0f,
                -0.018325727896057686f,
                0.0f,
                0.0075927497927344764f,
                0.0f,
                -0.002691614694785393f,
                0.0f,
                0.00076219685751140838f,
                0.0f,
                -0.00015425042851962818f,
                0.0f,
                0.000017017718072971716f
            };

            public static readonly float[] Kernel39 =
            {
                -0.000010175082832074367f,
                0.0f,
                0.000088036416015024345f,
                0.0f,
                -0.00042370835558387595f,
                0.0f,
                0.0014772557414459019f,
                0.0f,
                -0.0041468438954260153f,
                0.0f,
                0.0099579126901608011f,
                0.0f,
                -0.021433527104289002f,
                0.0f,
                0.043598963493432855f,
                0.0f,
                -0.092695953625928404f,
                0.0f,
                0.31358799113382152f,
                0.5f,
                0.31358799113382152f,
                0f,
                -0.092695953625928404f,
                0.0f,
                0.043598963493432855f,
                0.0f,
                -0.021433527104289002f,
                0.0f,
                0.0099579126901608011f,
                0.0f,
                -0.0041468438954260153f,
                0.0f,
                0.0014772557414459019f,
                0.0f,
                -0.00042370835558387595f,
                0.0f,
                0.000088036416015024345f,
                0.0f,
                -0.000010175082832074367f
            };

            public static readonly float[] Kernel43 =
            {
                0.0000067666739082756387f,
                0.0f,
                -0.000055275221547958285f,
                0.0f,
                0.00025654074579418561f,
                0.0f,
                -0.0008748125689163153f,
                0.0f,
                0.0024249876017061502f,
                0.0f,
                -0.0057775190656021748f,
                0.0f,
                0.012299834239523121f,
                0.0f,
                -0.024244050662087069f,
                0.0f,
                0.046354303503099069f,
                0.0f,
                -0.094729903598633314f,
                0.0f,
                0.31433918020123208f,
                0.5f,
                0.31433918020123208f,
                0.0f,
                -0.094729903598633314f,
                0.0f,
                0.046354303503099069f,
                0.0f,
                -0.024244050662087069f,
                0.0f,
                0.012299834239523121f,
                0.0f,
                -0.0057775190656021748f,
                0.0f,
                0.0024249876017061502f,
                0.0f,
                -0.0008748125689163153f,
                0.0f,
                0.00025654074579418561f,
                0.0f,
                -0.000055275221547958285f,
                0.0f,
                0.0000067666739082756387f
            };

            public static readonly float[] Kernel47 =
            {
                -0.0000045298314172004251f,
                0.0f,
                0.000035333704512843228f,
                0.0f,
                -0.00015934776420643447f,
                0.0f,
                0.0005340788063118928f,
                0.0f,
                -0.0014667949695500761f,
                0.0f,
                0.0034792089350833247f,
                0.0f,
                -0.0073794356720317733f,
                0.0f,
                0.014393786384683398f,
                0.0f,
                -0.026586603160193314f,
                0.0f,
                0.048538673667907428f,
                0.0f,
                -0.09629115286535718f,
                0.0f,
                0.31490673428547367f,
                0.5f,
                0.31490673428547367f,
                0.0f,
                -0.09629115286535718f,
                0.0f,
                0.048538673667907428f,
                0.0f,
                -0.026586603160193314f,
                0.0f,
                0.014393786384683398f,
                0.0f,
                -0.0073794356720317733f,
                0.0f,
                0.0034792089350833247f,
                0.0f,
                -0.0014667949695500761f,
                0.0f,
                0.0005340788063118928f,
                0.0f,
                -0.00015934776420643447f,
                0.0f,
                0.000035333704512843228f,
                0.0f,
                -0.0000045298314172004251f
            };

            public static readonly float[] Kernel51 =
            {
                0.0000033359253688981639f,
                0.0f,
                -0.000024584155158361803f,
                0.0f,
                0.00010677777483317733f,
                0.0f,
                -0.00034890723143173914f,
                0.0f,
                0.00094239127078189603f,
                0.0f,
                -0.0022118302078923137f,
                0.0f,
                0.0046575030752162277f,
                0.0f,
                -0.0090130973415220566f,
                0.0f,
                0.016383673864361164f,
                0.0f,
                -0.028697281101743237f,
                0.0f,
                0.05043292242400841f,
                0.0f,
                -0.097611898315791965f,
                0.0f,
                0.31538104435015801f,
                0.5f,
                0.31538104435015801f,
                0.0f,
                -0.097611898315791965f,
                0.0f,
                0.05043292242400841f,
                0.0f,
                -0.028697281101743237f,
                0.0f,
                0.016383673864361164f,
                0.0f,
                -0.0090130973415220566f,
                0.0f,
                0.0046575030752162277f,
                0.0f,
                -0.0022118302078923137f,
                0.0f,
                0.00094239127078189603f,
                0.0f,
                -0.00034890723143173914f,
                0.0f,
                0.00010677777483317733f,
                0.0f,
                -0.000024584155158361803f,
                0.0f,
                0.0000033359253688981639f
            };

            #endregion
        }

        public unsafe class IQDecimator
        {
            private readonly bool _isMultithreaded = false;
            private readonly FloatDecimator _rDecimator;
            private readonly FloatDecimator _iDecimator;
            //private readonly SharpEvent _event = new SharpEvent(false);

            public IQDecimator(int stageCount, double samplerate, bool useFastFilters, bool isMultithreaded)
            {
                _isMultithreaded = isMultithreaded;
                int childThreads;
                if (_isMultithreaded)
                {
                    childThreads = Environment.ProcessorCount / 2;
                }
                else
                {
                    childThreads = 1;
                }
                var filterType = useFastFilters ? DecimationFilterType.Fast : DecimationFilterType.Baseband;
                _rDecimator = new FloatDecimator(stageCount, samplerate, filterType, childThreads);
                _iDecimator = new FloatDecimator(stageCount, samplerate, filterType, childThreads);
            }

            public void Process(float* buffer_l, float* buffer_r, int length)
            {
                var rPtr = buffer_l;
                var iPtr = buffer_r;

                /*if (_isMultithreaded)
                {
                    DSPThreadPool.QueueUserWorkItem(
                        delegate
                        {
                            _rDecimator.ProcessInterleaved(rPtr, length);
                            _event.Set();
                        });
                }
                else*/
                {
                    _rDecimator.ProcessInterleaved(rPtr, length);
                }

                _iDecimator.ProcessInterleaved(iPtr, length);

                /*if (_isMultithreaded)
                {
                    //_event.WaitOne();
                }*/
            }

            public int StageCount
            {
                get { return _rDecimator.StageCount; }
            }
        }

        public unsafe sealed class FloatDecimator
        {
            private readonly int _stageCount;
            private readonly int _threadCount;
            private readonly int _cicCount;
            private readonly UnsafeBuffer _cicDecimatorsBuffer;
            private readonly CicDecimator* _cicDecimators;
            private readonly FirFilter[] _firFilters;

            public FloatDecimator(int stageCount)
                : this(stageCount, 0, DecimationFilterType.Audio, 1)
            {
            }

            public FloatDecimator(int stageCount, double samplerate, DecimationFilterType filterType) :
                this(stageCount, samplerate, filterType, 1)
            {
            }

            public FloatDecimator(int stageCount, double samplerate, DecimationFilterType filterType, int threadCount)
            {
                _stageCount = stageCount;
                _threadCount = threadCount;

                _cicCount = 0;
                var firCount = 0;

                switch (filterType)
                {
                    case DecimationFilterType.Fast:
                        _cicCount = stageCount;
                        break;

                    case DecimationFilterType.Audio:
                        firCount = stageCount;
                        break;

                    case DecimationFilterType.Baseband:
                        while (_cicCount < stageCount && samplerate >= 500000)
                        {
                            _cicCount++;
                            samplerate /= 2;
                        }
                        firCount = stageCount - _cicCount;
                        break;
                }

                _cicDecimatorsBuffer = UnsafeBuffer.Create(_threadCount * _cicCount, sizeof(CicDecimator));
                _cicDecimators = (CicDecimator*)_cicDecimatorsBuffer;
                for (var i = 0; i < _threadCount; i++)
                {
                    for (var j = 0; j < _cicCount; j++)
                    {
                        _cicDecimators[i * _cicCount + j] = new CicDecimator();
                    }
                }

                var kernel = filterType == DecimationFilterType.Audio ? DecimationKernels.Kernel47 : DecimationKernels.Kernel23;
                _firFilters = new FirFilter[firCount];
                for (var i = 0; i < firCount; i++)
                {
                    _firFilters[i] = new FirFilter(kernel);
                }
            }

            public int StageCount
            {
                get { return _stageCount; }
            }

            public void Process(float* buffer, int length)
            {
                DecimateStage1(buffer, length);
                length >>= _cicCount;
                DecimateStage2(buffer, length);
            }

            public void ProcessInterleaved(float* buffer, int length)
            {
                DecimateStage1Interleaved(buffer, length);
                length >>= _cicCount;
                DecimateStage2Interleaved(buffer, length);
            }

            private void DecimateStage1(float* buffer, int sampleCount)
            {
                for (var n = 0; n < _cicCount; n++)
                {
                    var contextId = 0;
                    var chunk = buffer;
                    var chunkLength = sampleCount;

                    #region Filter and down-sample

                    _cicDecimators[contextId * _cicCount + n].Process(chunk, chunkLength);

                    #endregion

                    sampleCount /= 2;
                }
            }

            private void DecimateStage2(float* buffer, int length)
            {
                for (var n = 0; n < _firFilters.Length; n++)
                {
                    _firFilters[n].Process(buffer, length);
                    length /= 2;
                    for (int i = 0, j = 0; i < length; i++, j += 2)
                    {
                        buffer[i] = buffer[j];
                    }
                }
            }

            private void DecimateStage1Interleaved(float* buffer, int sampleCount)
            {
                for (var n = 0; n < _cicCount; n++)
                {
                    var contextId = 0;
                    var chunk = buffer;
                    var chunkLength = sampleCount;

                    #region Filter and down-sample

                    _cicDecimators[contextId * _cicCount + n].ProcessInterleaved(chunk, chunkLength);

                    #endregion

                    sampleCount /= 2;
                }
            }

            private void DecimateStage2Interleaved(float* buffer, int length)
            {
                for (var n = 0; n < _firFilters.Length; n++)
                {
                    _firFilters[n].ProcessInterleaved(buffer, length);
                    for (int i = 0, j = 0; i < length; i += 2, j += 4)
                    {
                        buffer[i] = buffer[j];
                    }
                    length /= 2;
                }
            }
        }

#if !__MonoCS__
        [StructLayout(LayoutKind.Sequential, Pack = 16, Size = 32)]
#endif
        public unsafe struct CicDecimator
        {
            private float _xOdd;
            private float _xEven;

            public float XOdd
            {
                get { return _xOdd; }
                set { _xOdd = value; }
            }

            public float XEven
            {
                get { return _xEven; }
                set { _xEven = value; }
            }

            public void Process(float* buffer, int length)
            {
                for (int i = 0, j = 0; i < length; i += 2, j++)
                {
                    var even = buffer[i];
                    var odd = buffer[i + 1];
                    buffer[j] = (float)(0.125 * (odd + _xEven + 3.0 * (_xOdd + even)));
                    _xOdd = odd;
                    _xEven = even;
                }
            }

            public void ProcessInterleaved(float* buffer, int length)
            {
                length *= 2;
                for (int i = 0, j = 0; i < length; i += 4, j += 2)
                {
                    var even = buffer[i];
                    var odd = buffer[i + 2];
                    buffer[j] = 0.125f * (odd + _xEven + 3.0f * (_xOdd + even));
                    _xOdd = odd;
                    _xEven = even;
                }
            }
        }
    }
}
