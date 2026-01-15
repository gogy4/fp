using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace TagCloud.Abstractions.Generic;

public abstract class DocxFileProviderBase<T>(string filePath)
{
    private readonly string FilePath = filePath;

    protected IEnumerable<string> ReadParagraphs()
    {
        using var doc = WordprocessingDocument.Open(FilePath, false);
        var body = doc.MainDocumentPart.Document.Body;

        foreach (var paragraph in body.Elements<Paragraph>())
        {
            var text = paragraph.InnerText.Trim();
            if (!string.IsNullOrWhiteSpace(text))
                yield return text;
        }
    }
}