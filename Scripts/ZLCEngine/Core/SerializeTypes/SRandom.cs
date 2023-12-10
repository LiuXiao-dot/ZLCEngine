using System;
using UnityEngine;
using Random = System.Random;
namespace ZLCEngine.SerializeTypes
{
    /// <summary>
    ///     可序列化的随机数(默认范围：0~10000)
    /// </summary>
    [Serializable]
    public class SRandom : ISerializationCallbackReceiver
    {

        /// <summary>
        ///     当前种子(初始种子需要自行保存，不属于随机数应该保存的数据)
        /// </summary>
        [SerializeField] private int seed;
        /// <summary>
        ///     实际的随机数
        /// </summary>
        private Random _random;

        /// <summary>
        ///     无参构造
        /// </summary>
        public SRandom()
        {
            _random = new Random();
            seed = _random.Next();
        }

        /// <summary>
        /// </summary>
        /// <param name="seed">随机数种子</param>
        public SRandom(int seed)
        {
            this.seed = seed;
            _random = new Random(seed);
        }

        /// <inheritdoc />
        public void OnBeforeSerialize()
        {
        }

        /// <inheritdoc />
        public void OnAfterDeserialize()
        {
            _random = new Random(seed);
        }

        /// <summary>
        ///     随机一次
        /// </summary>
        /// <returns></returns>
        public int Roll(int min = 0, int max = 10000)
        {
            seed = _random.Next(min, max);
            return seed;
        }
    }
}