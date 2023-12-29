/*包含全部shader的结构*/

/* 定义一个shader
在一个shader中，可以包含以下内容：
    - Properties代码块：定义材质属性
    - SubShader代码块：定义子着色器
    - CustomEditor:自定义编辑器
    - Fallback:其他子着色器都不生效时，改用Fallback设定的着色器
*/
Shader "ZLC/SampleShaders/ShaderStruct"
{
    /* 定义材质属性
    在一个材质属性块中，材质属性声明语法为：
    [optional: attribute] name("display text in Inspector", type name) = default value
    
    各个部分解释:
        - [optional: attribute]:可选项，是类似C#Attribtue的特性，标记材质属性，实现不同的效果
        - name:材质属性的实际名称，在代码中使用
        - "display text in Inspector":材质在Inspector面板中显示的名字
        - type name:材质属性的类型
        - default value:默认值
    
    材质属性有以下这些:
    - Integer：整型
    - Int：旧版的整型
    - Float: 浮点数
    - Texture2D:纹理
        - 内置纹理可用于默认值："white"（RGBA：1,1,1,1）、"black"（RGBA：0,0,0,1）、"gray"（RGBA：0.5,0.5,0.5,1）、"bump"（RGBA：0.5,0.5,1,0.5）或"red"（RGBA：1,0,0,1）
    - Texture2DArray:纹理数组
    - Texture3D:3D纹理
        - 默认值为 "gray"（RGBA：0.5,0.5,0.5,1）纹理。
    - Cubemap:立方体贴图
        - 默认值为 "gray"（RGBA：0.5,0.5,0.5,1）纹理。
    - CubemapArray:立方体贴图数组
    - Color:颜色
        - 在Inspector中会显示一个拾色器，在代码中可以转换为一个float4
    - Vector:向量
        - 在Inspector中显示为4个单独的值，在代码中可以转换为一个float4
    
    材质属性Attribute:
    [Gamma]:指示浮点数或矢量属性使用 sRGB 值，这意味着如果项目中的颜色空间需要，则它必须与其他 sRGB 值一起转换。
    [HDR]:指示纹理或颜色属性使用高动态范围 (HDR) 值。
        - 对于纹理属性，如果分配了 LDR 纹理，则 Unity 编辑器会显示警告。对于颜色属性，Unity 编辑器会使用 HDR 拾色器编辑此值。
    [MainTexture]:为材质设置主纹理，可以使用 Material.mainTexture 进行访问。
        - 默认情况下，Unity 将具有属性名称 _BaseMap(旧版为_MainTex) 的纹理视为主纹理。如果纹理具有不同的属性名称，但希望 Unity 将它视为主纹理，请使用此特性。
        - 如果多次使用此特性，则 Unity 会使用第一个属性并忽略后续属性。
        - 注意：使用此特性设置主纹理时，如果使用纹理串流调试视图模式或自定义调试工具，则该纹理在游戏视图中不可见。
    [MainColor]:为材质设置主色，可以使用 Material.color进行访问。
        - 默认情况下，Unity 将具有属性名称 _BaseColor(旧版为_Color) 的颜色视为主色。如果您的颜色具有其他属性 (property) 名称，但您希望 Unity 将这个颜色视为主色，请使用此属性 (attribute)。如果您多次使用此属性 (attribute)，则 Unity 会使用第一个属性 (property)，而忽略后续属性 (property)。
    [NoScaleOffset]:告知 Unity 编辑器隐藏此纹理属性的平铺和偏移字段。
    [Normal]:指示纹理属性需要法线贴图。
        - 如果分配了不兼容的纹理，则 Unity 编辑器会显示警告。
    [PerRendererData]:指示纹理属性将来自每渲染器数据，形式为 MaterialPropertyBlock。
        - 材质 Inspector 会将这些属性显示为只读。
    
    材质属性在 C# 代码中通过 MaterialProperty 类进行表示。
    在 Unity 编辑器中，可以控制材质属性在 Inspector 窗口中的显示方式。为此，最简单的方法是使用 MaterialPropertyDrawer。对于更复杂的需求，可以使用 MaterialEditor、MaterialProperty 和 ShaderGUI 类。
    */
    Properties {}

    /*使用 Category 代码块可对设置渲染状态的命令进行分组，这样您可以“继承”该代码块内的分组渲染状态*/
    
    /*子着色器
    Shader 对象包含一个或多个 SubShader。通过 SubShader 您可以为不同的硬件、渲染管线和运行时设置定义不同的 GPU 设置和着色器程序。某些 Shader 对象只包含一个 SubShader；另一些包含多个 SubShader 以支持一系列不同的配置。
    在 SubShader 代码块中，可以：
        - 使用 LOD 代码块为 SubShader 分配 LOD（细节级别）值。参阅向 SubShader 分配 LOD 值。
        - 使用 Tags 代码块将数据的键值对分配给子着色器。参阅 ShaderLab：向子着色器分配标签。
        - 使用 ShaderLab 命令将 GPU 指令或着色器代码添加到 SubShader。请参阅 ShaderLab：使用命令。
        - 使用 Pass 代码块定义一个或多个通道。参阅 ShaderLab：定义通道。
        - 使用PackageRequirements使得只有在安装了指定包时unity才能加载SubShader
    */
    SubShader
    {
        /*SubShader中的命令
            - UsePass 定义一个通道，它从另一个 Shader 对象导入指定的通道的内容。
            - GrabPass 创建一个通道，将屏幕内容抓取到纹理中，以便在之后的通道中使用。（URP不支持）
        */
        
        
        /*可以使用 Material.GetTag API 从 C# 脚本中读取子着色器标签*/
        Tags
        {
            /*向 Unity 告知此子着色器是否与 URP 或 HDRP 兼容。
            [name]	UniversalRenderPipeline	此子着色器仅与 URP 兼容。
                        HighDefinitionRenderPipeline	此子着色器仅与 HDRP 兼容。
                        （任何其他值，或未声明）	此子着色器与 URP 和 HDRP 不兼容。*/
            "RenderPipeline" = "[name]"
            /*Queue 标签向 Unity 告知要用于它渲染的几何体的渲染队列。渲染队列是确定 Unity 渲染几何体的顺序的因素之一。
            “Queue” = “[queue name]”	使用命名渲染队列。
            “Queue” = “[queue name] + [offset]”	在相对于命名队列的给定偏移处使用未命名队列。
                这种用法十分有用的一种示例情况是透明的水，它应该在不透明对象之后绘制，但是在透明对象之前绘制。
            
            [queue name]:
                - Background	指定背景渲染队列。
                - Geometry	指定几何体渲染队列。
                - AlphaTest	指定 AlphaTest 渲染队列。
                - Transparent	指定透明渲染队列。
                - Overlay	指定覆盖渲染队列。
            [offset]:整数;指定 Unity 渲染未命名队列处的索引（相对于命名队列）。
            */
            "Queue" = "Transparent"

            /*使用 RenderType 标签可覆盖 Shader 对象的行为。
            在基于可编程渲染管线的渲染管线中，可以使用 RenderStateBlock 结构覆盖在 Shader 对象中定义的渲染状态。可以使用 RenderType 标签的值标识要覆盖的子着色器。*/
            "RenderType" = "TransparentCutout"

            /*ForceNoShadowCasting
                - true:标签阻止子着色器中的几何体投射（有时是接收）阴影。确切行为取决于渲染管线和渲染路径。
                - false:Unity 不会阻止此子着色器中的几何体投射或接收阴影。这是默认值。*/
            "ForceNoShadowCasting" = "True"

            /*DisableBatching
                - True	Unity 对使用此子着色器的几何体阻止动态批处理。
                - False	Unity 不会对使用此子着色器的几何体阻止动态批处理。这是默认值。
                - LODFading	对于属于 Fade Mode 值不为 None 的 LODGroup 一部分的所有几何体，Unity 会阻止动态批处理。否则，Unity 不会阻止动态批处理。
            */
            "DisableBatching" = "True"
            
            /*IgnoreProjector
                - True	Unity 在渲染此几何体时忽略投影器。
                - False	Unity 在渲染此几何体时不会忽略投影器。这是默认值。
            */
            "IgnoreProjector" = "True"
            
            /*PreviewType
            告知 Unity 编辑器如何在材质 Inspector 中显示使用此子着色器的材质。
                - 球体	在球体上显示材质。这是默认值。
                - 平面 (Plane)	在平面上显示材质。
                - Skybox	在天空盒上显示材质。
            */
            "PreviewType" = "Plane"
            
            /*CanUseSpriteAtlas 
            在使用 Legacy Sprite Packer 的项目中使用此子着色器标签可警告用户着色器依赖于原始纹理坐标，因此不应将其纹理打包到图集中。
                - True	使用此子着色器的精灵与 Legacy Sprite Packer 兼容。这是默认值。
                - False	使用此子着色器的精灵与 Legacy Sprite Packer 不兼容。
                - 当 CanUseSpriteAtlas 值为 False 的子着色器与带有 Legacy Sprite Packer 打包标签的精灵一起使用时，Unity 会在 Inspector 中显示错误消息。
            */
            "CanUseSpriteAtlas" = "False"
        }

        /*LOD
        Unity 内置着色器的 LOD 值
        在内置渲染管线中，Unity 的内置着色器具有以下 LOD 值：
            - LOD 值	着色器名称
            - 100	Unlit/Texture
            - Unlit/Color
            - Unlit/Transparent
            - Unlit/Transparent Cutout
            - 300	Standard
            - Standard (Specular Setup)
            - Autodesk Interactive
        */
        
        /*
            - 使用 Name 代码块为通道指定一个名称。请参阅 ShaderLab：为通道指定名称。
            - 使用 Tags 代码块将数据的键值对分配给通道。请参阅 ShaderLab：为通道分配标签。
            - 使用 ShaderLab 命令执行操作。请参阅 ShaderLab：使用命令。
            - 使用着色器代码块将着色器代码添加到通道。请参阅 ShaderLab：着色器代码块。
            - 使用PackageRequirements使得只有在安装了指定包时unity才能加载SubShader.
        */
        Pass
        {
            /*
            LightMode
            一个预定义的通道标签，Unity 使用它来确定是否在给定帧期间执行该通道，在该帧期间 Unity 何时执行该通道，以及 Unity 对输出执行哪些操作。
                - UniversalForward	该通道会渲染对象几何体并评估所有光源影响。URP 在前向渲染路径中使用此标签值。
                - UniversalGBuffer	该通道会渲染对象几何体，但不评估任何光源影响。在 Unity 必须在延迟渲染路径中执行的通道中使用此标签值。
                - UniversalForwardOnly	该通道会渲染对象几何体并评估所有光源影响，类似于 LightMode 具有 UniversalForward 值的情况。与 UniversalForward 的区别在于，URP 可以将该通道用于前向渲染路径和延迟渲染路径两者。
                - 如果在 URP 使用延迟渲染路径时某个通道必须使用前向渲染路径来渲染对象，请使用此值。例如，如果 URP 使用延迟渲染路径来渲染某个场景，并且该场景包含的某些对象具有不适合 G 缓冲区的着色器数据（例如透明涂层法线），则应使用此标签。
                - 如果着色器必须在前向渲染路径和延迟渲染路径两者中进行渲染，请使用 UniversalForward 和 UniversalGBuffer 标签值声明两个通道。
                - 如果着色器必须使用前向渲染路径进行渲染，而不管 URP 渲染器使用的渲染路径如何，请仅声明一个 LightMode 标签设置为 UniversalForwardOnly 的通道。
                - 如果使用了 SSAO 渲染器功能，请添加一个 LightMode 标签设置为 DepthNormalsOnly 的通道。有关更多信息，请参阅 DepthNormalsOnly 值。
                - DepthNormalsOnly	将此值与延迟渲染路径中的 UniversalForwardOnly 结合使用。此值允许 Unity 在深度和法线预通道中渲染着色器。在延迟渲染路径中，如果缺少具有 DepthNormalsOnly 标签值的通道，则 Unity 不会在网格周围生成环境光遮挡。
                - Universal2D	该通道会渲染对象并评估 2D 光源影响。URP 在 2D 渲染器中使用此标签值。
                - ShadowCaster	该通道从光源的角度将对象深度渲染到阴影贴图或深度纹理中。
                - DepthOnly	该通道仅从摄像机的角度将深度信息渲染到深度纹理中。
                - Meta	Unity 仅在 Unity 编辑器中烘焙光照贴图时执行该通道。Unity 在构建播放器时会从着色器中剥离该通道。
                - SRPDefaultUnlit	渲染对象时，使用此 LightMode 标签值绘制额外的通道。应用示例：绘制对象轮廓。此标签值对前向渲染路径和延迟渲染路径均有效。当通道没有 LightMode 标签时，URP 使用此标签值作为默认值。
            
            UniversalMaterialType
            Unity 在延迟渲染路径中使用此标签。
                - Lit	此值指示着色器类型为光照 (Lit)。在 G 缓冲区通道期间，Unity 使用模板来标记使用光照着色器类型（镜面反射模型为 PBR）的像素。
                  如果未在通道中设置标签，Unity 默认使用此值。
                - SimpleLit	此值指示着色器类型为简单光照 (SimpleLit)。在 G 缓冲区通道期间，Unity 使用模板来标记使用简单光照着色器类型（镜面反射模型为 Blinn-Phong）的像素。            
            */
            Tags{
            }
            
            /*在 Pass 代码块中使用这些命令可为该 Pass 设置渲染状态，或者在 SubShader 代码块中使用这些命令可为该 SubShader 以及其中的所有 Pass 设置渲染状态。
                - AlphaToMask：设置 alpha-to-coverage 模式。alpha-to-coverage 模式可以减少将多样本抗锯齿 (MSAA) 与使用 Alpha 测试的着色器（如植被着色器）一起使用时出现的过度锯齿.如果在不使用 MSAA 时启用 alpha-to-coverage 模式，结果无法预测
                - Blend：启用和配置 alpha 混合。确定 GPU 如何将片元着色器的输出与渲染目标进行合并。https://docs.unity3d.com/cn/current/Manual/SL-Blend.html
                - BlendOp：设置 Blend 命令使用的操作。
                - ColorMask：设置颜色通道写入掩码。
                - Conservative：启用和禁用保守光栅化。
                - Cull：设置多边形剔除模式。
                - Offset：设置多边形深度偏移。
                - Stencil：配置模板测试，以及向模板缓冲区写入的内容。
                - ZClip：设置深度剪辑模式。
                - ZTest：设置深度测试模式。
                - ZWrite：设置深度缓冲区写入模式。            
            */
            
            HLSLPROGRAM
                // 此 HLSL 着色器程序自动包含上面的 HLSLINCLUDE 块的内容
                // 在此编写 HLSL 着色器代码
                
            ENDHLSL
        }
    }

    // CustomEditor "[custom editor class name]"	Unity 使用在命名类中定义的自定义编辑器，除非它被 CustomEditorForRenderPipeline 代码块覆盖。
    // CustomEditorForRenderPipeline "[custom editor class name]" "[render pipeline asset class name]"	当活动渲染管线资源是命名类型时，Unity 使用该命名类中定义的自定义编辑器。

    /*默认着色器*/
    // eg:Fallback "ExampleOtherShader"
    /*不启用默认着色器*/
    Fallback Off
}