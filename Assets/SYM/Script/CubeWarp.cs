using UnityEngine;

[ExecuteAlways] // ✅ 실행하지 않아도 자동으로 적용됨
public class CubeWarp : MonoBehaviour
{
    public Transform[] controlPoints = new Transform[8]; // 8개 점을 개별 조정

    private MeshFilter meshFilter;
    private Vector3[] originalVertices;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) return;

        originalVertices = meshFilter.mesh.vertices;
    }

    void Update()
    {
        if (controlPoints.Length < 8 || meshFilter == null || originalVertices == null) return;

        Vector3[] vertices = originalVertices.Clone() as Vector3[];

        for (int i = 0; i < 8; i++)
        {
            if (controlPoints[i] != null)
                vertices[i] = transform.InverseTransformPoint(controlPoints[i].position); // 월드 좌표 → 로컬 변환
        }

        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateNormals();
    }
}
