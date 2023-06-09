using System;
using System.IO;
﻿using System.Text;
using System.Text.Json;
using CommandLine;

namespace Learn;

class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<LearnOptions>(args).WithParsed<LearnOptions>(Learn);
    }

    public static void Learn(LearnOptions options)
    {
        //Console.WriteLine("Geben Sie den Pfad zur JSON-Datei ein:");
        //string hmmSimplePath = Console.ReadLine();

        string hmmSimplePath = options.InputFilePath;
        string hmmSimpleJson;
        string hmmObservationPath = options.ObservationFilePath;
        string hmmObservationJson;
        
        if (String.IsNullOrWhiteSpace(hmmSimplePath))
        {
            Console.WriteLine("HMM-Datei wurde nicht gefunden.");
            return;
        }

        if (String.IsNullOrWhiteSpace(hmmObservationPath))
        {
            Console.WriteLine("Observation-Datei wurde nicht gefunden.");
            return;
        }

        try
        {
            hmmSimpleJson = File.ReadAllText(hmmSimplePath);
            hmmObservationJson = File.ReadAllText(hmmObservationPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Lesen der Eingabedatei: {ex.ToString()}");
            return;
        }

        var hmmSimple = JsonSerializer.Deserialize<HmmSimple>(hmmSimpleJson);
        var hmmObservation = JsonSerializer.Deserialize<string[]>(hmmObservationJson);

        //Console.WriteLine("Wählen Sie den Algorithmus 'f' für Forward-Algorithmus, 'v' für Viterbi-Algorithmus:");
        //string userInput = Console.ReadLine();
        
        if (options.forward)
        {
            RunForwardAlgorithm(hmmSimple, hmmObservation);
        }
        else if (options.viterbi)
        {
            RunViterbiAlgorithm(hmmSimple, hmmObservation);
        }
        else
        {
            Console.WriteLine("Ungültige Eingabe. -f für ForwardAlgorithmus oder -v für ViterbiAlgorithmus.");
        }
    }

    static void RunViterbiAlgorithm(HmmSimple? hmmSimple, string[]? hmmObservation)
    {
        Console.WriteLine("Running Viterbi Algorithm");
        // Öffne eine neue Datei oder überschreibe eine vorhandene Datei
        using (StreamWriter writer = new StreamWriter("../data/hmm_output_viterbi.json"))
        {
            int[,] argmax = new int[hmmObservation.Length, hmmSimple.states.Length];
            double[,] delta = new double[hmmObservation.Length, hmmSimple.states.Length];

            // Initialisierung -> delta und argmax für t = 0
            for (int j = 0; j < hmmSimple.states.Length; j++)
            {
                delta[0, j] = hmmSimple.initial_distribution[j] * hmmSimple.output_probabilities.GetProbability(j, Array.IndexOf(hmmSimple.alphabet, hmmObservation[0]));
                argmax[0, j] = 0;
            }

            // Rekursion -> berechne delta und argmax für t > 0
            for (int t = 1; t < hmmObservation.Length; t++)
            {
                for (int j = 0; j < hmmSimple.states.Length; j++)
                {
                    double maxDelta = 0.0;
                    int argmaxDelta = 0;
                    for (int i = 0; i < hmmSimple.states.Length; i++)
                    {
                        double delta_i = delta[t - 1, i] * hmmSimple.transition_probabilities.GetProbability(i, j);
                        if (delta_i > maxDelta)
                        {
                            maxDelta = delta_i;
                            argmaxDelta = i;
                        }
                    }
                    delta[t, j] = maxDelta * hmmSimple.output_probabilities.GetProbability(j, Array.IndexOf(hmmSimple.alphabet, hmmObservation[t]));
                    argmax[t, j] = argmaxDelta;
                }
            }

            // Terminierung+Pfadermittlung -> finde das Ende des Pfads durch Rückverfolgung von argmax
            int[] path = new int[hmmObservation.Length];
            double maxDeltaEnd = 0.0;
            int argmaxDeltaEnd = 0;
            for (int j = 0; j < hmmSimple.states.Length; j++)
            {
                if (delta[hmmObservation.Length - 1, j] > maxDeltaEnd)
                {
                    maxDeltaEnd = delta[hmmObservation.Length - 1, j];
                    argmaxDeltaEnd = j;
                }
            }
            path[hmmObservation.Length - 1] = argmaxDeltaEnd;

            for (int t = hmmObservation.Length - 2; t >= 0; t--)
            {
                path[t] = argmax[t + 1, path[t + 1]];
            }

            //Ausgabe in hmm_output_viterbi.json und Konsole
            writer.WriteLine($"[\"{string.Join("\", \"", path.Select(i => hmmSimple.states[i]))}\"]");
            Console.WriteLine("File \"hmm_output_viterbi.json\" was created.");
            Console.WriteLine($"Path: {string.Join(", ", path.Select(i => hmmSimple.states[i]))}");
        }
    }

    static void RunForwardAlgorithm(HmmSimple hmmSimple, string[] hmmObservation)
    {
        Console.WriteLine("Running Forward Algorithm");
        // Öffne eine neue Datei oder überschreibe eine vorhandene Datei
        using (StreamWriter writer = new StreamWriter("../data/hmm_output_forward.txt"))
        {
        double[,] alpha = new double[hmmObservation.Length, hmmSimple.states.Length];

        // Initialisierung -> alpha für t = 0
        for (int j = 0; j < hmmSimple.states.Length; j++)
        {
            alpha[0, j] = hmmSimple.initial_distribution[j] * hmmSimple.output_probabilities.GetProbability(j, Array.IndexOf(hmmSimple.alphabet, hmmObservation[0]));
        }

        // Rekursion -> berechne alpha für t > 0
        for (int t = 1; t < hmmObservation.Length; t++)
        {
            for (int j = 0; j < hmmSimple.states.Length; j++)
            {
                double sum = 0.0;
                for (int i = 0; i < hmmSimple.states.Length; i++)
                {
                    sum += alpha[t - 1, i] * hmmSimple.transition_probabilities.GetProbability(i, j);
                }
                alpha[t, j] = sum * hmmSimple.output_probabilities.GetProbability(j, Array.IndexOf(hmmSimple.alphabet, hmmObservation[t]));
            }
        }

        // Terminierung -> berechne die Gesamt-Wahrscheinlichkeit der Beobachtungen
        double p = 0.0;
        for (int j = 0; j < hmmSimple.states.Length; j++)
        {
            p += alpha[hmmObservation.Length - 1, j];
        }

        //Ausgabe in hmm_output_forward.txt und Konsole -> Gesamt-Wahrscheinlichkeit
        writer.WriteLine($"{p}");
        Console.WriteLine("File \"hmm_output_forward.txt\" was created.");
        Console.WriteLine($"Forward Algorithm Result: {p}");
        }
    }
}

class HmmSimple
{
    public string[] states { get; set; }
    public double[] initial_distribution { get; set; }
    public TransitionProbabilities transition_probabilities { get; set; }
    public string[] alphabet { get; set; }
    public OutputProbabilities output_probabilities { get; set; }
}
class TransitionProbabilities
{
    public double[] s_0 { get; set; }
    public double[] s_1 { get; set; }
    public double GetProbability(int i, int j)
    {
        if (i == 0)
            return s_0[j];
        else
            return s_1[j];
    }
}
class OutputProbabilities
{
    public double[] s_0 { get; set; }
    public double[] s_1 { get; set; }
    public double GetProbability(int i, int o)
    {
        if (i == 0)
            return s_0[o];
        else
            return s_1[o];
    }
}

