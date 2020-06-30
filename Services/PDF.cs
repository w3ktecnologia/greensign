using iText.Forms;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Signatures;
using Org.BouncyCastle.X509;
using System;
using System.IO;

namespace Greensign
{
    class PDF
    {
        public static byte[] Sign(IExternalSignature externalSignature, X509Certificate[] certChain, string src, string friendlyName, string subject, string sourceName, string documentLink, string documentName)
        {
            int numberOfSignatures = 0;
            int numberOfPages = 0;

            using (PdfReader reader = new PdfReader(src))
            {
                using (PdfDocument pdf = new PdfDocument(reader))
                {
                    numberOfPages = pdf.GetNumberOfPages();

                    PdfAcroForm form = PdfAcroForm.GetAcroForm(pdf, false);
                    if (form != null)
                    {
                        foreach (var field in form.GetFormFields())
                        {
                            if (field.Value is iText.Forms.Fields.PdfSignatureFormField)
                                numberOfSignatures++;
                        }
                    }
                }
            }            

            if (numberOfSignatures == 0)
            {
                string hash = GetMD5HashFromFile(src);

                src = AddPage(src, sourceName, documentLink, documentName, hash);
                numberOfPages += 1;
            }
            
            float posSignY = 615 - (numberOfSignatures * 70);

            using (PdfReader reader = new PdfReader(src))
            {
                StampingProperties stampingProperties = new StampingProperties();
                stampingProperties.UseAppendMode();

                using (MemoryStream ms = new MemoryStream())
                {
                    PdfSigner signer =
                    new PdfSigner(reader, ms, stampingProperties);

                    Rectangle rect = new Rectangle(36, posSignY, 520, 65);

                    PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
                    appearance
                        .SetPageRect(rect)
                        .SetPageNumber(numberOfPages)
                        .SetCertificate(certChain[0]);

                    PdfFormXObject n2 = appearance.GetLayer2();
                    Canvas canvas = new Canvas(n2, signer.GetDocument());
                    
                    canvas.Add(new Paragraph(friendlyName).SetMargin(0));
                    canvas.Add(new Paragraph("Assinado digitalmente por: " + friendlyName).SetFontSize(10).SetMargin(0));
                    canvas.Add(new Paragraph("Data: " +  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz")).SetFontSize(10).SetMargin(0));
                    canvas.Add(new Paragraph("Subject: " + subject).SetFontSize(10).SetMargin(0));

                    signer.SignDetached(externalSignature, certChain, null, null, null, 0,
                        PdfSigner.CryptoStandard.CADES);

                    return ms.ToArray();
                    
                }
            }
        }


        private static string AddPage(string src, string sourceName, string documentLink, string documentName, string hash)
        {
            string dest = System.IO.Path.GetTempFileName();

            string temp = System.IO.Path.GetTempFileName();

            if (!string.IsNullOrEmpty(sourceName))
                sourceName = Uri.UnescapeDataString(sourceName);
            if (!string.IsNullOrEmpty(documentName))
                documentName = Uri.UnescapeDataString(documentName);

            PdfDocument pdfSignaturesPage = new PdfDocument(new PdfWriter(temp));
            Document docSignatures = new Document(pdfSignaturesPage);
            docSignatures.Add(new Paragraph("PROTOCOLO DE ASSINATURA(S)").SetFontSize(10).SetBold().SetUnderline());
            docSignatures.Add(new Paragraph("O documento acima foi proposto para assinatura digital na plataforma " + sourceName + ".").SetFontSize(10).SetMargin(0));
            docSignatures.Add(new Paragraph("Para verificar as assinaturas clique no link: " + documentLink).SetFontSize(10).SetMargin(0));
            docSignatures.Add(new Paragraph("Código do documento: " + documentName).SetFontSize(10).SetMargin(0));
            docSignatures.Add(new Paragraph("Hash do documento").SetFontSize(10).SetMargin(0));
            docSignatures.Add(new Paragraph(hash).SetFontSize(10).SetMargin(0));
            docSignatures.Add(new Paragraph("O(s) assinante(s) do documento é(são):").SetFontSize(10).SetMarginLeft(0).SetMarginTop(5));

            docSignatures.Close();
            pdfSignaturesPage.Close();

            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            PdfDocumentInfo info = pdfDoc.GetDocumentInfo();
            info.SetTitle(documentName);

            pdfSignaturesPage = new PdfDocument(new PdfReader(temp));

            pdfSignaturesPage.CopyPagesTo(1, 1, pdfDoc);
            pdfSignaturesPage.Close();
            pdfDoc.Close();

            return dest;
        }

        private static string GetMD5HashFromFile(string filename)
        {
            FileStream file = new FileStream(filename, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            string checkSum = BitConverter.ToString(retVal);

            return checkSum;
        }
    }
}
