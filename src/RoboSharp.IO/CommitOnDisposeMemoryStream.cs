namespace RoboSharp.IO;

internal sealed class CommitOnDisposeMemoryStream : MemoryStream
{
    private readonly Action<byte[]> _commit;
    private bool _committed;

    public CommitOnDisposeMemoryStream(Action<byte[]> commit) => _commit = commit;

    protected override void Dispose(bool disposing)
    {
        if (disposing && !_committed)
        {
            _committed = true;
            _commit(ToArray());
        }

        base.Dispose(disposing);
    }
}
