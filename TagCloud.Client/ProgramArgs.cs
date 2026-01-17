namespace TagCloud.Client;

public record ProgramArgs(
    string WordsFile,
    string StopWordsFile,
    string OutputFile,
    int Width,
    int Height,
    string FontName);