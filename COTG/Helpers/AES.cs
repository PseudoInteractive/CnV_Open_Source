using System;
using System.Text;

using System.Text.Encodings.Web;
using System.Web;
using System.Diagnostics;
using System.Linq;
using System.IO;
using static COTG.Debug;
using System.Text.Json;
//using System.Security.Cryptography;
using System.Collections.Generic;
//using System.Convert;
using Cysharp.Text;

namespace COTG
{
    public unsafe static class Aes
    {
        //      static System.Security.Cryptography.Aes aesDec = System.Security.Cryptography.Aes.Create();
        static Aes()
        {

        }


        const int keySize = 256;
        static Random random = new Random();
        const long k9p = 0x100000000l;
        public static byte charCodeAtSafe(this string s, int index)
        {
            Assert(index >= 0);
            if (index >= s.Length)
                return 0;
            return (byte) s[index];
        }

        static int DateTimeNow()
        {
            return (int)(DateTimeOffset.UtcNow - new DateTimeOffset(1970, 1, 1,0,0,0,new TimeSpan())).TotalMilliseconds;


        }
        static byte charCodeAt(this string s, int index)
        {
            return (byte)s[index];
        }
        static string utf8Encode(this string a)
        {
            try
            {
                return System.Uri.UnescapeDataString(HttpUtility.UrlEncode(a, Encoding.UTF8));

            }
            catch (Exception e)
            {
                Log(e);
                return a;
            }

        }
        static string utf8Decode(this string a)
        {
            //   System.Buffers.Text.Utf8Formatter ba;
            //   System.Utf8String text;
            //   System.Security.Cryptography.Aes aaa; 


            try
            {
                return HttpUtility.UrlDecode(System.Uri.EscapeDataString(a), Encoding.UTF8);

            }
            catch (Exception e)
            {
                Log(e);
                return a;
            }

        }
        static byte[] base64Decode(this byte[] a)
        {
            return System.Convert.FromBase64String(Encoding.UTF8.GetString(a));

        }
        static string base64Encode(byte[] a, long length)
        {
            return (System.Convert.ToBase64String(a, 0, (int)length, Base64FormattingOptions.None));

        }
        struct Block
        {
            const int dim = 4;
            public fixed byte v[dim * dim];
            public byte this[int i]
            {
                get
                {
                    Assert(i < dim * dim);
                    Assert(i >= 0);
                    return v[i];
                }
            }
            public byte this[int i, int j]
            {
                get
                {
                    Assert(i < dim);
                    Assert(i >= 0);
                    Assert(j < dim);
                    Assert(j >= 0);
                    return v[i * dim + j];
                }
                set
                {
                    Assert(i < dim);
                    Assert(i >= 0);
                    Assert(j < dim);
                    Assert(j >= 0);
                    v[i * dim + j] = value;
                }
            }
        }
        struct Column
        {
            public const int length = 4;
            public fixed byte v[length];
            public byte this[int i]
            {
                get
                {
                    Assert(i < length * length);
                    Assert(i >= 0);
                    return v[i];
                }
                set
                {
                    Assert(i < length * length);
                    Assert(i >= 0);
                    v[i] = value;
                }
            }

