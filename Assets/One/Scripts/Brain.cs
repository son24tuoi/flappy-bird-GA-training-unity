using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace One.ML.GA.TappyPane
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Brain : MonoBehaviour
    {
        private int dnaLength = 5;

        public DNA dna;

        public GameObject eyes;

        private bool seeDownWall = false;
        private bool seeUpWall = false;
        private bool seeBottom = false;
        private bool seeTop = false;

        private RaycastHit2D hit;
        private Vector3 startPosition;

        public float timeAlive = 0;
        public float distanceTravelled = 0;
        public int crash = 0;

        private bool isAlive = true;

        private Rigidbody2D rb;

        public void Init()
        {
            // 0 đi thẳng
            // 1 tường trên
            // 2 tường dưới
            // 3 bay lên

            dna = new DNA(dnaLength, 200);
            this.transform.Translate(Random.Range(-0.5f, 0.5f), Random.Range(-1.5f, 1.5f), 0);
            startPosition = this.transform.position;
            rb = GetComponent<Rigidbody2D>();
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

            hit = Physics2D.Raycast(eyes.transform.position, eyes.transform.forward, 1f);

            Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 1f, Color.red);
            Debug.DrawRay(eyes.transform.position, eyes.transform.up * 1f, Color.red);
            Debug.DrawRay(eyes.transform.position, -eyes.transform.up * 1f, Color.red);

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

            hit = Physics2D.Raycast(eyes.transform.position, eyes.transform.up, 1f);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Top"))
                {
                    seeTop = true;
                }
            }

            hit = Physics2D.Raycast(eyes.transform.position, -eyes.transform.up, 1f);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Bottom"))
                {
                    seeBottom = true;
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
            else
            {
                upForce = dna.GetGene(4);
            }

            rb.AddForce(this.transform.right * forwardForce);
            rb.AddForce(this.transform.up * upForce * 0.1f);
            distanceTravelled = Vector3.Distance(startPosition, this.transform.position);
        }
    }
}
