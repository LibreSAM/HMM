using CommandLine;

namespace Learn;
public class LearnOptions
{
    [Option('i', "inputFilePath", Required = true, HelpText = "Path to file with input text to learn.")]
    public string InputFilePath { get; set; } = "";

    [Option('f', "forwardAlg", Required = true, HelpText = "Run Forward Algorithmus.")]
    public bool forward { get; set; }

    [Option('v', "viterbiAlg", Required = false, HelpText = "Run Viterbi Algorithmus.")]
    public bool viterbi { get; set; }
}