            internal void Set(byte v0, byte v1, byte v2, byte v3)
            {
                v[0] = v0;
                v[1] = v1;
                v[2] = v2;
                v[3] = v3;
            }
        }
        struct Row16
        {
            public const int Length = 16;
            public fixed byte v[Length];
            public byte this[int i]
            {
                get
                {
                    Assert(i < Length * Length);
                    Assert(i >= 0);
                    return v[i];
                }
                set
                {
                    Assert(i < Length * Length);
                    Assert(i >= 0);
                    v[i] = value;
                }
            }

        }
        static byte* __a6cipher(byte* l6v, Column[] j6v, byte*src)
        {
            const int u6v = 4;
            var I6v = j6v.Length / u6v - 1;
            Block s6v;// = [[], [], [], []];
            for (var H6v = +0; H6v < 16; H6v++)
                s6v[H6v % 4, (H6v / 4)] = l6v[H6v];
            s6v = __a6addRoundKey(s6v, j6v, 0, u6v);
            for (var w6v = +1; w6v < I6v; w6v++)
            {
                s6v = __a6subBytes(s6v, u6v);
                s6v = __a6shiftRows(s6v, u6v);
                s6v = __a6mixColumns(s6v);
                s6v = __a6addRoundKey(s6v, j6v, w6v, u6v);
            }
            s6v = __a6subBytes(s6v, u6v);
            s6v = __a6shiftRows(s6v, u6v);
            s6v = __a6addRoundKey(s6v, j6v, I6v, u6v);
        //    var v6v = new Row16();
            for (var H6v = 0; H6v < 16; H6v++)
                src[H6v] = (byte) s6v[H6v % 4, H6v / (4)];
            return src;
        }
        static Column[] __a6keyExpansion(byte* o6v)
        {
            var L6v = +4;
            var x6v = 32 / +4;
            var X6v = x6v + +6;
            var t6v = new Column[(L6v * (X6v + +1))];
            var O6v = new Column();
            for (var Q6v = 0; Q6v < x6v; Q6v++)
            {

                t6v[Q6v].Set(o6v[+4 * Q6v], o6v[+4 * Q6v + 1], o6v[+4 * Q6v + (2)], o6v[(4 * Q6v + +3)]);
            }
            for (var Q6v = x6v; Q6v < L6v * (X6v + (1)); Q6v++)
            {
                t6v[Q6v] = new Column();
                for (var T6v = 0; T6v < +4; T6v++)
                    O6v[T6v] = t6v[Q6v - (1)][T6v];
                if (Q6v % x6v == +0)
                {
                    O6v = __a6subWord(__a6rotWord(O6v));
                    for (var T6v = +0; T6v < 4; T6v++)
                        O6v[T6v] ^= __a6rCon[Q6v / x6v, T6v];
                }
                else if (x6v > 6 && Q6v % x6v == (4))
                    O6v = __a6subWord(O6v);
                for (var T6v = 0; T6v < +4; T6v++)
                    t6v[Q6v][T6v] = (byte)(t6v[Q6v - x6v][T6v] ^ O6v[T6v]);
            }
            return t6v;
        }
        static Block __a6subBytes(Block C6v, int W6v)
        {
            //	i011.R6();
            for (var M6v = 0; M6v < 4; M6v++)
                for (var G6v = +0; G6v < W6v; G6v++)
                    C6v[M6v, G6v] = __a6sBox[C6v[M6v, G6v]];
            return C6v;
        }
        static Block __a6shiftRows(Block b6v, int J6v)
        {
            var S6v = new Column();
            for (var d6v = +1; d6v < 4; d6v++)
            {
                for (var i6v = 0; i6v < 4; i6v++)
                    S6v[i6v] = b6v[d6v, (i6v + d6v) % J6v];
                for (var i6v = 0; i6v < 4; i6v++)
                    b6v[d6v, i6v] = S6v[i6v];
            }
            return b6v;
        }
        static Block __a6mixColumns(Block A6v)
        {
            for (var V6v = 0; V6v < (4); V6v++)
            {
                var h6v = new Column();
                var r6v = new Column();
                for (var n6v = 0; n6v < +4; n6v++)
                {
                    h6v[n6v] = A6v[n6v, V6v];
                    r6v[n6v] = (byte)(((A6v[n6v, V6v] & 0x80) != 0) ? (A6v[n6v, V6v] << (1)) ^ 0x011b : A6v[n6v, V6v] << (1));
                }
                A6v[0, V6v] = (byte)(r6v[0] ^ h6v[+1] ^ r6v[1 * 1] ^ h6v[2] ^ h6v[+3]);
                A6v[1, V6v] = (byte)(h6v[0] ^ r6v[1] ^ h6v[2] ^ r6v[2] ^ h6v[+3]);
                A6v[2, V6v] = (byte)(h6v[0] ^ h6v[1] ^ r6v[2] ^ h6v[3] ^ r6v[3]);
                A6v[3, V6v] = (byte)(h6v[0] ^ r6v[0] ^ h6v[1] ^ h6v[2] ^ r6v[3]);
            }
            return A6v;
        }
        static Block __a6addRoundKey(Block Z6v, Column[] U6v, int P6v, int B6v)
        {
            for (var K6v = 0; K6v < 4; K6v++)
                for (var g6v = 0; g6v < B6v; g6v++)
                    Z6v[K6v, g6v] ^= U6v[P6v * 4 + g6v][K6v];
            return Z6v;
        }
        static Column __a6subWord(Column y6v)
        {
            for (var R6v = 0; R6v < 4; R6v++)
                y6v[R6v] = __a6sBox[y6v[R6v]];
            return y6v;
        }
        static Column __a6rotWord(Column D6v)
        {
            var Y6v = D6v[0];
            for (var p6v = 0; p6v < 3; p6v++)
                D6v[p6v] = D6v[p6v + 1];
            D6v[3] = Y6v;
            return D6v;
        }
        static byte[] __a6sBox = { 99, 124, 119, 123, 242, 107, 0x6f, 197, 48, 1, 103, 43, 254, 215, 171, 118, 202, 130, 201, 125, 250, 89, 71, 240, 0xad, 212, 162, 175, 0x9c, 164, 114, 192, 183, 253, 147, 38, 0x36, 63, 247, 204, 52, 165, 229, 241, 113, 216, 0x31, 21, 4, 199, 35, 195, 24, 150, 5, 154, 7, 18, 128, 226, 235, 39, 178, 117, 0x09, 131, 44, 26, 0x1b, 110, 90, 160, 82, 59, 214, 179, 41, 227, 47, 132, 83, 209, 0, 0xed, 32, 0xfc, 177, 91, 106, 203, 190, 57, 74, 76, 88, 207, 208, 239, 0xaa, 0xfb, 0x43, 0x4d, 51, 133, 69, 249, 2, 127, 80, 60, 159, 168, 81, 163, 64, 143, 146, 157, 56, 245, 188, 182, 218, 33, 16, 255, 243, 210, 205, 12, 19, 236, 95, 151, 68, 23, 196, 167, 126, 61, 0x64, 93, 25, 115, 96, 129, 79, 220, 34, 42, 144, 136, 70, 238, 0xb8, 20, 222, 94, 11, 219, 224, 50, 58, 10, 0x49, 0x06, 36, 92, 194, 211, 172, 98, 145, 149, 228, 121, 231, 200, 55, 109, 141, 213, 78, 169, 108, 86, 244, 234, 101, 122, 174, 8, 186, 120, 37, 46, 28, 166, 0xb4, 198, 232, 221, 116, 31, 75, 0xbd, 139, 138, 112, 62, 181, 102, 0x48, 3, 246, 14, 97, 53, 87, 185, 134, 193, 29, 158, 0xe1, 248, 152, 17, 105, 217, 142, 148, 155, 0x1e, 135, 233, 206, 85, 40, 0xdf, 140, 161, 137, 13, 191, 230, 0x42, 104, 65, 153, 45, 15, 176, 84, 187, 22 };
        static byte[,] __a6rCon = {
        {0, 0, 0, 0}, {1, 0, 0, 0}, {2, 0, 0, 0}, {4, 0, 0, 0}, {8, 0, 0, 0}, {16, 0, 0, 0}, {32, 0, 0, 0}, {64, 0, 0, 0}, {128, 0, 0, 0}, {27, 0, 0, 0}, {54, 0, 0, 0}};

