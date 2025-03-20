using UnityEngine;

public class HomographyParamsSender : MonoBehaviour
{
    public Material targetMaterial;

    // 원본 좌표들 (예시)
    public Vector2 cornerA = new Vector2(0, 0);
    public Vector2 cornerB = new Vector2(1, 0);
    public Vector2 cornerC = new Vector2(0, 1);
    public Vector2 cornerD = new Vector2(1, 1);

    // 변형된 좌표들 (예시)
    public Vector2 distortedA = new Vector2(0.1f, 0.1f);
    public Vector2 distortedB = new Vector2(0.9f, 0.2f);
    public Vector2 distortedC = new Vector2(0.2f, 0.8f);
    public Vector2 distortedD = new Vector2(0.8f, 0.9f);

    void Start()
    {
        // Homography 매개변수 계산
        Vector4 homographyParams = CalculateHomographyParams(cornerA, cornerB, cornerC, cornerD, distortedA, distortedB, distortedC, distortedD);

        // c1, c2만 쉐이더로 전달
        targetMaterial.SetVector("_c1c2", new Vector2(homographyParams.x, homographyParams.y));
    }

    // c1, c2 계산 함수
    Vector4 CalculateHomographyParams(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4,
                                       Vector2 q1, Vector2 q2, Vector2 q3, Vector2 q4)
    {
        // 예시로 c1, c2 계산
        // 여기에 실제로 c1, c2 계산이 들어갑니다.

        // c1, c2 계산 (예시: 좌표 차이)
        float dx = p1.x - p2.x;
        float dy = p1.y - p2.y;

        // c1 = 변형된 좌표 차이 / 원본 좌표 차이
        float c1 = (q1.x - q2.x) / dx;
        float c2 = (q1.y - q2.y) / dy;

        // c1, c2 값을 반환
        return new Vector4(c1, c2, 0, 0);  // c3, c4는 계산할 필요 없으므로 0으로 처리
    }
}
