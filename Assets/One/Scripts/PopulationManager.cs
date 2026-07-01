using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace One.ML.GA.TappyPane
{
    public class PopulationManager : MonoBehaviour
    {
        public GameObject botPrefab;
        public GameObject startingPos;

        public int populationSize = 50;

        private List<GameObject> population = new List<GameObject>();

        public static float elapsed = 0;

        public float trialTime = 5;

        private int generation = 1;

        public float timeScale = 1f;

        public static int finishedCount = 0;

        public static int maxFinishedCount = 0;

        private GUIStyle guiStyle = new GUIStyle();

        private void OnGUI()
        {
            guiStyle.fontSize = 36;
            guiStyle.normal.textColor = Color.black;

            // tạo box hiển thị thông số có tên Stats
            // nội dung có Gen: generation,
            // nội dung có elapsed: elapsed dạng string Time: 0:0:00
            // nội dung có population: population.Count
            GUI.BeginGroup(new Rect(10, 10, 360, 300));
            GUI.Box(new Rect(0, 0, 360, 230), "Stats", guiStyle);
            GUI.Label(new Rect(20, 50, 330, 45), "Gen: " + generation, guiStyle);
            GUI.Label(new Rect(20, 100, 330, 45), string.Format("Time: {0:0.00}", elapsed), guiStyle);
            GUI.Label(new Rect(20, 150, 330, 45), "Population: " + population.Count, guiStyle);
            GUI.Label(new Rect(20, 200, 330, 45), "Finished: " + finishedCount, guiStyle);
            GUI.Label(new Rect(20, 250, 330, 45), "Max Finished: " + maxFinishedCount, guiStyle);
            GUI.EndGroup();
        }

        private void Start()
        {
            for (int i = 0; i < populationSize; i++)
            {
                GameObject bot = Instantiate(botPrefab, startingPos.transform.position, Quaternion.identity);
                bot.GetComponent<Brain>().Init();
                population.Add(bot);
            }

            Time.timeScale = timeScale;

            maxFinishedCount = 0;
        }

        private GameObject Breed(GameObject parent1, GameObject parent2)
        {
            return Breed(parent1, parent2, true);
        }

        private GameObject Breed(GameObject parent1, GameObject parent2, bool allowMutation)
        {
            GameObject offspring = Instantiate(botPrefab, startingPos.transform.position, Quaternion.identity);
            Brain brain = offspring.GetComponent<Brain>();
            brain.Init();

            if (allowMutation && Random.Range(0, 100) == 1)
            {
                brain.dna.Mutate();
            }
            else
            {
                brain.dna.Combine(parent1.GetComponent<Brain>().dna, parent2.GetComponent<Brain>().dna);
            }

            return offspring;
        }

        private GameObject SelectParent(List<GameObject> rankedPopulation)
        {
            int selectionWindow = Mathf.Max(1, rankedPopulation.Count / 2);
            int index = Mathf.FloorToInt(Mathf.Pow(Random.value, 2f) * selectionWindow);
            return rankedPopulation[Mathf.Clamp(index, 0, rankedPopulation.Count - 1)];
        }

        private void BreedNewPopulation()
        {
            finishedCount = 0;

            List<GameObject> sortedList =
                population.OrderByDescending(
                    o => o.GetComponent<Brain>().GetFitness()).ToList();

            population.Clear();

            int eliteCount = Mathf.Clamp(populationSize / 10, 1, populationSize);

            // Giữ lại một phần nhỏ cá thể tốt nhất để tránh mất nghiệm tốt qua từng thế hệ.
            for (int i = 0; i < eliteCount && i < sortedList.Count; i++)
            {
                population.Add(Breed(sortedList[i], sortedList[i], false));
            }

            while (population.Count < populationSize)
            {
                GameObject parent1 = SelectParent(sortedList);
                GameObject parent2 = SelectParent(sortedList);
                population.Add(Breed(parent1, parent2));
            }

            for (int i = 0; i < sortedList.Count; i++)
            {
                Destroy(sortedList[i]);
            }

            generation++;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;

            if (elapsed >= trialTime)
            {
                BreedNewPopulation();
                elapsed = 0;
            }
        }
    }
}