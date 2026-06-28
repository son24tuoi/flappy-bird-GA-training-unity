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

        private GUIStyle guiStyle = new GUIStyle();

        private void OnGUI()
        {
            guiStyle.fontSize = 36;
            guiStyle.normal.textColor = Color.black;

            // tạo box hiển thị thông số có tên Stats
            // nội dung có Gen: generation,
            // nội dung có elapsed: elapsed dạng string Time: 0:0:00
            // nội dung có population: population.Count
            GUI.BeginGroup(new Rect(10, 10, 360, 230));
            GUI.Box(new Rect(0, 0, 360, 230), "Stats", guiStyle);
            GUI.Label(new Rect(20, 50, 330, 45), "Gen: " + generation, guiStyle);
            GUI.Label(new Rect(20, 100, 330, 45), string.Format("Time: {0:0.00}", elapsed), guiStyle);
            GUI.Label(new Rect(20, 150, 330, 45), "Population: " + population.Count, guiStyle);
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
        }

        private GameObject Breed(GameObject parent1, GameObject parent2)
        {
            GameObject offspring = Instantiate(botPrefab, startingPos.transform.position, Quaternion.identity);
            Brain brain = offspring.GetComponent<Brain>();
            brain.Init();

            if (Random.Range(0, 100) == 1)
            {
                brain.dna.Mutate();
            }
            else
            {
                brain.dna.Combine(parent1.GetComponent<Brain>().dna, parent2.GetComponent<Brain>().dna);
            }

            return offspring;
        }

        private void BreedNewPopulation()
        {
            List<GameObject> sortedList =
                population.OrderBy(
                    o => o.GetComponent<Brain>().distanceTravelled * 3 -
                    o.GetComponent<Brain>().crash +
                    o.GetComponent<Brain>().timeAlive * 2).ToList();

            population.Clear();

            for (int i = (int)(sortedList.Count * 0.75f) - 1; i < sortedList.Count - 1; i++)
            {
                population.Add(Breed(sortedList[i], sortedList[i + 1]));
                population.Add(Breed(sortedList[i + 1], sortedList[i]));
                population.Add(Breed(sortedList[i], sortedList[i + 1]));
                population.Add(Breed(sortedList[i + 1], sortedList[i]));
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