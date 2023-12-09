using UnityEngine;
namespace ZLCEngine.Inspector
{
    public class SampleInspector : MonoBehaviour
    {
        public int i;

        [Button]
        public void SampleButton(SampleInspector pa0, int pa1)
        {
            Debug.Log($"{pa0.i}  {pa1}");
        }

        [Button]
        public void Sample2Button()
        {
            Debug.Log("Sample2");
        }
    }
}