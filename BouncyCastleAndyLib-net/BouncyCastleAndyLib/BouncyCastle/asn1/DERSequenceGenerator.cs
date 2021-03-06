using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
    public class DerSequenceGenerator
        : DerGenerator
    {
        private readonly MemoryStream _bOut = new MemoryStream();

        public DerSequenceGenerator(
            Stream outStream)
            : base(outStream)
        {
        }

        public DerSequenceGenerator(
            Stream outStream,
            int          tagNo,
            bool      isExplicit)
            : base(outStream, tagNo, isExplicit)
        {
        }

        public void AddObject(
            Asn1Object obj)
        {
            byte[] bytes = obj.GetEncoded();
            _bOut.Write(bytes, 0, bytes.Length);
        }

        public override Stream GetRawOutputStream()
        {
            return _bOut;
        }

        public void Close()
        {
            WriteDerEncoded(Asn1Tags.Constructed | Asn1Tags.Sequence, _bOut.ToArray());
        }
    }
}
