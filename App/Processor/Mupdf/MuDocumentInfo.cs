namespace MuPdfSharp;

public class MuDocumentInfo
{
    private readonly MuPdfDictionary _info;

    public MuDocumentInfo(MuPdfDictionary dictionary) {
        _info = dictionary;
    }

    public string Title => _info["Title"].StringValue;
    public string Subject => _info["Subject"].StringValue;
    public string Producer => _info["Producer"].StringValue;
    public string Creator => _info["Creator"].StringValue;
    public string Author => _info["Author"].StringValue;
    public string Keywords => _info["Keywords"].StringValue;
    public string CreationDate => _info["CreationDate"].StringValue;
    public string ModificationDate => _info["ModDate"].StringValue;
}