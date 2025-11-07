using axionpro.application.Interfaces.IQRService;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.infrastructure.QRServies
{
    public class QRService : IQRService
    {
        public byte[] GenerateQrCode(string jsonData, int pixelsPerModule = 20)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(jsonData, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);  // SkiaSharp ke liye PNG byte QR

            // Generate QR code as PNG byte array directly
            var qrBytes = qrCode.GetGraphic(pixelsPerModule);
            return qrBytes;
        }

        public async Task<string> SaveQrCodeToFileAsync(string jsonData, string filePath, int pixelsPerModule = 20)
        {
            var qrBytes = GenerateQrCode(jsonData, pixelsPerModule);
            await File.WriteAllBytesAsync(filePath, qrBytes);
            return filePath;
        }
    }
     


}