        const int blockSize = 16;
        //__a6.ccazzx={};
        public static string Encode(string payload, string key)
        {
            payload = utf8Encode(payload);
            key = utf8Encode(key);
            const int e2v = 32;// keySize / (8);
            var u2v = stackalloc byte[e2v];
            var temp = stackalloc byte[32];
            var temp2 = stackalloc byte[16];
            for (var N6v = 0; N6v < e2v; N6v++)
                u2v[N6v] = key.charCodeAtSafe(N6v);
            var E6v = __a6cipher(u2v, __a6keyExpansion(u2v),temp);
            for (int i = 0; i < 16; ++i)
                E6v[i + 16] = E6v[i];
            var q6v = stackalloc byte[(blockSize)];
            var Q2v = DateTimeNow();
            var T2v = Q2v % 1000;
            var t2v = (Q2v / 1000);
            var o2v = random.Next(0xffff);
            for (var N6v = 0; N6v < +2; N6v++)
                q6v[N6v] = (byte) ((T2v >> (N6v * 8)) & +0xff);
            for (var N6v = 0; N6v < 2; N6v++)
                q6v[N6v + 2] = (byte)((o2v >> N6v * 8) & 0xff);
            for (var N6v = +0; N6v < 4; N6v++)
                q6v[N6v + (4)] = (byte)((t2v >> N6v * (8)) & 0xff);
            var payloadLength = payload.Length;
            var buffer = new byte[payloadLength * 2+8]; // this should be smaller

            {
                int put = 0;
                for (var N6v = 0; N6v < (8); N6v++)
                    buffer[put++] = ((byte)(q6v[N6v]));
                var x2v = __a6keyExpansion(E6v);
                var j2v = (payloadLength + blockSize - 1) / blockSize;
                var I2v = new string[j2v];
                for (var c6v = +0; c6v < j2v; c6v++)
                {
                    for (var F6v = 0; F6v < (4); F6v++)
                        q6v[(15 | 2) - F6v] = (byte)((c6v >> F6v * (8)) & (0xff));
                    for (var F6v = +0; F6v < (4); F6v++)
                        q6v[+15 - F6v - +4] = (byte)((int)(c6v / +k9p >> F6v * (8)));
                    var O2v = __a6cipher(q6v, x2v,temp2);
                    var l2v = c6v < j2v - (1) ? blockSize : (payloadLength - +1) % blockSize + +1;
                    for (var N6v = 0; N6v < l2v; N6v++)
                    {
                        buffer[put++] = ((byte)(O2v[N6v] ^ payload.charCodeAt(c6v * blockSize + N6v)));
                    }
                }

                var w2v = base64Encode(buffer, put);
                return w2v;
            }
        }



