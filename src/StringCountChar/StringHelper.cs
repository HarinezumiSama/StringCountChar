using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StringCountChar
{
    [SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
    internal static class StringHelper
    {
        public static int CountUsingLinqAndLambda(this string text, char c) => text.Count(character => character == c);

        public static int CountUsingLinqAndLocalFunction(this string text, char c)
        {
            bool IsMatch(char character) => character == c;
            return text.Count(IsMatch);
        }

        public static int CountUsingForEach(this string text, char c)
        {
            var count = 0;

            foreach (var character in text)
            {
                if (character == c)
                {
                    count++;
                }
            }

            return count;
        }

        public static int CountUsingForEachButNoBranching(this string text, char c)
        {
            var count = 0;

            foreach (var character in text)
            {
                /* Comparing two values for bitwise equality does not
                 * produce a conditional branch if we're not using the
                 * result to decide what code to execute. A bool is simply
                 * a struct with a size of 1 byte which is set to either 1
                 * or 0. We can use Unsafe.As to reinterpret a reference to the
                 * bool value as one to a byte type, which we can then read
                 * directly and implicitly convert to int. This effectively
                 * converts our bool variable to either 1 or 0, with no
                 * conditional branches: we always sum this value to the total. */
                var equals = character == c;
                count += Unsafe.As<bool, byte>(ref equals);
            }

            return count;
        }

        public static int CountUsingSimdWithUShortLimit(this string text, char c)
        {
            /* Get a reference to the first string character.
             * Strings are supposed to be immutable in .NET, so
             * in order to do this we first get a ReadOnlySpan<char>
             * from our string, and then use the MemoryMarshal.GetReference
             * API, which returns a mutable reference to the first
             * element of the input span. As a result, we now have
             * a mutable char reference to the first character in the string. */
            var span = text.AsSpan();
            ref var r0 = ref MemoryMarshal.GetReference(span);
            var length = span.Length;
            int i = 0, result;

            /* As before, only execute the SIMD-enabled branch if the Vector<T> APIs
             * are hardware accelerated. Note that we're using ushort instead of char
             * in the Vector<T> type, because the char type is not supported.
             * But that is fine: ushort and char have the same byte size and the same
             * numerical range, and they behave exactly the same in this context. */
            if (Vector.IsHardwareAccelerated)
            {
                var end = length - Vector<ushort>.Count;

                // SIMD register all set to 0, to store partial results
                var partials = Vector<ushort>.Zero;

                // SIMD register with the target character c copied in every position
                var vc = new Vector<ushort>(c);

                for (; i <= end; i += Vector<ushort>.Count)
                {
                    // Get the reference to the current characters chunk
                    ref var ri = ref Unsafe.Add(ref r0, i);

                    /* Read a Vector<ushort> value from that offset, by
                     * reinterpreting the char reference as a ref Vector<ushort>.
                     * As with the previous example, doing this allows us to read
                     * the series of consecutive character starting from the current
                     * offset, and to load them in a single SIMD register. */

                    // vi = { text[i], ..., text[i + Vector<char>.Count - 1] }
                    var vi = Unsafe.As<char, Vector<ushort>>(ref ri);

                    /* The Vector.Equals method sets each T item in a Vector<T> to
                     * either all 1s if the two elements match (as if we had used
                     * the == operator), or to all 0s if a pair doesn't match. */
                    var ve = Vector.Equals(vi, vc);

                    /* First we load Vector<ushort>.One, which is a Vector<ushort> with
                     * just 1 in each position. Then we do a bitwise and with the
                     * previous result. Since matching values were all 1s, and non
                     * matching values were all 0s, we will have 1 in the position
                     * of pairs of values that were the same, or 0 otherwise. */
                    var va = Vector.BitwiseAnd(ve, Vector<ushort>.One);

                    // Accumulate the partial results in each position
                    partials += va;
                }

                /* The dot product of a vector with a vector with 1 in each
                 * position results in the horizontal sum of all the values
                 * in the first vector, because:
                 * 
                 * { a, b, c } DOT { 1, 1, 1 } = a * 1 + b * 1 + c * 1.
                 * 
                 * So result will hold all the matching characters up to this point. */
                result = Vector.Dot(partials, Vector<ushort>.One);
            }
            else
                result = 0;

            // Iterate over the remaining characters and count those that match
            for (; i < length; i++)
            {
                var equals = Unsafe.Add(ref r0, i) == c;
                result += Unsafe.As<bool, byte>(ref equals);
            }

            return result;
        }

        public static int CountUsingSimd(this string text, char c)
        {
            /* Get a reference to the first string character.
             * Strings are supposed to be immutable in .NET, so
             * in order to do this we first get a ReadOnlySpan<char>
             * from our string, and then use the MemoryMarshal.GetReference
             * API, which returns a mutable reference to the first
             * element of the input span. As a result, we now have
             * a mutable char reference to the first character in the string. */
            var span = text.AsSpan();
            ref var r0 = ref MemoryMarshal.GetReference(span);
            var length = span.Length;
            var i = 0;
            var result = 0;

            /* As before, only execute the SIMD-enabled branch if the Vector<T> APIs
             * are hardware accelerated. Note that we're using ushort instead of char
             * in the Vector<T> type, because the char type is not supported.
             * But that is fine: ushort and char have the same byte size and the same
             * numerical range, and they behave exactly the same in this context. */
            if (Vector.IsHardwareAccelerated)
            {
                var end = length - Vector<ushort>.Count;

                // SIMD register all set to 0, to store partial results
                var partials = Vector<uint>.Zero;

                // SIMD register with the target character c copied in every position
                var vc = new Vector<ushort>(c);

                for (; i <= end; i += Vector<ushort>.Count)
                {
                    // Get the reference to the current characters chunk
                    ref var ri = ref Unsafe.Add(ref r0, i);

                    /* Read a Vector<ushort> value from that offset, by
                     * reinterpreting the char reference as a ref Vector<ushort>.
                     * As with the previous example, doing this allows us to read
                     * the series of consecutive character starting from the current
                     * offset, and to load them in a single SIMD register. */

                    // vi = { text[i], ..., text[i + Vector<char>.Count - 1] }
                    var vi = Unsafe.As<char, Vector<ushort>>(ref ri);

                    /* The Vector.Equals method sets each T item in a Vector<T> to
                     * either all 1s if the two elements match (as if we had used
                     * the == operator), or to all 0s if a pair doesn't match. */
                    var ve = Vector.Equals(vi, vc);

                    /* First we load Vector<ushort>.One, which is a Vector<ushort> with
                     * just 1 in each position. Then we do a bitwise and with the
                     * previous result. Since matching values were all 1s, and non
                     * matching values were all 0s, we will have 1 in the position
                     * of pairs of values that were the same, or 0 otherwise. */
                    var va = Vector.BitwiseAnd(ve, Vector<ushort>.One);

                    Vector.Widen(va, out var va1, out var va2);

                    // Accumulate the partial results in each position
                    partials += va1;
                    partials += va2;
                }

                /* The dot product of a vector with a vector with 1 in each
                 * position results in the horizontal sum of all the values
                 * in the first vector, because:
                 * 
                 * { a, b, c } DOT { 1, 1, 1 } = a * 1 + b * 1 + c * 1.
                 * 
                 * So result will hold all the matching characters up to this point. */
                result = Convert.ToInt32(Vector.Dot(partials, Vector<uint>.One));
            }

            // Iterate over the remaining characters and count those that match
            for (; i < length; i++)
            {
                var equals = Unsafe.Add(ref r0, i) == c;
                result += Unsafe.As<bool, byte>(ref equals);
            }

            return result;
        }
    }
}