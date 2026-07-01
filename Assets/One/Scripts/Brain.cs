using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace One.ML.GA.TappyPane
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Brain : MonoBehaviour
    {
        private int dnaLength = 7;
        private float rayDistance = 0.5f;

        public DNA dna;

        public GameObject eyes;

        private bool seeDownWall = false;
        private bool seeUpWall = false;
        private bool seeBottom = false;
        private bool seeTop = false;

        // thấy tường chéo 45 độ lên
        private bool seeUp45 = false;
        // thấy tường chéo 45 độ xuống
        private bool seeDown45 = false;

        private RaycastHit2D hit;
        private Vector3 startPosition;

        public float timeAlive = 0;
        public float distanceTravelled = 0;
        public int crash = 0;

        private bool isAlive = true;
        public bool isFinished = false;

        private Rigidbody2D rb;

        public void Init()
        {
            // 0 đi thẳng
            // 1 tường trên
            // 2 tường dưới
            // 3 bay lên
            // 4 chéo 45 lên
            // 5 chéo 45 xuống

            dna = new DNA(dnaLength, 200);
            this.transform.Translate(Random.Range(-0.1f, 0.1f), Random.Range(-0.3f, 0.3f), 0);
            startPosition = this.transform.position;
            rb = GetComponent<Rigidbody2D>();

            timeAlive = 0;
            distanceTravelled = 0;
            crash = 0;
            isAlive = true;
            isFinished = false;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Top") ||
                collision.gameObject.CompareTag("Bottom") ||
                collision.gameObject.CompareTag("UpWall") ||
                collision.gameObject.CompareTag("DownWall"))
            {
                crash++;
            }
            else if (collision.gameObject.CompareTag("Finish"))
            {
                isFinished = true;
                rb.bodyType = RigidbodyType2D.Static;
                PopulationManager.finishedCount++;
                PopulationManager.maxFinishedCount = Mathf.Max(PopulationManager.maxFinishedCount, PopulationManager.finishedCount);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Top") ||
                collision.gameObject.CompareTag("Bottom") ||
                collision.gameObject.CompareTag("UpWall") ||
                collision.gameObject.CompareTag("DownWall"))
            {
                isAlive = false;
            }
        }

        private void Update()
        {
            if (!isAlive) return;

            seeDownWall = false;
            seeUpWall = false;
            seeBottom = false;
            seeTop = false;
            seeUp45 = false;
            seeDown45 = false;

            Debug.DrawRay(eyes.transform.position, eyes.transform.forward * rayDistance, Color.red);
            Debug.DrawRay(eyes.transform.position, eyes.transform.up * rayDistance, Color.red);
            Debug.DrawRay(eyes.transform.position, -eyes.transform.up * rayDistance, Color.red);
            // vẽ ray chéo 45 độ lên
            Debug.DrawRay(eyes.transform.position, (eyes.transform.forward + eyes.transform.up).normalized * rayDistance, Color.red);
            // vẽ ray chéo 45 độ xuống
            Debug.DrawRay(eyes.transform.position, (eyes.transform.forward - eyes.transform.up).normalized * rayDistance, Color.red);

            hit = Physics2D.Raycast(eyes.transform.position, eyes.transform.forward, rayDistance);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("DownWall"))
                {
                    seeDownWall = true;
                }
                else if (hit.collider.CompareTag("UpWall"))
                {
                    seeUpWall = true;
                }
            }

            hit = Physics2D.Raycast(eyes.transform.position, eyes.transform.up, rayDistance);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Top"))
                {
                    seeTop = true;
                }
            }

            hit = Physics2D.Raycast(eyes.transform.position, -eyes.transform.up, rayDistance);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Bottom"))
                {
                    seeBottom = true;
                }
            }

            // ray chéo 45 độ lên
            hit = Physics2D.Raycast(eyes.transform.position, (eyes.transform.forward + eyes.transform.up).normalized, rayDistance);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("UpWall") ||
                    hit.collider.CompareTag("Top"))
                {
                    seeUp45 = true;
                }
            }

            // ray chéo 45 độ xuống
            hit = Physics2D.Raycast(eyes.transform.position, (eyes.transform.forward - eyes.transform.up).normalized, rayDistance);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("DownWall") ||
                    hit.collider.CompareTag("Bottom"))
                {
                    seeDown45 = true;
                }
            }

            timeAlive = PopulationManager.elapsed;
        }

        private void FixedUpdate()
        {
            if (!isAlive) return;

            float upForce = 0;
            float forwardForce = 1f;

            if (seeUpWall)
            {
                upForce = dna.GetGene(0);
            }
            else if (seeDownWall)
            {
                upForce = dna.GetGene(1);
            }
            else if (seeTop)
            {
                upForce = dna.GetGene(2);
            }
            else if (seeBottom)
            {
                upForce = dna.GetGene(3);
            }
            else if (seeUp45)
            {
                upForce = dna.GetGene(4);
            }
            else if (seeDown45)
            {
                upForce = dna.GetGene(5);
            }
            else
            {
                upForce = dna.GetGene(6);
            }

            rb.AddForce(this.transform.right * forwardForce);
            rb.AddForce(this.transform.up * upForce * 0.2f);
            distanceTravelled = Vector3.Distance(startPosition, this.transform.position);
        }

        public float GetFitness()
        {
            float fitness = distanceTravelled * 1 - crash + timeAlive * 3;

            if (isFinished)
            {
                fitness *= 4;
            }

            return fitness;
        }
    }
}
