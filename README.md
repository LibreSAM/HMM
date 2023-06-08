# Aufgabe 3 - HIDDEN MARKOV MODELS

| Gruppenmitglieder |
|-|
| Robin Augenstein |
| Kai Petelski |
| Jannick Gutekunst |

## TOC

- [Aufgabe 3 - HIDDEN MARKOV MODELS](#aufgabe-3---hidden-markov-models)
  - [TOC](#toc)
  - [Bauen der Programme](#bauen-der-programme)
  - [Starten der Programme](#starten-der-programme)
  - [Einlesen der Datei](#einlesen-der-datei)
  - [Ablauf des Forward Algorthmus](#ablauf-des-fordward-algorithmus)
  - [Ablauf des Viterbi Algorithmus](#ablauf-des-viterbi-algorithmus)

## Bauen der Programme

Die Anwendung kann mithilfe von Docker gebaut werden. Hierfür in das Hauptverzeichnis des Repositories wechseln und `docker build . -t hmm` ausführen. Hierdurch wird das Programm gebaut und direkt ausführbar in einem Docker-Image bereitgestellt.

Im Image wird kein `ENTRYPOINT` und kein `CMD` gesetzt, damit der Anwender flexibel auswählen kann, welche Datei bzw. Parametern er verwenden möchte.

## Starten der Programme

Auf Basis des zuvor erstellten Docker-Images können die Anwendungungen innerhalb eines Docker-Containers direkt gestartet werden. Nachdem dieses wie beschrieben erstellt wurde, können die Programme mithilfe eines Befehls nach dem Muster `docker run hmm dotnet Learn.dll <Parameter>` gestartet werden. Hierbei muss der Platzhalter `<Parameter>` durch die gewünschten Werte ersetzt werden.

- Das Programm akzeptiert folgende Aufrufparameter:
  - `Learn`

    | Parameter | Bedeutung |
    |-|-|
    | `-i`, `--inputFilePath` | Required. Path to file with input text to learn. |
    | `-f`, `--forward` | Run Forward-Algorithmus. |
    | `-v`, `--viterbi` | Run Viterbi-Algorithmus. |
    | `--help` | Display help screen. |

- Beispiel
  - Programm `Learn`
  - Eingabedatei `/data/hmm_simple.json`
  - Algorithmus: Forward
  - Datenaustausch zwischen Host und Container über Volume, Pfad zum Volume `./data`
  - Als Befehl ergibt sich hieraus:  
    `docker run -v $(pwd)/data:/data language_model dotnet Learn.dll -i /data/hmm_simple.json -f`

## Einlesen der Datei

 - Dateiname durch übergabeparameter -i definieren
 - Einlesen der HMM_Observation.json
 - Dateien werden mit .ReadAllText eingelesen -> Annahme dass die Daten im richtigen Format vorliegen
 - Abbildung der Daten auf sequenzielle Darstellungsform (hmmSimpleJson/hmmObservationJson)
 - Initialisieren des 5-Tupel im Objekt hmmSimple -> 'vgl. PDF-Seite 21 "Ein HMM ist ein 5-Tupel" (HMM-Foliensatz)'
 - Gewünschter Algorithmus durch Übergabeparameter -f oder -v definieren
 - Funktionsaufruf des entsprechendem Algorithmus mit Übergabe von hmmSimple und hmmObservation

## Ablauf des Forward Algorthmus

'vgl. PDF-Seite 52 (HMM-Foliensatz)'
 - Zuerst wird die Matrix "alpha", mit der Dimension [Anzahl der Beobachtungen] mal [Anzahl der Zustände], für t=0 berechnet (Wahrscheinlichkeit für jeden Zustand "j") -> for-Schleife mit Anzahl der Zustände
 - Dabei wird die Startverteilung mit der Ausgabewahrscheinlichkeit des aktuellen Zustands und der ersten Beobachtung multipliziert
 - Rekursive Berechnung der alpha-Werte: t>0 Wahrscheinlichkeit für jeden Zustand 'j' basierend auf vorherige Alpha-Werte -> verschachtelte for-Schleife
 - Für jeden Zustand 'j' wird die Summe der Produkte der vorherigen alpha-Werte (alpha[t-1, i]) mit der Übergangswahrscheinlichkeit von Zustand 'i' zu Zustand 'j' berechnet. Ergebnis wird mit Ausgabewahrscheinlichkeit von 'j' multipliziert.
 - Gesamtwahrscheinlichkeit der Beobachtungen: Gesamtwahrscheinlichkeit 'p' wird berechnet durch Summe der alpha-Werte des letzten Zeitpunkts für jeden Zustand 'j' -> for-Schleife wie bei Initialisierung -> vgl. PDF-Seite 51 "Schluss" (HMM-Foliensatz)
 - Schreiben von 'p' in txt-Datei / Ausgabe auf Konsole

## Ablauf des Viterbi Algorthmus

'wie Forward aber argmax statt E (Summe)  -> vgl. PDF-Seite 55 (HMM-Foliensatz)'
 - Initialisierung von 'argmax'- und 'delta'-Matrix (Dimension Anzahl Beobachtungen und Anzahl Zustände -> Länge von hmmObservation und hmmSimple.states) -> for-Schleife mit Anzahl der Zustände
 - 'argmax'- und 'delta'-Werte für ersten Zeitpunkt (t=0) -> Produkt aus Startverteilung und Ausgabewahrscheinlichkeit von aktuell 'j' und ersten Beobachtung in Delta
 - Rekursive Berechnung von 'argmax'- und 'delta'-Werten: t>0 'argmax'- und 'delta'-Werte basierend auf vorherigen Wert berechnen -> verschachtelte for-Schleife
 - 'delta'-Wert ist Produkt aus max. vorherigen 'delta'-Wert und Ausgabewahrscheinlichkeit des Zustands 'j' und der aktuellen Beobachtungen
 - 'argmax' speichert Zustand 'i', welcher zum max. vorherigen 'delta'-Wert geführt hat
 - Rückverfolgung: Ende des Pfades basierend auf max. 'delta'-Wert im letzten Zeitpunkt bestimmen -> for-Schleife wie bei Initialisierung + if-Abfrage
 - Pfad durch Rückverfolgung von 'argmax'-Werten von hinten nach vorne bestimmen
 - Schreiben des Zustandpfades in txt-Datei / Ausgabe auf Konsole