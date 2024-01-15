using UnityEngine;
namespace ZLCEngine.TerrainSystem.Boolean.Shapes
{
    /// <summary>
    /// 圆形
    /// </summary>
    public class CircleShape : AShape
    {
        /// <summary>
        /// 板半径
        /// </summary>
        public float radius;

        /// <summary>
        /// 圆心坐标
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// 分段，值越大圆越平滑，但是需要越多的点
        /// </summary>
        public int segments;

        public override Mesh CreateMesh()
        {
            var centerCircle = Vector2.zero;
            //顶点
            Vector3[] vertices = new Vector3[segments + 1];
            vertices[0] = centerCircle;
            float deltaAngle = Mathf.Deg2Rad * 360f / segments;
            float currentAngle = 0;
            for (int i = 1; i < vertices.Length; i++) {
                float cosA = Mathf.Cos(currentAngle);
                float sinA = Mathf.Sin(currentAngle);
                vertices[i] = new Vector3(cosA * radius + centerCircle.x, sinA * radius + centerCircle.y, 0);
                currentAngle += deltaAngle;
            }

            //三角形
            int[] triangles = new int[segments * 3];
            for (int i = 0, j = 1; i < segments * 3 - 3; i += 3, j++) {
                triangles[i] = 0;
                triangles[i + 1] = j + 1;
                triangles[i + 2] = j;

            }

            triangles[segments * 3 - 3] = 0;
            triangles[segments * 3 - 2] = 1;
            triangles[segments * 3 - 1] = segments;


            Mesh mesh = new Mesh();
            mesh.Clear();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            return mesh;
        }

    }
}