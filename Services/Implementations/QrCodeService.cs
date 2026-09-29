using QRCoder;

namespace AsqueraLms.Api.Services.Implementations;

public interface IQrCodeService
{
    string GenerateBase64QrCode(string payload);
}

public class QrCodeService : IQrCodeService
{
    public string GenerateBase64QrCode(string payload)
    {
        if (string.IsNullOrWhiteSpace(payload)) return string.Empty;
        
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        using var pngByteQrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = pngByteQrCode.GetGraphic(20);
        return Convert.ToBase64String(qrCodeBytes);
    }
}
