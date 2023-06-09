using CommandLine;

namespace Learn;
public class LearnOptions
{
    [Option('h', "hmmFilePath", Required = true, HelpText = "Path to file with hmm.")]
    public string InputFilePath { get; set; } = "";

    [Option('o', "observationFilePath", Required = true, HelpText = "Path to file with observation.")]
    public string ObservationFilePath { get; set; } = "";

    [Option('f', "forwardAlg", Required = false, HelpText = "Run Forward Algorithmus.")]
    public bool forward { get; set; }

    [Option('v', "viterbiAlg", Required = false, HelpText = "Run Viterbi Algorithmus.")]
    public bool viterbi { get; set; }
}