using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zaawansowane_programowanie
{
    internal class Algorytm
    {
        public readonly BackgroundWorker worker = new BackgroundWorker();
        public delegate void AlgorytmZakonczonyHandler(AlgorytmWynik wynik);
        public event AlgorytmZakonczonyHandler AlgorytmZakonczony;
        public event Action<List<int>> NajlepszeRozwiazanieZmienione;
        static Queue<int> tabuAdded = new Queue<int>();
        static Queue<int> tabuRemoved = new Queue<int>();
        static HashSet<string> previouslyGeneratedSolutions = new HashSet<string>();
        List<int> najlepszeRozwiazanie = new List<int>();


        public Algorytm()
        {
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        public class AlgorytmWynik
        {
            public List<int> Rozwiazanie { get; set; }
            public TimeSpan CzasWykonania { get; set; }

            public AlgorytmWynik(List<int> rozwiazanie, TimeSpan czasWykonania)
            {
                Rozwiazanie = rozwiazanie;
                CzasWykonania = czasWykonania;
            }
        }

        public void StartTask(object argument)
        {
            if (!worker.IsBusy)
                worker.RunWorkerAsync(argument);
        }
        public void CancelTask()
        {
            if (worker.WorkerSupportsCancellation)
            {
                worker.CancelAsync();
            }
        }

        private void ZglosZmianeNajlepszegoRozwiazania()
        {
            NajlepszeRozwiazanieZmienione?.Invoke(najlepszeRozwiazanie);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                AlgorytmZakonczony?.Invoke(new AlgorytmWynik(najlepszeRozwiazanie, TimeSpan.Zero));
            }
            else if (e.Error != null)
            {
                MessageBox.Show($"Wystąpił błąd: {e.Error.Message}");
            }
            else if (e.Result is AlgorytmWynik wynik)
            {
                AlgorytmZakonczony?.Invoke(wynik);
            }
        }

        static List<int> GenerateInitialSolution(List<int> distances)
        {
            var counts = distances.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            var keys = counts.Keys.ToList();
            var rng = new Random();
            var solution = new List<int>();

            keys = keys.OrderBy(x => rng.Next()).ToList();

            foreach (var x1 in keys)
            {
                foreach (var x2 in keys)
                {
                    if (x1 == x2 && counts[x1] < 2) continue;
                    int x3 = x1 + x2;

                    if (counts.ContainsKey(x3) && counts[x3] > 0)
                    {
                        if (x1 == x2 && counts[x1] > 1)
                        {
                            solution = new List<int> { 0, x1, x3 };
                        }
                        else if (x1 != x2)
                        {
                            solution = new List<int> { 0, x1, x3 };
                        }

                        if (solution.Any())
                        {
                            if (AddSolutionIfUnique(solution))
                            {
                                return solution; 
                            }
                            else
                            {
                                solution.Clear();
                            }
                        }
                    }
                }
            }

            return new List<int> { 0 };
        }


        public static List<List<int>> GenerateNeighbours(List<int> aktualneRozwiazanie, List<int> instancja)
        {
            List<List<int>> neighbours = new List<List<int>>();
            int maxPossiblePoint = instancja.Max();

            for (int i = 1; i <= maxPossiblePoint; i++)
            {
                if (!aktualneRozwiazanie.Contains(i) && !tabuRemoved.Contains(i)) 
                {
                    List<int> newSolution = new List<int>(aktualneRozwiazanie) { i };
                    newSolution.Sort();

                    if (AllDistancesValid(newSolution, instancja))
                    {
                        neighbours.Add(newSolution);
                    }
                }
            }

            return neighbours;
        }

        private static bool AddSolutionIfUnique(List<int> solution)
        {
            string solutionKey = string.Join(",", solution);
            if (!previouslyGeneratedSolutions.Contains(solutionKey))
            {
                previouslyGeneratedSolutions.Add(solutionKey);
                return true;
            }
            return false;
        }

        public static List<int> EscapeLocalOptimum(List<int> aktualneRozwiazanie, List<int> instancja, int liczbaProgowPogarszania, ref int liczbaUsunietychElementow)
        {
            bool allElementsTabu = aktualneRozwiazanie.All(e => tabuAdded.Contains(e));

            if (aktualneRozwiazanie.Count > 1 && !allElementsTabu && liczbaUsunietychElementow < liczbaProgowPogarszania) //dodano 3 warunek
            {
                for (int i = aktualneRozwiazanie.Count - 1; i >= 0; i--)
                {
                    if (!tabuAdded.Contains(aktualneRozwiazanie[i]) && !aktualneRozwiazanie[i].Equals(0))
                    {
                        tabuRemoved.Enqueue(aktualneRozwiazanie[i]);
                        aktualneRozwiazanie.RemoveAt(i);
                        liczbaUsunietychElementow++;
                        break; 
                    }
                }
            }
            else if(allElementsTabu || liczbaUsunietychElementow >= liczbaProgowPogarszania) //zamiana na else if i dodanie >= zamiast >
            {
                liczbaUsunietychElementow = 0;
                aktualneRozwiazanie = GenerateInitialSolution(instancja);
            }

            return aktualneRozwiazanie;
        }

        private static bool AllDistancesValid(List<int> solution, List<int> instancja)
        {
            Dictionary<int, int> distanceCounts = new Dictionary<int, int>();

            foreach (var distance in instancja)
            {
                if (distanceCounts.ContainsKey(distance))
                    distanceCounts[distance]++;
                else
                    distanceCounts.Add(distance, 1);
            }

            for (int i = 0; i < solution.Count; i++)
            {
                for (int j = i + 1; j < solution.Count; j++)
                {
                    int currentDistance = Math.Abs(solution[j] - solution[i]);
                    if (distanceCounts.ContainsKey(currentDistance) && distanceCounts[currentDistance] > 0)
                    {
                        distanceCounts[currentDistance]--;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static List<int> GenerateAllDistances(List<int> solution)
        {
            List<int> distances = new List<int>();
            for (int i = 0; i < solution.Count; i++)
            {
                for (int j = i + 1; j < solution.Count; j++)
                {
                    distances.Add(Math.Abs(solution[j] - solution[i]));
                }
            }
            return distances;
        }

        private static IEnumerable<int> GeneratePotentialCuts(List<int> solution, List<int> instancja)
        {
            int maxDistance = instancja.Max();
            for (int i = 0; i <= maxDistance; i++)
            {
                if (!solution.Contains(i))
                {
                    yield return i;
                }
            }
        }

        private static double EvaluateFuturePotential(List<int> solution, List<int> instancja)
        {
            var instancjaDistances = new HashSet<int>(instancja);

            double potentialScore = 0;

            foreach (var newCut in GeneratePotentialCuts(solution, instancja))
            {
                var tempSolution = new List<int>(solution) { newCut };
                tempSolution.Sort();
                var newDistances = GenerateAllDistances(tempSolution);

                var uniqueNewDistances = newDistances.Where(dist => !solution.Contains(dist) && instancjaDistances.Contains(dist)).Distinct().Count();

                potentialScore += uniqueNewDistances;
            }

            return potentialScore;
        }

        private static void UpdateTabuLists(int wielkoscTabu, List<int> oldSolution, List<int> newSolution)
        {
            var addedElements = newSolution.Except(oldSolution).ToList();
            var removedElements = oldSolution.Except(newSolution).ToList();

            foreach (var element in addedElements)
            {
                tabuAdded.Enqueue(element);
                if (tabuAdded.Count > wielkoscTabu)
                {
                    tabuAdded.Dequeue(); 
                }
            }

            foreach (var element in removedElements)
            {
                tabuRemoved.Enqueue(element);
                if (tabuRemoved.Count > wielkoscTabu)
                {
                    tabuRemoved.Dequeue();
                }
            }
        }

        public static List<int> SelectBestCandidate(List<int> aktualneRozwiazanie, List<int> instancja, int wielkoscTabu)
        {
            List<List<int>> neighbours = GenerateNeighbours(aktualneRozwiazanie, instancja);

            double bestScore = Double.NegativeInfinity;
            List<int> najlepsze = null;

            foreach (var neighbour in neighbours)
            {
                double score = EvaluateFuturePotential(neighbour, instancja);
                if (score > bestScore)
                {
                    bestScore = score;
                    najlepsze = neighbour;
                }
            }

            if (najlepsze != null)
            {
                UpdateTabuLists(wielkoscTabu, aktualneRozwiazanie, najlepsze);
            }

            return najlepsze;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            List<int> instancja = new List<int>();
            List<int> aktualneRozwiazanie = new List<int>();

            double iteracjeBezPoprawy = 0;
            var parameters = e.Argument as Tuple<string, int, int, int, int>;
            instancja.AddRange(parameters.Item1.Split(' ').Select(int.Parse));
            int wielkoscListTabu = parameters.Item2;
            int liczbaProgowPogarszania = parameters.Item3;
            int maxIteracjiBezPoprawy = parameters.Item4;
            int maxCzas = parameters.Item5;
            int liczbaUsunietychElementow = 0;

            var worker = sender as BackgroundWorker;

            aktualneRozwiazanie = GenerateInitialSolution(instancja);
            AddSolutionIfUnique(aktualneRozwiazanie);
            foreach (int element in aktualneRozwiazanie)
            {
                najlepszeRozwiazanie.Add(element);
            }

            var startTime = DateTime.Now;
            while (iteracjeBezPoprawy < maxIteracjiBezPoprawy && (DateTime.Now - startTime).TotalSeconds < maxCzas && !e.Cancel == true)
            {
                List<int> kandydat = SelectBestCandidate(aktualneRozwiazanie, instancja, wielkoscListTabu);

                if (kandydat == null || !kandydat.Any())
                {
                    aktualneRozwiazanie = EscapeLocalOptimum(aktualneRozwiazanie, instancja, liczbaProgowPogarszania, ref liczbaUsunietychElementow);
                    //iteracjeBezPoprawy += 0.1;
                    continue;
                }
                else
                {
                    if (kandydat.Count > najlepszeRozwiazanie.Count)
                    {
                        int sumaKandydat = 0;
                        foreach (int element in kandydat)
                        {
                            sumaKandydat += element;
                        }
                        int sumaNajlepsze = 0;
                        foreach (int element in najlepszeRozwiazanie)
                        {
                            sumaNajlepsze += element;
                        }
                        if(sumaKandydat > sumaNajlepsze)
                        {
                            najlepszeRozwiazanie.Clear();
                            foreach (int element in kandydat)
                            {
                                najlepszeRozwiazanie.Add(element);
                            }
                            iteracjeBezPoprawy = 0; 
                            e.Result = najlepszeRozwiazanie;
                            ZglosZmianeNajlepszegoRozwiazania(); 
                        }
                    }
                    else
                    {
                        iteracjeBezPoprawy += 1;
                    }

                    foreach (int element in kandydat)
                    {
                        aktualneRozwiazanie.Add(element);
                    }
                }

                if (worker.CancellationPending) 
                {
                    e.Cancel = true; 
                    e.Result = najlepszeRozwiazanie;
                    return; 
                }
            }
            var czasWykonania = DateTime.Now - startTime;
            e.Result = new AlgorytmWynik(najlepszeRozwiazanie, czasWykonania);          
        }
    }
}