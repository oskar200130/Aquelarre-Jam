using Unity.Mathematics;
using UnityEngine;

public class Level4 : MonoBehaviour
{
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;
    private float[] _cubeYOffsets;
    private Matrix4x4[] _matrices;

    private Vector3[] _positions;
    private RenderParams _rp;

    private void Start()
    {
        var count = SceneTools.GetCount;
        _positions = new Vector3[count];
        _cubeYOffsets = new float[count];
        _matrices = new Matrix4x4[_positions.Length];

        SceneTools.LoopPositions((i, p) =>
        {
            _cubeYOffsets[i] = p.y;
            _positions[i] = p;
        });

        _rp = new RenderParams(_material);

        SceneTools.Instance.SetCountText(count);
        SceneTools.Instance.SetNameText("GPU Instancing");
    }


    public Vector3 screenPosition;
    public Vector3 worldPosition = new Vector3(0, 0, 0);
    private void Update()
    {
        var time = Time.time;

        screenPosition = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hitData))
        {
            worldPosition = hitData.point;
        }


        for (var i = 0; i < _positions.Length; i++)
        {

            var (pos, rot) = _positions[i].CalculatePosMouse(_cubeYOffsets[i], time, worldPosition);

            _matrices[i].SetTRS(pos, rot, SceneTools.CubeScale);


            _positions[i].y = pos.y;
        }

        Graphics.RenderMeshInstanced(_rp, _mesh, 0, _matrices);
    }
}