        public static string Decode(string payloadString, string key)
        {
          //  return AES.DecryptString(key, payloadString);


            var M2v =  System.Convert.FromBase64String(payloadString );

            // key = utf8Encode((key));
            const int J2v = keySize / 8;
            var temp2 = stackalloc byte[16];
            var temp = stackalloc byte[32];
            var b2v = stackalloc byte[J2v];
            for (var L2v = +0; L2v < J2v; L2v++)
                b2v[L2v] = key.charCodeAtSafe(L2v);
            var i2v = __a6cipher(b2v, __a6keyExpansion(b2v),temp);
            for (int i = 0; i < 16; ++i)
                i2v[i + 16] = i2v[i];
            var G2v = stackalloc byte[16];
            var n2v = M2v.Take(8).ToArray();
            for (var L2v = +0; L2v < +8; L2v++)
                G2v[L2v] = n2v[L2v];
            var K2v = __a6keyExpansion(i2v);
            var m2vLg = M2v.Length;
            var S2v = ((m2vLg-8 + blockSize - 1) / blockSize);
            var r2v = new Row16[S2v];
            for (var X2v = +0; X2v < S2v; X2v++)
                for (int i = 0; i < blockSize; ++i)
                {
                    var idx = (8) + X2v * blockSize + i;
                    r2v[X2v][i] = (byte)((idx < m2vLg) ? M2v[idx] : 0);
                }
            var MXv = r2v;
            string sbs;
            var sb = new StringBuilder();
            int chRemaining = m2vLg-8;
            for (var X2v = 0; X2v < S2v; X2v++)
            {
                for (var z2v = +0; z2v < 4; z2v++)
                    G2v[15 - z2v] = (byte)((X2v >> z2v * 8) & 0xff);
                for (var z2v = +0; z2v < (4); z2v++)
                    G2v[15 - z2v - 4] = 0;// (int)(((((X2v + 1) / k9p) - 1) >> (z2v * 8)) & 0xff); // this part will always be 0, unless there is >= 4gb
                var f2v = __a6cipher(G2v, K2v,temp2);
                //		var W2v = new Array(MXv[X2v].length);
 //               var l2v = X2v < S2v - (1) ? blockSize : (payloadLength - +1) % blockSize + +1;
                for (var L2v = 0; L2v < blockSize; L2v++)
                {
                    sb.Append((char)(f2v[L2v] ^ MXv[X2v][L2v]));
                    if (--chRemaining <= 0)
                        break;
                }

            }
            sbs = sb.ToString();

            var V2v = utf8Decode(sbs);
            return V2v;
        }
//  void dummy()
//    {
//              const C2v = q3y;
//            if (!(h2v == +G5y || h2v == v1R* 1 || h2v == U7y >> 1416089920))
//                return "";
//            M2v = String(M2v).base64Decode();
//        d2v = String(d2v).utf8Encode();
//        const J2v = h2v / +8;
//        const b2v = new Array(J2v);
//            for (var L2v = +'0'; L2v<J2v; L2v++)
//                b2v[L2v] = isNaN(d2v.charCodeAt(L2v)) ? 0 - 0 : d2v.charCodeAt(L2v);
//            let i2v = a6.cipher(b2v, a6.keyExpansion(b2v));
//        i2v = i2v.concat(i2v.slice(0 - 0, J2v - +q3y));
//            const G2v = new Array('8' * 1);
//        const n2v = M2v.slice(0, +8);
//            for (var L2v = +'0'; L2v< +'8'; L2v++)
//                G2v[L2v] = n2v.charCodeAt(L2v);
//            const K2v = a6.keyExpansion(i2v);
//        const S2v = Math.ceil((M2v.length - (8 >> 320676576)) / C2v);
//        const r2v = new Array(S2v);
//            for (var X2v = +0; X2v<S2v; X2v++)
//                r2v[X2v] = M2v.slice((8 ) + X2v* C2v, +'8' + X2v* C2v + C2v);
//            M2v = r2v;
//            const A2v = new Array(M2v.length);
//            for (var X2v = 0 | 0; X2v<S2v; X2v++) {
//                for (var z2v = +'0'; z2v< '4' >> 668238304; z2v++)
//                    G2v[q7y * 1 - z2v] = X2v >>> z2v* +8 & (N9p | 51);
//                for (var z2v = +'0'; z2v<('4' ); z2v++)
//                    G2v[q7y - 0 - z2v - +'4'] = (`${X2v
//    }1`) / (k9p - 0) - +1 >>> z2v* +'8' & N9p* 1;
//                const f2v = a6.cipher(G2v, K2v);
//    const W2v = new Array(M2v[X2v].length);
//                for (var L2v = 0 * 1; L2v<M2v[X2v].length; L2v++) {
//                    W2v[L2v] = f2v[L2v] ^ M2v[X2v].charCodeAt(L2v);
//    W2v[L2v] = String.fromCharCode(W2v[L2v]);
//                }
//A2v[X2v] = W2v.join("");
//            }
//            let V2v = A2v.join("");
//V2v = V2v.utf8Decode();
//            return V2v;
    }
}
