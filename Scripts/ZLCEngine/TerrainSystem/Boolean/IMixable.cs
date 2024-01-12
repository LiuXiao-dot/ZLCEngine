namespace ZLCEngine.TerrainSystem.Boolean
{
    /// <summary>
    /// 可混合形状
    /// 要一个形状可以与其他形状混合，需要继承该接口，或者该接口的泛型版本
    /// </summary>
    internal interface IMixable
    {
        
    }

    /// <summary>
    /// 可混合形状
    /// </summary>
    /// <typeparam name="ShapeA"></typeparam>
    /// <typeparam name="ShapeB"></typeparam>
    internal interface IMixable<ShapeA,ShapeB>
    {
        
    }
}