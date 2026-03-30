using System.Text;

namespace RoboSharp.IO;

public interface ITextEncodingPolicy
{
    Encoding DefaultEncoding { get; }
}
