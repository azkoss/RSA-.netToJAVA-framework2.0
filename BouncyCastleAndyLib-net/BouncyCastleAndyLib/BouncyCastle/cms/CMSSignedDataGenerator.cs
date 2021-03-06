using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Cms
{
    /**
     * general class for generating a pkcs7-signature message.
     * <p>
     * A simple example of usage.
     *
     * <pre>
     *      IX509Store certs...
     *      IX509Store crls...
     *      CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
     *
     *      gen.AddSigner(privKey, cert, CmsSignedGenerator.DigestSha1);
     *      gen.AddCertificates(certs);
     *      gen.AddCrls(crls);
     *
     *      CmsSignedData data = gen.Generate(content);
     * </pre>
     */
    public class CmsSignedDataGenerator
        : CmsSignedGenerator
    {
		private static CmsSignedHelper Helper = CmsSignedHelper.Instance;

		private readonly ArrayList signerInfs = new ArrayList();

		internal class DigOutputStream
            : BaseOutputStream
        {
            private readonly IDigest dig;

            public DigOutputStream(
                IDigest dig)
            {
                this.dig = dig;
            }

            public override void Write(
                byte[]  b,
                int     off,
                int     len)
            {
                dig.BlockUpdate(b, off, len);
            }

            public override void WriteByte(
                byte b)
            {
                dig.Update(b);
            }
        }

        internal class SigOutputStream
            : BaseOutputStream
        {
            private readonly ISigner sig;

            public SigOutputStream(
                ISigner sig)
            {
                this.sig = sig;
            }

            public override void Write(
                byte[]	b,
                int		off,
                int		len)
            {
                try
                {
                    sig.BlockUpdate(b, off, len);
                }
                catch (SignatureException e)
                {
                    throw new IOException("signature problem: " + e);
                }
            }

            public override void WriteByte(
                byte b)
            {
                try
                {
                    sig.Update(b);
                }
                catch (SignatureException e)
                {
                    throw new IOException("signature problem: " + e);
                }
            }
        }

        private class SignerInf
        {
            CmsSignedGenerator			outer;
            AsymmetricKeyParameter		key;
            X509Certificate				cert;
            string						digestOID;
            string						encOID;
			CmsAttributeTableGenerator	sAttr;
			CmsAttributeTableGenerator	unsAttr;
			Asn1.Cms.AttributeTable		baseSignedTable;

			internal SignerInf(
                CmsSignedGenerator		outer,
                AsymmetricKeyParameter	key,
                X509Certificate			cert,
                string					digestOID,
                string					encOID)
            {
                this.outer = outer;
                this.key = key;
                this.cert = cert;
                this.digestOID = digestOID;
                this.encOID = encOID;
            }

            internal SignerInf(
                CmsSignedGenerator			outer,
                AsymmetricKeyParameter		key,
                X509Certificate				cert,
                string						digestOID,
                string						encOID,
				CmsAttributeTableGenerator	sAttr,
				CmsAttributeTableGenerator	unsAttr,
				Asn1.Cms.AttributeTable		baseSignedTable)
            {
                this.outer = outer;
                this.key = key;
                this.cert = cert;
                this.digestOID = digestOID;
                this.encOID = encOID;
                this.sAttr = sAttr;
                this.unsAttr = unsAttr;
				this.baseSignedTable = baseSignedTable;
			}

            internal AsymmetricKeyParameter GetKey()
            {
                return key;
            }

            internal X509Certificate GetCertificate()
            {
                return cert;
            }

			internal AlgorithmIdentifier DigestAlgorithmID
			{
				get { return new AlgorithmIdentifier(new DerObjectIdentifier(digestOID), null); }
			}

			internal string DigestAlgOid
            {
				get { return digestOID; }
            }

            internal Asn1Object DigestAlgParams
            {
				get { return null; }
            }

			internal AlgorithmIdentifier EncryptionAlgorithmID
			{
				get { return new AlgorithmIdentifier(new DerObjectIdentifier(encOID), DerNull.Instance); }
			}

            internal string EncryptionAlgOid
            {
				get { return encOID; }
            }

			internal CmsAttributeTableGenerator SignedAttributes
            {
				get { return sAttr; }
            }

            internal CmsAttributeTableGenerator UnsignedAttributes
            {
				get { return unsAttr; }
            }

			internal Asn1.Cms.SignerInfo ToSignerInfo(
                DerObjectIdentifier	contentType,
                CmsProcessable		content)
            {
                AlgorithmIdentifier digAlgId = new AlgorithmIdentifier(
                    new DerObjectIdentifier(this.DigestAlgOid), DerNull.Instance);
				AlgorithmIdentifier encAlgId = CmsSignedGenerator.GetEncAlgorithmIdentifier(this.EncryptionAlgOid);
				string digestName = Helper.GetDigestAlgName(digestOID);
				string signatureName = digestName + "with" + Helper.GetEncryptionAlgName(encOID);
				ISigner sig = Helper.GetSignatureInstance(signatureName);
				IDigest dig = Helper.GetDigestInstance(digestName);

                byte[] hash = null;

                if (content != null)
                {
                    content.Write(new DigOutputStream(dig));

					hash = DigestUtilities.DoFinal(dig);

					outer._digests.Add(digestOID, hash.Clone());
				}

				IDictionary parameters = outer.GetBaseParameters(contentType, digAlgId, hash);
				Asn1.Cms.AttributeTable signed = (sAttr != null)
//					?	sAttr.GetAttributes(Collections.unmodifiableMap(parameters))
					?	sAttr.GetAttributes(parameters)
					:	null;

				Asn1Set signedAttr = outer.GetAttributeSet(signed);


				//
                // sig must be composed from the DER encoding.
                //
				byte[] tmp;
				if (signedAttr != null)
                {
					tmp = signedAttr.GetDerEncoded();
                }
                else
                {
					MemoryStream bOut = new MemoryStream();
					content.Write(bOut);
					tmp = bOut.ToArray();
                }

				sig.Init(true, key);
				sig.BlockUpdate(tmp, 0, tmp.Length);

				Asn1OctetString encDigest = new DerOctetString(sig.GenerateSignature());

				IDictionary baseParameters = outer.GetBaseParameters(contentType, digAlgId, hash);
				baseParameters[CmsAttributeTableParameter.Signature] = encDigest.GetOctets().Clone();

				Asn1.Cms.AttributeTable unsigned = (unsAttr != null)
//					?	unsAttr.GetAttributes(Collections.unmodifiableMap(baseParameters))
					?	unsAttr.GetAttributes(baseParameters)
					:	null;

				Asn1Set unsignedAttr = outer.GetAttributeSet(unsigned);

                X509Certificate cert = this.GetCertificate();
                TbsCertificateStructure tbs = TbsCertificateStructure.GetInstance(
					Asn1Object.FromByteArray(cert.GetTbsCertificate()));
                Asn1.Cms.IssuerAndSerialNumber encSid = new Asn1.Cms.IssuerAndSerialNumber(
                    tbs.Issuer, tbs.SerialNumber.Value);

                return new Asn1.Cms.SignerInfo(new SignerIdentifier(encSid), digAlgId,
                    signedAttr, encAlgId, encDigest, unsignedAttr);
            }
        }

        /**
        * base constructor
        */
        public CmsSignedDataGenerator()
        {
        }

        /**
        * add a signer - no attributes other than the default ones will be
        * provided here.
        */
        public void AddSigner(
            AsymmetricKeyParameter	privateKey,
            X509Certificate			cert,
            string					digestOID)
        {
            string encOID = GetEncOid(privateKey, digestOID);

			signerInfs.Add(new SignerInf(this, privateKey, cert, digestOID, encOID,
				new DefaultSignedAttributeTableGenerator(), null, null));
		}

        /**
        * add a signer with extra signed/unsigned attributes.
        */
        public void AddSigner(
            AsymmetricKeyParameter	privateKey,
            X509Certificate			cert,
            string					digestOID,
            Asn1.Cms.AttributeTable	signedAttr,
            Asn1.Cms.AttributeTable	unsignedAttr)
        {
            string encOID = GetEncOid(privateKey, digestOID);

			signerInfs.Add(new SignerInf(this, privateKey, cert, digestOID, encOID,
				new DefaultSignedAttributeTableGenerator(signedAttr),
				new SimpleAttributeTableGenerator(unsignedAttr),
				signedAttr));
		}

		/**
		 * add a signer with extra signed/unsigned attributes based on generators.
		 */
		public void AddSigner(
			AsymmetricKeyParameter		privateKey,
			X509Certificate				cert,
			string						digestOID,
			CmsAttributeTableGenerator	signedAttrGen,
			CmsAttributeTableGenerator	unsignedAttrGen)
		{
			string encOID = GetEncOid(privateKey, digestOID);

			signerInfs.Add(new SignerInf(this, privateKey, cert, digestOID, encOID,
				signedAttrGen, unsignedAttrGen, null));
		}

		private static AlgorithmIdentifier FixAlgID(
			AlgorithmIdentifier algId)
		{
			if (algId.Parameters == null)
				return new AlgorithmIdentifier(algId.ObjectID, DerNull.Instance);

			return algId;
		}

		/**
        * generate a signed object that for a CMS Signed Data object
        */
        public CmsSignedData Generate(
            CmsProcessable content)
        {
            return Generate(content, false);
        }

        /**
        * generate a signed object that for a CMS Signed Data
        * object  - if encapsulate is true a copy
        * of the message will be included in the signature. The content type
        * is set according to the OID represented by the string signedContentType.
        */
        public CmsSignedData Generate(
            string			signedContentType,
            CmsProcessable	content,
            bool			encapsulate)
        {
            Asn1EncodableVector digestAlgs = new Asn1EncodableVector();
            Asn1EncodableVector signerInfos = new Asn1EncodableVector();

			DerObjectIdentifier contentTypeOID = new DerObjectIdentifier(signedContentType);

			_digests.Clear(); // clear the current preserved digest state

			//
            // add the precalculated SignerInfo objects.
            //
            foreach (SignerInformation signer in _signers)
            {
				digestAlgs.Add(FixAlgID(signer.DigestAlgorithmID));
				signerInfos.Add(signer.ToSignerInfo());
            }

			//
            // add the SignerInfo objects
            //
            foreach (SignerInf signer in signerInfs)
            {
				try
                {
					digestAlgs.Add(FixAlgID(signer.DigestAlgorithmID));
					signerInfos.Add(signer.ToSignerInfo(contentTypeOID, content));
                }
                catch (IOException e)
                {
                    throw new CmsException("encoding error.", e);
                }
                catch (InvalidKeyException e)
                {
                    throw new CmsException("key inappropriate for signature.", e);
                }
                catch (SignatureException e)
                {
                    throw new CmsException("error creating signature.", e);
                }
                catch (CertificateEncodingException e)
                {
                    throw new CmsException("error creating sid.", e);
                }
            }

			Asn1Set certificates = null;

			if (_certs.Count != 0)
			{
				certificates = CmsUtilities.CreateDerSetFromList(_certs);
			}

			Asn1Set certrevlist = null;

			if (_crls.Count != 0)
			{
				certrevlist = CmsUtilities.CreateDerSetFromList(_crls);
			}

			Asn1OctetString octs = null;
			if (encapsulate)
            {
                MemoryStream bOut = new MemoryStream();
                try
                {
                    content.Write(bOut);
                }
                catch (IOException e)
                {
                    throw new CmsException("encapsulation error.", e);
                }

				octs = new BerOctetString(bOut.ToArray());
            }

			Asn1.Cms.ContentInfo encInfo = new Asn1.Cms.ContentInfo(contentTypeOID, octs);

            Asn1.Cms.SignedData sd = new Asn1.Cms.SignedData(
                new DerSet(digestAlgs),
                encInfo,
                certificates,
                certrevlist,
                new DerSet(signerInfos));

            Asn1.Cms.ContentInfo contentInfo = new Asn1.Cms.ContentInfo(
                PkcsObjectIdentifiers.SignedData, sd);

            return new CmsSignedData(content, contentInfo);
        }

        /**
        * generate a signed object that for a CMS Signed Data
        * object - if encapsulate is true a copy
        * of the message will be included in the signature with the
        * default content type "data".
        */
        public CmsSignedData Generate(
            CmsProcessable	content,
            bool			encapsulate)
        {
            return this.Generate(Data, content, encapsulate);
        }
	}
}
