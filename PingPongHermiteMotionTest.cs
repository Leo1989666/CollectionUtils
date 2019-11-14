using UnityEngine;

public class PingPongHermiteMotionTest : MonoBehaviour
{
    public GameObject CubePrefab;
    public Transform[] Modes;

    public int Number = 10;

    public int Space = 2;
    
    [SerializeField]
    private float ZicZaglength = 2;
    
    public float Speed = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        Modes = new Transform[Number];
        for (int i = 0; i < Number; i++)
        {
            Transform cube = Instantiate(CubePrefab).transform;
            cube.name = string.Format("Cube {0}", i);
            cube.SetParent(transform);
            Vector3 position = new Vector3();
            position.x = Space * i - (Space * Number * 0.5f);
            position.z = 0f;
            
            cube.localPosition = position;
            
            Modes[i] = cube;
        }
        
        // Calc
        float heightOffset = ZicZaglength / Modes.Length;
        float halfLength = ZicZaglength * 0.5f;
        
        int length = Modes.Length;
        for (int i = 0; i < length; i++)
        {
            Transform cube = Modes[i];

            Vector3 position = cube.localPosition;
            position.y = ZicZaglength * heightOffset * i - halfLength;

            cube.localPosition = position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float heightOffset = ZicZaglength / Modes.Length;
        float halfLength = ZicZaglength * 0.5f;
        
        int length = Modes.Length;
        for (int i = 0; i < length; i++)
        {
            Transform cube = Modes[i];

            float offset = ZicZaglength * heightOffset * i - halfLength;
            
            Vector3 cubePos = cube.localPosition;
//            cubePos.y = Mathf.PingPong(Time.time * Speed + offset, ZicZaglength) - halfLength;
            cubePos.y = Hermite(Mathf.PingPong(Time.time * Speed + offset, 1.0f)) * ZicZaglength - halfLength;            

            cube.localPosition = cubePos;
        }
    }
    
    float Hermite(float t)
    {
        return -t*t*t*2f + t*t*3f;
    }
}
