using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MeshDrawer : MonoBehaviour
{
    public bool gameOn = true;
    public bool limitsEnabled = true; //change me

    public float lineWidth = 1f;
    public int howManyPhysicsObjectsAllowed = 7;
    public float yLineNotToPass;

    public float maxLimit = 5f;

    public Material lineColorMat;
    public Material meshColorMat;

    public List<GameObject> activeDrawings;
    public GameObject fillFab;
    public GameObject particles;
    public GameObject illustrativeLine;

    public GameObject limitText;

    //public List<Vector3> indeciesToDraw = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        activeDrawings = new List<GameObject>();
        illustrativeLine.transform.position = new Vector3(0f, yLineNotToPass, illustrativeLine.transform.position.z);
        if (LimitKeeper.limitKeeper != null)
        {
            limitsEnabled = LimitKeeper.limitKeeper.limit;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && gameOn)
        {
            StartCoroutine(Draw());
        }

        if(Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene(2);
        }

        if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            SceneManager.LoadScene(3);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (LimitKeeper.limitKeeper != null)
            {
                LimitKeeper.limitKeeper.limit = !LimitKeeper.limitKeeper.limit;
            }
        }

        /*if (Input.GetMouseButton(0))
        {

        }

        if (Input.GetMouseButtonUp(0))
        {

        }*/
    }

    //Method taken from internet
    IEnumerator Draw()
    {
        LineRenderer r = new GameObject().AddComponent<LineRenderer>();

        r.transform.SetParent(transform);

        r.material = lineColorMat;

        r.startWidth = lineWidth;
        r.endWidth = lineWidth;

        List<Vector3> positions = new List<Vector3>();
        float usedLimit = 0f;

        while (Input.GetMouseButton(0) && Camera.main.ScreenToWorldPoint(Input.mousePosition).y > yLineNotToPass)
        {
            positions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 4);

            particles.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            r.positionCount = positions.Count;

            r.SetPositions(positions.ToArray());

            if (limitsEnabled)
            {
                usedLimit = 0;
                for (int i = 0; i < positions.Count - 1; i++)
                {
                    usedLimit += (positions[i + 1] - positions[i]).magnitude;
                    limitText.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, limitText.transform.position.z);
                    limitText.GetComponent<Text>().text = usedLimit.ToString("F1") + "/" + maxLimit.ToString("F1");
                    if(usedLimit <= maxLimit)
                    {
                        limitText.GetComponent<Text>().color = Color.green;
                    } else
                    {
                        limitText.GetComponent<Text>().color = Color.red;
                    }
                }
            }

            yield return new WaitForSeconds(.02f);
        }

        limitText.transform.position = new Vector3(300f, 300f, limitText.transform.position.z);
        //particles.GetComponent<ParticleSystem>().shape.position = new Vector3(200f, 200f, 0f);
        particles.transform.position = new Vector3(200f, 200f, 0f);

        if (usedLimit > maxLimit)
        {
            Destroy(r.gameObject);
            yield return null;
        }
        else
        {

            r.useWorldSpace = false;
            r.loop = true;

            List<Vector2> positions2 = new List<Vector2>();

            for (int i = 0; i < positions.Count; i++)
            {
                positions2.Add(new Vector2(positions[i].x, positions[i].y));
            }

            PolygonCollider2D col = r.gameObject.AddComponent<PolygonCollider2D>();
            col.points = positions2.ToArray();

            col.gameObject.AddComponent<Rigidbody2D>();

            SpriteShapeController newFill = GameObject.Instantiate(fillFab, r.gameObject.transform.position, Quaternion.identity, r.gameObject.transform).GetComponent<SpriteShapeController>();
            SpriteShapeController newFill2 = GameObject.Instantiate(fillFab, r.gameObject.transform.position, Quaternion.identity, r.gameObject.transform).GetComponent<SpriteShapeController>();

            Spline spliner = newFill.spline;
            spliner.Clear();

            spliner.isOpenEnded = false;

            int counter = 0;
            for (int i = 0; i < positions.Count; i++)
            {
                if (i > 0)
                {
                    Vector3 diff = positions[i] * 10000 - positions[i - 1] * 10000;
                    if (diff.magnitude < .01f)
                    {
                        continue;
                    }
                }
                spliner.InsertPointAt(counter, positions[i] * 10000);
                counter++;
            }

            newFill.RefreshSpriteShape();
            newFill.transform.localScale = new Vector3(.0001f, .0001f, .0001f);

            Spline spliner2 = newFill2.spline;
            spliner2.Clear();

            counter = 0;
            for (int i = positions.Count - 1; i > 0; i--)
            {
                if (i < positions.Count - 1)
                {
                    Vector3 diff = positions[i] * 10000 - positions[i + 1] * 10000;
                    if (diff.magnitude < .01f)
                    {
                        continue;
                    }
                }
                spliner2.InsertPointAt(counter, positions[i] * 10000);
                counter++;
            }

            newFill2.RefreshSpriteShape();
            newFill2.transform.localScale = new Vector3(.0001f, .0001f, .0001f);

            //newFill.spriteShape.

            /*SpriteShapeController newFill = r.gameObject.AddComponent<SpriteShapeController>();
            SpriteShapeRenderer newRend = r.gameObject.GetComponent<SpriteShapeRenderer>();

            Spline spliner = new Spline();
            for (int i = 0; i < positions.Count; i++)
            {
                spliner.InsertPointAt(i, positions[i]);
            }

            newRend.material = meshColorMat;*/

            //mesh generation to fill in rocks
            /*Mesh newMesh = new Mesh();

            newMesh.SetVertices(positions.ToArray());

            int[] tris = new int[(positions.Count*6)];
            int triCounter = 0;
            for (int i = 0; i < positions.Count-1; i++)
            {
                tris[triCounter] = i;
                triCounter++;
                tris[triCounter] = i+1;
                triCounter++;
                tris[triCounter] = positions.Count-1;
                triCounter++;
            }
            for (int i = positions.Count-1; i > 1; i--)
            {
                tris[triCounter] = i;
                triCounter++;
                tris[triCounter] = i - 1;
                triCounter++;
                tris[triCounter] = 0;
                triCounter++;
            }
            newMesh.triangles = tris;

            Vector3[] norms = new Vector3[positions.Count];
            for (int i = 0; i < norms.Length; i++)
            {
                norms[i] = -Vector3.forward;
            }
            newMesh.normals = norms;

            col.gameObject.AddComponent<MeshFilter>().mesh = newMesh;
            col.gameObject.AddComponent<MeshRenderer>().material = meshColorMat;*/

            activeDrawings.Add(r.gameObject);
            if (activeDrawings.Count > howManyPhysicsObjectsAllowed)
            {
                Destroy(activeDrawings[0]);
                activeDrawings.RemoveAt(0);
            }
        }
    }
}
