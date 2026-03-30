using System.Text;

namespace RoboSharp.IO;

public sealed class Utf8TextEncodingPolicy : ITextEncodingPolicy
{
    public Encoding DefaultEncoding { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
}
